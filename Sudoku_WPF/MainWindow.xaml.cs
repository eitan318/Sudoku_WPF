using Sudoku_WPF.Pages;
using Sudoku_WPF.Themes;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DAL;
using System.Data.OleDb;
using static Sudoku_WPF.publico.Constants;
using Sudoku_WPF.publico;
using System.Media;
using System.IO;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GamePage? gamePage = null;
        public SaverPage? historyPage = null;
        public SaverPage? savedPage = null;



        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new OpenningPage());
            Resize.Visibility = Visibility.Visible;
            SetSavedGamesFromDB();
            SetSettingsFromDB();

            if(Settings.musicOn)
            {
                SoundPlayer.StartMusic(SoundConstants.GetMusicPath(SoundConstants.BACK_MUSIC_NAME));
            }



            /*GameInfo gameInfo = new GameInfo(
                                       "SHMOLIC",
                                       "board",
                                       "puzzle",
                                       "time",
                                       1,
                                       2,
                                       "today",
                                       false,
                                       false,
                                       3,
                                       3
                                       );

            */


        }




        /*InsertGame(gameInfo);
            ThemeControl.SetColors(Settings.Theme);
            MainFrame.Navigate(new OpenningPage());
            Resize.Visibility = Visibility.Visible;
            //BuildSavedGamesFromDB();

            GameInfo gameInfo = new GameInfo("", "", "", "", 3, 5, "", true, true, 3, 3);
            gameInfo.BoardCode = Code.Protect(";|;|;|1;|;|7;|;|2;|6;|;|9;|5;|1;|1;|2;|;|1;|1;|;|;|1;|8;|;|;|3;|;|;|8;|;|;|6;|3;|5;|;|;|;|;|;|7;|;|2;|8;|6;|;|3;|;|3;|;|;|;|;|;|;|2;|;|5;|;|;|;|;|;|;|;|;|;|8;|2;|4;|;|;|3;|5;|;|;|4;|;|1;|;|9;|;1,2,3,4,5,6,7,8,9,|8;");

            gameInfo.PuzzleCode = Code.Protect("3,3:4,X|8,X|3,X|1,V|9,X|7,V|5,X|2,V|6,V|7,X|9,V|5,V|3,X|6,X|2,V|8,X|4,X|1,V|2,X|6,X|1,V|8,V|5,X|4,X|3,V|9,X|7,X|8,V|4,X|2,X|6,V|3,V|5,V|7,X|1,X|9,X|9,X|1,X|7,V|4,X|2,V|8,V|6,V|5,X|3,V|5,X|3,V|6,X|9,X|7,X|1,X|4,X|8,X|2,V|1,X|5,V|9,X|7,X|8,X|3,X|2,X|6,X|4,X|6,X|7,X|8,V|2,V|4,V|9,X|1,X|3,V|5,V|3,X|2,X|4,V|5,X|1,V|6,X|9,V|7,X|8,V");
            this.historyPage = new HistoryPage();
            this.savedPage = new SavedPage();
            savedPage.AddItemToListAndDB(gameInfo); */



        /*
        private void InsertGame(GameInfo gameInfo)
        {
            string sqlStmt = @"INSERT INTO tbl_games 
              ([Current], Solved, [Time], GameDate, BoardCode, PuzzleCode, GameName, HintsTaken, ChecksTaken, BoxHeight, BoxWidth) 
              VALUES 
              (@Current, @Solved, @Time, @GameDate, @BoardCode, @PuzzleCode, @GameName, @HintsTaken, @ChecksTaken, @BoxHeight, @BoxWidth)";

            OleDbParameter[] parameters =
                       {
                    new OleDbParameter("@Current", gameInfo.Current),
                    new OleDbParameter("@Solved", gameInfo.Solved),
                    new OleDbParameter("@Time", gameInfo.Time),
                    new OleDbParameter("@GameDate", gameInfo.Date), // Use DateTime.UtcNow for UTC time
                    new OleDbParameter("@BoardCode", gameInfo.BoardCode),
                    new OleDbParameter("@PuzzleCode", gameInfo.PuzzleCode),
                    new OleDbParameter("@GameName", gameInfo.Name),
                    new OleDbParameter("@HintsTaken", gameInfo.Hints),
                    new OleDbParameter("@ChecksTaken", gameInfo.Checks),
                    new OleDbParameter("@BoxHeight", gameInfo.BoxHeight),
                    new OleDbParameter("@BoxWidth", gameInfo.BoxWidth)
                };

            DBHelper.ExecuteCommand(sqlStmt, parameters);
        }
        */


        private void SetSettingsFromDB()
        {
            string sqlstmt = "SELECT tbl_settings.* FROM   tbl_settings";
            DataTable dt = DBHelper.GetDataTable(sqlstmt);

            DataRow dr = dt.Rows[0];

            Settings.markSameText = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.SameText]);
            Settings.markRelated = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.MarkRelated]);
            Settings.soundOn = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.SoundOn]);
            Settings.musicOn = Convert.ToBoolean(dr[DBConstants.Settings_Parameters.MusicOn]);


            if (Enum.TryParse(dr[DBConstants.Settings_Parameters.Theme].ToString(), out ColorThemes theme))
            {
                Settings.Theme = theme;
            }
            ThemeControl.SetColors(Settings.Theme);

        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void SaveSettingsToDB()
        {
            string sqlStmt = DBConstants.InsertSettingsQuary;

            OleDbParameter[] parameters = new OleDbParameter[]
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

        private void SetSavedGamesFromDB()
        {
            this.historyPage = new SaverPage(true);
            this.savedPage = new SaverPage(false);

            string sqlstmt = "SELECT tbl_games.* FROM tbl_games";
            DataTable dt = DBHelper.GetDataTable(sqlstmt);

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
                        //GameInfo gameInfo = new GameInfo("", "", "", 3, 5, "", true, true);
                        //gameInfo.BoardCode = "1;|3;|1;|4;|1;|1;|2;|1;|1;|;|;|;|9;|;|5;|;|;|4;|6;|;|4;|;|2;|1;|;|7;|;|;|8;|1;|;|5;|9;|4;|2;|;|;|9;|;|;|;|;|7;|;|6;|;|;|;|;|;|;|;|9;|;|;|;|8;|1;|9;|;|6;|4;|7;|;|;|;|2;|4;|;|;|;|5;|;|;|;|;|8;|7;|;|;|;1,2,3,4,5,6,7,8,9,";
                        //.PuzzleCode = "AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAABR/DEf5PbU6E5jEabMTyBQAAAAACAAAAAAAQZgAAAAEAACAAAACTgxDsy6P1rAMpZbgLY4C9wIPfuLxTpPHt61BeL7hzTQAAAAAOgAAAAAIAACAAAAAFvg7yD09E/7b9FW/vr7ARyR9UhYNAQqWdPNebQD1BzVABAAA3iBydSM2GFaxpp7PfZV+AX6JuXQ1NAhfgu7orACn6VEJrhzjMEzDpM5RUeo1uRtX+6cMvBWb56Rtj1O54pRFnOfi4ZCBDNpwoXTghKlKHyn70SwCPqLQv5RBwj4dKgxwBa4FTFycCJbbjSK3CDvljVgjT2wQvKBV/RbNsfvnZGRI+bVew+IQAzxu8WDVql+izp4VYSWPOaHiO+jw7IyKvEW7p4UlG8fA9OD1ThnSjCB3Owo5zbb7qtBqKGJ1/bar0SXgBxFGKoI10Zrak8sjlQ/h0Dzmm355WTCB0ZuGC4k+o1HDO1/nnxChzwzu1fT1FxIkrUP4HyfTDTAnemwD8KxJO2/h17+/oR4j7Uf4BXClfUBzPkO2clFQGAyykbsENx/bSzSRRWeNfZFtGrqqtRfn5KuJX0az5zIvz7KD5iU+a28TaJ+cjDhqtLMe3dCtAAAAAQFeD4bXSaVEpKm1l9OzXued9qWM4WBf8+dGKMTGh8ruQiONcj7g0T/Ynw27c84ji+2PfLh+GoFoLLN8am2UDGA==";
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Allow dragging of the window
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            //Write to access
            if (gamePage != null)
            {
                gamePage.Game.End(false, false);//???
            }
            Application.Current.Shutdown();

            UpdateSettings();
        }

        private void UpdateSettings()
        {

        }

        private void TglSizeBtn_Checked(object sender, RoutedEventArgs e)
        {
            // Handle maximizing the window
            WindowState = WindowState.Maximized;
        }

        private void TglSizeBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            // Handle restoring the window to normal state
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

    }

}