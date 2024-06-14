﻿using DAL;
using Sudoku_WPF.GameClasses;
using Sudoku_WPF.publico;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.Pages
{
    /// <summary>
    /// Interaction logic for SavedPage.xaml
    /// </summary>
    public partial class SavedPage : Page
    {
        List<Border> Items = new List<Border>();
        List<GameInfo> games = new List<GameInfo>();
        public SavedPage()
        {
            InitializeComponent();

        }

        public void AddItemToListAndDB(GameInfo gameInfo)
        {
            AddItemToList(gameInfo);
            InsertGame(gameInfo);
        }

        public void AddItemToList(GameInfo gameInfo)
        {

            Border border = new Border
            {
                Margin = new Thickness(HistoryConstants.MARGIN),
                Background = BrushResources.HistoryItem_BG,
                Height = HistoryConstants.HEIGHT,
                CornerRadius = new CornerRadius(HistoryConstants.CORNER_RADIUS),
                Tag = ItemsPanel.Children.Count // Assuming ItemsPanel is a StackPanel
            };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            double fontSize = HistoryConstants.RELATIVE_FONT_SIZE * border.Height;

            // Add TextBlocks using the helper method
            AddTextBlockToGrid(grid, gameInfo.Hints.ToString(), fontSize, 0);
            AddTextBlockToGrid(grid, gameInfo.Checks.ToString(), fontSize, 1);
            AddTextBlockToGrid(grid, gameInfo.Time, fontSize, 2);
            AddTextBlockToGrid(grid, gameInfo.Date.ToString(), fontSize, 3);
            AddTextBlockToGrid(grid, gameInfo.Solved ? "solved" : "failed", fontSize, 4);

            // Add Grid to the Border
            border.Child = grid;

            // Add Border to the ItemsPanel
            ItemsPanel.Children.Add(border);
        }


        private void AddTextBlockToGrid(Grid grid, string text, double fontSize, int column)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = fontSize,
                Foreground = BrushResources.TextFore,
                Margin = new Thickness(2) // Optional, for spacing
            };

            Grid.SetColumn(textBlock, column);
            grid.Children.Add(textBlock);
        }

        private void InsertGame(GameInfo gameInfo)
        {
            string sqlStmt = @"INSERT INTO tbl_games 
              ([Current], [Time], Solved, GameDate, BoardCode, PuzzleCode, GameName, HintsTaken, ChecksTaken, BoxHeight, BoxWidth) 
              VALUES 
              (@Current, @Time, @Solved, @GameDate, @BoardCode, @PuzzleCode, @GameName, @HintsTaken, @ChecksTaken, @BoxHeight, @BoxWidth)";

            OleDbParameter[] parameters =
            {
        new OleDbParameter("@Current", gameInfo.Current),
        new OleDbParameter("@Time", gameInfo.Time),
        new OleDbParameter("@Solved", gameInfo.Solved),
        new OleDbParameter("@GameDate", gameInfo.Date), // Use DateTime.UtcNow for UTC time
        new OleDbParameter("@BoardCode", gameInfo.BoardCode),
        new OleDbParameter("@PuzzleCode", gameInfo.PuzzleCode),
        new OleDbParameter("@GameName", gameInfo.Name),
        new OleDbParameter("@HintsTaken", gameInfo.Hints),
        new OleDbParameter("@ChecksTaken", gameInfo.Checks),
        new OleDbParameter("@BoxHeight", gameInfo.BoxHeight),
        new OleDbParameter("@BoxWidth", gameInfo.BoxWidth)
    };

            DBHelper.ExecuteCommand(sqlStmt, parameters);
        }






        /*public void RemoveGame(GameInfo gameInfo)
        {
            string sqlStmt = @"DELETE FROM tbl_games WHERE Id = @Id";

            OleDbParameter parameter = new OleDbParameter("@Id", gameInfo.Id);

            DBHelper.ExecuteCommand(sqlStmt, parameter);
        }*/


    }
}
