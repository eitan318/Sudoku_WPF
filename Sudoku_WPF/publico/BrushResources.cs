using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sudoku_WPF.publico
{
    public static class BrushResources
    {
        public static Brush TextFore => (Brush)Application.Current.Resources["Text"];
        public static Brush WrongForeground => (Brush)Application.Current.Resources["Tbx_WrongForeground"];
        public static Brush Focus => (Brush)Application.Current.Resources["Tbx_Focus"];
        public static Brush WrongBackground => (Brush)Application.Current.Resources["Tbx_WrongBackground"];
        public static Brush SameText => (Brush)Application.Current.Resources["Tbx_SameText"];
        public static Brush Sign => (Brush)Application.Current.Resources["Tbx_Sign"];
        public static Brush Board => (Brush)Application.Current.Resources["Board"];
        public static Brush Border => (Brush)Application.Current.Resources["Border"];
        public static Brush RightBackground => (Brush)Application.Current.Resources["Tbx_RightBackground"];
        public static Brush HistoryItem_BG => (Brush)Application.Current.Resources["HistoryItem_BG"];

    }


}