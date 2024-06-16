using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_WPF
{
    using System.Media;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class ButtonSoundBehavior
    {
        public static readonly DependencyProperty ClickSoundProperty =
            DependencyProperty.RegisterAttached(
                "ClickSound",
                typeof(string),
                typeof(ButtonSoundBehavior),
                new PropertyMetadata(null, OnClickSoundChanged));

        public static string GetClickSound(Button button)
        {
            return (string)button.GetValue(ClickSoundProperty);
        }

        public static void SetClickSound(Button button, string value)
        {
            button.SetValue(ClickSoundProperty, value);
        }

        private static void OnClickSoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                if (e.NewValue != null)
                {
                    button.Click += OnButtonClick;
                }
                else
                {
                    button.Click -= OnButtonClick;
                }
            }
        }

        private static void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string soundFilePath = GetClickSound(button);
                if (!string.IsNullOrEmpty(soundFilePath))
                {
                    SoundPlayer player = new SoundPlayer(soundFilePath);
                    player.Play();
                }
            }
        }
    }

}
