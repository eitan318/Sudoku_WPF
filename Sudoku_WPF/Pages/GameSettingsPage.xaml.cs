using ControlLib;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF.publico;
using Sudoku_WPF.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class GameSettingsPage : Page
    {
        public Page gamePage;

        /// <summary>
        /// Constructor for initializing the GameSettingsPage.
        /// Sets initial theme, max values for number up down controls, and event handlers.
        /// </summary>
        public GameSettingsPage()
        {
            InitializeComponent();

            SetInitialTheme(GameSettings.dificultyLevel.ToString());

            NUD_boxHeight.MaxValue = 4;
            NUD_boxWidth.MaxValue = 4;

            NUD_boxHeight.Value = GameSettings.BoxHeight;
            NUD_boxWidth.Value = GameSettings.BoxWidth;

            NUD_boxWidth.ValueChanged += NUD_ValueChanged;
            NUD_boxHeight.ValueChanged += NUD_ValueChanged;

            gamePage = null;
        }

        /// <summary>
        /// Sets the initial theme based on the provided theme name.
        /// </summary>
        /// <param name="themeName">The name of the theme to set.</param>
        private void SetInitialTheme(string themeName)
        {
            // Find the ComboBoxItem with the specified theme name and set it as selected
            var selectedItem = DificultyLevel_CMBB.Items
                               .OfType<ComboBoxItem>()
                               .FirstOrDefault(item => item.Content.ToString() == themeName);

            if (selectedItem != null)
            {
                DificultyLevel_CMBB.SelectedItem = selectedItem;
            }
        }

        /// <summary>
        /// Event handler for value changes in the number up down controls (NUD).
        /// Plays a sound and adjusts max values based on current NUD values.
        /// </summary>
        private void NUD_ValueChanged(object sender, EventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            if (NUD_boxWidth.Value == 5)
            {
                NUD_boxHeight.MaxValue = 4;
            }
            else if (NUD_boxHeight.Value == 5)
            {
                NUD_boxWidth.MaxValue = 4;
            }
            else
            {
                NUD_boxWidth.MaxValue = 5;
                NUD_boxHeight.MaxValue = 5;
            }
        }

        /// <summary>
        /// Event handler for selection changes in the difficulty level combo box.
        /// Updates the game settings with the selected difficulty level.
        /// </summary>
        private void DificultyLevel_CMBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string option = "";
            ComboBoxItem cbi = (ComboBoxItem)DificultyLevel_CMBB.SelectedValue;
            option = cbi.Content.ToString();

            if (Enum.TryParse(option, out DificultyLevel difLvl))
            {
                GameSettings.dificultyLevel = difLvl;
            }
        }

        /// <summary>
        /// Event handler for clicking the game starter button.
        /// Plays a sound and initiates the game based on user input or puzzle code.
        /// </summary>
        private void GameStarterBtn_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            var window = (MainWindow)Application.Current.MainWindow;
            if (CodeTXTBox.Text == "" || CodeTXTBox.Text == "Enter puzzle code")
            {
                GameSettings.BoxHeight = Convert.ToInt32(NUD_boxHeight.Value);
                GameSettings.BoxWidth = Convert.ToInt32(NUD_boxWidth.Value);
                GameSettings.BoardSide = GameSettings.BoxWidth * GameSettings.BoxHeight;
                window.gamePage = new GamePage();
                NavigationService.Navigate(window.gamePage);
                //window.Settings_btn.Visibility = Visibility.Collapsed;
            }
            else if (IsValidCode(Code.Unprotect(CodeTXTBox.Text)))
            {
                window.gamePage = new GamePage(CodeTXTBox.Text);
                NavigationService.Navigate(window.gamePage);
            }
            else
            {
                MessageBox.Show("Puzzle code invalid! Enter it again");
                this.CodeTXTBox.Text = "";
            }
        }

        /// <summary>
        /// Checks if the provided puzzle code is valid.
        /// </summary>
        /// <param name="code">The puzzle code to validate.</param>
        /// <returns>True if the puzzle code is valid; otherwise, false.</returns>
        private bool IsValidCode(string code)
        {
            if (code == null)
            {
                return false;
            }

            // Check for the presence of the colon and commas
            int settingEnd = code.IndexOf(":");
            int firstSeparator = code.IndexOf(",");

            if (settingEnd == -1 || firstSeparator == -1 || firstSeparator >= settingEnd)
            {
                return false;
            }

            // Check if box height and width are integers and positive
            if (!int.TryParse(code.Substring(0, firstSeparator), out int boxHeight) || boxHeight <= 0)
            {
                return false;
            }

            if (!int.TryParse(code.Substring(firstSeparator + 1, settingEnd - firstSeparator - 1), out int boxWidth) || boxWidth <= 0)
            {
                return false;
            }

            int boardSide = boxWidth * boxHeight;

            // Check the puzzle data length and format
            string puzzleData = code.Substring(settingEnd + 1);
            string[] puzzleParts = puzzleData.Split('|');

            if (puzzleParts.Length != boardSide * boardSide)
            {
                return false;
            }

            // Check each puzzle part for valid characters and format
            foreach (string part in puzzleParts)
            {
                int commaIndex = part.IndexOf(',');
                if (commaIndex == -1 || commaIndex != 1 || (part[commaIndex + 1] != 'V' && part[commaIndex + 1] != 'X'))
                {
                    return false;
                }

                char puzzleChar = part[0];
                if (!Cell.IsValidInput(puzzleChar.ToString(), boardSide) && puzzleChar != '.')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
