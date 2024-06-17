using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sudoku_WPF.publico;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    /// <summary>
    /// Represents the Sudoku board consisting of cells and handles game logic.
    /// </summary>
    public class Board
    {
        private static Cell[,] cells; // 2D array of cells representing the Sudoku board
        private Puzzle puzzle; // Puzzle instance associated with the board
        public Grid sudokuGrid; // Grid that contains the Sudoku board UI
        private static bool solutionShown; // Flag indicating if the solution is shown

        public static event EventHandler GameEnded; // Event raised when the game ends

        /// <summary>
        /// Constructor for initializing the board with a given Sudoku grid and puzzle.
        /// </summary>
        /// <param name="sudokuGrid">Grid representing the Sudoku board UI.</param>
        /// <param name="puzzle">Puzzle instance containing the Sudoku puzzle data.</param>
        public Board(Grid sudokuGrid, Puzzle puzzle)
        {
            this.sudokuGrid = sudokuGrid;
            cells = new Cell[GameSettings.BoardSide, GameSettings.BoardSide];
            this.puzzle = puzzle;

            CreateSudokuGrid(sudokuGrid);
            Initialize();
            sudokuGrid.SizeChanged += OnGridSizeChanged;

            solutionShown = false;
        }

        /// <summary>
        /// Constructor for initializing the board with a given Sudoku grid, puzzle, and board code.
        /// </summary>
        /// <param name="sudokuGrid">Grid representing the Sudoku board UI.</param>
        /// <param name="puzzle">Puzzle instance containing the Sudoku puzzle data.</param>
        /// <param name="boardCode">Encrypted board code string to initialize board state.</param>
        public Board(Grid sudokuGrid, Puzzle puzzle, string boardCode)
        {
            this.sudokuGrid = sudokuGrid;
            cells = new Cell[GameSettings.BoardSide, GameSettings.BoardSide];
            this.puzzle = puzzle;

            CreateSudokuGrid(sudokuGrid);
            Initialize(boardCode);
            sudokuGrid.SizeChanged += OnGridSizeChanged;

            solutionShown = false;
        }

        /// <summary>
        /// Handles the visual state and validation of the board when a cell's text changes.
        /// </summary>
        /// <param name="focusCell">The cell currently in focus.</param>
        /// <param name="validateForeground">Flag indicating if foreground validation is required.</param>
        /// <param name="markText">Optional text to mark in the cells.</param>
        public static void VisualizeState(Cell focusCell, bool validateForeground, string markText = "")
        {
            if (solutionShown)
                return;
            if (markText == "")
                markText = focusCell.Text;
            if (validateForeground)
                focusCell.SetResourceReference(Control.ForegroundProperty, "Text");

            foreach (Cell cell in cells)
            {
                if (cell != focusCell)
                {
                    cell.ColorBy(focusCell, markText, validateForeground);
                }
            }

            focusCell.SetResourceReference(Control.BackgroundProperty, "Tbx_Focus");
        }

        /// <summary>
        /// Checks if the Sudoku board is fully solved.
        /// </summary>
        /// <returns>True if the board is fully solved, otherwise false.</returns>
        private static bool IsSolved()
        {
            foreach (Cell cell in cells)
            {
                if (cell.Text == "" || cell.Foreground == (Brush)Application.Current.FindResource("Tbx_WrongForeground") || cell.Background == (Brush)Application.Current.FindResource("Tbx_WrongBackground"))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Shows the solved animation and triggers the game ended event.
        /// </summary>
        private static void ShowSolvedAnimation()
        {
            SoundPlayer.PlaySound(SoundConstants.SOLVED);

            foreach (Cell cell in cells)
            {
                cell.SetResourceReference(Control.BackgroundProperty, "Tbx_RightBackground");
                cell.IsReadOnly = true;
            }

            GameEnded?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes the Sudoku board by setting up cells based on the puzzle.
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                for (int j = 0; j < GameSettings.BoardSide; j++)
                {
                    Cell cell = cells[i, j];
                    if (puzzle.IsCellInitial(i, j))
                    {
                        cell.Text = puzzle.CellValue(i, j).ToString();
                        cell.IsReadOnly = true;
                        cell.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        cell.IsReadOnly = false;
                        cell.Text = "";
                    }

                    // Set visual properties using SetResourceReference
                    cell.SetResourceReference(Control.BackgroundProperty, ColorConstants.Tbx_Board);
                    cell.AttachEventHandlers();
                }
            }
        }

        /// <summary>
        /// Initializes the Sudoku board with a given encrypted board code.
        /// </summary>
        /// <param name="boardCode">Encrypted board code string to initialize board state.</param>
        public void Initialize(string boardCode)
        {
            Initialize();
            boardCode = Code.Unprotect(boardCode);
            string[] cellsStrs = boardCode.Split('|');
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                for (int j = 0; j < GameSettings.BoardSide; j++)
                {
                    Cell cell = cells[i, j];
                    if (cell.IsReadOnly)
                    {
                        continue;
                    }
                    string cellStr = cellsStrs[i * GameSettings.BoardSide + j];
                    string[] cellParts = cellStr.Split(';');

                    cell.Text = cellParts[0];
                    string[] notes = cellParts[1].Split(',');
                    if (notes[0] != "")
                    {
                        cell.ShowNotes();

                        foreach (string note in notes)
                        {
                            cell.notesGrid.AddNote(note);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the Sudoku grid UI by adding rows, columns, cells, and borders.
        /// </summary>
        /// <param name="mainGrid">Main grid to contain the Sudoku board UI.</param>
        private void CreateSudokuGrid(Grid mainGrid)
        {
            CreateSeparation(sudokuGrid);
            CreateCells(sudokuGrid);
            AddBorders(sudokuGrid);
        }

        /// <summary>
        /// Creates separation (rows and columns) in the Sudoku grid.
        /// </summary>
        /// <param name="sudokuGrid">Grid representing the Sudoku board UI.</param>
        private void CreateSeparation(Grid sudokuGrid)
        {
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                sudokuGrid.RowDefinitions.Add(new RowDefinition());
                sudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        /// <summary>
        /// Creates cells (text boxes) for the Sudoku board.
        /// </summary>
        /// <param name="sudokuGrid">Grid representing the Sudoku board UI.</param>
        private void CreateCells(Grid sudokuGrid)
        {
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                for (int j = 0; j < GameSettings.BoardSide; j++)
                {
                    Cell cell = new Cell(i, j);
                    sudokuGrid.Children.Add(cell);
                    sudokuGrid.Children.Add(cell.notesGrid);
                    cells[i, j] = cell;
                    cell.InitializeProperties(); // Initialize cell properties with resource references
                }
            }
        }

        /// <summary>
        /// Adds borders (internal and main) to the Sudoku grid.
        /// </summary>
        /// <param name="sudokuGrid">Grid representing the Sudoku board UI.</param>
        private void AddBorders(Grid sudokuGrid)
        {
            for (int rows = 0; rows < GameSettings.BoxHeight; rows++)
            {
                for (int cols = 0; cols < GameSettings.BoxWidth; cols++)
                {
                    Border internalBorder = new Border();
                    internalBorder.SetResourceReference(Border.BorderBrushProperty, "Border");
                    Grid.SetRow(internalBorder, cols * GameSettings.BoxHeight);
                    Grid.SetRowSpan(internalBorder, GameSettings.BoxHeight);
                    Grid.SetColumn(internalBorder, rows * GameSettings.BoxWidth);
                    Grid.SetColumnSpan(internalBorder, GameSettings.BoxWidth);
                    sudokuGrid.Children.Add(internalBorder);
                }
            }

            // Adding the main border
            Border gridBorder = new Border();
            gridBorder.SetResourceReference(Border.BorderBrushProperty, "Border");
            Grid.SetRowSpan(gridBorder, GameSettings.BoardSide);
            Grid.SetColumnSpan(gridBorder, GameSettings.BoardSide);
            sudokuGrid.Children.Add(gridBorder);

            sudokuGrid.UpdateLayout(); // Ensure layout is updated immediately
        }

        /// <summary>
        /// Adjusts the border size dynamically based on the Sudoku grid size.
        /// </summary>
        /// <param name="sender">Event sender (Sudoku grid).</param>
        /// <param name="e">Size changed event arguments.</param>
        private void OnGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustBorderSize();
        }

        /// <summary>
        /// Adjusts the border thickness of internal and cell borders based on the Sudoku grid size.
        /// </summary>
        private void AdjustBorderSize()
        {
            double newBorderThickness = sudokuGrid.ActualWidth / (GameSettings.BoardSide * 50); // Example calculation
            foreach (UIElement element in sudokuGrid.Children)
            {
                if (element is Border border)
                {
                    if (Grid.GetRowSpan(border) == GameSettings.BoardSide && Grid.GetColumnSpan(border) == GameSettings.BoardSide)
                    {
                        border.BorderThickness = new Thickness(newBorderThickness * BoardConstants.EXTERNAL_BORDER_TO_REGULAR);
                    }
                    else
                    {
                        border.BorderThickness = new Thickness(newBorderThickness * BoardConstants.INTERNAL_BORDER_TO_REGULAR);
                    }
                }
                else if (element is Cell cell)
                {
                    cell.BorderThickness = new Thickness(newBorderThickness);
                }
            }
        }

        /// <summary>
        /// Moves focus to the text box of the cell at the specified row and column.
        /// </summary>
        /// <param name="row">Row index of the cell.</param>
        /// <param name="column">Column index of the cell.</param>
        public static void MoveFocusToTextBox(int row, int column)
        {
            Cell nextCell = cells[row, column];
            nextCell.Focus();
        }

        /// <summary>
        /// Shows the solution by filling in all cells and disabling input.
        /// </summary>
        public void ShowSolution()
        {
            solutionShown = true;
            foreach (Cell cell in cells)
            {
                cell.notesGrid.Clear();
                cell.IsEnabled = false;
                cell.Solve();
            }
        }

        /// <summary>
        /// Retrieves the currently focused cell on the Sudoku board.
        /// </summary>
        /// <returns>The currently focused cell, or null if none focused.</returns>
        public static Cell FocusedCell()
        {
            foreach (Cell cell in cells)
            {
                if (cell.Background == (Brush)Application.Current.FindResource("Tbx_Focus"))
                {
                    return cell;
                }
            }
            return null;
        }

        /// <summary>
        /// Disables input on the Sudoku board.
        /// </summary>
        public void Disable()
        {
            sudokuGrid.IsEnabled = false;
        }

        /// <summary>
        /// Checks the current board against the puzzle to validate correct cell entries.
        /// </summary>
        public void CheckMyBoard()
        {
            foreach (Cell cell in cells)
            {
                if (cell.Text == this.puzzle.CellValue(cell.row, cell.column).ToString())
                {
                    cell.IsReadOnly = true;
                    cell.SetResourceReference(Control.BackgroundProperty, ColorConstants.Tbx_RightBackground);
                    cell.SetResourceReference(Control.ForegroundProperty, ColorConstants.TextFore);
                }
                else if (cell.Text != "")
                {
                    cell.SetResourceReference(Control.BackgroundProperty, ColorConstants.Tbx_WrongBackground);
                }
                else
                {
                    cell.SetResourceReference(Control.BackgroundProperty, ColorConstants.Tbx_Board);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of cells related to the specified cell based on Sudoku rules.
        /// </summary>
        /// <param name="cellRow">Row index of the cell.</param>
        /// <param name="cellCol">Column index of the cell.</param>
        /// <returns>List of cells related to the specified cell.</returns>
        public static List<Cell> GetRelatedCells(int cellRow, int cellCol)
        {
            List<Cell> relatedCells = new List<Cell>();

            for (int col = 0; col < GameSettings.BoardSide; col++)
            {
                if (col != cellCol)
                {
                    relatedCells.Add(cells[cellRow, col]);
                }
            }

            for (int row = 0; row < GameSettings.BoardSide; row++)
            {
                if (row != cellRow)
                {
                    relatedCells.Add(cells[row, cellCol]);
                }
            }

            int boxRowStart = cellRow / GameSettings.BoxHeight * GameSettings.BoxHeight;
            int boxColStart = cellCol / GameSettings.BoxWidth * GameSettings.BoxWidth;

            for (int row = boxRowStart; row < boxRowStart + GameSettings.BoxHeight; row++)
            {
                for (int col = boxColStart; col < boxColStart + GameSettings.BoxWidth; col++)
                {
                    if (row != cellRow || col != cellCol)
                    {
                        relatedCells.Add(cells[row, col]);
                    }
                }
            }

            return relatedCells;
        }

        /// <summary>
        /// Generates an encrypted board code representing the current board state.
        /// </summary>
        /// <returns>Encrypted board code string.</returns>
        public string GenerateBoardCode()
        {
            string boardCode = "";
            for (int i = 0; i < GameSettings.BoardSide; i++)
            {
                for (int j = 0; j < GameSettings.BoardSide; j++)
                {
                    Cell cell = cells[i, j];
                    boardCode += $"{cell.Text};";

                    for (int k = 0; k < cell.notesGrid.notes.Count; k++)
                    {
                        boardCode += $"{cell.notesGrid.notes[k].Text},";
                    }

                    boardCode += "|";
                }
            }
            return Code.Protect(boardCode.Substring(0, boardCode.Length - 1));
        }

        /// <summary>
        /// Checks if the game is solved and triggers the solved animation if true.
        /// </summary>
        public static void ForSolvedAnimation()
        {
            if (solutionShown)
                return;
            if (IsSolved())
            {
                ShowSolvedAnimation();
                GameEnded?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}
