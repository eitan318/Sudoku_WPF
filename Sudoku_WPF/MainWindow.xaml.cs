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
        private HistoryPage? historyPage = null;
        private SavedPage? savedPage = null;



        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new OpenningPage());
            BuildSavedGamesFromDB();
        }

        private void BuildSavedGamesFromDB()
        {
            this.historyPage = new HistoryPage();
            this.savedPage = new SavedPage();

            string sqlstmt = "SELECT tbl_games.* FROM   tbl_games";
            DataTable dt = DBHelper.GetDataTable(sqlstmt);

            foreach (DataRow dr in dt.Rows)
            {
                GameInfo gameInfo = new GameInfo(
                                        Convert.ToInt32(dr["Id"]),
                                        dr["GameName"].ToString(),
                                        dr["BoardCode"].ToString(),   // Assuming 'boardCode' is the correct column name
                                        dr["PuzzleCode"].ToString(),  // Assuming 'puzzleCode' is the correct column name
                                        dr["Time"].ToString(),        // Assuming 'time' is the correct column name
                                        Convert.ToInt32(dr["Hints"]), // Assuming 'hints' is the correct column name
                                        Convert.ToInt32(dr["Checks"]),// Assuming 'checks' is the correct column name
                                        dr["GameDate"].ToString(),        // Assuming 'date' is the correct column name
                                        Convert.ToBoolean(dr["Solved"]), // Assuming 'solved' is the correct column name
                                        Convert.ToBoolean(dr["Current"]) // Assuming 'current' is the correct column name
                                        );
                if (gameInfo.Solved)
                {
                    historyPage.AddItemToList(gameInfo);
                }
                else
                {
                    if (gameInfo.Current)
                    {
                        ThemeControl.SetColors(ColorMode.Light);
                        MainFrame.Navigate(new OpenningPage());
                        Resize.Visibility = Visibility.Visible;



                        //GameInfo gameInfo = new GameInfo("", "", "", 3, 5, "", true, true);
                        //gameInfo.BoardCode = "1;|3;|1;|4;|1;|1;|2;|1;|1;|;|;|;|9;|;|5;|;|;|4;|6;|;|4;|;|2;|1;|;|7;|;|;|8;|1;|;|5;|9;|4;|2;|;|;|9;|;|;|;|;|7;|;|6;|;|;|;|;|;|;|;|9;|;|;|;|8;|1;|9;|;|6;|4;|7;|;|;|;|2;|4;|;|;|;|5;|;|;|;|;|8;|7;|;|;|;1,2,3,4,5,6,7,8,9,";
                        //.PuzzleCode = "AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAABR/DEf5PbU6E5jEabMTyBQAAAAACAAAAAAAQZgAAAAEAACAAAACTgxDsy6P1rAMpZbgLY4C9wIPfuLxTpPHt61BeL7hzTQAAAAAOgAAAAAIAACAAAAAFvg7yD09E/7b9FW/vr7ARyR9UhYNAQqWdPNebQD1BzVABAAA3iBydSM2GFaxpp7PfZV+AX6JuXQ1NAhfgu7orACn6VEJrhzjMEzDpM5RUeo1uRtX+6cMvBWb56Rtj1O54pRFnOfi4ZCBDNpwoXTghKlKHyn70SwCPqLQv5RBwj4dKgxwBa4FTFycCJbbjSK3CDvljVgjT2wQvKBV/RbNsfvnZGRI+bVew+IQAzxu8WDVql+izp4VYSWPOaHiO+jw7IyKvEW7p4UlG8fA9OD1ThnSjCB3Owo5zbb7qtBqKGJ1/bar0SXgBxFGKoI10Zrak8sjlQ/h0Dzmm355WTCB0ZuGC4k+o1HDO1/nnxChzwzu1fT1FxIkrUP4HyfTDTAnemwD8KxJO2/h17+/oR4j7Uf4BXClfUBzPkO2clFQGAyykbsENx/bSzSRRWeNfZFtGrqqtRfn5KuJX0az5zIvz7KD5iU+a28TaJ+cjDhqtLMe3dCtAAAAAQFeD4bXSaVEpKm1l9OzXued9qWM4WBf8+dGKMTGh8ruQiONcj7g0T/Ynw27c84ji+2PfLh+GoFoLLN8am2UDGA==";

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
            Application.Current.Shutdown();
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
                        historyPage = new HistoryPage();
                    }
                    MainFrame.Navigate(historyPage);
                    break;
                case "Saved":
                    if (savedPage == null)
                    {
                        savedPage = new SavedPage();
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