using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sudoku_WPF.publico
{
    public class Constants
    {
        public static int NUM_DIGITS = 10;
        public static int ERROR = -1;
        public class BoardConstants 
        {
            public static int BOARD_WIDTH = 600;
            public static double RELATIVE_FONT_SIZE = 0.65;
            public static string SHOW_SOLUTION = "Show a solution";
            public static double INTERNAL_BORDER_TO_REGULAR = 2;
            public static double EXTERNAL_BORDER_TO_REGULAR = 4;
            public static double DEFAULT_SIDE = 9.0;
            public static double DEFAULT_WIDTH = 500.0;
            public static int DEFAULT_BOX_HEIGHT = 3;
            public static int DEFAULT_BOX_WIDTH = 3;
            
        }
        public class HistoryConstants
        {
            public static double RELATIVE_FONT_SIZE = 0.1;
            public static int MARGIN = 10;
            public static int CORNER_RADIUS = 20;
            public static int HEIGHT = 150;
        }

        public class TimerConstants
        {
            public static string DEFAULT_TIME = "00:00:00";
            public static string FORMAT = @"hh\:mm\:ss";
        }

        public class GameConstants
        {
            public static string REMEINING_STR = " Left";
            public static string COPIED_STR = "Text copied to clipboard.";
            public static string PAUSE_STR = "Verify to continue";
            public static string END_GAME_CONFIRM_MSG = "Do you want to save this game before leaving it?";
            public static int HINTS = 5;
            public static int CHECKS = 5;
        }

        public class DBConstants
        {
            public const string AT = "@";

            // Define parameter names used in the SQL statement without the "@" symbol
            public class Parameters
            {
                public const string Current = "Current";
                public const string Time = "Time";
                public const string Solved = "Solved";
                public const string GameDate = "GameDate";
                public const string BoardCode = "BoardCode";
                public const string PuzzleCode = "PuzzleCode";
                public const string GameName = "GameName";
                public const string HintsTaken = "HintsTaken";
                public const string ChecksTaken = "ChecksTaken";
                public const string BoxHeight = "BoxHeight";
                public const string BoxWidth = "BoxWidth";
            }

            // Define the SQL statement for inserting a game into the tbl_games table
            public const string InsertGameQuary =
                $@"INSERT INTO tbl_games 
                       ([{Parameters.Current}], [{Parameters.Time}], [{Parameters.Solved}], [{Parameters.GameDate}], 
                        [{Parameters.BoardCode}], [{Parameters.PuzzleCode}], [{Parameters.GameName}], [{Parameters.HintsTaken}], 
                        [{Parameters.ChecksTaken}], [{Parameters.BoxHeight}], [{Parameters.BoxWidth}]) 
                   VALUES 
                       ({AT + Parameters.Current}, {AT + Parameters.Time}, {AT + Parameters.Solved}, {AT + Parameters.GameDate}, 
                        {AT + Parameters.BoardCode}, {AT + Parameters.PuzzleCode}, {AT + Parameters.GameName}, {AT + Parameters.HintsTaken}, 
                        {AT + Parameters.ChecksTaken}, {AT + Parameters.BoxHeight}, {AT + Parameters.BoxWidth})";

            public const string DeletGameQuary = @"DELETE FROM tbl_games WHERE Id = @Id";
        }


        public class ColorConstants
        {
            public const string Text = "Text";
            public const string Tbx_WrongForeground = "Tbx_WrongForeground";
            public const string Background = "Background";
            public const string Tbx_Board = "Tbx_Board";
            public const string Border = "Border";
            public const string Tbx_Focus = "Tbx_Focus";
            public const string Tbx_WrongBackground = "Tbx_WrongBackground";
            public const string Tbx_SameText = "Tbx_SameText";
            public const string Tbx_Sign = "Tbx_Sign";
            public const string Tbx_RightBackground = "Tbx_RightBackground";
            public const string Menu_BG = "Menu_BG";
            public const string Botton_BG = "Botton_BG";
            public const string Botton_FG = "Botton_FG";
            public const string Botton_Border = "Botton_Border";
            public const string HistoryItem_BG = "HistoryItem_BG";
            public const string MenuBtn_MO = "MenuBtn_MO";
            public const string MenuBtn_Checked = "MenuBtn_Checked";
        }


        public class UriConstants
        {
            public static Uri GAME_SETTINGS_PAGE = new Uri("/Pages/GameSettingsPage.xaml", UriKind.Relative);
        }
    }
}
