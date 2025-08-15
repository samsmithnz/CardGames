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

namespace CardGames
{
    /// <summary>
    /// Interaction logic for CardUserControl.xaml
    /// </summary>
    public partial class CardUserControl : UserControl
    {
        private Card _card;
        public Card Card 
        { 
            get { return _card; }
            set 
            { 
                _card = value;
                // Update visibility when card changes
                UpdateVisibility();
            }
        }

        /// <summary>
        /// Event raised when a card is being dragged from this control
        /// </summary>
        public event EventHandler<Card> CardDragStarted;

        /// <summary>
        /// Event raised when a card is dropped onto this control
        /// </summary>
        public event EventHandler<CardDropEventArgs> CardDropped;

        /// <summary>
        /// Event raised to validate if a drop is allowed
        /// </summary>
        public event EventHandler<ValidateDropEventArgs> ValidateDrop;

        /// <summary>
        /// Event raised when the stock pile card is clicked to draw a card
        /// </summary>
        public event EventHandler<EventArgs> StockPileClicked;

        /// <summary>
        /// Indicates if this card control represents the stock pile
        /// </summary>
        public bool IsStockPile { get; set; }

        public CardUserControl()
        {
            InitializeComponent();
            AllowDrop = true;
        }

        public void SetupCard(Card card)
        {
            Card = card;
            //lblTopLeftNumber.Text = card.Number.ToString().Replace("_", "");
            //lblSuite.Text = card.Suite.ToString();
            string fileName = "1920px-Playing_card_" + card.Suite.ToString().ToLower() + "_" + card.Number.ToString().Replace("_", "") + ".svg.png";
            
            try
            {
                PicCard.Source = new BitmapImage(new Uri($"Images/{fileName}", UriKind.Relative));
                PicBack.Source = new BitmapImage(new Uri("Images/cardback1.jpg", UriKind.Relative));
            }
            catch (System.Exception)
            {
                // Fallback to pack URI if relative path doesn't work
                PicCard.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}"));
                PicBack.Source = new BitmapImage(new Uri("pack://application:,,,/Images/cardback1.jpg"));
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
                UpdateVisibility();
            }
        }

        /// <summary>
        /// Updates the visibility of card images based on current state
        /// </summary>
        private void UpdateVisibility()
        {
            // If there's no card, hide both images
            if (Card == null)
            {
                PicBack.Visibility = Visibility.Hidden;
                PicCard.Visibility = Visibility.Hidden;
            }
            else if (_cardVisible == true)
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

        private void PicBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Card != null)
            {
                // Raise the drag started event
                CardDragStarted?.Invoke(this, Card);
                
                // Initiate drag and drop operation
                DragDrop.DoDragDrop(this, Card, DragDropEffects.Move);
            }
        }

        private void PicBack_Click(object sender, RoutedEventArgs e)
        {
            if (IsStockPile)
            {
                // For stock pile, raise the stock pile clicked event instead of flipping
                StockPileClicked?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // For other cards, flip face up/down as before
                IsFaceUp = !IsFaceUp;
            }
        }

        private void CardUserControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                Card draggedCard = (Card)e.Data.GetData(typeof(Card));
                
                // Request validation from parent
                ValidateDropEventArgs args = new ValidateDropEventArgs(draggedCard, this);
                ValidateDrop?.Invoke(this, args);
                
                // Highlight based on validation result
                if (args.IsValid)
                {
                    this.Background = new SolidColorBrush(Colors.LightGreen);
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    this.Background = new SolidColorBrush(Colors.LightCoral);
                    e.Effects = DragDropEffects.None;
                }
                
                this.Opacity = 0.8;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void CardUserControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void CardUserControl_DragLeave(object sender, DragEventArgs e)
        {
            // Remove highlight when drag leaves the control
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.Opacity = 1.0;
            e.Handled = true;
        }

        private void CardUserControl_Drop(object sender, DragEventArgs e)
        {
            // Remove highlight after drop
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.Opacity = 1.0;
            
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                Card droppedCard = (Card)e.Data.GetData(typeof(Card));
                CardDropped?.Invoke(this, new CardDropEventArgs(droppedCard, this));
            }
            e.Handled = true;
        }
    }

    /// <summary>
    /// Event arguments for card drop operations
    /// </summary>
    public class CardDropEventArgs : EventArgs
    {
        public Card DroppedCard { get; }
        public CardUserControl TargetControl { get; }

        public CardDropEventArgs(Card droppedCard, CardUserControl targetControl)
        {
            DroppedCard = droppedCard;
            TargetControl = targetControl;
        }
    }

    /// <summary>
    /// Event arguments for validating drop operations
    /// </summary>
    public class ValidateDropEventArgs : EventArgs
    {
        public Card DraggedCard { get; }
        public CardUserControl TargetControl { get; }
        public bool IsValid { get; set; }

        public ValidateDropEventArgs(Card draggedCard, CardUserControl targetControl)
        {
            DraggedCard = draggedCard;
            TargetControl = targetControl;
            IsValid = false; // Default to invalid
        }
    }
}
