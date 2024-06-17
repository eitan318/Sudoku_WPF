using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    /// <summary>
    /// Represents a Sudoku game session.
    /// </summary>
    public class Game
    {
        private Board board;
        private TextBlock timerTxtB;
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private Puzzle puzzle;
        private bool inProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class for starting a new Sudoku game.
        /// </summary>
        /// <param name="sodukoGrid">The grid where the Sudoku game is displayed.</param>
        /// <param name="timerTxtB">The text block for displaying the game timer.</param>
        public Game(Grid sodukoGrid, TextBlock timerTxtB)
        {
            this.timerTxtB = timerTxtB;
            InitTimer();
            this.inProgress = true;
            timerTxtB.Text = TimerConstants.DEFAULT_TIME;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait; // Change cursor to Wait
                puzzle = new Puzzle();
                board = new Board(sodukoGrid, puzzle);
            }
            finally
            {
                Mouse.OverrideCursor = null; // Restore cursor
            }
            Board.GameEnded += OnGameSolved;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class for starting a new Sudoku game with a specific puzzle code.
        /// </summary>
        /// <param name="sodukoGrid">The grid where the Sudoku game is displayed.</param>
        /// <param name="timerTxtB">The text block for displaying the game timer.</param>
        /// <param name="puzzleCode">The puzzle code to initialize the Sudoku game.</param>
        public Game(Grid sodukoGrid, TextBlock timerTxtB, string puzzleCode)
        {
            this.timerTxtB = timerTxtB;
            InitTimer();
            this.inProgress = true;
            timerTxtB.Text = TimerConstants.DEFAULT_TIME;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait; // Change cursor to Wait
                puzzle = new Puzzle(puzzleCode);
                board = new Board(sodukoGrid, puzzle);
            }
            finally
            {
                Mouse.OverrideCursor = null; // Restore cursor
            }
            Board.GameEnded += OnGameSolved;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class for starting a new Sudoku game from saved game information.
        /// </summary>
        /// <param name="sodukoGrid">The grid where the Sudoku game is displayed.</param>
        /// <param name="timerTxtB">The text block for displaying the game timer.</param>
        /// <param name="info">The saved game information to initialize the Sudoku game.</param>
        public Game(Grid sodukoGrid, TextBlock timerTxtB, GameInfo info)
        {
            this.timerTxtB = timerTxtB;
            InitTimer(info.Time);
            this.inProgress = true;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait; // Change cursor to Wait
                puzzle = new Puzzle(info.PuzzleCode);
                board = new Board(sodukoGrid, puzzle, info.BoardCode);
            }
            finally
            {
                Mouse.OverrideCursor = null; // Restore cursor
            }

            timerTxtB.Text = info.Time;
            Board.GameEnded += OnGameSolved;
        }

        /// <summary>
        /// Handles the event when the Sudoku game is solved.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameSolved(object sender, EventArgs e)
        {
            End(true, true);
        }

        /// <summary>
        /// Ends the current Sudoku game session.
        /// </summary>
        /// <param name="isSolved">Indicates whether the game is solved.</param>
        /// <param name="toSave">Indicates whether to save the game.</param>
        public void End(bool isSolved, bool toSave)
        {
            this.timer.Stop();
            this.inProgress = false;

            var window = (MainWindow)Application.Current.MainWindow;

            if (window.gamePage != null)// wasnt already ended
            {
                if (toSave)
                {
                    GameInfo gameInfo = window.gamePage.GetGameInfo(isSolved, false);
                    if (isSolved)
                    {
                        window.historyPage.AddItemToListAndDB(gameInfo);
                    }
                    else
                    {
                        window.savedPage.AddItemToListAndDB(gameInfo);
                    }
                }

                window.gamePage.DisableGameControls();
                window.gamePage = null;
            }

        }

        /// <summary>
        /// Checks if a Sudoku game is currently in progress.
        /// </summary>
        /// <returns>True if a game is in progress; otherwise, false.</returns>
        public bool IsInGame()
        {
            return inProgress;
        }

        /// <summary>
        /// Retrieves the puzzle code associated with the current Sudoku game.
        /// </summary>
        /// <returns>The puzzle code string.</returns>
        public string GetPuzzleCode()
        {
            return puzzle.GetCode();
        }

        /// <summary>
        /// Initializes the game timer.
        /// </summary>
        private void InitTimer()
        {
            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Tick every second
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Initializes the game timer with a specified starting time.
        /// </summary>
        /// <param name="startTime">The starting time for the game timer.</param>
        private void InitTimer(string startTime)
        {
            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Tick every second
            timer.Tick += Timer_Tick;

            // Parse the string and convert it to a TimeSpan
            if (TimeSpan.TryParse(startTime, out TimeSpan parsedTime))
            {
                elapsedTime = parsedTime;
            }
        }

        /// <summary>
        /// Event handler for the game timer tick event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the elapsed time and display it
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            timerTxtB.Text = GetTime();
        }

        /// <summary>
        /// Retrieves the formatted string representation of the elapsed game time.
        /// </summary>
        /// <returns>The formatted game time string.</returns>
        public string GetTime()
        {
            return elapsedTime.ToString(TimerConstants.FORMAT);
        }

        /// <summary>
        /// Accessor for retrieving the Sudoku game board.
        /// </summary>
        public Board Board => board;

        /// <summary>
        /// Accessor for retrieving the game timer.
        /// </summary>
        public DispatcherTimer Timer => timer;
    }
}
