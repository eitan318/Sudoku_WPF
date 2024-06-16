
using Sudoku_WPF.publico;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.Behaviors
{
    public static class ToggleButtonSoundBehavior
    {
        public static readonly DependencyProperty ClickSoundProperty =
            DependencyProperty.RegisterAttached(
                "ClickSound",
                typeof(string),
                typeof(ToggleButtonSoundBehavior),
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
                string relativePath = GetClickSound(button);
                if (!string.IsNullOrEmpty(relativePath))
                {
                    // Manually combine the base directory and the relative path
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    string absolutePath = 
                        baseDirectory.Substring(0, baseDirectory.IndexOf(Constants.PROJ_DIRECTORY) + Constants.PROJ_DIRECTORY.Length) 
                        + relativePath;

                    // Ensure the file exists before playing
                    if (System.IO.File.Exists(absolutePath))
                    {
                        MediaPlayer player = new MediaPlayer();
                        player.Open(new Uri(absolutePath));
                        player.Play();
                    }
                    else
                    {
                        MessageBox.Show("Sound file not found: " + absolutePath);
                    }
                }
            }
        }



    }
}
