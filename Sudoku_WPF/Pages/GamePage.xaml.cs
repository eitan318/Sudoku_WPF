using System;
using System.Data.OleDb;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using DAL;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF.publico;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Sudoku_WPF.publico.Constants;


namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        public Game game;
        private bool isLoaded = true;
        private int hintsLeft;
        private int checksLeft;

        // Default constructor for XAML
        public GamePage()
        {
            InitializeComponent();
            Init();
            game = new Game(SudokuGrid, timerTxtB);
        }

        public GamePage(string puzzleCode)
        {
            InitializeComponent();
            Init();

            game = new Game(SudokuGrid, timerTxtB, puzzleCode);
        }

        public GamePage(GameInfo gameInfo)
        {
            InitializeComponent();
            Init();
            this.nameTxtB.Text = gameInfo.Name;
            this.nameTxtB.IsReadOnly = true;

            this.hintsLeft -= gameInfo.Hints;
            this.checksLeft -= gameInfo.Checks;

            hintsTxtB.Text = hintsLeft.ToString() + GameConstants.REMEINING_STR;
            checksTxtB.Text = checksLeft.ToString() + GameConstants.REMEINING_STR;

            game = new Game(SudokuGrid, timerTxtB, gameInfo);
        }

        public GameInfo GetGameInfo(bool solved, bool current)
        {
            GameInfo gameInfo = new GameInfo(
                DBHelper.GetNextId("tbl_games",DBConstants.Games_Parameters.Id),
                 nameTxtB.Text,
                 this.game.Board.GenerateBoardCode(),
                 this.game.GetPuzzleCode(),
                 this.game.GetTime(),
                 GameConstants.HINTS - hintsLeft,
                 GameConstants.CHECKS - checksLeft,
                 DateTime.Now.ToString(),
                 GameSettings.dificultyLevel,
                 solved,
                 current,
                 GameSettings.BoxHeight,
                 GameSettings.BoxWidth
                 );
            return gameInfo;

        }
        


        private void Init()
        {

            this.hintsLeft = GameConstants.HINTS;
            this.checksLeft = GameConstants.CHECKS;

            hintsTxtB.Text = hintsLeft.ToString() + GameConstants.REMEINING_STR;
            checksTxtB.Text = checksLeft.ToString() + GameConstants.REMEINING_STR;

            this.Loaded += MyPage_Loaded;
            this.Unloaded += MyPage_Unloaded;
        }

        private void MyPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to the Navigating event
            if (NavigationService != null)
            {
                NavigationService.Navigating += OnNavigatingFrom;
                game.Timer.Start();
            }
        }

        private void MyPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from the Navigating event
            if (NavigationService != null)
            {
                NavigationService.Navigating -= OnNavigatingFrom;
                game.Timer.Stop();
            }
        }

        private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            // Check if the current content is GamePage and we're navigating away
            if (NavigationService != null && NavigationService.Content is GamePage && isLoaded)
            {
                isLoaded = false;
                game.Timer.Stop();
            }
            if (e.Content is GamePage)
            {
                isLoaded = true;
                game.Timer.Start();
            }
        }

        public void Disable()
        {
            this.game.Board.Disable();
            this.btn_pause.Visibility = Visibility.Collapsed;
            this.btn_hint.IsEnabled = false;
            this.btn_checkBoard.IsEnabled = false;
            this.timerTxtB.IsEnabled = false;
            this.nameTxtB.IsEnabled = false;
            this.btn_endGame.Visibility = Visibility.Collapsed;
        }

        private void EndGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msbxRes = MessageBox.Show("Do you want to save this game?", "Save Game", MessageBoxButton.YesNoCancel);
            if ( msbxRes == MessageBoxResult.Yes || msbxRes == MessageBoxResult.No)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait; // Change cursor to 
                    game.End(false, msbxRes == MessageBoxResult.Yes);
                    game.Board.ShowSolution();
                }
                finally
                {
                    Mouse.OverrideCursor = null; // Restore cursor
                }
                

            }

            
        }

        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            if (hintsLeft == 1)
            {
                btn_hint.IsEnabled = false;
            }
            Cell focusCell = Board.FocusedCell();
            if (focusCell != null && !focusCell.IsReadOnly)
            {
                Board.FocusedCell().Solve(true);
                RemoveHint();
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            if (this == null)
            {
                NavigationService.Navigate(UriConstants.GAME_SETTINGS_PAGE);
                return;
            }

            if (this != null)
            {
                MessageBoxResult msbxRes = MessageBox.Show("Do you want to save this game?", "Save Game", MessageBoxButton.YesNoCancel);

                if (msbxRes == MessageBoxResult.Yes || msbxRes == MessageBoxResult.No)
                {
                        
                    game.End(false, msbxRes == MessageBoxResult.Yes);
                    NavigationService.Navigate(UriConstants.GAME_SETTINGS_PAGE);
                }
            }


        }

        private void RemoveHint()
        {
            this.hintsLeft--;
            hintsTxtB.Text = hintsLeft.ToString() + GameConstants.REMEINING_STR;
        }

        private void RemoveCheck()
        {
            this.checksLeft--;
            checksTxtB.Text = checksLeft.ToString() + GameConstants.REMEINING_STR;
        }

        private void CopyPuzzleCode_Click(object sender, RoutedEventArgs e)
        {
            //Clipboard.SetText(Puzzle.GetCurrentCode());
            Clipboard.SetText( game.GetPuzzleCode() + "&&&&&&&&&&&" /*+ Board.GenerateBoardCode()*/);
            MessageBox.Show(GameConstants.COPIED_STR);
        }

        private void CheckBoard_Click(object sender, RoutedEventArgs e)
        {
            if (checksLeft == 1)
            {
                btn_checkBoard.IsEnabled = false;
            }

            this.game.Board.CheckMyBoard();

            RemoveCheck();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            game.Timer.Stop();
            MessageBox.Show(GameConstants.PAUSE_STR);
            game.Timer.Start();
        }

        public Game Game => this.game;


    }
}
