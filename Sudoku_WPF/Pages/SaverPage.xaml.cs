using DAL;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF.publico;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private List<GameInfo> games = new List<GameInfo>();
        private List<Border> Items = new List<Border>();
        private bool isHistory;

        public SaverPage(bool isHistory)
        {
            InitializeComponent();
            this.isHistory = isHistory;
            title_Txb.Text = isHistory ? "History" : "Saved Games";
        }


        public void AddItemToListAndDB(GameInfo gameInfo)
        {
            AddItemToList(gameInfo);
            InsertGame(gameInfo);
        }

        public void AddItemToList(GameInfo gameInfo)
        {
            games.Add(gameInfo);

            Border border = new Border
            {
                Margin = new Thickness(SaverConstants.MARGIN),
                Height = SaverConstants.HEIGHT,
                Width = SaverConstants.HEIGHT * 2,
                CornerRadius = new CornerRadius(SaverConstants.CORNER_RADIUS),
                Tag = ItemsWrapPanel.Children.Count // Assuming ItemsPanel is a StackPanel
            };

            // Add shadow effect
            border.Effect = new DropShadowEffect
            {
                Color = Colors.Gray,
                BlurRadius = 10,
                ShadowDepth = 5,
                Opacity = 0.5
            };

            Items.Add(border);

            border.SetResourceReference(BackgroundProperty, ColorConstants.HistoryItem_BG);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Button deletGameBtn = new Button
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
            Grid.SetColumn(deletGameBtn, 1);
            Grid.SetRowSpan(deletGameBtn, 3);
            grid.Children.Add(deletGameBtn);

            deletGameBtn.Click += DeleteGame_Click;

            double fontSize = SaverConstants.RELATIVE_FONT_SIZE * border.Height;

            AddTextBlockToGrid(grid, gameInfo.Name.ToString(), fontSize, 2, true);
            AddTextBlockToGrid(grid, "Hints taken: " + gameInfo.Hints.ToString(), fontSize);
            AddTextBlockToGrid(grid, "Checks taken: " + gameInfo.Checks.ToString(), fontSize);
            AddTextBlockToGrid(grid, "Time: " + gameInfo.Time, fontSize);
            AddTextBlockToGrid(grid, "Dif Lvl: " + gameInfo.DifficultyLevel.ToString(), fontSize);
            AddTextBlockToGrid(grid, gameInfo.BoxHeight.ToString() + "*" + gameInfo.BoxWidth.ToString(), fontSize);
            AddTextBlockToGrid(grid, "Date&Hour: " + (gameInfo.Date.ToString().Substring(0, gameInfo.Date.ToString().Length - 3)), fontSize, 2);

            StackPanel btnPanel = CreateBtnPanel(isHistory);
            Grid.SetRowSpan(btnPanel, grid.RowDefinitions.Count - 2);
            Grid.SetRow(btnPanel, 1);
            Grid.SetColumn(btnPanel, 1);
            grid.Children.Add(btnPanel);

            border.Child = grid;
            ItemsWrapPanel.Children.Insert(0, border);
        }


        private StackPanel CreateBtnPanel(bool isHistory)
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
                    Tag = Items.Count() - 1
                };
                continueBtn.Click += ContinueSavedGame_Click;


                btnPanel.Children.Add(continueBtn);
            }

            Button copyPuzzleCodeBtn = new Button
            {
                Height = SaverConstants.BTN_HEIGHT,
                Width = SaverConstants.BTN_WIDTH,
                Content = "Copy Code",
                Margin = new Thickness(5),
                Visibility = Visibility.Visible,
                Style = FindResource("RoundedButtonStyle") as Style
            };
            copyPuzzleCodeBtn.Click += CopyPuzzleCode_Click;

            btnPanel.Children.Add(copyPuzzleCodeBtn);

            return btnPanel;
        }

        private void ContinueSavedGame_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            Button btn = sender as Button;
            var window = Application.Current.MainWindow as MainWindow;

            if (window.gamePage == null)
            {
                GameInfo gameInfo = games[Convert.ToInt32(btn.Tag)];
                window.gamePage = new GamePage(games[Convert.ToInt32(btn.Tag)]);
                window?.MainFrame.Navigate(window.gamePage);
                DeleteGame(gameInfo);
                return;
            }
            else
            {
                MessageBoxResult msbxRes = MessageBox.Show("Do you want to save the running game?", "Save Game", MessageBoxButton.YesNoCancel);

                if (msbxRes == MessageBoxResult.Yes || msbxRes == MessageBoxResult.No)
                {
                    GameInfo gameInfo = games[Convert.ToInt32(btn.Tag)];
                    DeleteGame(gameInfo);

                    window.gamePage.game.End(false, msbxRes == MessageBoxResult.Yes);

                    window.gamePage = new GamePage(gameInfo);
                    window?.MainFrame.Navigate(window.gamePage);
                }
            }
            
        }

        private void CopyPuzzleCode_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            Button btn = sender as Button;
            Clipboard.SetText(games[Convert.ToInt32(btn.Tag)].PuzzleCode);
            MessageBox.Show(GameConstants.COPIED_STR);
        }

        private void DeleteGame_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            Button btn = sender as Button;
            int index = Convert.ToInt32(btn.Tag);

            // Show a confirmation dialog
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this game?", "Delete Game", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            // If the user confirms, delete the game
            if (result == MessageBoxResult.Yes)
            {
                DeleteGame(games[index]);
            }
        }

        private void DeleteGame(GameInfo gameToRemove)
        {
            // Remove the game from the database
            DeleteGameFromDB(gameToRemove);

            // Find the index of the game to remove
            int indexToRemove = games.IndexOf(gameToRemove);

            // Remove the game from the list of games
            games.RemoveAt(indexToRemove);

            // Remove the border from the Items list and the WrapPanel
            Border borderToRemove = Items[indexToRemove];
            Items.RemoveAt(indexToRemove);
            ItemsWrapPanel.Children.Remove(borderToRemove);

            // Update the tags for the remaining items
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Tag = i;
                UpdateButtonTags((Border)Items[i], i);
            }
        }

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


        public static void DeleteGameFromDB(GameInfo gameInfo)
        {
            string sqlStmt = DBConstants.DeletGameQuary;
            OleDbParameter parameter = new OleDbParameter("@Id", gameInfo.Id);
            DBHelper.ExecuteCommand(sqlStmt, parameter);
        }


        private void AddTextBlockToGrid(Grid grid, string text, double fontSize, int columnSpan = 1, bool Title = false)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = Title ? HorizontalAlignment.Center : HorizontalAlignment.Left,
                FontSize = Title ? fontSize * 1.1 : fontSize,
                Margin = new Thickness(10, 0, 0, 0),
                FontWeight = Title ? FontWeights.Bold : FontWeights.Normal,

            };

            textBlock.SetResourceReference(ForegroundProperty, ColorConstants.TextFore);

            Grid.SetRow(textBlock, grid.RowDefinitions.Count - 1);
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumnSpan(textBlock, columnSpan);
            grid.Children.Add(textBlock);
        }


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
    }
}
