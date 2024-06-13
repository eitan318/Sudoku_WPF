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
    enum DIF_LVL
    {
        EAZY,
        MDDIUM,
        HARD
    }
    public class Settings
    {
        public static int BOX_WIDTH = BoardConstants.DEFAULT_BOX_WIDTH;
        public static int BOX_HEIGHT = BoardConstants.DEFAULT_BOX_HEIGHT;
        public static int BOARD_SIDE = BOX_WIDTH * BOX_HEIGHT;

    }
}
