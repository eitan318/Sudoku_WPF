using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Sudoku_WPF
{
    enum ADDITIONAL_PAGES
    {
        HISTORY,
        INSTRUCTIONS,
        SETTINGS
    }
    /// <summary>
    /// Interaction logic for OpenningPage.xaml
    /// </summary>
    public partial class OpenningPage : Page
    {
        public OpenningPage()
        {
            InitializeComponent();
        }


    }
}
