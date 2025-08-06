using CardGames.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    /// Interaction logic for CardUserControl.xaml
    /// </summary>
    public partial class CardUserControl : UserControl
    {
        public Card Card { get; set; }

        public CardUserControl()
        {
            InitializeComponent();
        }

        public void SetupCard(Card card)
        {
            Card = card;
            string fileName = "1920px-Playing_card_" + card.Suite.ToString().ToLower() + "_" + card.Number.ToString().Replace("_", "") + ".svg.png";
            
            // Use relative path from the application directory
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            string fullPath = Path.Combine(basePath, fileName);
            string backCardPath = Path.Combine(basePath, "cardback1.jpg");
            
            try
            {
                if (File.Exists(fullPath))
                {
                    PicCard.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                }
                else
                {
                    // Fallback to relative URI
                    PicCard.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}"));
                }
                
                if (File.Exists(backCardPath))
                {
                    PicBack.Source = new BitmapImage(new Uri(backCardPath, UriKind.Absolute));
                }
                else
                {
                    // Fallback to relative URI
                    PicBack.Source = new BitmapImage(new Uri("pack://application:,,,/cardback1.jpg"));
                }
            }
            catch
            {
                // Use fallback images if file operations fail
                try
                {
                    PicCard.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}"));
                    PicBack.Source = new BitmapImage(new Uri("pack://application:,,,/cardback1.jpg"));
                }
                catch
                {
                    // If all else fails, leave default images
                }
            }
            
            IsFaceUp = false;
        }

        private bool _cardVisible;
        public bool IsFaceUp
        {
            get
            {
                return _cardVisible;
            }
            set
            {
                _cardVisible = value;
                if (_cardVisible == true)
                {
                    PicBack.Visibility = Visibility.Hidden;
                    PicCard.Visibility = Visibility.Visible;
                }
                else
                {
                    PicBack.Visibility = Visibility.Visible;
                    PicCard.Visibility = Visibility.Hidden;
                }
            }
        }

        private void PicBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Enable drag and drop
                this.Cursor = Cursors.Hand;
                DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
                this.Cursor = Cursors.Arrow;
            }
        }

        private void PicBack_Click(object sender, RoutedEventArgs e)
        {
            // Toggle card face up/down on click
            IsFaceUp = !IsFaceUp;
        }
    }
}
