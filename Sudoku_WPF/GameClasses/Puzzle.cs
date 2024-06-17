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
        public static bool[,] initialCells; // Array to track which cells are initial (pre-filled)
        public static char[,] solvedPuzzle; // Array to store the solved puzzle
        public string code = "NO CODE YET"; // Code representing the puzzle state
        private static Random rnd = new Random(); // Random number generator for shuffling and selecting initial cells

        // Constructor to create a new puzzle
        public Puzzle()
        {
            initialCells = new bool[GameSettings.BoardSide, GameSettings.BoardSide];
            solvedPuzzle = new char[GameSettings.BoardSide, GameSettings.BoardSide];

            CreatePuzzle();
            this.code = GeneratePuzzleCode();
        }

        // Constructor to import a puzzle from a given code
        public Puzzle(string code)
        {
            ImportPuzzleCode(code);
            this.code = GeneratePuzzleCode();
        }

        // Check if a cell is an initial cell
        public bool IsCellInitial(int row, int col)
        {
            return initialCells[row, col];
        }

        // Static method to get the value of a cell in the solved puzzle
        public static char CellValueS(int row, int col)
        {
            return solvedPuzzle[row, col];
        }

        // Instance method to get the value of a cell in the solved puzzle
        public char CellValue(int row, int col)
        {
            return solvedPuzzle[row, col];
        }

        // Get the code representing the puzzle
        public string GetCode() => code;

        // Create a new puzzle
        private void CreatePuzzle()
        {
            double fullCellsPercent;

            switch (GameSettings.dificultyLevel)
            {
                case DificultyLevel.Easy:
                    fullCellsPercent = GameSettingsConstants.FULL_CELLS_EASY;
                    break;
                case DificultyLevel.Medium:
                    fullCellsPercent = GameSettingsConstants.FULL_CELLS_MEDIUM;
                    break;
                case DificultyLevel.Hard:
                    fullCellsPercent = GameSettingsConstants.FULL_CELLS_HARD;
                    break;
                default:
                    fullCellsPercent = 0;
                    break;
            }
            CreateSolvedPuzzle();
            ChooseInitialCells((int)(GameSettings.BoardSide * GameSettings.BoardSide * fullCellsPercent));
        }

        // Create a solved puzzle
        private void CreateSolvedPuzzle()
        {
            FillBoard();
        }

        // Fill the board with a valid Sudoku solution
        private void FillBoard()
        {
            FillRemaining(0, 0);
        }

        // Recursively fill the remaining cells in the board
        private bool FillRemaining(int row, int col)
        {
            if (row == GameSettings.BoardSide - 1 && col == GameSettings.BoardSide)
                return true;
            if (col == GameSettings.BoardSide)
            {
                row++;
                col = 0;
            }
            if (solvedPuzzle[row, col] != '\0')
                return FillRemaining(row, col + 1);

            char[] nums = GetShuffledNumbers();

            foreach (char num in nums)
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

        // Get an array of numbers (or letters) shuffled randomly
        private char[] GetShuffledNumbers()
        {
            char[] nums = new char[GameSettings.BoardSide];
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                nums[i] = ToHexa(i + 1);
            }
            Shuffle(nums);
            return nums;
        }

        // Shuffle an array of characters
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

        // Check if it's safe to place a number in the specified cell
        private bool IsSafe(int row, int col, char num)
        {
            // Check row
            for (int x = 0; x < GameSettings.BoardSide; x++)
                if (solvedPuzzle[row, x] == num)
                    return false;

            // Check column
            for (int x = 0; x < GameSettings.BoardSide; x++)
                if (solvedPuzzle[x, col] == num)
                    return false;

            // Check box
            int boxStartRow = row / GameSettings.BoxHeight * GameSettings.BoxHeight;
            int boxStartCol = col / GameSettings.BoxWidth * GameSettings.BoxWidth;

            for (int r = 0; r < GameSettings.BoxHeight; r++)
                for (int d = 0; d < GameSettings.BoxWidth; d++)
                    if (solvedPuzzle[boxStartRow + r, boxStartCol + d] == num)
                        return false;

            return true;
        }

        // Convert a number to its corresponding character in hexadecimal format
        private char ToHexa(int num)
        {
            if (num < Constants.NUM_DIGITS)
                return (char)('0' + num);
            return (char)('A' + num - Constants.NUM_DIGITS);
        }

        // Choose a specified number of initial cells to be pre-filled
        private void ChooseInitialCells(int amountOfInitCells)
        {
            Random rnd = new Random();

            while (amountOfInitCells != 0)
            {
                int i = rnd.Next(GameSettings.BoardSide);
                int j = rnd.Next(GameSettings.BoardSide);

                if (!initialCells[i, j])
                {
                    amountOfInitCells--;
                    initialCells[i, j] = true;
                }
            }
        }

        // Generate a code representing the current state of the puzzle
        private string GeneratePuzzleCode()
        {
            string puzzleCode = $"{GameSettings.BoxHeight},{GameSettings.BoxWidth}:";
            for (int i = 0; i < solvedPuzzle.GetLength(0); i++)
            {
                for (int j = 0; j < solvedPuzzle.GetLength(1); j++)
                {
                    puzzleCode += $"{solvedPuzzle[i, j]},";
                    puzzleCode += initialCells[i, j] ? "V" : "X";
                    puzzleCode += "|";
                }
            }
            return Code.Protect(puzzleCode.Substring(0, puzzleCode.Length - 1));
        }

        // Import a puzzle from a given code
        private void ImportPuzzleCode(string puzzleCode)
        {
            puzzleCode = Code.Unprotect(puzzleCode);
            int settingEnd = puzzleCode.IndexOf(":");
            int separator = puzzleCode.IndexOf(",");
            GameSettings.BoxHeight = int.Parse(puzzleCode.Substring(0, separator));
            GameSettings.BoxWidth = int.Parse(puzzleCode.Substring(separator + 1, settingEnd - separator - 1));
            GameSettings.BoardSide = GameSettings.BoxWidth * GameSettings.BoxHeight;

            initialCells = new bool[GameSettings.BoardSide, GameSettings.BoardSide];
            solvedPuzzle = new char[GameSettings.BoardSide, GameSettings.BoardSide];

            int startIdx = settingEnd + 1;
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                for (int j = 0; j < GameSettings.BoardSide; j++)
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
