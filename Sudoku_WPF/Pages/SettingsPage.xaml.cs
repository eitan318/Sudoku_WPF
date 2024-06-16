using static Sudoku_WPF.publico.Constants;
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
            allowNotes.IsChecked = Settings.allowNotes;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.ON_OFF);

            ToggleButton toggleButton = sender as ToggleButton;
            if (toggleButton != null)
            {
                string buttonName = toggleButton.Tag as string;
                if (!string.IsNullOrEmpty(buttonName))
                {
                    // Handle the logic based on the button name
                    switch (buttonName)
                    {
                        case DBConstants.Settings_Parameters.SoundOn:
                            Settings.soundOn = toggleButton.IsChecked ?? false;
                            break;
                        case DBConstants.Settings_Parameters.MusicOn:
                            Settings.musicOn = toggleButton.IsChecked ?? false;
                            if (Settings.musicOn)
                            {
                                SoundPlayer.StartMusic(SoundConstants.GetMusicPath(SoundConstants.BACK_MUSIC_NAME));
                            }
                            else
                            {
                                SoundPlayer.StopMusic();
                            }
                            break;
                        case DBConstants.Settings_Parameters.MarkRelated:
                            Settings.markRelated = toggleButton.IsChecked ?? false;
                            break;
                        case DBConstants.Settings_Parameters.AllowNotes:
                            Settings.allowNotes = toggleButton.IsChecked ?? false;
                            break;
                        case DBConstants.Settings_Parameters.SameText:
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
                Settings.Theme = theme;
            }
        }
    }
}
