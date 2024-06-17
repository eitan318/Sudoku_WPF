using Sudoku_WPF.publico;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.GameClasses
{
    /// <summary>
    /// Represents a single cell in the Sudoku board, derived from TextBox.
    /// </summary>
    public class Cell : TextBox
    {
        public int row, column; // Row and column indices of the cell
        public string solvedValue; // The solved value of the cell
        public Notes notesGrid; // Notes grid associated with the cell

        private List<Cell> relatedCells; // List of cells related to this cell
        private string previousText; // Previous text content of the cell
        private bool notesVisible = false; // Flag indicating if notes are currently visible

        /// <summary>
        /// Constructor to initialize a cell.
        /// </summary>
        /// <param name="row">Row index of the cell.</param>
        /// <param name="column">Column index of the cell.</param>
        public Cell(int row, int column)
        {
            this.row = row;
            this.column = column;

            notesGrid = new Notes { Visibility = System.Windows.Visibility.Collapsed };

            Grid.SetRow(this, row);
            Grid.SetColumn(this, column);

            Grid.SetRow(notesGrid, row);
            Grid.SetColumn(notesGrid, column);

            solvedValue = Puzzle.CellValueS(row, column).ToString();

            InitializeProperties();
        }

        /// <summary>
        /// Clears all content in the cell and notes grid.
        /// </summary>
        public void ClearAll()
        {
            this.notesGrid.Clear();
            this.Clear();
        }

        /// <summary>
        /// Initializes visual properties of the cell.
        /// </summary>
        public void InitializeProperties()
        {
            Text = "";
            VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            FontSize = BoardConstants.BOARD_WIDTH * BoardConstants.RELATIVE_FONT_SIZE / GameSettings.BoardSide;
            SetResourceReference(BorderBrushProperty, "Border"); // Set border brush using resource reference
            SetResourceReference(BackgroundProperty, "Tbx_Board"); // Set background brush using resource reference
            SetResourceReference(ForegroundProperty, "Text"); // Set foreground brush using resource reference
            CaretBrush = Brushes.Transparent;
        }

        /// <summary>
        /// Attaches event handlers to the cell.
        /// </summary>
        public void AttachEventHandlers()
        {
            TextChanged += TextBox_TextChanged;
            PreviewTextInput += TextBox_PreviewTextInput;
            PreviewKeyDown += TextBox_PreviewKeyDown;
            GotFocus += TextBox_GotFocus;
            SizeChanged += OnSizeChanged; // Attach SizeChanged event for font size adjustment
        }

        /// <summary>
        /// Detaches event handlers from the cell.
        /// </summary>
        public void DetachEventHandlers()
        {
            TextChanged -= TextBox_TextChanged;
            PreviewTextInput -= TextBox_PreviewTextInput;
            PreviewKeyDown -= TextBox_PreviewKeyDown;
            GotFocus -= TextBox_GotFocus;
            SizeChanged -= OnSizeChanged; // Detach SizeChanged event
        }

        /// <summary>
        /// Adjusts font size dynamically based on cell size.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Size changed event arguments.</param>
        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            AdjustFontSize();
        }

        /// <summary>
        /// Adjusts the font size of the cell based on its dimensions.
        /// </summary>
        private void AdjustFontSize()
        {
            double fontSize = FontSize;
            double minFontSize = 6; // Minimum font size to avoid being too small
            double maxFontSize = 200; // Maximum font size to avoid being too large

            double width = ActualWidth - Padding.Left - Padding.Right;
            double height = ActualHeight - Padding.Top - Padding.Bottom;

            if (width <= 0 || height <= 0)
                return;

            System.Windows.Media.FormattedText formattedText = new System.Windows.Media.FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                fontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            double scaleFactor = System.Math.Min(width / formattedText.Width, height / formattedText.Height);
            double newFontSize = System.Math.Clamp(fontSize * scaleFactor, minFontSize, maxFontSize);

            FontSize = newFontSize;
        }

        /// <summary>
        /// Colors the cell based on the focus cell and validates the content.
        /// </summary>
        /// <param name="focusCell">The currently focused cell.</param>
        /// <param name="previewText">Text to preview in the cell.</param>
        /// <param name="validateForeground">Flag indicating if foreground validation is required.</param>
        public void ColorBy(Cell focusCell, string previewText, bool validateForeground)
        {
            bool isValid = Foreground == (Brush)FindResource(ColorConstants.TextFore);

            SetResourceReference(BorderBrushProperty, ColorConstants.Border);

            if (IsRelatedTo(focusCell))
            {
                SetResourceReference(BackgroundProperty, Text == focusCell.Text && Text != "" && !focusCell.IsReadOnly ? ColorConstants.Tbx_WrongBackground : (Settings.markRelated ? ColorConstants.Tbx_Sign : ColorConstants.Tbx_Board));

                if (validateForeground)
                {
                    if (Text == focusCell.previousText && Text != focusCell.Text && Text != "")
                    {
                        isValid = IsRelativelyValid();
                    }
                    else if (Text == focusCell.Text && Text != "")
                    {
                        if (focusCell.Foreground != (Brush)FindResource(ColorConstants.Tbx_WrongForeground))
                        {
                            SoundPlayer.PlaySound(SoundConstants.WRONG);
                        }
                        isValid = false;
                        focusCell.SetResourceReference(ForegroundProperty, ColorConstants.Tbx_WrongForeground);
                    }
                    if (!IsReadOnly)
                    {
                        SetResourceReference(ForegroundProperty, isValid ? ColorConstants.TextFore : ColorConstants.Tbx_WrongForeground);
                    }
                }
            }
            else
            {
                SetResourceReference(BackgroundProperty, Text == previewText && Text != "" && Settings.markSameText ? ColorConstants.Tbx_SameText : ColorConstants.Tbx_Board);
            }
        }

        /// <summary>
        /// Checks if this cell is related to another cell.
        /// </summary>
        /// <param name="anotherCell">Another cell to compare relation.</param>
        /// <returns>True if related, otherwise false.</returns>
        private bool IsRelatedTo(Cell anotherCell)
        {
            bool sameRow = row == anotherCell.row;
            bool sameColumn = column == anotherCell.column;
            bool sameBox = row / GameSettings.BoxHeight == anotherCell.row / GameSettings.BoxHeight && column / GameSettings.BoxWidth == anotherCell.column / GameSettings.BoxWidth;
            return sameRow || sameColumn || sameBox;
        }

        /// <summary>
        /// Handles text input in the cell.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Text composition event arguments.</param>
        public void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsValidInput(e.Text, GameSettings.BoardSide))
            {
                if (IsReadOnly)
                {
                    Board.VisualizeState(this, false, e.Text);
                }
                else
                {
                    if (Text.Length == 1)
                    {
                        if (Settings.allowNotes)
                        {
                            ShowNotes(Text, e.Text);
                        }
                        else
                        {
                            Text = e.Text;
                        }
                    }
                    else
                    {
                        if (notesVisible)
                        {
                            SwitchNote(e.Text);
                        }
                        else
                        {
                            Text = e.Text.ToUpper();
                            CaretIndex = Text.Length;
                        }
                    }
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Solves the cell, displaying its solved value and making it read-only.
        /// </summary>
        /// <param name="hint">Optional parameter indicating if the solve is a hint.</param>
        public void Solve(bool hint = false)
        {
            if (!IsReadOnly)
            {
                Text = solvedValue;
                IsReadOnly = true;
            }
            Foreground = (Brush)FindResource(ColorConstants.TextFore);
            BorderBrush = (Brush)FindResource(ColorConstants.Border);
            if (!hint)
            {
                SetResourceReference(BackgroundProperty, ColorConstants.Tbx_Board);
            }
        }

        /// <summary>
        /// Shows notes in the cell for two numbers.
        /// </summary>
        /// <param name="firstNumber">First number to show as a note.</param>
        /// <param name="secondNumber">Second number to show as a note.</param>
        private void ShowNotes(string firstNumber, string secondNumber)
        {
            ShowNotes();

            notesGrid.Clear();
            notesGrid.ManipulateNote(firstNumber);
            notesGrid.ManipulateNote(secondNumber);
        }

        /// <summary>
        /// Shows the notes grid in the cell.
        /// </summary>
        public void ShowNotes()
        {
            if (notesVisible)
                return;

            notesVisible = true;
            notesGrid.Visibility = System.Windows.Visibility.Visible;
            Text = "";
        }

        /// <summary>
        /// Switches the note in the cell.
        /// </summary>
        /// <param name="number">Number to manipulate as a note.</param>
        private void SwitchNote(string number)
        {
            if (notesGrid != null)
            {
                notesGrid.ManipulateNote(number);
                if (notesGrid.IsLastOne())
                {
                    Text = notesGrid.LastOne();
                    notesGrid.Visibility = System.Windows.Visibility.Collapsed;
                    notesVisible = false;
                }
            }
        }

        /// <summary>
        /// Validates if the input text is valid for the cell.
        /// </summary>
        /// <param name="input">Input text to validate.</param>
        /// <param name="maxValue">Maximum value allowed.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool IsValidInput(string input, int maxValue)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            int value = HexaToInt(input[0]);
            return value >= 1 && value <= maxValue;
        }

        /// <summary>
        /// Handles key down events in the cell.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Key event arguments.</param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (row > 0)
                    {
                        Board.MoveFocusToTextBox(row - 1, column);
                    }
                    break;
                case Key.Down:
                    if (row < GameSettings.BoardSide - 1)
                    {
                        Board.MoveFocusToTextBox(row + 1, column);
                    }
                    break;
                case Key.Left:
                    if (column > 0)
                    {
                        Board.MoveFocusToTextBox(row, column - 1);
                    }
                    break;
                case Key.Right:
                    if (column < GameSettings.BoardSide - 1)
                    {
                        Board.MoveFocusToTextBox(row, column + 1);
                    }
                    break;
            }

            if ((e.Key == Key.Delete || e.Key == Key.Space) && !IsReadOnly)
            {
                Text = "";
                notesGrid.Clear();
                e.Handled = true;
            }
            else if (e.Key == Key.Back && !IsReadOnly)
            {
                Text = "";
                notesGrid.RemoveNoteByIdx(notesGrid.notes.Count - 1);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Checks if the cell content is relatively valid.
        /// </summary>
        /// <returns>True if relatively valid, otherwise false.</returns>
        public bool IsRelativelyValid()
        {
            if (relatedCells == null)
                relatedCells = Board.GetRelatedCells(row, column);

            foreach (Cell cell in relatedCells)
            {
                if (this != cell && cell.Text != "" && Text == cell.Text)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the cell content is totally valid.
        /// </summary>
        /// <returns>True if totally valid, otherwise false.</returns>
        public bool IsTotallyValid()
        {
            return Text == Puzzle.CellValueS(row, column).ToString();
        }

        /// <summary>
        /// Handles text changed event in the cell.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Text changed event arguments.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this != null && !IsReadOnly)
            {
                AdjustFontSize(); // Adjust font size when text changes
                Board.VisualizeState(this, true, Text);
                Board.ForSolvedAnimation();

                previousText = Text;
            }
        }

        /// <summary>
        /// Handles focus event in the cell.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Routed event arguments.</param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CaretIndex = Text.Length;
            Board.VisualizeState(this, false, Text);
            //Background = Foreground;
        }

        /// <summary>
        /// Converts a hexadecimal character to an integer value.
        /// </summary>
        /// <param name="character">Hexadecimal character to convert.</param>
        /// <returns>Integer value corresponding to the hexadecimal character.</returns>
        private static int HexaToInt(char character)
        {
            if (char.IsDigit(character))
            {
                return character - '0';
            }
            else if (char.IsLetter(character))
            {
                return char.ToUpper(character) - 'A' + Constants.NUM_DIGITS;
            }

            return Constants.ERROR;
        }
    }
}
