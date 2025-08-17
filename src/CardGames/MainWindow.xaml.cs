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
        private List<Canvas> tableauCanvases; // Canvas containers for each tableau column
        
        // Track which positions in each tableau column should be face-up
        // This preserves face-up state during card moves and stacking
        private List<List<bool>> tableauFaceUpStates;

        // Debug toggle
        private bool debugEnabled = true;
        
        // Drag and drop debug mode toggle
        private bool dragDropDebugMode = false;

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
        /// Handle debug log messages from card controls
        /// </summary>
        private void OnCardDebugLog(object sender, string message)
        {
            CardUserControl control = sender as CardUserControl;
            string controlDescription = DescribeControl(control);
            DebugLog($"CardControl({controlDescription}): {message}");
        }

        /// <summary>
        /// Toggle drag and drop debug mode for troubleshooting
        /// </summary>
        private void ToggleDragDropDebugMode()
        {
            dragDropDebugMode = !dragDropDebugMode;
            
            // Apply debug mode to all card controls
            SetDragDropDebugModeForAllControls(dragDropDebugMode);
            
            StatusLabel.Content = $"Drag & Drop Debug Mode: {(dragDropDebugMode ? "ON" : "OFF")}";
            DebugLog($"Drag & Drop Debug Mode toggled to: {dragDropDebugMode}");
        }

        /// <summary>
        /// Set debug mode for all card controls
        /// </summary>
        private void SetDragDropDebugModeForAllControls(bool enabled)
        {
            // Stock pile
            StockPile.IsDebugMode = enabled;
            
            // Waste pile
            WastePile.IsDebugMode = enabled;
            
            // Foundation piles
            foreach (CardUserControl foundation in foundationControls)
            {
                foundation.IsDebugMode = enabled;
            }
            
            // Tableau columns
            foreach (List<CardUserControl> column in tableauControls)
            {
                foreach (CardUserControl control in column)
                {
                    control.IsDebugMode = enabled;
                }
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

            // Map the Canvas for each tableau column for dynamic UI growth
            tableauCanvases = new List<Canvas>
            {
                TableauColumn1,
                TableauColumn2,
                TableauColumn3,
                TableauColumn4,
                TableauColumn5,
                TableauColumn6,
                TableauColumn7
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
            StockPile.DebugLog += OnCardDebugLog;
            StockPile.IsStockPile = true;
            StockPile.StockPileClicked += OnStockPileClicked;
            StockPile.VisibleHeight = CardVisualConstants.CardHeight; // Full height for stock pile

            // Waste pile
            WastePile.CardDragStarted += OnCardDragStarted;
            WastePile.CardDropped += OnCardDropped;
            WastePile.ValidateDrop += OnValidateDrop;
            WastePile.CardClicked += OnCardClicked;
            WastePile.DebugLog += OnCardDebugLog;
            WastePile.VisibleHeight = CardVisualConstants.CardHeight; // Full height for waste pile

            // Foundations
            foreach (CardUserControl foundation in foundationControls)
            {
                foundation.CardDragStarted += OnCardDragStarted;
                foundation.CardDropped += OnCardDropped;
                foundation.ValidateDrop += OnValidateDrop;
                foundation.CardClicked += OnCardClicked;
                foundation.DebugLog += OnCardDebugLog;
                foundation.VisibleHeight = CardVisualConstants.CardHeight; // Full height for foundation piles
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
                    control.DebugLog += OnCardDebugLog;
                    control.VisibleHeight = CardVisualConstants.CardHeight; // Initialize to full height, will be adjusted during layout
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
        /// Recycle waste pile back to stock pile (button click handler)
        /// </summary>
        private void RecycleButton_Click(object sender, RoutedEventArgs e)
        {
            DrawCardFromStock(); // Uses the same logic as clicking the empty stock pile
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
                    
                    // Validate UI synchronization after drawing card
                    ValidateUiDataSynchronization();
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
                
                // Validate UI synchronization after stock reset
                ValidateUiDataSynchronization();
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
            
            // Display tableau columns (use centralized stacking/positioning logic)
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                RefreshTableauColumn(col);
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
            
            // Update recycle button visibility when stock pile changes
            UpdateRecycleButtonVisibility();
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
            
            // Update recycle button visibility when waste pile changes
            UpdateRecycleButtonVisibility();
        }

        /// <summary>
        /// Update the visibility of the recycle button based on stock and waste pile states
        /// </summary>
        private void UpdateRecycleButtonVisibility()
        {
            // Show recycle button when stock is empty and waste has cards
            if (solitaireRules.StockPile.Count == 0 && solitaireRules.WastePile.Count > 0)
            {
                RecycleButton.Visibility = Visibility.Visible;
            }
            else
            {
                RecycleButton.Visibility = Visibility.Collapsed;
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

            // Get the sequence of cards that should move together
            List<Card> cardsToMove = GetCardSequenceToMove(sourceControl, card);
            DebugLog($"ExecuteMove: Moving {cardsToMove.Count} card(s) in sequence");

            // Check if moving to foundation pile
            if (foundationControls.Contains(targetControl))
            {
                int foundationIndex = foundationControls.IndexOf(targetControl);
                
                // Foundation moves can only be single cards
                if (cardsToMove.Count > 1)
                {
                    DebugLog("ExecuteMove: Cannot move multiple cards to foundation - using single card only");
                    cardsToMove = new List<Card> { card };
                }
                
                // Remove card from source
                RemoveCardSequenceFromSource(sourceControl, cardsToMove);
                
                // Add to foundation pile
                solitaireRules.FoundationPiles[foundationIndex].Add(card);
                targetControl.SetupCard(card);
                targetControl.IsFaceUp = true;
                
                // Validate UI synchronization after move completion
                ValidateUiDataSynchronization();
                return;
            }
            
            // Check if moving to tableau
            int targetColumnIndex = GetTableauColumnIndex(targetControl);
            if (targetColumnIndex >= 0)
            {
                // Remove cards from source
                RemoveCardSequenceFromSource(sourceControl, cardsToMove);
                
                // Add cards to target tableau column
                AddCardSequenceToTableau(targetColumnIndex, cardsToMove);
                
                // Validate UI synchronization after move completion
                ValidateUiDataSynchronization();
                return;
            }
            
            // Handle other pile types (waste, stock, etc.)
            if (targetControl.Card == null)
            {
                // Non-tableau moves can only be single cards
                if (cardsToMove.Count > 1)
                {
                    DebugLog("ExecuteMove: Cannot move multiple cards to non-tableau - using single card only");
                    cardsToMove = new List<Card> { card };
                }
                
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
                        // Remove cards from source
                        RemoveCardSequenceFromSource(sourceControl, cardsToMove);
                        
                        // Add cards to target tableau column
                        AddCardSequenceToTableau(targetColumnIndexForExistingCard, cardsToMove);
                        
                        // Validate UI synchronization after move completion
                        ValidateUiDataSynchronization();
                    }
                }
                else
                {
                    // Non-tableau moves can only be single cards
                    if (cardsToMove.Count > 1)
                    {
                        DebugLog("ExecuteMove: Cannot move multiple cards to non-tableau - using single card only");
                        cardsToMove = new List<Card> { card };
                    }
                    
                    // Non-tableau move - replace the card (for waste pile, etc.)
                    targetControl.SetupCard(card);
                    targetControl.IsFaceUp = true;
                    sourceControl.Card = null;
                }
            }
            
            // Validate UI synchronization after move completion
            ValidateUiDataSynchronization();
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

            // Ensure we have enough UI controls and face-up states
            EnsureTableauUiCapacity(columnIndex, columnCards.Count == 0 ? 1 : columnCards.Count);
            EnsureFaceUpStateCapacity(columnIndex, columnCards.Count);

            DebugLog($"RefreshTableauColumn[{columnIndex}]: {columnCards.Count} cards in data, {tableauControls[columnIndex].Count} controls available");
            
            // Refresh local reference in case the list grew
            columnControls = tableauControls[columnIndex];
            
            for (int row = 0; row < columnControls.Count; row++)
            {
                if (row < columnCards.Count)
                {
                    Card card = columnCards[row];
                    CardUserControl control = columnControls[row];
                    control.SetupCard(card);
                    control.IsTableauDropTarget = false; // Clear drop target flag when card is present
                    bool faceUp = row < tableauFaceUpStates[columnIndex].Count ? tableauFaceUpStates[columnIndex][row] : (row == columnCards.Count - 1);
                    control.IsFaceUp = faceUp;
                    control.Visibility = Visibility.Visible;

                    double topPosition = row * CardVisualConstants.TableauVerticalOffset;
                    Canvas.SetTop(control, topPosition);
                    Panel.SetZIndex(control, row);
                    
                    // Set visible height for hit testing: last card gets full height, others get only the offset
                    bool isLastCard = (row == columnCards.Count - 1);
                    control.VisibleHeight = isLastCard ? CardVisualConstants.CardHeight : CardVisualConstants.TableauVerticalOffset;
                    
                    DebugLog($"  Card[{row}] = {DescribeCard(card)}, faceUp={faceUp}, Canvas.Top={topPosition}, VisibleHeight={control.VisibleHeight}");
                }
                else
                {
                    // For empty columns, keep the first placeholder control visible to serve as a drop target
                    if (columnCards.Count == 0 && row == 0)
                    {
                        CardUserControl control = columnControls[row];
                        control.Card = null;
                        control.IsFaceUp = false;
                        control.IsTableauDropTarget = true; // Mark as drop target for visual styling
                        control.Visibility = Visibility.Visible; // keep visible to accept drops
                        control.VisibleHeight = CardVisualConstants.CardHeight; // Full height for empty drop targets
                        Canvas.SetTop(control, 0);
                        Panel.SetZIndex(control, 0);
                        DebugLog("  Empty column -> keeping row 0 control visible for drop target");
                    }
                    else
                    {
                        columnControls[row].Card = null;
                        columnControls[row].IsTableauDropTarget = false; // Clear drop target flag
                        columnControls[row].Visibility = Visibility.Hidden;
                        DebugLog($"  Control[{row}] = hidden (no card)");
                    }
                }
            }
        }

        /// <summary>
        /// Ensure there are enough UI CardUserControl slots for the given tableau column.
        /// If not, create additional controls, wire events, and add to the Canvas.
        /// </summary>
        private void EnsureTableauUiCapacity(int columnIndex, int requiredCount)
        {
            if (columnIndex < 0 || columnIndex >= tableauControls.Count)
            {
                return;
            }

            List<CardUserControl> controls = tableauControls[columnIndex];
            Canvas canvas = tableauCanvases[columnIndex];

            while (controls.Count < requiredCount)
            {
                CardUserControl control = new CardUserControl();
                control.CardDragStarted += OnCardDragStarted;
                control.CardDropped += OnCardDropped;
                control.ValidateDrop += OnValidateDrop;
                control.CardClicked += OnCardClicked;
                control.DebugLog += OnCardDebugLog;
                control.IsDebugMode = dragDropDebugMode; // Apply current debug mode state
                canvas.Children.Add(control);
                controls.Add(control);
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
            DebugLog($"  targetControl.Card = {DescribeCard(targetControl.Card)}");
            
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
            DebugLog($" -> GetTableauColumnIndex returned: {targetColumnIndex}");
            if (targetColumnIndex >= 0)
            {
                // Get the sequence of cards that would move together
                List<Card> cardsToMove = GetCardSequenceToMove(dragSourceControl, card);
                
                // This is a tableau move - use SolitaireRules validation
                bool canPlace = solitaireRules.CanPlaceCardOnTableau(card, targetColumnIndex);
                // Gather current top card info
                List<Card> columnCards = solitaireRules.TableauColumns[targetColumnIndex];
                Card top = columnCards.Count > 0 ? columnCards[columnCards.Count - 1] : null;
                bool rankOk = top == null ? (card.Number == Card.CardNumber.K) : IsOneRankLower(card.Number, top.Number);
                bool colorOk = top == null ? true : IsOppositeColor(card, top);
                DebugLog($" -> Tableau[{targetColumnIndex}] top={DescribeCard(top)} canPlace={canPlace} rankOk={rankOk} colorOk={colorOk} sequenceSize={cardsToMove.Count}");
                DebugLog($" -> Column has {columnCards.Count} cards, targetControl.Card={DescribeCard(targetControl.Card)}");
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
                DebugLog(" -> Target control has no card, checking fallback logic for empty tableau columns");
                // Before rejecting, check if this might be an empty tableau column
                // that wasn't detected by GetTableauColumnIndex
                for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
                {
                    DebugLog($"   Checking column {col}: data={solitaireRules.TableauColumns[col].Count} cards, UI={tableauControls[col].Count} controls");
                    if (solitaireRules.TableauColumns[col].Count == 0 && 
                        tableauControls[col].Count > 0 && 
                        tableauControls[col].Contains(targetControl))
                    {
                        // This is an empty tableau column - validate the move
                        bool canPlace = solitaireRules.CanPlaceCardOnTableau(card, col);
                        DebugLog($" -> Empty tableau column[{col}] detected via fallback, canPlace={canPlace}");
                        if (canPlace)
                        {
                            return "Valid";
                        }
                        else
                        {
                            return "Only Kings can be placed on empty tableau spaces";
                        }
                    }
                }
                
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
                    // Export rules data first
                    SolitaireState state = solitaireRules.ExportState("saved from UI");

                    // Persist UI face-up states per column, trimmed to current card counts
                    state.TableauFaceUpStates.Clear();
                    for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
                    {
                        List<bool> src = tableauFaceUpStates[col];
                        List<bool> copy = new List<bool>();
                        int cardCount = solitaireRules.TableauColumns[col].Count;
                        for (int i = 0; i < cardCount; i++)
                        {
                            bool value = (i < src.Count) ? src[i] : (i == cardCount - 1);
                            copy.Add(value);
                        }
                        state.TableauFaceUpStates.Add(copy);
                    }

                    string json = state.ToJson(true);
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

                    // Restore UI face-up states if present and well-formed; else recompute defaults
                    if (state.TableauFaceUpStates != null && state.TableauFaceUpStates.Count == solitaireRules.TableauColumns.Count)
                    {
                        // Rebuild tableauFaceUpStates to match the saved data per column length
                        tableauFaceUpStates.Clear();
                        for (int col = 0; col < state.TableauFaceUpStates.Count; col++)
                        {
                            List<bool> src = state.TableauFaceUpStates[col] ?? new List<bool>();
                            List<bool> copy = new List<bool>();
                            int cardCount = solitaireRules.TableauColumns[col].Count;
                            for (int i = 0; i < cardCount; i++)
                            {
                                bool value = (i < src.Count) ? src[i] : (i == cardCount - 1);
                                copy.Add(value);
                            }
                            tableauFaceUpStates.Add(copy);
                        }
                    }
                    else
                    {
                        InitializeTableauFaceUpStates();
                    }

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

        /// <summary>
        /// Gets the sequence of cards that should move together when dragging from a tableau column
        /// </summary>
        /// <param name="sourceControl">The control where the drag started</param>
        /// <param name="draggedCard">The card being dragged</param>
        /// <returns>List of cards that should move together, or single card list if not a sequence move</returns>
        private List<Card> GetCardSequenceToMove(CardUserControl sourceControl, Card draggedCard)
        {
            // Check if this is a tableau move
            int sourceColumnIndex = GetTableauColumnIndex(sourceControl);
            if (sourceColumnIndex < 0)
            {
                // Not a tableau move - return single card
                return new List<Card> { draggedCard };
            }

            List<Card> sourceColumn = solitaireRules.TableauColumns[sourceColumnIndex];
            List<Card> sequence = new List<Card>();

            // Find the position of the dragged card in the column
            int draggedCardIndex = -1;
            for (int i = 0; i < sourceColumn.Count; i++)
            {
                if (sourceColumn[i] == draggedCard)
                {
                    draggedCardIndex = i;
                    break;
                }
            }

            if (draggedCardIndex < 0)
            {
                // Card not found in column - return single card
                return new List<Card> { draggedCard };
            }

            // Check if all cards from the dragged card to the end are face-up and form a valid sequence
            bool isValidSequence = true;
            for (int i = draggedCardIndex; i < sourceColumn.Count; i++)
            {
                // Check if card is face-up
                if (i >= tableauFaceUpStates[sourceColumnIndex].Count || !tableauFaceUpStates[sourceColumnIndex][i])
                {
                    isValidSequence = false;
                    break;
                }

                sequence.Add(sourceColumn[i]);

                // Check if this card can be placed on the previous card in the sequence (descending order, alternating colors)
                if (i > draggedCardIndex)
                {
                    Card previousCard = sourceColumn[i - 1];
                    Card currentCard = sourceColumn[i];
                    
                    if (!IsOneRankLower(currentCard.Number, previousCard.Number) || !IsOppositeColor(currentCard, previousCard))
                    {
                        isValidSequence = false;
                        break;
                    }
                }
            }

            if (!isValidSequence)
            {
                // Not a valid sequence - return single card
                return new List<Card> { draggedCard };
            }

            return sequence;
        }

        /// <summary>
        /// Remove a sequence of cards from the source tableau column
        /// </summary>
        /// <param name="sourceControl">The source control</param>
        /// <param name="cardsToRemove">The cards to remove</param>
        private void RemoveCardSequenceFromSource(CardUserControl sourceControl, List<Card> cardsToRemove)
        {
            // Check if removing from tableau
            int sourceColumnIndex = GetTableauColumnIndex(sourceControl);
            if (sourceColumnIndex >= 0)
            {
                List<Card> sourceColumn = solitaireRules.TableauColumns[sourceColumnIndex];
                
                // Remove the cards from the end of the column (top-down) to maintain order
                for (int i = cardsToRemove.Count - 1; i >= 0; i--)
                {
                    if (sourceColumn.Count > 0 && sourceColumn[sourceColumn.Count - 1] == cardsToRemove[i])
                    {
                        sourceColumn.RemoveAt(sourceColumn.Count - 1);
                    }
                }
                
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
                return;
            }
            
            // For non-tableau sources, fall back to single card removal
            RemoveCardFromSource(sourceControl, cardsToRemove[0]);
        }

        /// <summary>
        /// Add a sequence of cards to the target tableau column
        /// </summary>
        /// <param name="targetColumnIndex">The target column index</param>
        /// <param name="cardsToAdd">The cards to add</param>
        private void AddCardSequenceToTableau(int targetColumnIndex, List<Card> cardsToAdd)
        {
            // Add all cards to the target tableau column
            foreach (Card card in cardsToAdd)
            {
                solitaireRules.TableauColumns[targetColumnIndex].Add(card);
                
                // Update face-up state tracking - newly added card should be face-up
                int newCardPosition = solitaireRules.TableauColumns[targetColumnIndex].Count - 1;
                EnsureFaceUpStateCapacity(targetColumnIndex, newCardPosition + 1);
                tableauFaceUpStates[targetColumnIndex][newCardPosition] = true;
            }
            
            // Refresh the display for the target column to show proper stacking
            RefreshTableauColumn(targetColumnIndex);
        }

        /// <summary>
        /// Validate that the UI state is synchronized with the underlying data model
        /// This method compares the displayed cards with the actual data and reports any discrepancies
        /// </summary>
        /// <returns>True if UI and data are synchronized, false if discrepancies are found</returns>
        private bool ValidateUiDataSynchronization()
        {
            bool isValid = true;
            DebugLog("=== UI-Data Synchronization Check ===");

            // Check Foundation Piles
            for (int i = 0; i < foundationControls.Count; i++)
            {
                CardUserControl foundationControl = foundationControls[i];
                List<Card> foundationData = solitaireRules.FoundationPiles[i];
                
                if (foundationData.Count == 0)
                {
                    if (foundationControl.Card != null)
                    {
                        DebugLog($"MISMATCH: Foundation[{i}] UI shows {DescribeCard(foundationControl.Card)}, data shows empty");
                        isValid = false;
                    }
                }
                else
                {
                    Card expectedTopCard = foundationData[foundationData.Count - 1];
                    if (foundationControl.Card == null)
                    {
                        DebugLog($"MISMATCH: Foundation[{i}] UI shows empty, data shows {DescribeCard(expectedTopCard)}");
                        isValid = false;
                    }
                    else if (!CardsEqual(foundationControl.Card, expectedTopCard))
                    {
                        DebugLog($"MISMATCH: Foundation[{i}] UI shows {DescribeCard(foundationControl.Card)}, data shows {DescribeCard(expectedTopCard)}");
                        isValid = false;
                    }
                }
            }

            // Check Tableau Columns
            for (int col = 0; col < tableauControls.Count; col++)
            {
                List<Card> tableauData = solitaireRules.TableauColumns[col];
                List<CardUserControl> tableauUi = tableauControls[col];
                
                DebugLog($"Tableau[{col}]: Data has {tableauData.Count} cards, UI has {tableauUi.Count} controls");
                
                // Check each visible card in the column
                for (int row = 0; row < tableauData.Count; row++)
                {
                    Card expectedCard = tableauData[row];
                    
                    if (row >= tableauUi.Count)
                    {
                        DebugLog($"MISMATCH: Tableau[{col}][{row}] data shows {DescribeCard(expectedCard)}, but UI control doesn't exist");
                        isValid = false;
                        continue;
                    }
                    
                    CardUserControl uiControl = tableauUi[row];
                    if (uiControl.Card == null)
                    {
                        DebugLog($"MISMATCH: Tableau[{col}][{row}] UI shows empty, data shows {DescribeCard(expectedCard)}");
                        isValid = false;
                    }
                    else if (!CardsEqual(uiControl.Card, expectedCard))
                    {
                        DebugLog($"MISMATCH: Tableau[{col}][{row}] UI shows {DescribeCard(uiControl.Card)}, data shows {DescribeCard(expectedCard)}");
                        isValid = false;
                    }
                    else if (uiControl.Visibility != Visibility.Visible)
                    {
                        DebugLog($"MISMATCH: Tableau[{col}][{row}] has card {DescribeCard(expectedCard)} but control is not visible");
                        isValid = false;
                    }
                }
                
                // Check for extra visible UI controls beyond the data
                for (int row = tableauData.Count; row < tableauUi.Count; row++)
                {
                    CardUserControl uiControl = tableauUi[row];
                    if (uiControl.Visibility == Visibility.Visible && uiControl.Card != null)
                    {
                        DebugLog($"MISMATCH: Tableau[{col}][{row}] UI shows {DescribeCard(uiControl.Card)}, but data has no card at this position");
                        isValid = false;
                    }
                }
            }

            // Check Stock Pile
            if (solitaireRules.StockPile.Count == 0)
            {
                if (StockPile.Card != null)
                {
                    DebugLog($"MISMATCH: StockPile UI shows {DescribeCard(StockPile.Card)}, data shows empty");
                    isValid = false;
                }
            }
            else
            {
                // Stock pile shows the back of cards, so we just check if it has any card when data is not empty
                if (StockPile.Card == null && StockPile.Visibility == Visibility.Visible)
                {
                    DebugLog($"MISMATCH: StockPile UI shows empty but is visible, data has {solitaireRules.StockPile.Count} cards");
                    isValid = false;
                }
            }

            // Check Waste Pile
            if (solitaireRules.WastePile.Count == 0)
            {
                if (WastePile.Card != null)
                {
                    DebugLog($"MISMATCH: WastePile UI shows {DescribeCard(WastePile.Card)}, data shows empty");
                    isValid = false;
                }
            }
            else
            {
                Card expectedTopCard = solitaireRules.WastePile[solitaireRules.WastePile.Count - 1];
                if (WastePile.Card == null)
                {
                    DebugLog($"MISMATCH: WastePile UI shows empty, data shows {DescribeCard(expectedTopCard)}");
                    isValid = false;
                }
                else if (!CardsEqual(WastePile.Card, expectedTopCard))
                {
                    DebugLog($"MISMATCH: WastePile UI shows {DescribeCard(WastePile.Card)}, data shows {DescribeCard(expectedTopCard)}");
                    isValid = false;
                }
            }

            if (isValid)
            {
                DebugLog("UI-Data synchronization: OK");
            }
            else
            {
                DebugLog("UI-Data synchronization: FAILED - discrepancies found");
            }
            
            DebugLog("=== End Synchronization Check ===");
            return isValid;
        }

        /// <summary>
        /// Helper method to compare two cards for equality
        /// </summary>
        /// <param name="card1">First card to compare</param>
        /// <param name="card2">Second card to compare</param>
        /// <returns>True if cards are equal (same number and suite)</returns>
        private bool CardsEqual(Card card1, Card card2)
        {
            if (card1 == null && card2 == null)
            {
                return true;
            }
            if (card1 == null || card2 == null)
            {
                return false;
            }
            return card1.Number == card2.Number && card1.Suite == card2.Suite;
        }

        /// <summary>
        /// Handle key press events for debug mode toggle and other shortcuts
        /// </summary>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+D to toggle drag and drop debug mode
            if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ToggleDragDropDebugMode();
                e.Handled = true;
            }
        }
    }
}
