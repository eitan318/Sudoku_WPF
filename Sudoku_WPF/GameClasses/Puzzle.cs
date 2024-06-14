using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku_WPF.publico;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    public class Puzzle
    {
        public static bool[,] initialCells;
        public static char[,] solvedPuzzle;
        public string code = "NO CODE YET";
        private static Random rnd = new Random();

        public Puzzle()
        {
            initialCells = new bool[Settings.BOARD_SIDE, Settings.BOARD_SIDE];
            solvedPuzzle = new char[Settings.BOARD_SIDE, Settings.BOARD_SIDE];

            CreatePuzzle();
            code = GeneratePuzzleCode();
        }

        public Puzzle(string code)
        {
            ImportPuzzleCode(code);
            code = GeneratePuzzleCode();
        }

        public bool IsCellInitial(int row, int col)
        {
            return initialCells[row, col];
        }

        public static char CellValueS(int row, int col)
        {
            return solvedPuzzle[row, col];
        }

        public char CellValue(int row, int col)
        {
            return solvedPuzzle[row, col];
        }

        public string GetCode() => code;

        private void CreatePuzzle()
        {
            CreateSolvedPuzzle();
            ChooseInitialCells((int)(Settings.BOARD_SIDE * Settings.BOARD_SIDE * 0.4));
        }

        private void CreateSolvedPuzzle()
        {
            FillBoard();
        }

        private void FillBoard()
        {
            FillRemaining(0, 0);
        }

        private bool FillRemaining(int row, int col)
        {
            if (row == Settings.BOARD_SIDE - 1 && col == Settings.BOARD_SIDE)
                return true;
            if (col == Settings.BOARD_SIDE)
            {
                row++;
                col = 0;
            }
            if (solvedPuzzle[row, col] != '\0')
                return FillRemaining(row, col + 1);

            char[] nums = GetShuffledNumbers();

            foreach (var num in nums)
            {
                if (IsSafe(row, col, num))
                {
                    solvedPuzzle[row, col] = num;
                    if (FillRemaining(row, col + 1))
                        return true;
                    solvedPuzzle[row, col] = '\0';
                }
            }
            return false;
        }

        private char[] GetShuffledNumbers()
        {
            char[] nums = new char[Settings.BOARD_SIDE];
            for (int i = 0; i < Settings.BOARD_SIDE; i++)
            {
                nums[i] = ToHexa(i + 1);
            }
            Shuffle(nums);
            return nums;
        }

        private void Shuffle(char[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + rnd.Next(n - i);
                char temp = array[i];
                array[i] = array[r];
                array[r] = temp;
            }
        }

        private bool IsSafe(int row, int col, char num)
        {
            // Check row
            for (int x = 0; x < Settings.BOARD_SIDE; x++)
                if (solvedPuzzle[row, x] == num)
                    return false;

            // Check column
            for (int x = 0; x < Settings.BOARD_SIDE; x++)
                if (solvedPuzzle[x, col] == num)
                    return false;

            // Check box
            int boxStartRow = row / Settings.BOX_HEIGHT * Settings.BOX_HEIGHT;
            int boxStartCol = col / Settings.BOX_WIDTH * Settings.BOX_WIDTH;

            for (int r = 0; r < Settings.BOX_HEIGHT; r++)
                for (int d = 0; d < Settings.BOX_WIDTH; d++)
                    if (solvedPuzzle[boxStartRow + r, boxStartCol + d] == num)
                        return false;

            return true;
        }

        private char ToHexa(int num)
        {
            if (num < Constants.NUM_DIGITS)
                return (char)('0' + num);
            return (char)('A' + num - Constants.NUM_DIGITS);
        }

        private void ChooseInitialCells(int amountOfInitCells)
        {
            Random rnd = new Random();

            while (amountOfInitCells != 0)
            {
                int i = rnd.Next(Settings.BOARD_SIDE);
                int j = rnd.Next(Settings.BOARD_SIDE);

                if (!initialCells[i, j])
                {
                    amountOfInitCells--;
                    initialCells[i, j] = true;
                }
            }
        }

        private string GeneratePuzzleCode()
        {
            string puzzleCode = $"{Settings.BOX_HEIGHT},{Settings.BOX_WIDTH}:";
            for (int i = 0; i < solvedPuzzle.GetLength(0); i++)
            {
                for (int j = 0; j < solvedPuzzle.GetLength(1); j++)
                {
                    puzzleCode += $"{solvedPuzzle[i, j]},";
                    puzzleCode += initialCells[i, j] ? "V" : "X";
                    puzzleCode += "|";
                }
            }
            return Code.Protect(puzzleCode.Substring(0, puzzleCode.Length-1));
            //return puzzleCode.Substring(0, puzzleCode.Length - 1);
        }


        
        private void ImportPuzzleCode(string puzzleCode)
        {
            puzzleCode = Code.Unprotect(puzzleCode);
            int settingEnd = puzzleCode.IndexOf(":");
            int separator = puzzleCode.IndexOf(",");
            Settings.BOX_HEIGHT = int.Parse(puzzleCode.Substring(0, separator));
            Settings.BOX_WIDTH = int.Parse(puzzleCode.Substring(separator + 1, settingEnd - separator - 1)) ;
            Settings.BOARD_SIDE = Settings.BOX_WIDTH * Settings.BOX_HEIGHT;


            initialCells = new bool[Settings.BOARD_SIDE, Settings.BOARD_SIDE];
            solvedPuzzle = new char[Settings.BOARD_SIDE, Settings.BOARD_SIDE];

            int startIdx = settingEnd + 1;
            for (int i = 0; i < Settings.BOARD_SIDE; i++)
            {
                for (int j = 0; j < Settings.BOARD_SIDE; j++)
                {
                    separator = puzzleCode.IndexOf(',', startIdx + 1);
                    solvedPuzzle[i, j] = puzzleCode[startIdx];
                    initialCells[i, j] = puzzleCode[separator + 1] == 'V';
                    startIdx = puzzleCode.IndexOf('|', startIdx + 1) + 1;
                }
            }
        }

    }
}
