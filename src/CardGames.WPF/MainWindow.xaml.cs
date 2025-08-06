using CardGames.Core;
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

namespace CardGames.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Deck deck;

        public MainWindow()
        {
            InitializeComponent();
            deck = new Deck();
            deck.Shuffle();
            LoadCards();
        }

        private void LoadCards()
        {
            // Display first few cards as an example
            Card1.SetupCard(deck.Cards[0]);
            Card2.SetupCard(deck.Cards[1]);
            Card3.SetupCard(deck.Cards[2]);
            Card4.SetupCard(deck.Cards[3]);
            
            // Show some cards face up
            Card1.IsFaceUp = true;
            Card3.IsFaceUp = true;
            
            StatusLabel.Content = $"Total cards: {deck.Cards.Count}";
        }

        private void FlipButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle all cards
            Card1.IsFaceUp = !Card1.IsFaceUp;
            Card2.IsFaceUp = !Card2.IsFaceUp;
            Card3.IsFaceUp = !Card3.IsFaceUp;
            Card4.IsFaceUp = !Card4.IsFaceUp;
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            deck.Shuffle();
            LoadCards();
        }
    }
}
