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
    /// Interaction logic for MainWindow.xaml - Solitaire Game
    /// </summary>
    public partial class MainWindow : Window
    {
        private Deck deck;
        private SolitaireRules solitaireRules;
        private CardUserControl dragSourceControl;
        
        // Collections to manage all card controls by pile type
        private List<CardUserControl> foundationControls;
        private List<List<CardUserControl>> tableauControls;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }
        
        /// <summary>
        /// Initialize the game components and layout
        /// </summary>
        private void InitializeGame()
        {
            deck = new Deck();
            solitaireRules = new SolitaireRules();
            
            // Initialize control collections
            foundationControls = new List<CardUserControl> { Foundation1, Foundation2, Foundation3, Foundation4 };
            tableauControls = new List<List<CardUserControl>>
            {
                new List<CardUserControl> { Tableau1_1 },
                new List<CardUserControl> { Tableau2_1, Tableau2_2 },
                new List<CardUserControl> { Tableau3_1, Tableau3_2, Tableau3_3 },
                new List<CardUserControl> { Tableau4_1, Tableau4_2, Tableau4_3, Tableau4_4 },
                new List<CardUserControl> { Tableau5_1, Tableau5_2, Tableau5_3, Tableau5_4, Tableau5_5 },
                new List<CardUserControl> { Tableau6_1, Tableau6_2, Tableau6_3, Tableau6_4, Tableau6_5, Tableau6_6 },
                new List<CardUserControl> { Tableau7_1, Tableau7_2, Tableau7_3, Tableau7_4, Tableau7_5, Tableau7_6, Tableau7_7 }
            };
            
            SetupCardEvents();
            
            // Automatically start a new game when the program starts
            StartNewGame();
        }

        
        /// <summary>
        /// Wire up drag and drop events for all card controls
        /// </summary>
        private void SetupCardEvents()
        {
            // Setup events for stock and waste piles
            StockPile.CardDragStarted += OnCardDragStarted;
            StockPile.CardDropped += OnCardDropped;
            StockPile.ValidateDrop += OnValidateDrop;
            StockPile.IsStockPile = true;
            StockPile.StockPileClicked += OnStockPileClicked;
            
            WastePile.CardDragStarted += OnCardDragStarted;
            WastePile.CardDropped += OnCardDropped;
            WastePile.ValidateDrop += OnValidateDrop;
            
            // Setup events for foundation piles
            foreach (CardUserControl foundation in foundationControls)
            {
                foundation.CardDragStarted += OnCardDragStarted;
                foundation.CardDropped += OnCardDropped;
                foundation.ValidateDrop += OnValidateDrop;
            }
            
            // Setup events for tableau controls
            foreach (List<CardUserControl> column in tableauControls)
            {
                foreach (CardUserControl card in column)
                {
                    card.CardDragStarted += OnCardDragStarted;
                    card.CardDropped += OnCardDropped;
                    card.ValidateDrop += OnValidateDrop;
                }
            }
        }

        /// <summary>
        /// Start a new game of Solitaire
        /// </summary>
        private void StartNewGame()
        {
            // Clear all existing cards from UI
            ClearAllCards();
            
            // Shuffle deck and deal new game
            deck.Shuffle();
            solitaireRules.DealCards(deck);
            
            // Display dealt cards on the table
            DisplayGame();
            
            StatusLabel.Content = "New game started! Drag cards to move them.";
        }

        /// <summary>
        /// Start a new game of Solitaire (button click handler)
        /// </summary>
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
        
        /// <summary>
        /// Draw a card from stock to waste pile
        /// </summary>
        private void DrawCardButton_Click(object sender, RoutedEventArgs e)
        {
            DrawCardFromStock();
        }

        /// <summary>
        /// Handle stock pile clicks to draw cards
        /// </summary>
        private void OnStockPileClicked(object sender, EventArgs e)
        {
            DrawCardFromStock();
        }

        /// <summary>
        /// Draw a card from stock to waste pile - shared logic
        /// </summary>
        private void DrawCardFromStock()
        {
            if (solitaireRules.StockPile.Count > 0)
            {
                // Move top card from stock to waste
                Card drawnCard = solitaireRules.StockPile[solitaireRules.StockPile.Count - 1];
                solitaireRules.StockPile.RemoveAt(solitaireRules.StockPile.Count - 1);
                solitaireRules.WastePile.Add(drawnCard);
                
                // Update UI
                UpdateStockPile();
                UpdateWastePile();
                
                StatusLabel.Content = $"Drew {drawnCard.Number} of {drawnCard.Suite}s";
            }
            else if (solitaireRules.WastePile.Count > 0)
            {
                // Reset stock pile from waste pile (when stock is empty)
                while (solitaireRules.WastePile.Count > 0)
                {
                    Card card = solitaireRules.WastePile[solitaireRules.WastePile.Count - 1];
                    solitaireRules.WastePile.RemoveAt(solitaireRules.WastePile.Count - 1);
                    solitaireRules.StockPile.Add(card);
                }
                
                UpdateStockPile();
                UpdateWastePile();
                StatusLabel.Content = "Stock pile reset";
            }
            else
            {
                StatusLabel.Content = "No more cards to draw";
            }
        }
        
        /// <summary>
        /// Reset the current game
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ClearAllCards();
            StatusLabel.Content = "Game reset. Click 'New Game' to start playing.";
        }

        
        /// <summary>
        /// Clear all cards from the UI
        /// </summary>
        private void ClearAllCards()
        {
            // Clear stock and waste piles
            StockPile.Card = null;
            StockPile.IsFaceUp = false;
            WastePile.Card = null;
            WastePile.IsFaceUp = false;
            
            // Clear foundation piles
            foreach (CardUserControl foundation in foundationControls)
            {
                foundation.Card = null;
                foundation.IsFaceUp = false;
            }
            
            // Clear tableau columns
            foreach (List<CardUserControl> column in tableauControls)
            {
                foreach (CardUserControl card in column)
                {
                    card.Card = null;
                    card.IsFaceUp = false;
                    card.Visibility = Visibility.Hidden;
                }
            }
        }
        
        /// <summary>
        /// Display the current game state on the table
        /// </summary>
        private void DisplayGame()
        {
            // Display stock pile
            UpdateStockPile();
            
            // Display waste pile 
            UpdateWastePile();
            
            // Display foundation piles
            for (int i = 0; i < foundationControls.Count; i++)
            {
                if (solitaireRules.FoundationPiles[i].Count > 0)
                {
                    Card topCard = solitaireRules.FoundationPiles[i][solitaireRules.FoundationPiles[i].Count - 1];
                    foundationControls[i].SetupCard(topCard);
                    foundationControls[i].IsFaceUp = true;
                }
            }
            
            // Display tableau columns
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                List<Card> columnCards = solitaireRules.TableauColumns[col];
                List<CardUserControl> columnControls = tableauControls[col];
                
                for (int row = 0; row < columnControls.Count; row++)
                {
                    if (row < columnCards.Count)
                    {
                        Card card = columnCards[row];
                        columnControls[row].SetupCard(card);
                        // Only the last card in each column should be face-up initially
                        columnControls[row].IsFaceUp = (row == columnCards.Count - 1);
                        columnControls[row].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        columnControls[row].Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        
        /// <summary>
        /// Update the stock pile display
        /// </summary>
        private void UpdateStockPile()
        {
            if (solitaireRules.StockPile.Count > 0)
            {
                // Show face-down card back for stock pile
                StockPile.SetupCard(solitaireRules.StockPile[solitaireRules.StockPile.Count - 1]);
                StockPile.IsFaceUp = false;
            }
            else
            {
                StockPile.Card = null;
            }
        }
        
        /// <summary>
        /// Update the waste pile display
        /// </summary>
        private void UpdateWastePile()
        {
            if (solitaireRules.WastePile.Count > 0)
            {
                Card topCard = solitaireRules.WastePile[solitaireRules.WastePile.Count - 1];
                WastePile.SetupCard(topCard);
                WastePile.IsFaceUp = true;
            }
            else
            {
                WastePile.Card = null;
            }
        }

        
        /// <summary>
        /// Handle when a card drag operation starts
        /// </summary>
        private void OnCardDragStarted(object sender, Card card)
        {
            dragSourceControl = sender as CardUserControl;
            StatusLabel.Content = $"Dragging {card.Number} of {card.Suite}s";
        }

        /// <summary>
        /// Validate if a card can be dropped on a target
        /// </summary>
        private void OnValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Validate the move using Solitaire rules
            string validationResult = ValidateMoveDetailed(e.DraggedCard, e.TargetControl);
            e.IsValid = validationResult == "Valid";
        }

        /// <summary>
        /// Handle when a card is dropped
        /// </summary>
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
                
                // Check for game win condition
                if (solitaireRules.IsGameWon())
                {
                    StatusLabel.Content = "Congratulations! You won the game!";
                }
                else
                {
                    StatusLabel.Content = $"Moved {droppedCard.Number} of {droppedCard.Suite}s successfully";
                }
            }
            else
            {
                StatusLabel.Content = $"Invalid move: {validationResult}";
            }
            
            dragSourceControl = null;
        }

        /// <summary>
        /// Execute a valid move between card piles
        /// </summary>
        private void ExecuteMove(CardUserControl sourceControl, CardUserControl targetControl, Card card)
        {
            // Check if moving to foundation pile
            if (foundationControls.Contains(targetControl))
            {
                int foundationIndex = foundationControls.IndexOf(targetControl);
                
                // Remove card from source
                RemoveCardFromSource(sourceControl, card);
                
                // Add to foundation pile
                solitaireRules.FoundationPiles[foundationIndex].Add(card);
                targetControl.SetupCard(card);
                targetControl.IsFaceUp = true;
                
                return;
            }
            
            // Check if moving to tableau
            int targetColumnIndex = GetTableauColumnIndex(targetControl);
            if (targetColumnIndex >= 0)
            {
                // Remove card from source
                RemoveCardFromSource(sourceControl, card);
                
                // Add to target tableau column
                solitaireRules.TableauColumns[targetColumnIndex].Add(card);
                
                // Refresh the display for the target column to show proper stacking
                RefreshTableauColumn(targetColumnIndex);
                
                return;
            }
            
            // Handle other pile types (waste, stock, etc.)
            if (targetControl.Card == null)
            {
                // Move to empty space
                targetControl.SetupCard(card);
                targetControl.IsFaceUp = true;
                sourceControl.Card = null;
            }
            else
            {
                // Move to existing card (should stack)
                targetControl.SetupCard(card);
                targetControl.IsFaceUp = true;
                sourceControl.Card = null;
            }
        }

        /// <summary>
        /// Get the tableau column index for a given CardUserControl
        /// </summary>
        /// <param name="control">The CardUserControl to find</param>
        /// <returns>The column index (0-6) or -1 if not found in tableau</returns>
        private int GetTableauColumnIndex(CardUserControl control)
        {
            for (int col = 0; col < tableauControls.Count; col++)
            {
                if (tableauControls[col].Contains(control))
                {
                    return col;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get the position within a tableau column for a given CardUserControl
        /// </summary>
        /// <param name="control">The CardUserControl to find</param>
        /// <returns>The position index within the column or -1 if not found</returns>
        private int GetTableauPositionIndex(CardUserControl control)
        {
            int columnIndex = GetTableauColumnIndex(control);
            if (columnIndex >= 0)
            {
                return tableauControls[columnIndex].IndexOf(control);
            }
            return -1;
        }

        /// <summary>
        /// Remove a card from its source location (tableau, foundation, waste, etc.)
        /// </summary>
        /// <param name="sourceControl">The source CardUserControl</param>
        /// <param name="card">The card being moved</param>
        private void RemoveCardFromSource(CardUserControl sourceControl, Card card)
        {
            // Check if removing from tableau
            int sourceColumnIndex = GetTableauColumnIndex(sourceControl);
            if (sourceColumnIndex >= 0)
            {
                List<Card> sourceColumn = solitaireRules.TableauColumns[sourceColumnIndex];
                if (sourceColumn.Count > 0 && sourceColumn[sourceColumn.Count - 1] == card)
                {
                    sourceColumn.RemoveAt(sourceColumn.Count - 1);
                    
                    // Refresh the source column display
                    RefreshTableauColumn(sourceColumnIndex);
                    
                    // If there are still cards in the column, make the new top card face-up
                    if (sourceColumn.Count > 0)
                    {
                        // The newly exposed card should be face-up
                        CardUserControl newTopControl = tableauControls[sourceColumnIndex][sourceColumn.Count - 1];
                        newTopControl.IsFaceUp = true;
                    }
                }
                return;
            }
            
            // Check if removing from foundation
            if (foundationControls.Contains(sourceControl))
            {
                int foundationIndex = foundationControls.IndexOf(sourceControl);
                List<Card> foundation = solitaireRules.FoundationPiles[foundationIndex];
                if (foundation.Count > 0 && foundation[foundation.Count - 1] == card)
                {
                    foundation.RemoveAt(foundation.Count - 1);
                    
                    // Update foundation display
                    if (foundation.Count > 0)
                    {
                        sourceControl.SetupCard(foundation[foundation.Count - 1]);
                        sourceControl.IsFaceUp = true;
                    }
                    else
                    {
                        sourceControl.Card = null;
                    }
                }
                return;
            }
            
            // Check if removing from waste pile
            if (sourceControl == WastePile)
            {
                if (solitaireRules.WastePile.Count > 0 && solitaireRules.WastePile[solitaireRules.WastePile.Count - 1] == card)
                {
                    solitaireRules.WastePile.RemoveAt(solitaireRules.WastePile.Count - 1);
                    UpdateWastePile();
                }
                return;
            }
            
            // For other sources, just clear the UI control
            sourceControl.Card = null;
        }

        /// <summary>
        /// Refresh the display of a tableau column to show all cards properly stacked
        /// </summary>
        /// <param name="columnIndex">The column index to refresh</param>
        private void RefreshTableauColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= tableauControls.Count)
            {
                return;
            }
            
            List<Card> columnCards = solitaireRules.TableauColumns[columnIndex];
            List<CardUserControl> columnControls = tableauControls[columnIndex];
            
            for (int row = 0; row < columnControls.Count; row++)
            {
                if (row < columnCards.Count)
                {
                    Card card = columnCards[row];
                    columnControls[row].SetupCard(card);
                    // Only the last card in each column should be face-up
                    columnControls[row].IsFaceUp = (row == columnCards.Count - 1);
                    columnControls[row].Visibility = Visibility.Visible;
                }
                else
                {
                    columnControls[row].Card = null;
                    columnControls[row].Visibility = Visibility.Hidden;
                }
            }
        }

        
        /// <summary>
        /// Validate a move with detailed feedback
        /// </summary>
        private string ValidateMoveDetailed(Card card, CardUserControl targetControl)
        {
            if (card == null)
            {
                return "No card to move";
            }
            
            // Check if moving to foundation pile
            if (foundationControls.Contains(targetControl))
            {
                int foundationIndex = foundationControls.IndexOf(targetControl);
                if (solitaireRules.CanPlaceCardOnFoundation(card, foundationIndex))
                {
                    return "Valid";
                }
                else
                {
                    return "Card cannot be placed on this foundation pile";
                }
            }
            
            // Check if moving to tableau (implement basic tableau rules)
            if (targetControl.Card == null)
            {
                // Empty tableau space - only allow Kings
                if (card.Number == Card.CardNumber.K)
                {
                    return "Valid";
                }
                else
                {
                    return "Only Kings can be placed on empty tableau spaces";
                }
            }
            
            Card targetCard = targetControl.Card;
            
            // Check rank (must be one lower)
            if (!IsOneRankLower(card.Number, targetCard.Number))
            {
                return $"{card.Number} cannot be placed on {targetCard.Number} - must be one rank lower";
            }
            
            // Check color (must be opposite)
            if (!IsOppositeColor(card, targetCard))
            {
                return $"{GetColorName(card)} {card.Number} cannot be placed on {GetColorName(targetCard)} {targetCard.Number} - must be opposite color";
            }
            
            return "Valid";
        }

        /// <summary>
        /// Get the color name of a card for display purposes
        /// </summary>
        private string GetColorName(Card card)
        {
            return (card.Suite == Card.CardSuite.Heart || card.Suite == Card.CardSuite.Diamond) ? "Red" : "Black";
        }

        /// <summary>
        /// Check if one card rank is exactly one lower than another
        /// </summary>
        private bool IsOneRankLower(Card.CardNumber lower, Card.CardNumber higher)
        {
            // Convert card numbers to integers for comparison
            int lowerValue = GetCardValue(lower);
            int higherValue = GetCardValue(higher);
            
            return lowerValue == higherValue - 1;
        }

        /// <summary>
        /// Check if two cards have opposite colors
        /// </summary>
        private bool IsOppositeColor(Card card1, Card card2)
        {
            bool card1IsRed = (card1.Suite == Card.CardSuite.Heart || card1.Suite == Card.CardSuite.Diamond);
            bool card2IsRed = (card2.Suite == Card.CardSuite.Heart || card2.Suite == Card.CardSuite.Diamond);
            
            return card1IsRed != card2IsRed;
        }

        /// <summary>
        /// Get numeric value of card for comparison
        /// </summary>
        private int GetCardValue(Card.CardNumber cardNumber)
        {
            switch (cardNumber)
            {
                case Card.CardNumber.A: return 1;
                case Card.CardNumber._2: return 2;
                case Card.CardNumber._3: return 3;
                case Card.CardNumber._4: return 4;
                case Card.CardNumber._5: return 5;
                case Card.CardNumber._6: return 6;
                case Card.CardNumber._7: return 7;
                case Card.CardNumber._8: return 8;
                case Card.CardNumber._9: return 9;
                case Card.CardNumber._10: return 10;
                case Card.CardNumber.J: return 11;
                case Card.CardNumber.Q: return 12;
                case Card.CardNumber.K: return 13;
                default: return 0;
            }
        }
    }
}
