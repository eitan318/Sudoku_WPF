using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sudoku_WPF.Themes
{
    public enum ColorThemes
    {
        Dark,
        Light,
        LightBrown,
        Red,
        DarkBlue,
        LightBlue,
        Pink

    }

    internal class ThemeControl
    {
        public static void SetColors(ColorThemes mode)
        {
            string themeUri = "";

            themeUri = "Themes/" + mode.ToString() + ".xaml";

            Uri uri = new Uri(themeUri, UriKind.Relative);

            ResourceDictionary theme = new ResourceDictionary() { Source = uri };
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(theme);

        }
    }
}