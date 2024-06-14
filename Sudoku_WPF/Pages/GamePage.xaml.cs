using System;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Sudoku_WPF.GameClasses;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private Game game;
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
            }
        }

        private void MyPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from the Navigating event
            if (NavigationService != null)
            {
                NavigationService.Navigating -= OnNavigatingFrom;
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
            if ( msbxRes == MessageBoxResult.Yes)
            {
                game.Board.ShowSolution();
                game.End();
                this.Disable();

                

            }
            else if (msbxRes == MessageBoxResult.No)
            {
                game.Board.ShowSolution();
                game.End();
                this.Disable();
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
            if(!game.IsInGame())
            {
                game.End();

                NavigationService.Navigate(UriConstants.GAME_SETTINGS_PAGE);
                return;
            }

            MessageBoxResult msbxRes = MessageBox.Show("Do you want to save this game?", "Save Game", MessageBoxButton.YesNoCancel);
            
            if (msbxRes == MessageBoxResult.Yes )
            {
                
                game.End();
                NavigationService.Navigate(UriConstants.GAME_SETTINGS_PAGE);

            }
            else if (msbxRes == MessageBoxResult.No)
            {
                game.End();
                NavigationService.Navigate(UriConstants.GAME_SETTINGS_PAGE);
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
            Clipboard.SetText( Puzzle.GetCurrentCode() + "&&&&&&&&&&&" + Board.GenerateBoardCode());
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
