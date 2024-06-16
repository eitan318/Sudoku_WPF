using static Sudoku_WPF.publico.Constants;
using static System.Net.Mime.MediaTypeNames;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF.publico;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

public class Cell : TextBox
{
    public int row, column;
    public string solvedValue;
    public Notes notesGrid;

    private List<Cell> relatedCells;
    private string previousText;
    private bool notesVisible = false;

    // Constructor to initialize cell properties
    public Cell(int row, int column)
    {
        this.row = row;
        this.column = column;

        notesGrid = new Notes { Visibility = Visibility.Collapsed };

        Grid.SetRow(this, row);
        Grid.SetColumn(this, column);

        Grid.SetRow(notesGrid, row);
        Grid.SetColumn(notesGrid, column);

        solvedValue = Puzzle.CellValueS(row, column).ToString();

        InitializeProperties();
    }

    public void ClearAll()
    {
        this.notesGrid.Clear();
        this.Clear();
    }

    // Initialize cell properties
    public void InitializeProperties()
    {
        Text = "";
        VerticalContentAlignment = VerticalAlignment.Center;
        HorizontalContentAlignment = HorizontalAlignment.Center;
        FontSize = BoardConstants.BOARD_WIDTH * BoardConstants.RELATIVE_FONT_SIZE / GameSettings.BoardSide;
        SetResourceReference(BorderBrushProperty, "Border"); // Set border brush using resource reference
        SetResourceReference(BackgroundProperty, "Tbx_Board"); // Set background brush using resource reference
        SetResourceReference(ForegroundProperty, "Text"); // Set foreground brush using resource reference
        CaretBrush = Brushes.Transparent;
    }

    // Attach event handlers to the cell
    public void AttachEventHandlers()
    {
        TextChanged += TextBox_TextChanged;
        PreviewTextInput += TextBox_PreviewTextInput;
        PreviewKeyDown += TextBox_PreviewKeyDown;
        GotFocus += TextBox_GotFocus;
        SizeChanged += OnSizeChanged; // Attach SizeChanged event for font size adjustment
    }

    // Detach event handlers from the cell
    public void DettachEventHandlers()
    {
        TextChanged -= TextBox_TextChanged;
        PreviewTextInput -= TextBox_PreviewTextInput;
        PreviewKeyDown -= TextBox_PreviewKeyDown;
        GotFocus -= TextBox_GotFocus;
        SizeChanged -= OnSizeChanged; // Detach SizeChanged event
    }

    // Adjust font size based on cell size
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        AdjustFontSize();
    }

    // Adjust the font size dynamically
    private void AdjustFontSize()
    {
        double fontSize = FontSize;
        double minFontSize = 6; // Minimum font size to avoid being too small
        double maxFontSize = 200; // Maximum font size to avoid being too large

        double width = ActualWidth - Padding.Left - Padding.Right;
        double height = ActualHeight - Padding.Top - Padding.Bottom;

        if (width <= 0 || height <= 0)
            return;

        FormattedText formattedText = new FormattedText(
            Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
            fontSize,
            Brushes.Black,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

        double scaleFactor = Math.Min(width / formattedText.Width, height / formattedText.Height);
        double newFontSize = Math.Clamp(fontSize * scaleFactor, minFontSize, maxFontSize);

        FontSize = newFontSize;
    }

    // Color cells based on the focus cell
    public void ColorBy(Cell focusCell, string previewText, bool validateForeground)
    {
        bool isValid = Foreground == (Brush)FindResource(ColorConstants.TextFore);

        SetResourceReference(BorderBrushProperty, ColorConstants.Border);

        if (IsRelatedTo(focusCell))
        {
            SetResourceReference(BackgroundProperty, Text == focusCell.Text && Text != "" ? ColorConstants.Tbx_WrongBackground : (Settings.markRelated ? ColorConstants.Tbx_Sign : ColorConstants.Tbx_Board));

            if (validateForeground)
            {
                if (Text == focusCell.previousText && Text != focusCell.Text && Text != "")
                {
                    isValid = IsRelativelyValid();
                }
                else if (Text == focusCell.Text && Text != "")
                {
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
            SetResourceReference(BackgroundProperty, Text == previewText && Text != "" && Settings.markSameText ?  ColorConstants.Tbx_SameText  : ColorConstants.Tbx_Board);
        }
    }

    // Check if this cell is related to another cell
    private bool IsRelatedTo(Cell anotherCell)
    {
        bool sameRow = row == anotherCell.row;
        bool sameColumn = column == anotherCell.column;
        bool sameBox = row / GameSettings.BoxHeight == anotherCell.row / GameSettings.BoxHeight && column / GameSettings.BoxWidth == anotherCell.column / GameSettings.BoxWidth;
        return sameRow || sameColumn || sameBox;
    }

    // Handle text input in the cell
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
                    ShowNotes(Text, e.Text);
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

    // Validate the cell content
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

    // Show notes in the cell
    private void ShowNotes(string firstNumber, string secondNumber)
    {
        ShowNotes();

        notesGrid.Clear();
        notesGrid.ManipulateNote(firstNumber);
        notesGrid.ManipulateNote(secondNumber);
    }

    public void ShowNotes()
    {
        if (notesVisible)
            return;

        notesVisible = true;
        notesGrid.Visibility = Visibility.Visible;
        Text = "";
    }

    // Switch notes in the cell
    private void SwitchNote(string number)
    {
        if (notesGrid != null)
        {
            notesGrid.ManipulateNote(number);
            if (notesGrid.IsLastOne())
            {
                Text = notesGrid.LastOne();
                notesGrid.Visibility = Visibility.Collapsed;
                notesVisible = false;
            }
        }
    }

    // Check if the input is valid
    public static bool IsValidInput(string input, int maxValue)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        int value = HexaToInt(input[0]);
        return value >= 1 && value <= maxValue;
    }

    // Handle key down events in the cell
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
            notesGrid.RemoveNoteByIdx(notesGrid.notes.Count - 1); ;
            e.Handled = true;
        }
    }

    // Check if the cell content is relatively valid
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

    // Check if the cell content is totally valid
    public bool IsTotallyValid()
    {
        return Text == Puzzle.CellValueS(row, column).ToString();
    }

    // Handle text changed event in the cell
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

    // Handle focus event in the cell
    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        CaretIndex = Text.Length;
        Board.VisualizeState(this, false, Text);
        //Background = Foreground;
    }

    // Convert hex character to integer
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
