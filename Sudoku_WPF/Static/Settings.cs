using Sudoku_WPF.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.publico
{
    public static class Settings
    {
        public static Themes.ColorThemes theme = SettingsConstants.DEFAULT_THEME;
        public static bool markRelated = true;
        public static bool markSameText = true;
        public static bool soundOn = true;
        public static bool musicOn = true;
        public static bool allowNotes = true;

    }
}
