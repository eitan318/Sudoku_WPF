using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sudoku_WPF.Themes
{
    public enum ColorMode
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
        public static void SetColors(ColorMode mode)
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