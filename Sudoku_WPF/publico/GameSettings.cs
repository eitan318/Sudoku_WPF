using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF.publico
{
    public enum DificultyLevel
    {
        Eazy,
        Medium,
        Hard
    }
    public class GameSettings
    {
        public static int BoxWidth = GameSettingsConstants.DEFAULT_BOX_WIDTH;
        public static int BoxHeight = GameSettingsConstants.DEFAULT_BOX_HEIGHT;
        public static int BoardSide = BoxWidth * BoxHeight;
        public static DificultyLevel dificultyLevel = DificultyLevel.Eazy;

    }
}
