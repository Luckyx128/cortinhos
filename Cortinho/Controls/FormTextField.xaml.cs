using System.Windows;
using System.Windows.Controls;

namespace Cortinho.Controls
{
    /// <summary>TextField.jsx — label + campo + erro/hint. Placeholder é um TextBlock por cima
    /// (WPF TextBox não tem placeholder nativo), igual ao padrão já usado na busca da MainWindow.</summary>
    public partial class FormTextField : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(FormTextField), new PropertyMetadata("", OnLabelChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(FormTextField), new PropertyMetadata("", OnPlaceholderChanged));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(FormTextField),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register(nameof(Error), typeof(string), typeof(FormTextField), new PropertyMetadata("", OnErrorChanged));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(object), typeof(FormTextField), new PropertyMetadata(null, OnIconChanged));

        public static readonly DependencyProperty TrailingProperty =
            DependencyProperty.Register(nameof(Trailing), typeof(object), typeof(FormTextField), new PropertyMetadata(null, OnTrailingChanged));

        public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public string Error { get => (string)GetValue(ErrorProperty); set => SetValue(ErrorProperty, value); }
        public object? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
        public object? Trailing { get => GetValue(TrailingProperty); set => SetValue(TrailingProperty, value); }

        public FormTextField()
        {
            InitializeComponent();
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (FormTextField)d;
            string text = (string)e.NewValue;
            self.LabelBlock.Text = text.ToUpperInvariant();
            self.LabelBlock.Visibility = string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((FormTextField)d).PlaceholderText.Text = (string)e.NewValue;

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (FormTextField)d;
            string text = (string)e.NewValue ?? "";
            if (self.InnerBox.Text != text) self.InnerBox.Text = text;
            self.PlaceholderText.Visibility = string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (FormTextField)d;
            string error = (string)e.NewValue;
            self.ErrorBlock.Text = error;
            self.ErrorBlock.Visibility = string.IsNullOrEmpty(error) ? Visibility.Collapsed : Visibility.Visible;
            self.Box.BorderBrush = string.IsNullOrEmpty(error)
                ? (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("BorderSubtleBrush")
                : (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("DangerBrush");
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((FormTextField)d).IconSlot.Content = e.NewValue;

        private static void OnTrailingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((FormTextField)d).TrailingSlot.Content = e.NewValue;

        /// <summary>Só dispara quando o usuário digita (não em Text=... programático) — pra telas de
        /// config persistirem sem precisar de binding two-way "de verdade" contra um objeto.</summary>
        public event EventHandler? TextEdited;

        private void InnerBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = InnerBox.Text;
            TextEdited?.Invoke(this, EventArgs.Empty);
        }

        private void InnerBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Error))
                Box.BorderBrush = (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("AccentBrush");
        }

        private void InnerBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Error))
                Box.BorderBrush = (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource("BorderSubtleBrush");
        }
    }
}
