using static Sudoku_WPF.publico.Constants;
using Sudoku_WPF.GameClasses;
using System.Windows.Controls;
using System.Windows.Threading;


namespace Sudoku_WPF.GameClasses
{
    public class GameInfo
    {
        public string BoardCode { get; set; }
        public string PuzzleCode { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public int Hints { get; set; }
        public int Checks { get; set; }
        public string Date { get; set; }
        public bool Solved { get; set; }
        public bool Current { get; set; }
        public int Id { get; set; }

        public GameInfo(int id, string name, string boardCode, string puzzleCode, string time, int hints, int checks, string date, bool solved, bool current)
        {
            Name = name;
            BoardCode = boardCode;
            PuzzleCode = puzzleCode;
            Time = time;
            Hints = hints;
            Checks = checks;
            Date = date;
            Solved = solved;
            Current = current;
            Id = id;
        }
    }
}

