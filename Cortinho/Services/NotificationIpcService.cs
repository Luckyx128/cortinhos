using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Cortinho.Services
{
    public sealed record NotificationPayload
    {
        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("message")]
        public string Message { get; init; } = string.Empty;

        [JsonPropertyName("kind")]
        public string Kind { get; init; } = "alert"; // "alert", "success", "error", "progress"

        [JsonPropertyName("progress")]
        public double? Progress { get; init; }

        [JsonPropertyName("durationSeconds")]
        public double? DurationSeconds { get; init; }

        [JsonPropertyName("source")]
        public string? Source { get; init; }

        public NotificationPayload() { }

        public NotificationPayload(
            string message,
            string? title = null,
            string kind = "alert",
            double? progress = null,
            double? durationSeconds = null,
            string? source = null)
        {
            Message = message;
            Title = title;
            Kind = string.IsNullOrWhiteSpace(kind) ? "alert" : kind;
            Progress = progress;
            DurationSeconds = durationSeconds;
            Source = source;
        }
    }

    public static class NotificationIpcService
    {
        public const string PipeName = "cortinho-notifications";
        public const string HttpUrlPrefix = "http://127.0.0.1:48123/notify/";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static CancellationTokenSource? _cts;
        private static HttpListener? _httpListener;
        private static bool _isRunning;
        private static readonly object _syncLock = new();

        /// <summary>
        /// Disparado sempre que uma nova notificação chega via Named Pipe, HTTP ou internamente.
        /// Garantido de ser executado na Thread de UI (Dispatcher) do WPF.
        /// </summary>
        public static event Action<NotificationPayload>? NotificationReceived;

        public static bool IsRunning
        {
            get
            {
                lock (_syncLock) return _isRunning;
            }
        }

        public static void Start()
        {
            lock (_syncLock)
            {
                if (_isRunning) return;
                _isRunning = true;
                _cts = new CancellationTokenSource();
            }

            var token = _cts.Token;
            Task.Run(() => RunNamedPipeServerAsync(token), token);
            Task.Run(() => RunHttpServerAsync(token), token);
        }

        public static void Stop()
        {
            lock (_syncLock)
            {
                if (!_isRunning) return;
                _isRunning = false;
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;

                try
                {
                    if (_httpListener?.IsListening == true)
                    {
                        _httpListener.Stop();
                        _httpListener.Close();
                    }
                }
                catch { }
                _httpListener = null;
            }
        }

        public static void RaiseNotification(NotificationPayload payload)
        {
            var app = System.Windows.Application.Current;
            if (app != null && app.Dispatcher != null && !app.Dispatcher.CheckAccess())
            {
                app.Dispatcher.BeginInvoke(new Action(() => NotificationReceived?.Invoke(payload)));
            }
            else
            {
                NotificationReceived?.Invoke(payload);
            }
        }

        public static async Task<bool> SendNotificationAsync(NotificationPayload payload, int timeoutMs = 2000)
        {
            try
            {
                using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                using var connectCts = new CancellationTokenSource(timeoutMs);
                await client.ConnectAsync(connectCts.Token).ConfigureAwait(false);

                var json = JsonSerializer.Serialize(payload, JsonOptions);
                using var writer = new StreamWriter(client, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
                await writer.WriteLineAsync(json).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task RunNamedPipeServerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using var server = new NamedPipeServerStream(
                        PipeName,
                        PipeDirection.In,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    await server.WaitForConnectionAsync(token).ConfigureAwait(false);

                    using var reader = new StreamReader(server, Encoding.UTF8);
                    string? line = await reader.ReadLineAsync(token).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var payload = JsonSerializer.Deserialize<NotificationPayload>(line, JsonOptions);
                        if (payload != null)
                        {
                            RaiseNotification(payload);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // Pequeno atraso em caso de falha transitória
                    try { await Task.Delay(250, token).ConfigureAwait(false); } catch { break; }
                }
            }
        }

        private static async Task RunHttpServerAsync(CancellationToken token)
        {
            try
            {
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add(HttpUrlPrefix);
                _httpListener.Start();
            }
            catch
            {
                // Porta em uso ou sem permissão local
                return;
            }

            while (!token.IsCancellationRequested && _httpListener?.IsListening == true)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync().ConfigureAwait(false);
                    _ = HandleHttpRequestAsync(context);
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    try { await Task.Delay(250, token).ConfigureAwait(false); } catch { break; }
                }
            }
        }

        private static async Task HandleHttpRequestAsync(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;

            try
            {
                res.Headers.Add("Access-Control-Allow-Origin", "*");
                res.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
                res.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (req.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    res.StatusCode = (int)HttpStatusCode.NoContent;
                    res.Close();
                    return;
                }

                if (!req.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    res.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    res.Close();
                    return;
                }

                using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                var body = await reader.ReadToEndAsync().ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(body))
                {
                    var payload = JsonSerializer.Deserialize<NotificationPayload>(body, JsonOptions);
                    if (payload != null)
                    {
                        RaiseNotification(payload);
                        res.StatusCode = (int)HttpStatusCode.OK;
                        var responseBytes = Encoding.UTF8.GetBytes("{\"status\":\"ok\"}");
                        res.ContentType = "application/json";
                        res.ContentLength64 = responseBytes.Length;
                        await res.OutputStream.WriteAsync(responseBytes).ConfigureAwait(false);
                        res.Close();
                        return;
                    }
                }

                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Close();
            }
            catch
            {
                try
                {
                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    res.Close();
                }
                catch { }
            }
        }
    }
}
