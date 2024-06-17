using Sudoku_WPF.Themes;
using System;
using System.Windows;

namespace Sudoku_WPF.publico
{
    public class Constants
    {
        public const int NUM_DIGITS = 10;
        public const int ERROR = -1;
        public const string PROJ_DIRECTORY = "\\Sudoku_WPF\\Sudoku_WPF";

        public class BoardConstants
        {
            public const int BOARD_WIDTH = 600;
            public const double RELATIVE_FONT_SIZE = 0.65;
            public const string SHOW_SOLUTION = "Show a solution";
            public const double INTERNAL_BORDER_TO_REGULAR = 2;
            public const double EXTERNAL_BORDER_TO_REGULAR = 4;
            public const double DEFAULT_SIDE = 9.0;
            public const double MAX_SIDE = 16;
            public const double DEFAULT_WIDTH = 500.0;
        }

        public class SoundConstants
        {
            public static string BACK_MUSIC_NAME = "relaxing_background_music";

            public const string WRONG = "wrong";
            public const string MENU_CLICK = "menu_click";
            public const string ON_OFF = "on_off";
            public const string SOLVED = "game_solved";
            public const string BOTTON_CLICK = "button_click";
            public static string GetMusicPath(string musicName)
            {
                string relativePath = $"\\Assets\\Music\\{musicName}.wav";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string musicPath = baseDirectory.Substring(0, baseDirectory.IndexOf(Constants.PROJ_DIRECTORY) + Constants.PROJ_DIRECTORY.Length) + relativePath;

                return musicPath;
            }

        }

        public class SaverConstants
        {
            public const double RELATIVE_FONT_SIZE = 0.1;
            public const int MARGIN = 15;
            public const int CORNER_RADIUS = 20;
            public const int HEIGHT = 155;
            public const int BTN_HEIGHT = 37;
            public const int BTN_WIDTH = 93;
        }

        public class TimerConstants
        {
            public const string DEFAULT_TIME = "00:00:00";
            public const string FORMAT = @"hh\:mm\:ss";
        }

        public class GameConstants
        {
            public const string REMEINING_STR = " Left";
            public const string COPIED_STR = "Text copied to clipboard.";
            public const string PAUSE_STR = "Verify to continue";
            public const string END_GAME_CONFIRM_MSG = "Do you want to save this game before leaving it?";
            public const int HINTS = 5;
            public const int CHECKS = 5;
        }

        public class GameSettingsConstants 
        {
            public const int DEFAULT_BOX_HEIGHT = 3;
            public const int DEFAULT_BOX_WIDTH = 3;
            public const double DIFICULTY_LEVEL_DELTA = 0.1;
            public const double MAX_FULL_CELLS = 0.28;
            public const DificultyLevel DEFAULT_DIFICULTY_LEVEL = DificultyLevel.Easy;
            public const double FULL_CELLS_EASY = 0.50;
            public const double FULL_CELLS_MEDIUM = 0.40;
            public const double FULL_CELLS_HARD = 0.30;
        }

        public class SettingsConstants
        {
            public static readonly Themes.ColorThemes DEFAULT_THEME = Themes.ColorThemes.Light;
        }

        public class DBConstants
        {
            public const string AT = "@";

            public const string InsertGameQuary =  @"INSERT INTO tbl_games 
              ([Current], Solved, [Time], GameDate, BoardCode, PuzzleCode, GameName, HintsTaken, ChecksTaken, BoxHeight, BoxWidth, DifficultyLevel) 
              VALUES 
              (@Current, @Solved, @Time, @GameDate, @BoardCode, @PuzzleCode, @GameName, @HintsTaken, @ChecksTaken, @BoxHeight, @BoxWidth, @DifficultyLevel)";

            public class Games_Parameters
            {
                public const string Id = "Id";
                public const string Current = "Current";
                public const string Time = "Time";
                public const string Solved = "Solved";
                public const string GameDate = "GameDate";
                public const string BoardCode = "BoardCode";
                public const string PuzzleCode = "PuzzleCode";
                public const string GameName = "GameName";
                public const string HintsTaken =  "HintsTaken";
                public const string ChecksTaken = "ChecksTaken";
                public const string BoxHeight =  "BoxHeight";
                public const string BoxWidth =  "BoxWidth";
                public const string DifficultyLevel =  "DifficultyLevel";
            }

            public class Settings_Parameters
            {
                public const string SameText = "MarkSameText";
                public const string MarkRelated = "MarkRelated";
                public const string SoundOn = "SoundOn";
                public const string MusicOn = "MusicOn";
                public const string Theme = "Theme";
                public const string AllowNotes = "AllowNotes";
            }

            public const string DeletGameQuary = @"DELETE FROM tbl_games WHERE Id = @Id";

            public const string UpdateSettingsQuary = @"UPDATE tbl_settings SET 
                        MarkSameText = @MarkSameText, 
                        MarkRelated = @MarkRelated, 
                        SoundOn = @SoundOn, 
                        MusicOn = @MusicOn, 
                        Theme = @Theme, 
                        AllowNotes = @AllowNotes 
                        WHERE Id = 1"; // Assuming there's a column ID to identify the row;


        }

        public class ColorConstants
        {
            public const string TextFore = "Text";
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
            public static readonly Uri GAME_SETTINGS_PAGE = new Uri("/Pages/GameSettingsPage.xaml", UriKind.Relative);
        }
    }
}
