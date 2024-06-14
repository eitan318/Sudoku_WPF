using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    public class Game
    {
        private Board board;
        private TextBlock timerTxtB;
        private static DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private Puzzle puzzle;
        private bool inProgress;

        public Game(Grid sodukoGrid, TextBlock timerTxtB)
        {
            this.timerTxtB = timerTxtB;
            initTimer();
            timer.Start();
            this.inProgress = true;
            timerTxtB.Text = TimerConstants.DEFAULT_TIME;
            puzzle = new Puzzle();
            board = new Board(sodukoGrid, puzzle);
        }

        public Game(Grid sodukoGrid, TextBlock timerTxtB, string puzzleCode)
        {
            this.timerTxtB = timerTxtB;
            initTimer();
            timer.Start();
            this.inProgress = true;
            timerTxtB.Text = TimerConstants.DEFAULT_TIME;
            puzzle = new Puzzle(puzzleCode);
            board = new Board(sodukoGrid, puzzle);
        }

        public Game(Grid sodukoGrid, TextBlock timerTxtB, GameInfo info)
        {
            this.timerTxtB = timerTxtB;
            initTimer(info.Time);
            timer.Start();
            this.inProgress = true;
            puzzle = new Puzzle(info.PuzzleCode);
            board = new Board(sodukoGrid, puzzle, info.BoardCode);
            timerTxtB.Text = info.Time;
        }


        public void End()
        {
            this.Timer.Stop();
            this.inProgress = false;

            var window = (MainWindow)Application.Current.MainWindow;
            window.gamePage.Disable();
            window.gamePage = null;
            window.Settings_btn.Visibility = Visibility.Visible;
        }

        public bool IsInGame()
        {
            return inProgress;
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

        private string GetTime()
        {
            return elapsedTime.ToString(TimerConstants.FORMAT);
        }

        // Accessor for the specific game items
        public Board Board => board;

        public DispatcherTimer Timer => timer;
    }
}
