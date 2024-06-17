using DAL;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF.publico;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.Pages
{
    /// <summary>
    /// Interaction logic for SavedPage.xaml
    /// </summary>
    public partial class SaverPage : Page
    {
        private List<GameInfo> games = new List<GameInfo>(); // List to store game information
        private List<Border> Items = new List<Border>(); // List to store UI border elements
        private bool isHistory; // Flag indicating if the page is showing history or saved games

        /**
         * Constructor for SaverPage class.
         * Initializes a new instance of the SaverPage.
         *
         * input: isHistory - Flag indicating if the page displays history or saved games.
         * output: None
         */
        public SaverPage(bool isHistory)
        {
            InitializeComponent();
            this.isHistory = isHistory;
            title_Txb.Text = isHistory ? "History" : "Saved Games";

            for (int i = 0; i < Items.Count; i++)
            {
                Border item = Items[i];
                item.Tag = i;
                UpdateButtonTags(item, i); // Update button tags in the UI
            }
        }

        /**
         * Adds a game item to the list and database.
         *
         * input: gameInfo - GameInfo object containing game information to add.
         * output: None
         */
        public void AddItemToListAndDB(GameInfo gameInfo)
        {
            AddItemToList(gameInfo);
            InsertGame(gameInfo);
        }

        /**
         * Adds a game item to the list.
         *
         * input: gameInfo - GameInfo object containing game information to add.
         * output: None
         */
        public void AddItemToList(GameInfo gameInfo)
        {
            games.Add(gameInfo);

            Border border = new Border
            {
                Margin = new Thickness(SaverConstants.MARGIN),
                Height = SaverConstants.HEIGHT,
                Width = SaverConstants.HEIGHT * 2,
                CornerRadius = new CornerRadius(SaverConstants.CORNER_RADIUS),
                Tag = games.Count - 1 // Store index of gameInfo in games list
            };

            // Add shadow effect
            border.Effect = new DropShadowEffect
            {
                Color = Colors.Gray,
                BlurRadius = 10,
                ShadowDepth = 5,
                Opacity = 0.5
            };

            border.SetResourceReference(BackgroundProperty, ColorConstants.HistoryItem_BG);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Button deleteGameBtn = new Button
            {
                Style = FindResource("RoundedButtonStyle") as Style,
                Height = 23,
                Width = 23,
                Content = "X",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 15,
                Margin = new Thickness(0, 5, 5, 0)
            };
            Grid.SetColumn(deleteGameBtn, 1);
            Grid.SetRowSpan(deleteGameBtn, 3);
            grid.Children.Add(deleteGameBtn);

            deleteGameBtn.Click += DeleteGame_Click;
            deleteGameBtn.Tag = border.Tag; // Set tag to match index in games list

            double fontSize = SaverConstants.RELATIVE_FONT_SIZE * border.Height;

            AddTextBlockToGrid(grid, gameInfo.Name.ToString(), fontSize, 2, true);
            AddTextBlockToGrid(grid, "Hints taken: " + gameInfo.Hints.ToString(), fontSize);
            AddTextBlockToGrid(grid, "Checks taken: " + gameInfo.Checks.ToString(), fontSize);
            AddTextBlockToGrid(grid, "Time: " + gameInfo.Time, fontSize);
            AddTextBlockToGrid(grid, "Dif Lvl: " + gameInfo.DifficultyLevel.ToString(), fontSize);
            AddTextBlockToGrid(grid, gameInfo.BoxHeight.ToString() + "*" + gameInfo.BoxWidth.ToString(), fontSize);
            AddTextBlockToGrid(grid, "Date&Hour: " + (gameInfo.Date.ToString().Substring(0, gameInfo.Date.ToString().Length - 3)), fontSize, 2);

            StackPanel btnPanel = CreateBtnPanel(isHistory, gameInfo);
            Grid.SetRowSpan(btnPanel, grid.RowDefinitions.Count - 2);
            Grid.SetRow(btnPanel, 1);
            Grid.SetColumn(btnPanel, 1);
            grid.Children.Add(btnPanel);

            border.Child = grid;
            ItemsWrapPanel.Children.Insert(0, border);

            Items.Add(border); // Add border to Items list
        }

        /**
         * Creates a button panel with specific buttons based on game history status.
         *
         * input: isHistory - Flag indicating if the page displays history or saved games.
         *        gameInfo - GameInfo object containing game information.
         * output: StackPanel containing buttons for the UI.
         */
        private StackPanel CreateBtnPanel(bool isHistory, GameInfo gameInfo)
        {
            StackPanel btnPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10) // Adjust margin as needed
            };

            if (!isHistory)
            {
                Button continueBtn = new Button
                {
                    Height = SaverConstants.BTN_HEIGHT,
                    Width = SaverConstants.BTN_WIDTH, // Adjust width as needed
                    Content = "Continue",
                    Margin = new Thickness(5),
                    Visibility = Visibility.Visible,
                    Style = FindResource("RoundedButtonStyle") as Style,
                };

                // Handle the click event for the "Continue" button
                continueBtn.Click += (sender, e) =>
                {
                    SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

                    var window = Application.Current.MainWindow as MainWindow;

                    // Check if gamePage is already instantiated
                    if (window.gamePage == null)
                    {
                        window.gamePage = new GamePage(gameInfo);
                        window.MainFrame.Navigate(window.gamePage);
                        DeleteGame(gameInfo);  // Remove game from the list and database
                        return;
                    }

                    MessageBoxResult result = MessageBox.Show("Do you want to save the running game?", "Save Game", MessageBoxButton.YesNoCancel);

                    if (result == MessageBoxResult.Yes)
                    {
                        window.gamePage.game.End(false, true);  // Save the current game state
                        DeleteGame(gameInfo);  // Remove game from the list and database
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        window.gamePage.game.End(false, false);  // Do not save the current game state
                        DeleteGame(gameInfo);  // Remove game from the list and database
                    }

                    // Navigate to the new game page
                    window.gamePage = new GamePage(gameInfo);
                    window.MainFrame.Navigate(window.gamePage);
                };

                btnPanel.Children.Add(continueBtn);
            }

            Button copyPuzzleCodeBtn = new Button
            {
                Height = SaverConstants.BTN_HEIGHT,
                Width = SaverConstants.BTN_WIDTH,
                Content = "Copy Code",
                Margin = new Thickness(5),
                Visibility = Visibility.Visible,
                Style = FindResource("RoundedButtonStyle") as Style,
                Tag = Items.Count - 1
            };
            copyPuzzleCodeBtn.Click += CopyPuzzleCode_Click;
            btnPanel.Children.Add(copyPuzzleCodeBtn);

            return btnPanel;
        }

        /**
         * Event handler for continuing a saved game.
         *
         * input: sender - The object that raised the event.
         *        e - The event arguments.
         * output: None
         */
        private void ContinueSavedGame_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            Button btn = sender as Button;

            int gameIndex = (int)btn.Tag;

            GameInfo gameInfo = games[gameIndex];
            var window = Application.Current.MainWindow as MainWindow;

            // Check if gamePage is already instantiated
            if (window.gamePage == null)
            {
                window.gamePage = new GamePage(gameInfo);
                window.MainFrame.Navigate(window.gamePage);
                DeleteGame(gameInfo);  // Remove game from the list and database
                return;
            }

            MessageBoxResult result = MessageBox.Show("Do you want to save the running game?", "Save Game", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {
                window.gamePage.game.End(false, true);  // Save the current game state
                DeleteGame(gameInfo);  // Remove game from the list and database
            }
            else if (result == MessageBoxResult.No)
            {
                window.gamePage.game.End(false, false);  // Do not save the current game state
                DeleteGame(gameInfo);  // Remove game from the list and database
            }

            // Navigate to the new game page
            window.gamePage = new GamePage(gameInfo);
            window.MainFrame.Navigate(window.gamePage);
        }

        /**
         * Event handler for copying the puzzle code to the clipboard.
         *
         * input: sender - The object that raised the event.
         *        e - The event arguments.
         * output: None
         */
        private void CopyPuzzleCode_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            Button btn = sender as Button;
            Clipboard.SetText(games[(int)btn.Tag].PuzzleCode);
            MessageBox.Show(GameConstants.COPIED_STR);
        }

        /**
         * Event handler for deleting a game.
         *
         * input: sender - The object that raised the event.
         *        e - The event arguments.
         * output: None
         */
        private void DeleteGame_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            Button btn = sender as Button;
            int index = (int)btn.Tag;

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this game?", "Delete Game", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DeleteGame(games[index]);
            }
        }

        /**
         * Deletes a game from the list and database.
         *
         * input: gameToRemove - GameInfo object containing the game information to remove.
         * output: None
         */
        private void DeleteGame(GameInfo gameToRemove)
        {
            DeleteGameFromDB(gameToRemove);

            int indexToRemove = games.IndexOf(gameToRemove);

            games.RemoveAt(indexToRemove);
            Border borderToRemove = Items[indexToRemove];

            Items.RemoveAt(indexToRemove);
            ItemsWrapPanel.Children.Remove(borderToRemove);

            // Update tags after deletion
            for (int i = indexToRemove; i < Items.Count; i++)
            {
                Border item = Items[i];
                item.Tag = i; // Update tag to match index in games list
                UpdateButtonTags(item, i); // Update button tags in the UI
            }
        }

        /**
         * Updates the tags of buttons inside a border element.
         *
         * input: border - Border element containing buttons to update.
         *        newTag - New tag value to assign to the buttons.
         * output: None
         */
        private void UpdateButtonTags(Border border, int newTag)
        {
            Grid grid = border.Child as Grid;
            foreach (UIElement element in grid.Children)
            {
                if (element is Button button)
                {
                    button.Tag = newTag;
                }
            }
        }

        /**
         * Deletes a game from the database.
         *
         * input: gameInfo - GameInfo object containing the game information to delete.
         * output: None
         */
        public static void DeleteGameFromDB(GameInfo gameInfo)
        {
            string sqlStmt = DBConstants.DeletGameQuary;
            OleDbParameter parameter = new OleDbParameter("@Id", gameInfo.Id);
            DBHelper.ExecuteCommand(sqlStmt, parameter);
        }

        /**
         * Inserts a new game into the database.
         *
         * input: gameInfo - GameInfo object containing the game information to insert.
         * output: None
         */
        public static void InsertGame(GameInfo gameInfo)
        {
            string sqlStmt = DBConstants.InsertGameQuary;

            OleDbParameter[] parameters =
            {
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.Current, gameInfo.Current),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.Solved, gameInfo.Solved),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.Time, gameInfo.Time),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.GameDate, gameInfo.Date),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.BoardCode, gameInfo.BoardCode),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.PuzzleCode, gameInfo.PuzzleCode),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.GameName, gameInfo.Name),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.HintsTaken, gameInfo.Hints),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.ChecksTaken, gameInfo.Checks),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.BoxHeight, gameInfo.BoxHeight),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.BoxWidth, gameInfo.BoxWidth),
                new OleDbParameter(DBConstants.AT + DBConstants.Games_Parameters.DifficultyLevel, gameInfo.DifficultyLevel.ToString())
             };

            DBHelper.ExecuteCommand(sqlStmt, parameters);
        }

        /**
         * Adds a text block to a grid.
         *
         * input: grid - Grid to add the text block.
         *        text - Text content of the text block.
         *        fontSize - Font size of the text block.
         *        columnSpan - Number of columns the text block spans.
         *        title - Flag indicating if the text block is a title.
         * output: None
         */
        private void AddTextBlockToGrid(Grid grid, string text, double fontSize, int columnSpan = 1, bool title = false)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = title ? HorizontalAlignment.Center : HorizontalAlignment.Left,
                FontSize = title ? fontSize * 1.1 : fontSize,
                Margin = new Thickness(10, 0, 0, 0),
                FontWeight = title ? FontWeights.Bold : FontWeights.Normal,
            };

            textBlock.SetResourceReference(ForegroundProperty, ColorConstants.TextFore);

            Grid.SetRow(textBlock, grid.RowDefinitions.Count - 1);
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumnSpan(textBlock, columnSpan);
            grid.Children.Add(textBlock);
        }
    }
}
