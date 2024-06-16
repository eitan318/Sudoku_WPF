// File: TextBoxBehavior.cs
namespace Sudoku_WPF.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;

    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.RegisterAttached(
                "DefaultText",
                typeof(string),
                typeof(TextBoxBehavior),
                new PropertyMetadata(string.Empty, OnDefaultTextChanged));

        public static string GetDefaultText(TextBox textBox)
        {
            return (string)textBox.GetValue(DefaultTextProperty);
        }

        public static void SetDefaultText(TextBox textBox, string value)
        {
            textBox.SetValue(DefaultTextProperty, value);
        }

        private static void OnDefaultTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.Text = e.NewValue as string;

                textBox.GotFocus += (sender, args) =>
                {
                    if (textBox.Text == e.NewValue as string)
                    {
                        textBox.Text = string.Empty;
                    }
                };

                textBox.LostFocus += (sender, args) =>
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Text = e.NewValue as string;
                    }
                };
            }
        }
    }
}
