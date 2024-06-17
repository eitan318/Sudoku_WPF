using Sudoku_WPF.publico;
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

        /// <summary>
        /// Constructor for initializing GamePage with a specific puzzle code.
        /// </summary>
        /// <param name="puzzleCode">The puzzle code to initialize the game with.</param>
        public GamePage(string puzzleCode)
        {
            InitializeComponent();
            Init();
            game = new Game(SudokuGrid, timerTxtB, puzzleCode);
        }

        /// <summary>
        /// Constructor for initializing GamePage with game information.
        /// </summary>
        /// <param name="gameInfo">The game information to initialize the game with.</param>
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

        /// <summary>
        /// Retrieves the current game information.
        /// </summary>
        /// <param name="solved">Whether the game is solved or not.</param>
        /// <param name="current">Whether the game is the current game or not.</param>
        /// <returns>The GameInfo object representing the current game state.</returns>
        public GameInfo GetGameInfo(bool solved, bool current)
        {
            int Id = DBHelper.GetNextId("tbl_games", DBConstants.Games_Parameters.Id);
            nameTxtB.Text = string.IsNullOrEmpty(nameTxtB.Text) ? "Board" + Id.ToString() : nameTxtB.Text;

            GameInfo gameInfo = new GameInfo(
                 Id,
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

        /// <summary>
        /// Initializes the GamePage by setting up initial values and event handlers.
        /// </summary>
        private void Init()
        {
            this.hintsLeft = GameConstants.HINTS;
            this.checksLeft = GameConstants.CHECKS;

            hintsTxtB.Text = hintsLeft.ToString() + GameConstants.REMEINING_STR;
            checksTxtB.Text = checksLeft.ToString() + GameConstants.REMEINING_STR;

            this.Loaded += MyPage_Loaded;
            this.Unloaded += MyPage_Unloaded;
        }

        /// <summary>
        /// Event handler when the GamePage is loaded into the visual tree.
        /// Starts the game timer and subscribes to navigation events.
        /// </summary>
        private void MyPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to the Navigating event
            if (NavigationService != null)
            {
                NavigationService.Navigating += OnNavigatingFrom;
                game.Timer.Start();
            }
        }

        /// <summary>
        /// Event handler when the GamePage is unloaded from the visual tree.
        /// Stops the game timer and unsubscribes from navigation events.
        /// </summary>
        private void MyPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from the Navigating event
            if (NavigationService != null)
            {
                NavigationService.Navigating -= OnNavigatingFrom;
                game.Timer.Stop();
            }
        }

        /// <summary>
        /// Event handler for navigating away from the GamePage.
        /// Stops the game timer based on navigation actions.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Disables the game controls and stops certain functionalities.
        /// </summary>
        public void DisableGameControls()
        {
            this.game.Board.Disable();
            this.btn_pause.Visibility = Visibility.Collapsed;
            this.btn_hint.IsEnabled = false;
            this.btn_checkBoard.IsEnabled = false;
            this.timerTxtB.IsEnabled = false;
            this.nameTxtB.IsEnabled = false;
            this.btn_endGame.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for ending the game.
        /// Asks user to save the game and shows the solution if chosen.
        /// </summary>
        private void EndGame_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            MessageBoxResult msbxRes = MessageBox.Show("Do you want to save this game?", "Save Game", MessageBoxButton.YesNoCancel);
            if (msbxRes == MessageBoxResult.Yes || msbxRes == MessageBoxResult.No)
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

        /// <summary>
        /// Event handler for using a hint during the game.
        /// Plays a sound and applies a hint to the focused cell.
        /// </summary>
        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            if (hintsLeft == 1)
            {
                btn_hint.IsEnabled = false;
            }
            Cell focusCell = Board.FocusedCell();
            if (focusCell != null && !focusCell.IsReadOnly)
            {
                SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);
                Board.FocusedCell().Solve(true);
                RemoveHint();
            }
            else
            {
                SoundPlayer.PlaySound(SoundConstants.WRONG);
            }
        }

        /// <summary>
        /// Event handler for starting a new game.
        /// Asks to save the current game and navigates to game settings.
        /// </summary>
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            if (this.game.IsInGame())
            {
                MessageBoxResult msbxRes = MessageBox.Show("Do you want to save this game?", "Save Game", MessageBoxButton.YesNoCancel);

                if (msbxRes == MessageBoxResult.Yes || msbxRes == MessageBoxResult.No)
                {
                    game.End(false, msbxRes == MessageBoxResult.Yes);
                    NavigationService.Navigate(UriConstants.GAME_SETTINGS_PAGE);
                }
            }
            else
            {
                NavigationService.Navigate(UriConstants.GAME_SETTINGS_PAGE);
                return;
            }
        }

        /// <summary>
        /// Removes a hint and updates the remaining hints count.
        /// </summary>
        private void RemoveHint()
        {
            this.hintsLeft--;
            hintsTxtB.Text = hintsLeft.ToString() + GameConstants.REMEINING_STR;
        }

        /// <summary>
        /// Removes a check and updates the remaining checks count.
        /// </summary>
        private void RemoveCheck()
        {
            this.checksLeft--;
            checksTxtB.Text = checksLeft.ToString() + GameConstants.REMEINING_STR;
        }

        /// <summary>
        /// Copies the current puzzle code to the clipboard.
        /// </summary>
        private void CopyPuzzleCode_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            Clipboard.SetText(game.GetPuzzleCode());
            MessageBox.Show(GameConstants.COPIED_STR);
        }

        /// <summary>
        /// Limits the text length in a TextBox to 20 characters.
        /// </summary>
        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // If the length exceeds the limit, truncate the text
            if (textBox.Text.Length > 20)
            {
                textBox.Text = textBox.Text.Substring(0, 20);

                // Set the caret to the end of the truncated text
                textBox.CaretIndex = textBox.Text.Length;
            }

            if (textBox == nameTxtB)
            {
                nameTextPlaceholder.Visibility = string.IsNullOrWhiteSpace(nameTxtB.Text) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>
        /// Event handler for checking the current board configuration.
        /// Plays a sound and checks the board with the remaining checks count.
        /// </summary>
        private void CheckBoard_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            if (checksLeft == 1)
            {
                btn_checkBoard.IsEnabled = false;
            }

            this.game.Board.CheckMyBoard();

            RemoveCheck();
        }


        /// <summary>
        /// Handles the toggled state of the pause button, pausing and resuming the game.
        /// </summary>
        private void Pause_Checked(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.ON_OFF);

            game.Timer.Stop();
            DisablePauseControls();
        }

        private void Pause_Unchecked(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.ON_OFF);
            game.Timer.Start();
            EnablePauseControls();
        }

        /// <summary>
        /// Disables all game controls when the game is paused.
        /// </summary>
        private void DisablePauseControls()
        {
            SudokuGrid.IsEnabled = false;
            btn_showPuzzleCode.IsEnabled = false;
            btn_newGame.IsEnabled = false;
            btn_checkBoard.IsEnabled = false;
            btn_hint.IsEnabled = false;
            btn_endGame.IsEnabled = false;
            nameTxtB.IsEnabled = false;
        }

        /// <summary>
        /// Enables all game controls when the game is resumed.
        /// </summary>
        private void EnablePauseControls()
        {
            SudokuGrid.IsEnabled = true;
            btn_showPuzzleCode.IsEnabled = true;
            btn_newGame.IsEnabled = true;
            btn_checkBoard.IsEnabled = true;
            btn_hint.IsEnabled = true;
            btn_endGame.IsEnabled = true;
            nameTxtB.IsEnabled = true;
        }

        /// <summary>
        /// Property to access the Game instance associated with this GamePage.
        /// </summary>
        public Game Game => this.game;
    }
}
