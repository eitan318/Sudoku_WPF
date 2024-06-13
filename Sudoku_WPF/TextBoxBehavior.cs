using System.Windows;
using System.Windows.Controls;

namespace Sudoku_WPF
{
    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.RegisterAttached("DefaultText", typeof(string), typeof(TextBoxBehavior),
                new PropertyMetadata("", OnDefaultTextChanged));

        public static string GetDefaultText(DependencyObject obj)
        {
            return (string)obj.GetValue(DefaultTextProperty);
        }

        public static void SetDefaultText(DependencyObject obj, string value)
        {
            obj.SetValue(DefaultTextProperty, value);
        }

        private static void OnDefaultTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if (textBox == null)
                return;

            textBox.GotFocus -= TextBox_GotFocus;
            textBox.LostFocus -= TextBox_LostFocus;

            if (!string.IsNullOrEmpty((string)e.NewValue))
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
            }

            UpdateText(textBox);
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == GetDefaultText(textBox))
            {
                textBox.Text = string.Empty;
            }
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = GetDefaultText(textBox);
            }
        }

        private static void UpdateText(TextBox textBox)
        {
            if (textBox.IsFocused || !string.IsNullOrWhiteSpace(textBox.Text))
                return;

            textBox.Text = GetDefaultText(textBox);
        }
    }
}
