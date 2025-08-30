using System.Windows;

namespace CardGames
{
    /// <summary>
    /// Interaction logic for GameSelectionWindow.xaml
    /// </summary>
    public partial class GameSelectionWindow : Window
    {
        /// <summary>
        /// Gets the selected game name based on user choice
        /// </summary>
        public string SelectedGameName { get; private set; }

        /// <summary>
        /// Gets whether the user confirmed their selection
        /// </summary>
        public bool DialogResult { get; private set; }

        /// <summary>
        /// Initializes a new instance of the GameSelectionWindow
        /// </summary>
        public GameSelectionWindow()
        {
            InitializeComponent();
            SelectedGameName = "Klondike Solitaire"; // Default selection
            DialogResult = false;
        }

        /// <summary>
        /// Handle OK button click
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (KlondikeRadio.IsChecked == true)
            {
                SelectedGameName = "Klondike Solitaire";
            }
            else if (FreecellRadio.IsChecked == true)
            {
                SelectedGameName = "Freecell";
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handle Cancel button click
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}