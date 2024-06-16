
using Sudoku_WPF.publico;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.Behaviors
{
    public static class ButtonSoundBehavior
    {
        public static readonly DependencyProperty ClickSoundProperty =
            DependencyProperty.RegisterAttached(
                "ClickSound",
                typeof(string),
                typeof(ButtonSoundBehavior));

        public static string GetClickSound(Button button)
        {
            return (string)button.GetValue(ClickSoundProperty);
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
