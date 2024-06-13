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

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for InstructionsPage.xaml
    /// </summary>
    public partial class InstructionsPage : Page
    {
        private int currentPageIndex = 0;
        private readonly Page[] instructionPages;

        public InstructionsPage()
        {
            InitializeComponent();

            // Initialize the pages
            instructionPages = new Page[]
            {
                new InstructionPage1(),
                new InstructionPage2()
            };

            // Load the first page
            LoadCurrentPage();
        }

        private void LoadCurrentPage()
        {
            InstructionFrame.Navigate(instructionPages[currentPageIndex]);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            BackButton.IsEnabled = currentPageIndex > 0;
            NextButton.IsEnabled = currentPageIndex < instructionPages.Length - 1;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                LoadCurrentPage();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < instructionPages.Length - 1)
            {
                currentPageIndex++;
                LoadCurrentPage();
            }
        }
    }
}