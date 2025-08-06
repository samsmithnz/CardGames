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

namespace CardGames
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Deck deck;
        private SolitaireRules solitaireRules;
        private CardUserControl dragSourceControl;

        public MainWindow()
        {
            InitializeComponent();
            deck = new Deck();
            solitaireRules = new SolitaireRules();
            deck.Shuffle();
            SetupCardEvents();
            LoadCards();
        }

        private void SetupCardEvents()
        {
            // Wire up drag and drop events for all card controls
            Card1.CardDragStarted += OnCardDragStarted;
            Card1.CardDropped += OnCardDropped;
            Card1.ValidateDrop += OnValidateDrop;
            
            Card2.CardDragStarted += OnCardDragStarted;
            Card2.CardDropped += OnCardDropped;
            Card2.ValidateDrop += OnValidateDrop;
            
            Card3.CardDragStarted += OnCardDragStarted;
            Card3.CardDropped += OnCardDropped;
            Card3.ValidateDrop += OnValidateDrop;
            
            Card4.CardDragStarted += OnCardDragStarted;
            Card4.CardDropped += OnCardDropped;
            Card4.ValidateDrop += OnValidateDrop;
        }

        private void OnCardDragStarted(object sender, Card card)
        {
            dragSourceControl = sender as CardUserControl;
            StatusLabel.Content = $"Dragging {card.Number} of {card.Suite}s";
        }

        private void OnValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Validate the move
            string validationResult = ValidateMoveDetailed(e.DraggedCard, e.TargetControl);
            e.IsValid = validationResult == "Valid";
        }

        private void OnCardDropped(object sender, CardDropEventArgs e)
        {
            CardUserControl targetControl = e.TargetControl;
            Card droppedCard = e.DroppedCard;
            
            // Validate the move and provide detailed feedback
            string validationResult = ValidateMoveDetailed(droppedCard, targetControl);
            
            if (validationResult == "Valid")
            {
                // Perform the move
                ExecuteMove(dragSourceControl, targetControl, droppedCard);
                StatusLabel.Content = $"Moved {droppedCard.Number} of {droppedCard.Suite}s successfully";
            }
            else
            {
                StatusLabel.Content = $"Invalid move: {validationResult}";
            }
            
            dragSourceControl = null;
        }

        private string ValidateMoveDetailed(Card card, CardUserControl targetControl)
        {
            // For demonstration purposes, implement basic solitaire-like rules
            if (targetControl.Card == null)
            {
                // Empty space - only allow Kings
                if (card.Number == Card.CardNumber.K)
                {
                    return "Valid";
                }
                else
                {
                    return "Only Kings can be placed on empty spaces";
                }
            }
            
            Card targetCard = targetControl.Card;
            
            // Check rank
            if (!IsOneRankLower(card.Number, targetCard.Number))
            {
                return $"{card.Number} cannot be placed on {targetCard.Number} - must be one rank lower";
            }
            
            // Check color
            if (!IsOppositeColor(card, targetCard))
            {
                return $"{GetColorName(card)} {card.Number} cannot be placed on {GetColorName(targetCard)} {targetCard.Number} - must be opposite color";
            }
            
            return "Valid";
        }

        private string GetColorName(Card card)
        {
            bool isRed = card.Suite == Card.CardSuite.Heart || card.Suite == Card.CardSuite.Diamond;
            return isRed ? "Red" : "Black";
        }

        private bool IsOneRankLower(Card.CardNumber lower, Card.CardNumber higher)
        {
            return (int)lower == (int)higher - 1;
        }

        private bool IsOppositeColor(Card card1, Card card2)
        {
            bool card1IsRed = card1.Suite == Card.CardSuite.Heart || card1.Suite == Card.CardSuite.Diamond;
            bool card2IsRed = card2.Suite == Card.CardSuite.Heart || card2.Suite == Card.CardSuite.Diamond;
            return card1IsRed != card2IsRed;
        }

        private void ExecuteMove(CardUserControl sourceControl, CardUserControl targetControl, Card card)
        {
            if (sourceControl != null && targetControl != null && sourceControl != targetControl)
            {
                // Swap the cards between controls
                Card tempCard = targetControl.Card;
                targetControl.SetupCard(card);
                
                // If there was a card in the target, put it in the source
                if (tempCard != null)
                {
                    sourceControl.SetupCard(tempCard);
                }
                else
                {
                    // Clear the source if target was empty
                    sourceControl.Card = null;
                    sourceControl.IsFaceUp = false;
                }
                
                // Ensure proper face-up state
                targetControl.IsFaceUp = true;
            }
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
            
            StatusLabel.Content = $"Total cards: {deck.Cards.Count} - Drag cards to move them! Kings can go on empty spaces.";
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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset all cards to initial state
            LoadCards();
            StatusLabel.Content = "Game reset - drag cards to move them!";
        }
    }
}
