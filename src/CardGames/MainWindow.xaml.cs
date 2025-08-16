using System.IO;
using Microsoft.Win32;
using CardGames.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        // Track which positions in each tableau column should be face-up
        // This preserves face-up state during card moves and stacking
        private List<List<bool>> tableauFaceUpStates;

        // Debug toggle
        private bool debugEnabled = true;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }
        
        /// <summary>
        /// Write a debug line to Output window
        /// </summary>
        private void DebugLog(string message)
        {
            if (debugEnabled)
            {
                Debug.WriteLine($"[DEBUG] {message}");
            }
        }

        /// <summary>
        /// Friendly description of a card
        /// </summary>
        private string DescribeCard(Card card)
        {
            if (card == null)
            {
                return "(no card)";
            }
            return $"{card.Number} of {card.Suite}s";
        }

        /// <summary>
        /// Friendly description of where a control lives
        /// </summary>
        private string DescribeControl(CardUserControl control)
        {
            if (control == null)
            {
                return "(null control)";
            }
            if (ReferenceEquals(control, StockPile))
            {
                return "StockPile";
            }
            if (ReferenceEquals(control, WastePile))
            {
                return "WastePile";
            }
            int fIndex = foundationControls != null ? foundationControls.IndexOf(control) : -1;
            if (fIndex >= 0)
            {
                return $"Foundation[{fIndex}]";
            }
            int tCol = GetTableauColumnIndex(control);
            if (tCol >= 0)
            {
                int tRow = GetTableauPositionIndex(control);
                return $"Tableau[col={tCol}, row={tRow}]";
            }
            return "UnknownControl";
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
            
            // Initialize face-up state tracking for tableau columns
            tableauFaceUpStates = new List<List<bool>>();
            for (int i = 0; i < 7; i++)
            {
                tableauFaceUpStates.Add(new List<bool>());
                // Initialize with enough slots for maximum possible cards initially (based on UI controls)
                for (int j = 0; j < tableauControls[i].Count; j++)
                {
                    tableauFaceUpStates[i].Add(false);
                }
            }
            
            SetupCardEvents();
            
            // Automatically start a new game when the program starts
            StartNewGame();
        }

        /// <summary>
        /// Wire up drag/drop and click events for all card controls
        /// </summary>
        private void SetupCardEvents()
        {
            // Stock pile
            StockPile.CardDragStarted += OnCardDragStarted;
            StockPile.CardDropped += OnCardDropped;
            StockPile.ValidateDrop += OnValidateDrop;
            StockPile.CardClicked += OnCardClicked;
            StockPile.IsStockPile = true;
            StockPile.StockPileClicked += OnStockPileClicked;

            // Waste pile
            WastePile.CardDragStarted += OnCardDragStarted;
            WastePile.CardDropped += OnCardDropped;
            WastePile.ValidateDrop += OnValidateDrop;
            WastePile.CardClicked += OnCardClicked;

            // Foundations
            foreach (CardUserControl foundation in foundationControls)
            {
                foundation.CardDragStarted += OnCardDragStarted;
                foundation.CardDropped += OnCardDropped;
                foundation.ValidateDrop += OnValidateDrop;
                foundation.CardClicked += OnCardClicked;
            }

            // Tableau columns
            foreach (List<CardUserControl> column in tableauControls)
            {
                foreach (CardUserControl control in column)
                {
                    control.CardDragStarted += OnCardDragStarted;
                    control.CardDropped += OnCardDropped;
                    control.ValidateDrop += OnValidateDrop;
                    control.CardClicked += OnCardClicked;
                }
            }
        }

        /// <summary>
        /// Ensures the face-up state list for a column has at least the specified capacity.
        /// Adds entries initialized to false as needed.
        /// </summary>
        private void EnsureFaceUpStateCapacity(int columnIndex, int requiredCount)
        {
            if (columnIndex < 0 || columnIndex >= tableauFaceUpStates.Count)
            {
                return;
            }

            List<bool> states = tableauFaceUpStates[columnIndex];
            while (states.Count < requiredCount)
            {
                states.Add(false);
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
            
            // Initialize face-up states for new game
            InitializeTableauFaceUpStates();
            
            // Display dealt cards on the table
            DisplayGame();
            
            StatusLabel.Content = "New game started! Drag cards to move them.";
        }

        /// <summary>
        /// Initialize face-up states for tableau columns based on standard Solitaire rules
        /// Only the last card in each column should be face-up initially
        /// </summary>
        private void InitializeTableauFaceUpStates()
        {
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                List<Card> columnCards = solitaireRules.TableauColumns[col];
                List<bool> columnFaceUpStates = tableauFaceUpStates[col];
                
                // Ensure the face-up states list can represent all cards in the column
                EnsureFaceUpStateCapacity(col, columnCards.Count);

                // Reset all states to false first
                for (int row = 0; row < columnFaceUpStates.Count; row++)
                {
                    columnFaceUpStates[row] = false;
                }
                
                // Set only the last card to face-up if the column has cards
                if (columnCards.Count > 0)
                {
                    columnFaceUpStates[columnCards.Count - 1] = true;
                }
            }
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
                // Use the core method to draw a card
                bool drawn = solitaireRules.DrawFromStock();
                if (drawn && solitaireRules.WastePile.Count > 0)
                {
                    Card drawnCard = solitaireRules.WastePile[solitaireRules.WastePile.Count - 1];
                    
                    // Update UI
                    UpdateStockPile();
                    UpdateWastePile();
                    
                    StatusLabel.Content = $"Drew {drawnCard.Number} of {drawnCard.Suite}s";
                    DebugLog($"Drew card -> {DescribeCard(drawnCard)}. Waste size: {solitaireRules.WastePile.Count}");
                }
            }
            else if (solitaireRules.WastePile.Count > 0)
            {
                // Use the core method to reset stock pile from waste pile
                DebugLog("Resetting stock from waste pile");
                solitaireRules.ResetStock();
                
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
                else
                {
                    // Foundation pile is empty - clear the control
                    foundationControls[i].Card = null;
                }
            }
            
            // Display tableau columns
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                List<Card> columnCards = solitaireRules.TableauColumns[col];
                List<CardUserControl> columnControls = tableauControls[col];

                // Ensure states list can represent all current cards
                EnsureFaceUpStateCapacity(col, columnCards.Count);
                
                for (int row = 0; row < columnControls.Count; row++)
                {
                    if (row < columnCards.Count)
                    {
                        Card card = columnCards[row];
                        columnControls[row].SetupCard(card);
                        // Use the tracked face-up state
                        bool faceUp = row < tableauFaceUpStates[col].Count ? tableauFaceUpStates[col][row] : (row == columnCards.Count - 1);
                        columnControls[row].IsFaceUp = faceUp;
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
                // Keep the stock pile clickable even when empty by showing an empty card back
                // This allows clicking to reset the stock from waste pile
                StockPile.Card = null;
                StockPile.IsFaceUp = false;
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
                WastePile.IsFaceUp = false;
            }
        }

        
        /// <summary>
        /// Handle when a card drag operation starts
        /// </summary>
        private void OnCardDragStarted(object sender, Card card)
        {
            dragSourceControl = sender as CardUserControl;
            StatusLabel.Content = $"Dragging {card.Number} of {card.Suite}s";
            DebugLog($"DragStarted from {DescribeControl(dragSourceControl)} with {DescribeCard(card)}");
        }

        /// <summary>
        /// Validate if a card can be dropped on a target
        /// </summary>
        private void OnValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Validate the move using Solitaire rules
            string validationResult = ValidateMoveDetailed(e.DraggedCard, e.TargetControl);
            e.IsValid = validationResult == "Valid";
            DebugLog($"ValidateDrop: {DescribeCard(e.DraggedCard)} from {DescribeControl(dragSourceControl)} to {DescribeControl(e.TargetControl)} -> {validationResult}");
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
            DebugLog($"Drop: {DescribeCard(droppedCard)} to {DescribeControl(targetControl)} -> {validationResult}");
            
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
        /// Handle when a card is clicked to check for auto-move to foundation
        /// </summary>
        private void OnCardClicked(object sender, Card clickedCard)
        {
            CardUserControl sourceControl = sender as CardUserControl;
            if (sourceControl == null || clickedCard == null)
            {
                return;
            }

            DebugLog($"CardClicked on {DescribeControl(sourceControl)} -> {DescribeCard(clickedCard)}");

            // Try to find an available foundation pile for this card
            int foundationIndex = solitaireRules.FindAvailableFoundationPile(clickedCard);
            if (foundationIndex >= 0)
            {
                // Found a valid foundation pile - execute the move
                CardUserControl targetFoundation = foundationControls[foundationIndex];
                ExecuteMove(sourceControl, targetFoundation, clickedCard);
                
                // Check for game win condition
                if (solitaireRules.IsGameWon())
                {
                    StatusLabel.Content = "Congratulations! You won the game!";
                }
                else
                {
                    StatusLabel.Content = $"Auto-moved {clickedCard.Number} of {clickedCard.Suite}s to foundation";
                }
            }
            else
            {
                // No valid foundation move found
                StatusLabel.Content = $"{clickedCard.Number} of {clickedCard.Suite}s cannot be moved to foundation";
            }
        }

        /// <summary>
        /// Execute a valid move between card piles
        /// </summary>
        private void ExecuteMove(CardUserControl sourceControl, CardUserControl targetControl, Card card)
        {
            DebugLog($"ExecuteMove: {DescribeCard(card)} from {DescribeControl(sourceControl)} to {DescribeControl(targetControl)}");

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
                
                // Update face-up state tracking - newly added card should be face-up
                int newCardPosition = solitaireRules.TableauColumns[targetColumnIndex].Count - 1;
                EnsureFaceUpStateCapacity(targetColumnIndex, newCardPosition + 1);
                tableauFaceUpStates[targetColumnIndex][newCardPosition] = true;
                
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
                // Check if this is a tableau move - try a more comprehensive detection
                int targetColumnIndexForExistingCard = GetTableauColumnIndex(targetControl);
                bool isTableauMove = (targetColumnIndexForExistingCard >= 0) || IsTableauCardControl(targetControl);
                
                if (isTableauMove)
                {
                    // This is a tableau move - handle it properly
                    if (targetColumnIndexForExistingCard < 0)
                    {
                        // Detection failed but we know it's a tableau card - find it manually
                        targetColumnIndexForExistingCard = FindTableauColumnForCard(targetControl);
                    }
                    
                    if (targetColumnIndexForExistingCard >= 0)
                    {
                        RemoveCardFromSource(sourceControl, card);
                        
                        // Add to target tableau column
                        solitaireRules.TableauColumns[targetColumnIndexForExistingCard].Add(card);
                        
                        // Update face-up state tracking - newly added card should be face-up
                        int newCardPosition = solitaireRules.TableauColumns[targetColumnIndexForExistingCard].Count - 1;
                        EnsureFaceUpStateCapacity(targetColumnIndexForExistingCard, newCardPosition + 1);
                        tableauFaceUpStates[targetColumnIndexForExistingCard][newCardPosition] = true;
                        
                        // Refresh the display for the target column to show proper stacking
                        RefreshTableauColumn(targetColumnIndexForExistingCard);
                    }
                }
                else
                {
                    // Non-tableau move - replace the card (for waste pile, etc.)
                    targetControl.SetupCard(card);
                    targetControl.IsFaceUp = true;
                    sourceControl.Card = null;
                }
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
        /// Check if a CardUserControl is part of any tableau column
        /// This provides a more comprehensive check than GetTableauColumnIndex
        /// </summary>
        /// <param name="control">The CardUserControl to check</param>
        /// <returns>True if the control is a tableau card control</returns>
        private bool IsTableauCardControl(CardUserControl control)
        {
            foreach (List<CardUserControl> column in tableauControls)
            {
                if (column.Contains(control))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find the tableau column index for a card control using a more thorough search
        /// This is a fallback when GetTableauColumnIndex fails
        /// </summary>
        /// <param name="control">The CardUserControl to find</param>
        /// <returns>The column index (0-6) or -1 if not found</returns>
        private int FindTableauColumnForCard(CardUserControl control)
        {
            for (int col = 0; col < tableauControls.Count; col++)
            {
                for (int row = 0; row < tableauControls[col].Count; row++)
                {
                    if (tableauControls[col][row] == control)
                    {
                        return col;
                    }
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
                        // Update the face-up state tracking - newly exposed card becomes face-up
                        EnsureFaceUpStateCapacity(sourceColumnIndex, sourceColumn.Count);
                        tableauFaceUpStates[sourceColumnIndex][sourceColumn.Count - 1] = true;
                        
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
            if (ReferenceEquals(sourceControl, WastePile))
            {
                if (solitaireRules.WastePile.Count > 0 && solitaireRules.WastePile[solitaireRules.WastePile.Count - 1] == card)
                {
                    DebugLog($"Removing from Waste -> {DescribeCard(card)}");
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

            // Ensure states list can represent all current cards
            EnsureFaceUpStateCapacity(columnIndex, columnCards.Count);

            DebugLog($"RefreshTableauColumn[{columnIndex}]: {columnCards.Count} cards in data, {columnControls.Count} controls available");
            
            for (int row = 0; row < columnControls.Count; row++)
            {
                if (row < columnCards.Count)
                {
                    Card card = columnCards[row];
                    columnControls[row].SetupCard(card);
                    // Only the cards with face-up state true should be visible face-up
                    bool faceUp = row < tableauFaceUpStates[columnIndex].Count ? tableauFaceUpStates[columnIndex][row] : (row == columnCards.Count - 1);
                    columnControls[row].IsFaceUp = faceUp;
                    columnControls[row].Visibility = Visibility.Visible;
                    
                    // Set proper positioning for partial stacking using TableauVerticalOffset
                    double topPosition = row * CardVisualConstants.TableauVerticalOffset;
                    Canvas.SetTop(columnControls[row], topPosition);
                    DebugLog($"  Card[{row}] = {DescribeCard(card)}, faceUp={faceUp}, Canvas.Top={topPosition}");
                }
                else
                {
                    columnControls[row].Card = null;
                    columnControls[row].Visibility = Visibility.Hidden;
                    DebugLog($"  Control[{row}] = hidden (no card)");
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

            DebugLog($"ValidateMoveDetailed: source={DescribeControl(dragSourceControl)}, target={DescribeControl(targetControl)}, card={DescribeCard(card)}");
            
            // Check if moving to foundation pile
            if (foundationControls.Contains(targetControl))
            {
                int foundationIndex = foundationControls.IndexOf(targetControl);
                bool canPlaceFoundation = solitaireRules.CanPlaceCardOnFoundation(card, foundationIndex);
                DebugLog($" -> Foundation[{foundationIndex}] canPlace={canPlaceFoundation}");
                if (canPlaceFoundation)
                {
                    return "Valid";
                }
                else
                {
                    return "Card cannot be placed on this foundation pile";
                }
            }
            
            // Check if moving to tableau
            int targetColumnIndex = GetTableauColumnIndex(targetControl);
            if (targetColumnIndex >= 0)
            {
                // This is a tableau move - use SolitaireRules validation
                bool canPlace = solitaireRules.CanPlaceCardOnTableau(card, targetColumnIndex);
                // Gather current top card info
                List<Card> columnCards = solitaireRules.TableauColumns[targetColumnIndex];
                Card top = columnCards.Count > 0 ? columnCards[columnCards.Count - 1] : null;
                bool rankOk = top == null ? (card.Number == Card.CardNumber.K) : IsOneRankLower(card.Number, top.Number);
                bool colorOk = top == null ? true : IsOppositeColor(card, top);
                DebugLog($" -> Tableau[{targetColumnIndex}] top={DescribeCard(top)} canPlace={canPlace} rankOk={rankOk} colorOk={colorOk}");
                if (canPlace)
                {
                    return "Valid";
                }
                else
                {
                    // Provide specific error message for empty vs non-empty tableau columns
                    if (columnCards.Count == 0)
                    {
                        return "Only Kings can be placed on empty tableau spaces";
                    }
                    else
                    {
                        return $"{card.Number} cannot be placed on {top.Number} - must be one rank lower and opposite color";
                    }
                }
            }
            
            // Check if moving to other empty spaces (non-tableau)
            if (targetControl.Card == null)
            {
                // This is not a tableau move to an empty space
                DebugLog(" -> Target is empty, but not a tableau slot. Rejecting.");
                return "Cannot place cards on this empty space";
            }
            
            // If we reach here, it's a move to a non-tableau target with an existing card
            // This shouldn't happen in normal Solitaire gameplay
            DebugLog(" -> Target is non-tableau with existing card. Rejecting.");
            return "Invalid move - cannot place card here";
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

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    Title = "Save Solitaire State",
                    Filter = "Solitaire State (*.json)|*.json",
                    FileName = "solitaire-state.json"
                };

                bool? result = dlg.ShowDialog(this);
                if (result == true)
                {
                    string json = solitaireRules.ExportState("saved from UI").ToJson(true);
                    File.WriteAllText(dlg.FileName, json);
                    StatusLabel.Content = $"Game saved to {System.IO.Path.GetFileName(dlg.FileName)}";
                    DebugLog($"Saved game state -> {dlg.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to save game: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Title = "Load Solitaire State",
                    Filter = "Solitaire State (*.json)|*.json"
                };

                bool? result = dlg.ShowDialog(this);
                if (result == true)
                {
                    string json = File.ReadAllText(dlg.FileName);
                    SolitaireState state = SolitaireState.FromJson(json);

                    // Replace rules state
                    solitaireRules.ImportState(state);

                    // Reset and recompute face-up states based on standard rules
                    InitializeTableauFaceUpStates();

                    // Redraw UI
                    ClearAllCards();
                    DisplayGame();

                    StatusLabel.Content = $"Game loaded from {System.IO.Path.GetFileName(dlg.FileName)}";
                    DebugLog($"Loaded game state <- {dlg.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to load game: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
