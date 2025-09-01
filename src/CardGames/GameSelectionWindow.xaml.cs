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
        /// Indicates whether the user confirmed their selection (separate from WPF's DialogResult)
        /// </summary>
        public bool UserConfirmed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the GameSelectionWindow
        /// </summary>
        public GameSelectionWindow()
        {
            InitializeComponent();
            SelectedGameName = "Klondike Solitaire"; // Default selection
            UserConfirmed = false;
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

            UserConfirmed = true;
            // Set the WPF Window.DialogResult so ShowDialog() returns true
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handle Cancel button click
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = false;
            DialogResult = false; // Ensure ShowDialog returns false
            Close();
        }
    }
}