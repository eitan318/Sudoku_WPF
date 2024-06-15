using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    public class Game
    {
        private Board board;
        private TextBlock timerTxtB;
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private Puzzle puzzle;
        //private bool inProgress;

        public Game(Grid sodukoGrid, TextBlock timerTxtB)
        {
            
            this.timerTxtB = timerTxtB;
            initTimer();
            //this.inProgress = true;
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
        }

        public Game(Grid sodukoGrid, TextBlock timerTxtB, string puzzleCode)
        {
            this.timerTxtB = timerTxtB;
            initTimer();
            //this.inProgress = true;
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
        }

        public Game(Grid sodukoGrid, TextBlock timerTxtB, GameInfo info)
        {
            this.timerTxtB = timerTxtB;
            initTimer(info.Time);
            //this.inProgress = true;
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

        private void OnGameSolved(object sender, EventArgs e)
        {
            End(true, true);
        }


        public void End(bool isSolved, bool toSave)
        {
            this.timer.Stop();
            //this.inProgress = false;

            var window = (MainWindow)Application.Current.MainWindow;

            if(window.gamePage != null)// wasnt already ended
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

                window.gamePage.Disable();
                window.gamePage = null;
            }
            
        }

        /*public bool IsInGame()
        {
            return inProgress;
        }*/
        public string GetPuzzleCode()
        {
            return puzzle.GetCode();
        }

        private void initTimer()
        {
            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Tick every second
            timer.Tick += Timer_Tick;
        }

        private void initTimer(string startTime)
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the elapsed time and display it
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            timerTxtB.Text = GetTime();
        }

        public string GetTime()
        {
            return elapsedTime.ToString(TimerConstants.FORMAT);
        }

        // Accessor for the specific game items
        public Board Board => board;

        public DispatcherTimer Timer => timer;
    }
}
