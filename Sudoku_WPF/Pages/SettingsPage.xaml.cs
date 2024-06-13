using System;
using System.Collections.Generic;
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
using Sudoku_WPF.Themes;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ColorMode_CMBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string option = "";
            ComboBoxItem cbi = (ComboBoxItem)ColorMode_CMBB.SelectedValue;
            option = cbi.Content.ToString();

            if (Enum.TryParse(option, out ColorMode color))
            {
                ThemeControl.SetColors(color);
            }

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new System.Uri("OpenningPage.xaml", UriKind.Relative));
        }

    }
}
