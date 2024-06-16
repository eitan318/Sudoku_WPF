using static Sudoku_WPF.publico.Constants;
using Sudoku_WPF.GameClasses;
using System.Windows.Controls;
using System.Windows.Threading;
using Sudoku_WPF.publico;


namespace Sudoku_WPF.GameClasses
{
    public class GameInfo
    {
        public string BoardCode { get; set; }
        public int BoxHeight { get; set; }
        public int BoxWidth { get; set; }
        public string PuzzleCode { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public int Hints { get; set; }
        public int Checks { get; set; }
        public string Date { get; set; }
        public bool Solved { get; set; }
        public bool Current { get; set; }
        public DificultyLevel DifficultyLevel { get; set; }
        
        public int Id { get; set; }

        public GameInfo(int id, string name, string boardCode, string puzzleCode, string time, int hintsTaken, int checksTaken, string date, DificultyLevel difficultyLevel, bool solved, bool current, int boxHeight, int boxWidth)
        {
            Name = name;
            BoardCode = boardCode;
            PuzzleCode = puzzleCode;
            Time = time;
            Hints = hintsTaken;
            Checks = checksTaken;
            Date = date;
            Solved = solved;
            Current = current;
            Id = id;
            BoxHeight = boxHeight;
            BoxWidth = boxWidth;
            DifficultyLevel = difficultyLevel;
        }
    }
}

