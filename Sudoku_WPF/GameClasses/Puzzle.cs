using System;
using Sudoku_WPF.publico;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    /// <summary>
    /// Represents a Sudoku puzzle and provides methods for puzzle generation, management, and access.
    /// </summary>
    public class Puzzle
    {
        public static bool[,] initialCells; // Array to track which cells are initial (pre-filled)
        public static char[,] solvedPuzzle; // Array to store the solved puzzle
        public string code = "NO CODE YET"; // Code representing the puzzle state
        private static Random rnd = new Random(); // Random number generator for shuffling and selecting initial cells

        /// <summary>
        /// Constructor to create a new Sudoku puzzle instance with a random arrangement of initial cells.
        /// </summary>
        public Puzzle()
        {
            initialCells = new bool[GameSettings.BoardSide, GameSettings.BoardSide];
            solvedPuzzle = new char[GameSettings.BoardSide, GameSettings.BoardSide];

            SetFullCells();
            this.code = CreatePuzzleCode();
        }

        /// <summary>
        /// Constructor to import a Sudoku puzzle from a given code representation.
        /// </summary>
        /// <param name="code">The code representing the puzzle state.</param>
        public Puzzle(string code)
        {
            ImportPuzzleCode(code);
            this.code = CreatePuzzleCode();
        }

        /// <summary>
        /// Checks if a specified cell is initially filled (pre-filled) in the puzzle.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>True if the cell is an initial cell; otherwise, false.</returns>
        public bool IsCellInitial(int row, int col)
        {
            return initialCells[row, col];
        }

        /// <summary>
        /// Retrieves the value of a specified cell in the solved puzzle.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>The character value at the specified cell position in the solved puzzle.</returns>
        public static char CellValueS(int row, int col)
        {
            return solvedPuzzle[row, col];
        }

        /// <summary>
        /// Retrieves the value of a specified cell in the solved puzzle.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>The character value at the specified cell position in the solved puzzle.</returns>
        public char CellValue(int row, int col)
        {
            return solvedPuzzle[row, col];
        }

        /// <summary>
        /// Retrieves the code representing the current state of the puzzle.
        /// </summary>
        /// <returns>The code representing the puzzle state as a string.</returns>
        public string GetCode() => code;

        /// <summary>
        /// Generates a new Sudoku puzzle based on the current game settings and difficulty level.
        /// </summary>
        private void SetFullCells()
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
            GenerateSolvedPuzzle();
            SelectInitialCells((int)(GameSettings.BoardSide * GameSettings.BoardSide * fullCellsPercent));
        }

        /// <summary>
        /// Generates a solved Sudoku puzzle based on the current state of the board.
        /// </summary>
        private void GenerateSolvedPuzzle()
        {
            FillCells(0, 0);
        }


        /// <summary>
        /// Recursively fills the remaining cells in the board to complete the Sudoku puzzle.
        /// </summary>
        /// <param name="row">The current row index for filling cells.</param>
        /// <param name="col">The current column index for filling cells.</param>
        /// <returns>True if the board is successfully filled, otherwise false.</returns>
        private bool FillCells(int row, int col)
        {
            if (row == GameSettings.BoardSide - 1 && col == GameSettings.BoardSide)
                return true;
            if (col == GameSettings.BoardSide)
            {
                row++;
                col = 0;
            }
            if (solvedPuzzle[row, col] != '\0')
                return FillCells(row, col + 1);

            char[] nums = GenerateShuffledNumbers();

            foreach (char num in nums)
            {
                if (IsSafeToPlace(row, col, num))
                {
                    solvedPuzzle[row, col] = num;
                    if (FillCells(row, col + 1))
                        return true;
                    solvedPuzzle[row, col] = '\0';
                }
            }
            return false;
        }

        /// <summary>
        /// Generates an array of characters (numbers or letters) shuffled randomly.
        /// </summary>
        /// <returns>An array of characters representing shuffled numbers or letters.</returns>
        private char[] GenerateShuffledNumbers()
        {
            char[] nums = new char[GameSettings.BoardSide];
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                nums[i] = ConvertToHexa(i + 1);
            }
            ShuffleArray(nums);
            return nums;
        }

        /// <summary>
        /// Shuffles an array of characters in place using a simple random swap technique.
        /// </summary>
        /// <param name="array">The array of characters to be shuffled.</param>
        private void ShuffleArray(char[] array)
        {
            int arrayLength = array.Length;
            for (int currentIndex = 0; currentIndex < arrayLength; currentIndex++)
            {
                int randomIndex = rnd.Next(arrayLength);
                char temporary = array[currentIndex];
                array[currentIndex] = array[randomIndex];
                array[randomIndex] = temporary;
            }
        }


        /// <summary>
        /// Checks if it's safe to place a number in the specified cell based on Sudoku rules.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <param name="num">The number (character) to be placed in the cell.</param>
        /// <returns>True if placing the number is safe, otherwise false.</returns>
        private bool IsSafeToPlace(int row, int col, char num)
        {
            // Check row
            for (int colC = 0; colC < GameSettings.BoardSide; colC++)
                if (solvedPuzzle[row, colC] == num)
                    return false;

            // Check column
            for (int rowC = 0; rowC < GameSettings.BoardSide; rowC++)
                if (solvedPuzzle[rowC, col] == num)
                    return false;

            // Check box
            int boxStartRow = (row / GameSettings.BoxHeight) * GameSettings.BoxHeight;
            int boxStartCol = (col / GameSettings.BoxWidth) * GameSettings.BoxWidth;

            for (int rowC = 0; rowC < GameSettings.BoxHeight; rowC++)
                for (int colC = 0; colC < GameSettings.BoxWidth; colC++)
                    if (solvedPuzzle[boxStartRow + rowC, boxStartCol + colC] == num)
                        return false;

            return true;
        }

        /// <summary>
        /// Converts a number (1-15) to its corresponding hexadecimal character ('0'-'F').
        /// </summary>
        /// <param name="num">The number to be converted.</param>
        /// <returns>The corresponding hexadecimal character.</returns>
        private char ConvertToHexa(int num)
        {
            if (num < Constants.NUM_DIGITS)
                return (char)('0' + num);
            return (char)('A' + num - Constants.NUM_DIGITS);
        }

        /// <summary>
        /// Randomly selects a specified number of initial cells to be pre-filled in the puzzle.
        /// </summary>
        /// <param name="amountOfInitCells">The number of initial cells to select.</param>
        private void SelectInitialCells(int amountOfInitCells)
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

        /// <summary>
        /// Generates a code representing the current state of the puzzle.
        /// </summary>
        /// <returns>A string representing the puzzle state code.</returns>
        private string CreatePuzzleCode()
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

        /// <summary>
        /// Imports a puzzle code and initializes game settings, initial cells, and solved puzzle matrix.
        /// </summary>
        /// <param name="puzzleCode">The puzzle code to import.</param>
        private void ImportPuzzleCode(string puzzleCode)
        {
            // Unprotect puzzle code if necessary
            puzzleCode = Code.Unprotect(puzzleCode);

            // Determine indices for parsing settings
            int settingEnd = puzzleCode.IndexOf(":");
            int separator = puzzleCode.IndexOf(",");

            // Parse game settings
            GameSettings.BoxHeight = int.Parse(puzzleCode.Substring(0, separator));
            GameSettings.BoxWidth = int.Parse(puzzleCode.Substring(separator + 1, settingEnd - separator - 1));
            GameSettings.BoardSide = GameSettings.BoxWidth * GameSettings.BoxHeight;

            // Initialize arrays for initial cells and solved puzzle
            initialCells = new bool[GameSettings.BoardSide, GameSettings.BoardSide];
            solvedPuzzle = new char[GameSettings.BoardSide, GameSettings.BoardSide];

            // Parse initial cells and solved puzzle from puzzle code
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
