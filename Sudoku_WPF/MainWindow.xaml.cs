using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DAL;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF.Pages;
using Sudoku_WPF.publico;
using Sudoku_WPF.Themes;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for the main window of the Sudoku application.
    /// </summary>
    public partial class MainWindow : Window
    {
        public GamePage gamePage; // Game page instance for the Sudoku game
        public SaverPage historyPage; // Page to display history of solved games
        public SaverPage savedPage; // Page to display saved but unsolved games

        /// <summary>
        /// Constructor for initializing the main window of the Sudoku application.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new OpenningPage()); // Navigate to the opening page
            Resize.Visibility = Visibility.Visible; // Set resize button visibility
            SetSavedGamesFromDB(); // Load saved games from database
            SetSettingsFromDB(); // Load settings from database
            Closing += MainWindow_Closing;

            // Start background music if enabled in settings
            if (Settings.musicOn)
            {
                SoundPlayer.StartMusic(SoundConstants.BACK_MUSIC_NAME);
            }
        }

        /// <summary>
        /// Sets the saved games pages from the database.
        /// </summary>
        private void SetSavedGamesFromDB()
        {
            historyPage = new SaverPage(true); // Initialize history page
            savedPage = new SaverPage(false); // Initialize saved games page

            DataTable dt = DBHelper.GetDataTable("SELECT * FROM tbl_games"); // Retrieve games data from database

            foreach (DataRow dr in dt.Rows)
            {
                // Create GameInfo object from database row
                GameInfo gameInfo = new GameInfo(
                                    Convert.ToInt32(dr["Id"]),
                                    dr["GameName"].ToString(),
                                    dr["BoardCode"].ToString(),
                                    dr["PuzzleCode"].ToString(),
                                    dr["Time"].ToString(),
                                    Convert.ToInt32(dr["HintsTaken"]),
                                    Convert.ToInt32(dr["ChecksTaken"]),
                                    dr["GameDate"].ToString(),
                                    (DificultyLevel)Enum.Parse(typeof(DificultyLevel), dr["DifficultyLevel"].ToString()),
                                    Convert.ToBoolean(dr["Solved"]),
                                    Convert.ToBoolean(dr["Current"]),
                                    Convert.ToInt32(dr["BoxHeight"]),
                                    Convert.ToInt32(dr["BoxWidth"])
                                    );

                // Determine where to add the game info based on solved and current flags
                if (gameInfo.Solved)
                {
                    historyPage.AddItemToList(gameInfo); // Add to history page
                }
                else
                {
                    if (gameInfo.Current)
                    {
                        SaverPage.DeleteGameFromDB(gameInfo); // Remove from database if current game
                        gamePage = new GamePage(gameInfo); // Initialize game page
                        return; // Exit method after initializing game page
                    }
                    else
                    {
                        savedPage.AddItemToList(gameInfo); // Add to saved games page
                    }
                }
            }
        }

        /// <summary>
        /// Sets the application settings from the database.
        /// </summary>
        private void SetSettingsFromDB()
        {
            DataTable dt = DBHelper.GetDataTable("SELECT * FROM tbl_settings");
            DataRow dr = dt.Rows[0]; // Get the first row (assuming single settings row)

            // Load settings values from database
            Settings.markSameText = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.SameText]);
            Settings.markRelated = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.MarkRelated]);
            Settings.soundOn = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.SoundOn]);
            Settings.musicOn = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.MusicOn]);

            // Set theme if valid theme found in database
            if (Enum.TryParse(dr[DBConstants.Settings_Parameters.Theme].ToString(), 
                out ColorThemes theme))
            {
                Settings.theme = theme;
                ThemeControl.SetColors(Settings.theme); // Apply theme colors
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop music if playing
            if (Settings.musicOn)
            {
                SoundPlayer.StopMusic();
            }

            // Insert current game into database if game page exists
            if (gamePage != null)
            {
                SaverPage.InsertGame(gamePage.GetGameInfo(false, true));
            }

            UpdateSettingsInDB(); // Update settings in database
            Application.Current.Shutdown(); // Shutdown the application
        }

        /// <summary>
        /// Minimizes the application window.
        /// </summary>
        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized; // Minimize the application window
        }

        /// <summary>
        /// Closes the application window and performs necessary cleanup.
        /// </summary>
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles toggling the window size between maximized and normal.
        /// </summary>
        private void TglSizeBtn_Checked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized; // Maximize the application window
        }

        /// <summary>
        /// Handles toggling the window size between normal and maximized.
        /// </summary>
        private void TglSizeBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal; // Restore application window to normal size
        }

        /// <summary>
        /// Handles click events for icon buttons in the application.
        /// </summary>
        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.MENU_CLICK); // Play menu click sound

            RadioButton menuBtn = sender as RadioButton; // Get the clicked radio button
            string content = menuBtn.Content.ToString(); // Get the content of the clicked button

            // Navigate to respective pages based on button content
            switch (content)
            {
                case "Home":
                    MainFrame.Navigate(new OpenningPage()); // Navigate to opening page
                    break;
                case "History":
                    if (historyPage == null)
                    {
                        historyPage = new SaverPage(true); // Initialize history page if not already
                    }
                    MainFrame.Navigate(historyPage); // Navigate to history page
                    break;
                case "Saved":
                    if (savedPage == null)
                    {
                        savedPage = new SaverPage(false); // Initialize saved games page if not already
                    }
                    MainFrame.Navigate(savedPage); // Navigate to saved games page
                    break;
                case "Settings":
                    MainFrame.Navigate(new SettingsPage()); // Navigate to settings page
                    break;
                case "Game":
                    if (gamePage == null)
                    {
                        Page gsp = new GameSettingsPage();
                        MainFrame.Navigate(gsp); // Navigate to game settings page
                    }
                    else
                    {
                        MainFrame.Navigate(gamePage); // Navigate to game page if already initialized
                    }
                    break;
                case "Instructions":
                    MainFrame.Navigate(new InstructionsPage()); // Navigate to instructions page
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles mouse left button down event to enable window dragging.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove(); // Allow dragging of the window
        }

        /// <summary>
        /// Updates the application settings in the database.
        /// </summary>
        public void UpdateSettingsInDB()
        {
            string sqlStmt = DBConstants.UpdateSettingsQuary; // SQL statement to update settings

            // Define parameters for the SQL statement
            OleDbParameter[] parameters =
            {
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.SameText, Settings.markSameText),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.MarkRelated, Settings.markRelated),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.SoundOn, Settings.soundOn),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.MusicOn, Settings.musicOn),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.Theme, Settings.theme.ToString()),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.AllowNotes, Settings.allowNotes)
            };

            DBHelper.ExecuteCommand(sqlStmt, parameters); // Execute the database command with parameters
        }
    }
}
