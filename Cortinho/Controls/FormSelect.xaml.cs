using System.Windows;
using System.Windows.Controls;

namespace Cortinho.Controls
{
    public sealed class FormSelectOption
    {
        public object Value { get; init; } = "";
        public string Label { get; init; } = "";
    }

    /// <summary>Select.jsx — dropdown com chrome próprio. Itens são FormSelectOption (Value/Label);
    /// usado pra Tipo do atalho (fase 4) e âncora/monitor do notch (fase 5).</summary>
    public partial class FormSelect : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(FormSelect), new PropertyMetadata("", OnLabelChanged));

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(nameof(Options), typeof(System.Collections.IEnumerable), typeof(FormSelect), new PropertyMetadata(null, OnOptionsChanged));

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(FormSelect),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

        public event EventHandler? SelectionChanged;

        public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
        public System.Collections.IEnumerable Options { get => (System.Collections.IEnumerable)GetValue(OptionsProperty); set => SetValue(OptionsProperty, value); }
        public object SelectedValue { get => GetValue(SelectedValueProperty); set => SetValue(SelectedValueProperty, value); }

        public FormSelect()
        {
            InitializeComponent();
            Combo.DisplayMemberPath = "Label";
            Combo.SelectedValuePath = "Value";
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (FormSelect)d;
            string text = (string)e.NewValue;
            self.LabelBlock.Text = text.ToUpperInvariant();
            self.LabelBlock.Visibility = string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
        }

        private static void OnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (FormSelect)d;
            self.Combo.ItemsSource = (System.Collections.IEnumerable)e.NewValue;
            self.Combo.SelectedValue = self.SelectedValue;
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (FormSelect)d;
            if (!Equals(self.Combo.SelectedValue, e.NewValue))
                self.Combo.SelectedValue = e.NewValue;
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedValue = Combo.SelectedValue;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
