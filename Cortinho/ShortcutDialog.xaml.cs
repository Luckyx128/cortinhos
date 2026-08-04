using Cortinho.Controls;
using Cortinho.Models;
using Cortinho.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cortinho
{
    /// <summary>Dialog.jsx — modal de adicionar/editar atalho. Só os campos que o handoff especifica
    /// (Nome/Tipo/Caminho/Rastrear Discord); Pinned não está no diálogo no spec — vira toggle no menu
    /// de contexto do tile (ver MainWindow.xaml.cs), e Arguments/DiscordServerName seguem só editáveis
    /// à mão no shortcuts.json, como sempre foram.</summary>
    public partial class ShortcutDialog : Window
    {
        private readonly ShortcutItem? _editing;
        private readonly bool _wasPinned;

        public ShortcutDialog(Window owner, ShortcutItem? editing = null)
        {
            InitializeComponent();
            Owner = owner;
            _editing = editing;
            _wasPinned = editing?.Pinned ?? false;

            TypeField.Options = new List<FormSelectOption>
            {
                new() { Value = ShortcutType.Application, Label = "Aplicativo" },
                new() { Value = ShortcutType.Folder, Label = "Pasta" },
                new() { Value = ShortcutType.Url, Label = "URL" },
                new() { Value = ShortcutType.Command, Label = "Comando" },
            };

            if (editing != null)
            {
                TitleText.Text = "Editar atalho";
                SaveButton.Content = "Salvar";
                NameField.Text = editing.Name;
                PathField.Text = editing.Path;
                TypeField.SelectedValue = editing.Type;
                TrackDiscordCheck.IsChecked = editing.TrackDiscordPresence;
            }
            else
            {
                TypeField.SelectedValue = ShortcutType.Application;
            }

            UpdateForType((ShortcutType)TypeField.SelectedValue);
        }

        private void TypeField_SelectionChanged(object sender, EventArgs e) => UpdateForType((ShortcutType)TypeField.SelectedValue);

        private void UpdateForType(ShortcutType type)
        {
            TrackDiscordCheck.Visibility = type == ShortcutType.Application ? Visibility.Visible : Visibility.Collapsed;

            bool canBrowse = type is ShortcutType.Application or ShortcutType.Folder;
            if (canBrowse)
            {
                var browse = new System.Windows.Controls.Button { Content = "Procurar", Style = (Style)FindResource("SecondaryButtonStyle") };
                browse.Click += (_, _) => Browse(type);
                PathField.Trailing = browse;
            }
            else
            {
                PathField.Trailing = null;
            }

            var (l1, l1op, l2, l2op) = ShortcutIconProvider.GetIconLayers(type);
            PathField.Icon = new Viewbox
            {
                Width = 16,
                Height = 16,
                Child = new Canvas
                {
                    Width = 24,
                    Height = 24,
                    Children =
                    {
                        new System.Windows.Shapes.Path { Data = Geometry.Parse(l1), Opacity = l1op, Fill = (System.Windows.Media.Brush)FindResource("TextTertiaryBrush") },
                        new System.Windows.Shapes.Path { Data = Geometry.Parse(l2), Opacity = l2op, Fill = (System.Windows.Media.Brush)FindResource("TextTertiaryBrush") },
                    }
                }
            };
        }

        private void Browse(ShortcutType type)
        {
            if (type == ShortcutType.Application)
            {
                var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "Executáveis (*.exe)|*.exe|Todos os arquivos (*.*)|*.*" };
                if (dialog.ShowDialog(this) == true)
                    PathField.Text = dialog.FileName;
            }
            else if (type == ShortcutType.Folder)
            {
                using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    PathField.Text = dialog.SelectedPath;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = NameField.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(name))
            {
                NameField.Error = "Preencha o nome.";
                return;
            }

            var items = ShortcutStore.Load();
            var target = _editing != null ? items.FirstOrDefault(i => ReferenceEquals(i, _editing) || SameItem(i, _editing)) : null;

            if (target == null)
            {
                target = new ShortcutItem();
                items.Add(target);
            }

            target.Name = name;
            target.Type = (ShortcutType)TypeField.SelectedValue;
            target.Path = PathField.Text?.Trim() ?? "";
            target.TrackDiscordPresence = target.Type == ShortcutType.Application && TrackDiscordCheck.IsChecked == true;
            target.Pinned = _wasPinned;

            ShortcutStore.Save(items);
            DialogResult = true;
            Close();
        }

        // ShortcutItem não tem identidade própria (sem Id) — casa pelo mesmo objeto de referência
        // quando possível; senão, por nome+caminho como já era antes (edição feita a partir da lista recarregada).
        private static bool SameItem(ShortcutItem a, ShortcutItem b) => a.Name == b.Name && a.Path == b.Path && a.Type == b.Type;

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Cancel_Click(sender, e);
        }
    }
}
