using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku_WPF.Pages.InstructionPages;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for InstructionsPage.xaml
    /// </summary>
    public partial class InstructionsPage : Page
    {
        private int currentPageIndex = 0;
        private readonly Page[] instructionPages;

        /// <summary>
        /// Constructor for initializing the InstructionsPage.
        /// Initializes the instruction pages array and loads the first page.
        /// </summary>
        public InstructionsPage()
        {
            InitializeComponent();

            // Initialize the pages
            instructionPages = new Page[]
            {
                new InstructionPage1(),
                new InstructionPage2(),
                new InstructionPage3()

            };

            // Load the first page
            LoadCurrentPage();
        }

        /// <summary>
        /// Loads the current instruction page into the InstructionFrame.
        /// Updates the state of the navigation buttons.
        /// </summary>
        private void LoadCurrentPage()
        {
            InstructionFrame.Navigate(instructionPages[currentPageIndex]);
            UpdateButtonStates();
        }

        /// <summary>
        /// Updates the enabled state of the navigation buttons based on the current page index.
        /// </summary>
        private void UpdateButtonStates()
        {
            BackButton.IsEnabled = currentPageIndex > 0;
            BackButton.Visibility = currentPageIndex > 0 ? Visibility.Visible : Visibility.Collapsed;
            NextButton.IsEnabled = currentPageIndex < instructionPages.Length - 1;
            NextButton.Visibility = currentPageIndex < instructionPages.Length - 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for the Back button click.
        /// Plays a button click sound and navigates to the previous instruction page if available.
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                LoadCurrentPage();
            }
        }

        /// <summary>
        /// Event handler for the Next button click.
        /// Plays a button click sound and navigates to the next instruction page if available.
        /// </summary>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer.PlaySound(SoundConstants.BOTTON_CLICK);

            if (currentPageIndex < instructionPages.Length - 1)
            {
                currentPageIndex++;
                LoadCurrentPage();
            }
        }
    }
}
