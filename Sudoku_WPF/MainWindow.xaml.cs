using System;
using System.Data;
using System.Data.OleDb;
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
    public partial class MainWindow : Window
    {
        public GamePage gamePage;
        public SaverPage historyPage;
        public SaverPage savedPage;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new OpenningPage());
            Resize.Visibility = Visibility.Visible;
            SetSavedGamesFromDB();
            SetSettingsFromDB();

            if (Settings.musicOn)
            {
                SoundPlayer.StartMusic(SoundConstants.GetMusicPath(SoundConstants.BACK_MUSIC_NAME));
            }
        }

        private void SetSavedGamesFromDB()
        {
            historyPage = new SaverPage(true);
            savedPage = new SaverPage(false);

            DataTable dt = DBHelper.GetDataTable("SELECT * FROM tbl_games");

            foreach (DataRow dr in dt.Rows)
            {
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

                if (gameInfo.Solved)
                {
                    historyPage.AddItemToList(gameInfo);
                }
                else
                {
                    if (gameInfo.Current)
                    {
                        SaverPage.DeleteGameFromDB(gameInfo);
                        gamePage = new GamePage(gameInfo);
                        return;
                    }
                    else
                    {
                        savedPage.AddItemToList(gameInfo);
                    }
                }
            }
        }

        private void SetSettingsFromDB()
        {
            DataTable dt = DBHelper.GetDataTable("SELECT * FROM tbl_settings");
            DataRow dr = dt.Rows[0];

            Settings.markSameText = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.SameText]);
            Settings.markRelated = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.MarkRelated]);
            Settings.soundOn = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.SoundOn]);
            Settings.musicOn = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.MusicOn]);

            if (Enum.TryParse(dr[DBConstants.Settings_Parameters.Theme].ToString(), out ColorThemes theme))
            {
                Settings.Theme = theme;
                ThemeControl.SetColors(Settings.Theme);
            }
        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.musicOn)
            {
                SoundPlayer.StopMusic();
            }
            
            if(gamePage != null)
            {
                SaverPage.InsertGame(gamePage.GetGameInfo(false, true));
            }
            

            UpdateSettingsInDB();
            Application.Current.Shutdown();
        }

        private void TglSizeBtn_Checked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void TglSizeBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.MENU_CLICK);

            RadioButton menuBtn = sender as RadioButton;
            string content = menuBtn.Content.ToString();

            switch (content)
            {
                case "Home":
                    MainFrame.Navigate(new OpenningPage());
                    break;
                case "History":
                    if (historyPage == null)
                    {
                        historyPage = new SaverPage(true);
                    }
                    MainFrame.Navigate(historyPage);
                    break;
                case "Saved":
                    if (savedPage == null)
                    {
                        savedPage = new SaverPage(false);
                    }
                    MainFrame.Navigate(savedPage);
                    break;
                case "Settings":
                    MainFrame.Navigate(new SettingsPage());
                    break;
                case "Game":
                    if (gamePage == null)
                    {
                        Page gsp = new GameSettingsPage();
                        MainFrame.Navigate(gsp);
                    }
                    else
                    {
                        MainFrame.Navigate(gamePage);
                    }
                    break;
                case "Instructions":
                    MainFrame.Navigate(new InstructionsPage());
                    break;
                default:
                    break;
            }
        }
            
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void UpdateSettingsInDB()
        {
            string sqlStmt = DBConstants.UpdateSettingsQuary;

            OleDbParameter[] parameters =
            {
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.SameText, Settings.markSameText),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.MarkRelated, Settings.markRelated),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.SoundOn, Settings.soundOn),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.MusicOn, Settings.musicOn),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.Theme, Settings.Theme.ToString()),
                new OleDbParameter(DBConstants.AT + DBConstants.Settings_Parameters.AllowNotes, Settings.allowNotes)
            };

            DBHelper.ExecuteCommand(sqlStmt, parameters);
        }
    }
}
