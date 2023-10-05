using CardGames.Core;
using System;
using System.Collections.Generic;
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
            //lblTopLeftNumber.Text = card.Number.ToString().Replace("_", "");
            //lblSuite.Text = card.Suite.ToString();
            string path = @"C:\Users\samsm\source\repos\CardGames\CardGames\CardGames.WPF\Images\";
            string fileName = "1920px-Playing_card_" + card.Suite.ToString().ToLower() + "_" + card.Number.ToString().Replace("_", "") + ".svg.png";
            PicCard.Source = new BitmapImage(new Uri(path + fileName));
            PicBack.Source = new BitmapImage(new Uri(path + "cardback1.jpg"));
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
                //lblTopLeftNumber.Visible = _cardVisible;
                //lblSuite.Visible = _cardVisible;
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
            //    this.DoDragDrop(sender, DragDropEffects.Move);
        }
        private void PicBack_Click(object sender, RoutedEventArgs e)
        {
            IsFaceUp = !IsFaceUp;
            //    this.DoDragDrop(sender, DragDropEffects.Move);
        }
    }
}
