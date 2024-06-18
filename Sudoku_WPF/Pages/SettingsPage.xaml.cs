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
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        /// <summary>
        /// Constructor for initializing the SettingsPage.
        /// Sets up initial UI state based on current settings.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
            SetInitialTheme(Settings.theme.ToString());

            // Initialize toggle button states based on Settings
            sound.IsChecked = Settings.soundOn;
            music.IsChecked = Settings.musicOn;
            markRelated.IsChecked = Settings.markRelated;
            markSameText.IsChecked = Settings.markSameText;
            allowNotes.IsChecked = Settings.allowNotes;
        }

        /// <summary>
        /// Event handler for toggle button clicks.
        /// Updates corresponding setting values based on button state.
        /// </summary>
        /// <param name="sender">The toggle button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
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
                                SoundPlayer.StartMusic(SoundConstants.BACK_MUSIC_NAME);
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

        /// <summary>
        /// Sets the initial theme selection in the ColorMode_CMBB ComboBox.
        /// </summary>
        /// <param name="themeName">The name of the theme to select initially.</param>
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

        /// <summary>
        /// Event handler for the ColorMode_CMBB ComboBox selection change.
        /// Applies the selected theme and updates the Settings.
        /// </summary>
        /// <param name="sender">The ComboBox that triggered the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ColorMode_CMBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string option = "";
            ComboBoxItem cbi = (ComboBoxItem)ColorMode_CMBB.SelectedValue;
            option = cbi.Content.ToString();

            if (Enum.TryParse(option, out ColorThemes theme))
            {
                ThemeControl.SetColors(theme);
                Settings.theme = theme;
            }
        }
    }
}
