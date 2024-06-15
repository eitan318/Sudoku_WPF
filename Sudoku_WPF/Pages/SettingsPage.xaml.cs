using Sudoku_WPF.publico;
using Sudoku_WPF.Themes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Sudoku_WPF
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            SetInitialTheme(Settings.Theme.ToString());

            // Initialize toggle button states based on Settings
            sound.IsChecked = Settings.soundOn;
            music.IsChecked = Settings.musicOn;
            markRelated.IsChecked = Settings.markRelated;
            markSameText.IsChecked = Settings.markSameText;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if (toggleButton != null)
            {
                string buttonName = toggleButton.Tag as string;
                if (!string.IsNullOrEmpty(buttonName))
                {
                    // Handle the logic based on the button name
                    switch (buttonName)
                    {
                        case "sound":
                            Settings.soundOn = toggleButton.IsChecked ?? false;
                            break;
                        case "music":
                            Settings.musicOn = toggleButton.IsChecked ?? false;
                            break;
                        case "markRelated":
                            Settings.markRelated = toggleButton.IsChecked ?? false;
                            break;
                        case "markSameText":
                            Settings.markSameText = toggleButton.IsChecked ?? false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void SetInitialTheme(string themeName)
        {
            // Find the ComboBoxItem with the specified theme name and set it as selected
            var selectedItem = ColorMode_CMBB.Items
                               .OfType<ComboBoxItem>()
                               .FirstOrDefault(item => item.Content.ToString() == themeName);

            if (selectedItem != null)
            {
                ColorMode_CMBB.SelectedItem = selectedItem;
            }
        }

        private void ColorMode_CMBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string option = "";
            ComboBoxItem cbi = (ComboBoxItem)ColorMode_CMBB.SelectedValue;
            option = cbi.Content.ToString();

            if (Enum.TryParse(option, out ColorThemes theme))
            {
                ThemeControl.SetColors(theme);
            }
        }
    }
}
