using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sudoku_WPF.publico;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    public class Board
    {
        private static Cell[,] cells;
        private Puzzle puzzle;
        public Grid sudokuGrid;
        private static bool solutionShown;


        public Board(Grid sudokuGrid, Puzzle puzzle)
        {
            this.sudokuGrid = sudokuGrid;
            cells = new Cell[Settings.BOARD_SIDE, Settings.BOARD_SIDE];
            this.puzzle = puzzle;

            CreateSudokuGrid(sudokuGrid);
            Initialize();
            sudokuGrid.SizeChanged += OnGridSizeChanged;

            solutionShown = false;
        }

        public Board(Grid sudokuGrid, Puzzle puzzle, string boardCode)
        {
            this.sudokuGrid = sudokuGrid;
            cells = new Cell[Settings.BOARD_SIDE, Settings.BOARD_SIDE];
            this.puzzle = puzzle;

            CreateSudokuGrid(sudokuGrid);
            boardCode = Code.Unprotect(boardCode);
            Initialize(boardCode);
            sudokuGrid.SizeChanged += OnGridSizeChanged;

            solutionShown = false;
        }

        public static void ForSolvedAnimation()
        {
            if (solutionShown)
                return;
            if (IsSolved())
            {
                ShowSolvedAnimation();
            }
        }


        public void Disable()
        {
            sudokuGrid.IsEnabled = false;
        }

        private void OnGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustBorderSize();
        }

        private void AdjustBorderSize()
        {
            double newBorderThickness = sudokuGrid.ActualWidth / (Settings.BOARD_SIDE * 50); // Example calculation
            foreach (UIElement element in sudokuGrid.Children)
            {
                if (element is Border border)
                {
                    if (Grid.GetRowSpan(border) == Settings.BOARD_SIDE && Grid.GetColumnSpan(border) == Settings.BOARD_SIDE)
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



        private static void ShowSolvedAnimation()
        {
            foreach (Cell cell in cells)
            {
                cell.SetResourceReference(Control.BackgroundProperty, "Tbx_RightBackground");
                cell.IsReadOnly = true;
            }
        }

        public void CheckMyBoard()
        {
            foreach (Cell cell in cells)
            {
                if (cell.Text == this.puzzle.CellValue(cell.row, cell.column).ToString())
                {
                    cell.IsReadOnly = true;
                    cell.SetResourceReference(Control.BackgroundProperty, "Tbx_RightBackground");
                }
                else if (cell.Text != "")
                {
                    cell.SetResourceReference(Control.BackgroundProperty, "Tbx_WrongBackground");
                }
                else
                {
                    cell.SetResourceReference(Control.BorderBrushProperty, "Border");
                }
            }
        }

        public static void MoveFocusToTextBox(int row, int column)
        {
            Cell nextCell = cells[row, column];
            nextCell.Focus();
        }

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

        public void Initialize()
        {
            for (int i = 0; i < Settings.BOARD_SIDE; i++)
            {
                for (int j = 0; j < Settings.BOARD_SIDE; j++)
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
                    cell.SetResourceReference(Control.BackgroundProperty, "Tbx_Board");
                    cell.AttachEventHandlers();
                }
            }
        }

        public void Initialize(string boardCode)
        {
            Initialize();
            string[] cellsStrs = boardCode.Split('|');
            for (int i = 0; i < Settings.BOARD_SIDE; i++)
            {
                for (int j = 0; j < Settings.BOARD_SIDE; j++)
                {
                    Cell cell = cells[i, j];
                    if (cell.IsReadOnly)
                    {
                        continue;
                    }
                    string cellStr = cellsStrs[i * Settings.BOARD_SIDE + j];
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

        private void CreateSudokuGrid(Grid mainGrid)
        {
            CreateSeparation(sudokuGrid);
            CreateCells(sudokuGrid);
            AddBorders(sudokuGrid);
        }

        private void CreateSeparation(Grid sudokuGrid)
        {
            for (int i = 0; i < Settings.BOARD_SIDE; i++)
            {
                sudokuGrid.RowDefinitions.Add(new RowDefinition());
                sudokuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void CreateCells(Grid sudokuGrid)
        {
            for (int i = 0; i < Settings.BOARD_SIDE; i++)
            {
                for (int j = 0; j < Settings.BOARD_SIDE; j++)
                {
                    Cell cell = new Cell(i, j);
                    sudokuGrid.Children.Add(cell);
                    sudokuGrid.Children.Add(cell.notesGrid);
                    cells[i, j] = cell;
                    cell.InitializeProperties(); // Initialize cell properties with resource references
                }
            }
        }

        private void AddBorders(Grid sudokuGrid)
        {
            for (int rows = 0; rows < Settings.BOX_HEIGHT; rows++)
            {
                for (int cols = 0; cols < Settings.BOX_WIDTH; cols++)
                {
                    Border internalBorder = new Border
                    {
                        BorderBrush = (Brush)Application.Current.FindResource("Border")
                    };
                    Grid.SetRow(internalBorder, cols * Settings.BOX_HEIGHT);
                    Grid.SetRowSpan(internalBorder, Settings.BOX_HEIGHT);
                    Grid.SetColumn(internalBorder, rows * Settings.BOX_WIDTH);
                    Grid.SetColumnSpan(internalBorder, Settings.BOX_WIDTH);
                    sudokuGrid.Children.Add(internalBorder);
                }
            }

            // Adding the main border
            Border gridBorder = new Border
            {
                BorderBrush = (Brush)Application.Current.FindResource("Border")
            };
            Grid.SetRowSpan(gridBorder, Settings.BOARD_SIDE);
            Grid.SetColumnSpan(gridBorder, Settings.BOARD_SIDE);
            sudokuGrid.Children.Add(gridBorder);

            sudokuGrid.UpdateLayout(); // Ensure layout is updated immediately
        }


        public static List<Cell> GetRelatedCells(int cellRow, int cellCol)
        {
            List<Cell> relatedCells = new List<Cell>();

            for (int col = 0; col < Settings.BOARD_SIDE; col++)
            {
                if (col != cellCol)
                {
                    relatedCells.Add(cells[cellRow, col]);
                }
            }

            for (int row = 0; row < Settings.BOARD_SIDE; row++)
            {
                if (row != cellRow)
                {
                    relatedCells.Add(cells[row, cellCol]);
                }
            }

            int boxRowStart = cellRow / Settings.BOX_HEIGHT * Settings.BOX_HEIGHT;
            int boxColStart = cellCol / Settings.BOX_WIDTH * Settings.BOX_WIDTH;

            for (int row = boxRowStart; row < boxRowStart + Settings.BOX_HEIGHT; row++)
            {
                for (int col = boxColStart; col < boxColStart + Settings.BOX_WIDTH; col++)
                {
                    if (row != cellRow || col != cellCol)
                    {
                        relatedCells.Add(cells[row, col]);
                    }
                }
            }

            return relatedCells;
        }

        public static string GenerateBoardCode()
        {
            string boardCode = "";
            for (int i = 0; i < Settings.BOARD_SIDE; i++)
            {
                for (int j = 0; j < Settings.BOARD_SIDE; j++)
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
    }
}
