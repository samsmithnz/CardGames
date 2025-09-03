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
        private List<CardUserControl> freeCellControls; // Free cell controls for Freecell
        
        // Track which positions in each tableau column should be face-up
        // This preserves face-up state during card moves and stacking
        private List<List<bool>> tableauFaceUpStates;

        // Current game type
        private string currentGameType = "Klondike Solitaire";

        // Debug toggle
        private bool debugEnabled = true;
        
        // Drag and drop debug mode toggle
        private bool dragDropDebugMode = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            
            // Start initial game
            StartNewGame(currentGameType);
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
            
            // Free cells
            if (freeCellControls != null)
            {
                foreach (CardUserControl freeCell in freeCellControls)
                {
                    freeCell.IsDebugMode = enabled;
                }
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
            int fcIndex = freeCellControls != null ? freeCellControls.IndexOf(control) : -1;
            if (fcIndex >= 0)
            {
                return $"FreeCell[{fcIndex}]";
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
        /// Initialize the game components and layout (refactored for per-variant setup)
        /// </summary>
        private void InitializeGame()
        {
            DebugLog($"InitializeGame: begin for '{currentGameType}'");
            // Core objects
            deck = new Deck();
            solitaireRules = new SolitaireRules(currentGameType);

            InitializeCommonCollections();
            if (IsFreecellGame())
            {
                InitializeFreecellLayout();
            }
            else
            {
                InitializeKlondikeLayout();
            }
            ResetTableauCanvasChildren();
            InitializeEmptyFaceUpStateTracking();
            if (IsFreecellGame())
            {
                SetupCardEventsFreecell();
            }
            else
            {
                SetupCardEventsKlondike();
            }
            SetupUIVisibility();
            DebugLog("InitializeGame: completed variant-specific initialization");
        }

        /// <summary>
        /// Returns true if the current game type is Freecell
        /// </summary>
        private bool IsFreecellGame()
        {
            return string.Equals(currentGameType, "Freecell", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initialize collections shared by all variants (foundation & free cell references)
        /// </summary>
        private void InitializeCommonCollections()
        {
            foundationControls = new List<CardUserControl> { Foundation1, Foundation2, Foundation3, Foundation4 };
            freeCellControls = new List<CardUserControl> { FreeCell1, FreeCell2, FreeCell3, FreeCell4 }; // Always tracked (visibility toggled later)
        }

        /// <summary>
        /// Configure tableau controls & canvases for Klondike (7 columns)
        /// </summary>
        private void InitializeKlondikeLayout()
        {
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
            DebugLog("InitializeKlondikeLayout: 7 tableau columns configured");
        }

        /// <summary>
        /// Configure tableau controls & canvases for Freecell (8 columns)
        /// </summary>
        private void InitializeFreecellLayout()
        {
            tableauControls = new List<List<CardUserControl>>
            {
                new List<CardUserControl> { Tableau1_1 },
                new List<CardUserControl> { Tableau2_1, Tableau2_2 },
                new List<CardUserControl> { Tableau3_1, Tableau3_2, Tableau3_3 },
                new List<CardUserControl> { Tableau4_1, Tableau4_2, Tableau4_3, Tableau4_4 },
                new List<CardUserControl> { Tableau5_1, Tableau5_2, Tableau5_3, Tableau5_4, Tableau5_5 },
                new List<CardUserControl> { Tableau6_1, Tableau6_2, Tableau6_3, Tableau6_4, Tableau6_5, Tableau6_6 },
                new List<CardUserControl> { Tableau7_1, Tableau7_2, Tableau7_3, Tableau7_4, Tableau7_5, Tableau7_6, Tableau7_7 },
                new List<CardUserControl> { Tableau8_1, Tableau8_2, Tableau8_3, Tableau8_4, Tableau8_5, Tableau8_6, Tableau8_7 }
            };
            tableauCanvases = new List<Canvas>
            {
                TableauColumn1,
                TableauColumn2,
                TableauColumn3,
                TableauColumn4,
                TableauColumn5,
                TableauColumn6,
                TableauColumn7,
                TableauColumn8
            };
            DebugLog("InitializeFreecellLayout: 8 tableau columns configured");
        }

        /// <summary>
        /// Clear all tableau canvases and reattach their base XAML CardUserControl children
        /// </summary>
        private void ResetTableauCanvasChildren()
        {
            for (int i = 0; i < tableauCanvases.Count; i++)
            {
                Canvas canvas = tableauCanvases[i];
                canvas.Children.Clear();
                foreach (CardUserControl ctrl in tableauControls[i])
                {
                    canvas.Children.Add(ctrl);
                }
            }
        }

        /// <summary>
        /// Initialize (empty) face-up state tracking lists sized to current tableau control structure
        /// </summary>
        private void InitializeEmptyFaceUpStateTracking()
        {
            tableauFaceUpStates = new List<List<bool>>();
            for (int i = 0; i < tableauControls.Count; i++)
            {
                List<bool> list = new List<bool>();
                for (int j = 0; j < tableauControls[i].Count; j++)
                {
                    list.Add(false);
                }
                tableauFaceUpStates.Add(list);
            }
        }

        /// <summary>
        /// Setup UI visibility based on current game type
        /// </summary>
        private void SetupUIVisibility()
        {
            bool isFreecell = currentGameType == "Freecell";
            
            // Show/hide stock and waste piles
            StockBorder.Visibility = isFreecell ? Visibility.Collapsed : Visibility.Visible;
            StockLabel.Visibility = isFreecell ? Visibility.Collapsed : Visibility.Visible;
            WasteBorder.Visibility = isFreecell ? Visibility.Collapsed : Visibility.Visible;
            WasteLabel.Visibility = isFreecell ? Visibility.Collapsed : Visibility.Visible;
            
            // Show/hide free cells
            FreeCellBorder1.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            FreeCellLabel1.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            FreeCellBorder2.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            FreeCellLabel2.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            FreeCellBorder3.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            FreeCellLabel3.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            FreeCellBorder4.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            FreeCellLabel4.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            
            // Show/hide 8th tableau column
            TableauColumn8Border.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            
            // Show/hide status elements
            FreeCellsStatus.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            EmptyColumnsStatus.Visibility = isFreecell ? Visibility.Visible : Visibility.Collapsed;
            
        }

        /// <summary>
        /// Wire up drag/drop and click events for Klondike controls (stock, waste, foundations, tableau)
        /// </summary>
        private void SetupCardEventsKlondike()
        {
            // Stock pile
            StockPile.CardDragStarted += OnCardDragStarted;
            StockPile.CardDropped += OnCardDropped;
            StockPile.ValidateDrop += OnValidateDrop;
            StockPile.CardClicked += OnCardClicked;
            StockPile.DebugLog += OnCardDebugLog;
            StockPile.IsStockPile = true;
            StockPile.StockPileClicked += OnStockPileClicked;
            StockPile.VisibleHeight = CardVisualConstants.CardHeight;
            StockPile.StackPosition = 0;

            // Waste pile
            WastePile.CardDragStarted += OnCardDragStarted;
            WastePile.CardDropped += OnCardDropped;
            WastePile.ValidateDrop += OnValidateDrop;
            WastePile.CardClicked += OnCardClicked;
            WastePile.DebugLog += OnCardDebugLog;
            WastePile.VisibleHeight = CardVisualConstants.CardHeight;
            WastePile.StackPosition = 0;

            // Foundations
            foreach (CardUserControl foundation in foundationControls)
            {
                foundation.CardDragStarted += OnCardDragStarted;
                foundation.CardDropped += OnCardDropped;
                foundation.ValidateDrop += OnValidateDrop;
                foundation.CardClicked += OnCardClicked;
                foundation.DebugLog += OnCardDebugLog;
                foundation.VisibleHeight = CardVisualConstants.CardHeight;
                foundation.StackPosition = 0;
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
                    control.VisibleHeight = CardVisualConstants.CardHeight;
                    control.StackPosition = 0;
                }
            }
        }

        /// <summary>
        /// Wire up drag/drop and click events for Freecell controls (stock, waste, foundations, tableau, free cells)
        /// </summary>
        private void SetupCardEventsFreecell()
        {
            // Stock pile
            StockPile.CardDragStarted += OnCardDragStarted;
            StockPile.CardDropped += OnCardDropped;
            StockPile.ValidateDrop += OnValidateDrop;
            StockPile.CardClicked += OnCardClicked;
            StockPile.DebugLog += OnCardDebugLog;
            StockPile.IsStockPile = true;
            StockPile.StockPileClicked += OnStockPileClicked;
            StockPile.VisibleHeight = CardVisualConstants.CardHeight;
            StockPile.StackPosition = 0;

            // Waste pile
            WastePile.CardDragStarted += OnCardDragStarted;
            WastePile.CardDropped += OnCardDropped;
            WastePile.ValidateDrop += OnValidateDrop;
            WastePile.CardClicked += OnCardClicked;
            WastePile.DebugLog += OnCardDebugLog;
            WastePile.VisibleHeight = CardVisualConstants.CardHeight;
            WastePile.StackPosition = 0;

            // Foundations
            foreach (CardUserControl foundation in foundationControls)
            {
                foundation.CardDragStarted += OnCardDragStarted;
                foundation.CardDropped += OnCardDropped;
                foundation.ValidateDrop += OnValidateDrop;
                foundation.CardClicked += OnCardClicked;
                foundation.DebugLog += OnCardDebugLog;
                foundation.VisibleHeight = CardVisualConstants.CardHeight;
                foundation.StackPosition = 0;
            }

            // Free cells
            foreach (CardUserControl freeCell in freeCellControls)
            {
                freeCell.CardDragStarted += OnCardDragStarted;
                freeCell.CardDropped += OnCardDropped;
                freeCell.ValidateDrop += OnValidateDrop;
                freeCell.CardClicked += OnCardClicked;
                freeCell.DebugLog += OnCardDebugLog;
                freeCell.VisibleHeight = CardVisualConstants.CardHeight;
                freeCell.StackPosition = 0;
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
                    control.VisibleHeight = CardVisualConstants.CardHeight;
                    control.StackPosition = 0;
                }
            }
        }

        /// <summary>
        /// Write the current game state to a file (debugging aid)
        /// </summary>
        private void WriteStateToFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write foundations
                    writer.WriteLine("Foundations:");
                    foreach (CardUserControl foundation in foundationControls)
                    {
                        WriteCardListToFile(writer, solitaireRules.FoundationPiles[foundationControls.IndexOf(foundation)]);
                    }

                    // Write tableau columns
                    writer.WriteLine("\nTableau:");
                    for (int col = 0; col < tableauControls.Count; col++)
                    {
                        writer.Write($"Column {col + 1}: ");
                        List<Card> columnData = solitaireRules.TableauColumns[col];
                        foreach (Card card in columnData)
                        {
                            writer.Write($"{DescribeCard(card)}, ");
                        }
                        writer.WriteLine();
                    }

                    // Write stock and waste piles
                    writer.WriteLine("\nStock Pile:");
                    WriteCardListToFile(writer, solitaireRules.StockPile);
                    writer.WriteLine("Waste Pile:");
                    WriteCardListToFile(writer, solitaireRules.WastePile);
                }

                MessageBox.Show($"Game state written to {filePath}", "State Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error writing state to file: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Write a list of cards to the file (helper for state export)
        /// </summary>
        private void WriteCardListToFile(StreamWriter writer, List<Card> cardList)
        {
            if (cardList == null || cardList.Count == 0)
            {
                writer.WriteLine("  (empty)");
                return;
            }
            foreach (Card card in cardList)
            {
                writer.Write($"{DescribeCard(card)}, ");
            }
            writer.WriteLine();
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
          
                    // Check if the loaded game is a different type than the current game
                    string loadedGameName = state.GameName ?? "Klondike Solitaire"; // Default for backward compatibility
                    if (currentGameType != loadedGameName)
                    {
                        DebugLog($"LoadGame: Switching from '{currentGameType}' to '{loadedGameName}'");
                        currentGameType = loadedGameName;
                        InitializeGame(); // Rebuild UI for the new game type
                        // Create new rules with the correct configuration
                        solitaireRules = new SolitaireRules(currentGameType);
                        SetupUIVisibility(); // Update UI visibility for the new game type
                    }

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
        /// /// <param name="sourceControl">The control where the drag started</param>
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
        /// /// <param name="sourceControl">The source control</param>
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
        /// /// <param name="targetColumnIndex">The target column index</param>
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
        /// Validate the move with detailed feedback
        /// </summary>
        private string ValidateMoveDetailed(Card card, CardUserControl targetControl)
        {
            if (card == null)
            {
                return "No card to move";
            }

            DebugLog($"ValidateMoveDetailed: source={DescribeControl(dragSourceControl)}, target={DescribeControl(targetControl)}, card={DescribeCard(card)}");
            DebugLog($"  targetControl.Card = {DescribeCard(targetControl.Card)}");
            
            // Check if moving to free cell
            if (freeCellControls.Contains(targetControl))
            {
                int freeCellIndex = freeCellControls.IndexOf(targetControl);
                bool canPlaceFreeCell = solitaireRules.CanPlaceCardInFreeCell(freeCellIndex);
                DebugLog($" -> FreeCell[{freeCellIndex}] canPlace={canPlaceFreeCell}");
                if (canPlaceFreeCell)
                {
                    return "Valid";
                }
                else
                {
                    return "Free cell is already occupied";
                }
            }
            
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
                
                // For FreeCell games, also validate sequence move size
                bool canMoveSequence = solitaireRules.CanMoveCardSequence(cardsToMove.Count, targetColumnIndex);
                
                // Gather current top card info
                List<Card> columnCards = solitaireRules.TableauColumns[targetColumnIndex];
                Card top = columnCards.Count > 0 ? columnCards[columnCards.Count - 1] : null;
                bool rankOk = top == null ? (card.Number == Card.CardNumber.K) : IsOneRankLower(card.Number, top.Number);
                bool colorOk = top == null ? true : IsOppositeColor(card, top);
                DebugLog($" -> Tableau[{targetColumnIndex}] top={DescribeCard(top)} canPlace={canPlace} rankOk={rankOk} colorOk={colorOk} sequenceSize={cardsToMove.Count} canMoveSequence={canMoveSequence}");
                DebugLog($" -> Column has {columnCards.Count} cards, targetControl.Card={DescribeCard(targetControl.Card)}");
                
                if (canPlace && canMoveSequence)
                {
                    return "Valid";
                }
                else if (!canPlace)
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
                else if (!canMoveSequence)
                {
                    // Sequence move validation failed
                    int maxMoveable = solitaireRules.CalculateMaxSequenceMoveSize();
                    if (columnCards.Count == 0)
                    {
                        maxMoveable = maxMoveable / 2; // Moving to empty column reduces max by half
                    }
                    return $"Cannot move {cardsToMove.Count} cards - maximum {maxMoveable} cards can be moved with current free space";
                }
            }
            
            // Check if moving to other empty spaces (non-tableau)
            if (targetControl.Card == null)
            {
                DebugLog(" -> Target is empty, but not a tableau slot. Rejecting.");
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
                        List<Card> cardsToMove = GetCardSequenceToMove(dragSourceControl, card);
                        bool canPlace = solitaireRules.CanPlaceCardOnTableau(card, col);
                        bool canMoveSequence = solitaireRules.CanMoveCardSequence(cardsToMove.Count, col);
                        DebugLog($" -> Empty tableau column[{col}] detected via fallback, canPlace={canPlace}, canMoveSequence={canMoveSequence}, sequenceSize={cardsToMove.Count}");
                        
                        if (canPlace && canMoveSequence)
                        {
                            return "Valid";
                        }
                        else if (!canPlace)
                        {
                            return "Only Kings can be placed on empty tableau spaces";
                        }
                        else if (!canMoveSequence)
                        {
                            int maxMoveable = solitaireRules.CalculateMaxSequenceMoveSize() / 2; // Moving to empty column reduces max by half
                            return $"Cannot move {cardsToMove.Count} cards - maximum {maxMoveable} cards can be moved to empty column with current free space";
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

        // ================== Restored / Utility Methods ==================

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

        private int GetTableauPositionIndex(CardUserControl control)
        {
            int c = GetTableauColumnIndex(control);
            if (c >= 0)
            {
                return tableauControls[c].IndexOf(control);
            }
            return -1;
        }

        private bool IsTableauCardControl(CardUserControl control)
        {
            return GetTableauColumnIndex(control) >= 0;
        }

        private int FindTableauColumnForCard(CardUserControl control)
        {
            return GetTableauColumnIndex(control);
        }

        private void ClearFreeCells()
        {
            if (freeCellControls == null)
            {
                return;
            }
            for (int i = 0; i < freeCellControls.Count; i++)
            {
                if (freeCellControls[i].Card != null)
                {
                    DebugLog($"ClearFreeCells: clearing FreeCell {i} -> {DescribeCard(freeCellControls[i].Card)}");
                }
                freeCellControls[i].Card = null;
                freeCellControls[i].IsFaceUp = false;
            }
        }

        private void ClearAllCards()
        {
            DebugLog("ClearAllCards: begin");
            // Stock / Waste
            StockPile.Card = null; StockPile.IsFaceUp = false;
            WastePile.Card = null; WastePile.IsFaceUp = false;
            // Foundations
            foreach (CardUserControl f in foundationControls)
            {
                f.Card = null; f.IsFaceUp = false; f.Visibility = Visibility.Visible;
            }
            // FreeCells (even if not visible)
            ClearFreeCells();
            // Tableau canvases: remove dynamic controls and reattach base controls
            for (int col = 0; col < tableauCanvases.Count; col++)
            {
                Canvas canvas = tableauCanvases[col];
                // Collect base controls list we track
                List<CardUserControl> baseList = tableauControls[col];
                canvas.Children.Clear();
                foreach (CardUserControl ctrl in baseList)
                {
                    ctrl.Card = null;
                    ctrl.IsFaceUp = false;
                    ctrl.Visibility = Visibility.Hidden; // hidden until a card placed
                    if (!canvas.Children.Contains(ctrl))
                    {
                        canvas.Children.Add(ctrl);
                    }
                }
            }
            DebugLog("ClearAllCards: complete");
        }

        private void InitializeTableauFaceUpStates()
        {
            tableauFaceUpStates = new List<List<bool>>();
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                List<Card> cards = solitaireRules.TableauColumns[col];
                List<bool> states = new List<bool>();
                for (int i = 0; i < cards.Count; i++) { states.Add(false); }
                if (cards.Count > 0) { states[cards.Count - 1] = true; }
                tableauFaceUpStates.Add(states);
            }
        }

        private void UpdateStockPile()
        {
            if (solitaireRules.StockPile.Count > 0)
            {
                StockPile.SetupCard(solitaireRules.StockPile[^1]);
                StockPile.IsFaceUp = false;
            }
            else
            {
                StockPile.Card = null; StockPile.IsFaceUp = false;
            }
            UpdateRecycleButtonVisibility();
        }

        private void UpdateWastePile()
        {
            if (solitaireRules.WastePile.Count > 0)
            {
                WastePile.SetupCard(solitaireRules.WastePile[^1]);
                WastePile.IsFaceUp = true;
            }
            else
            {
                WastePile.Card = null; WastePile.IsFaceUp = false;
            }
            UpdateRecycleButtonVisibility();
        }

        private void UpdateRecycleButtonVisibility()
        {
            RecycleButton.Visibility = (solitaireRules.StockPile.Count == 0 && solitaireRules.WastePile.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateFreeCells()
        {
            if (currentGameType != "Freecell") { return; }
            for (int i = 0; i < freeCellControls.Count; i++)
            {
                Card c = solitaireRules.GetCardFromFreeCell(i);
                if (c != null)
                {
                    freeCellControls[i].SetupCard(c); freeCellControls[i].IsFaceUp = true; freeCellControls[i].Visibility = Visibility.Visible;
                }
                else
                {
                    freeCellControls[i].Card = null; freeCellControls[i].IsFaceUp = false;
                }
            }
        }

        private void UpdateFreecellStatus()
        {
            if (currentGameType != "Freecell") { return; }
            int emptyFree = 0; for (int i = 0; i < 4; i++) { if (solitaireRules.GetCardFromFreeCell(i) == null) { emptyFree++; } }
            FreeCellsLabel.Content = $"Free Cells: {emptyFree}/4";
            int emptyCols = 0; for (int c = 0; c < solitaireRules.TableauColumns.Count; c++) { if (solitaireRules.TableauColumns[c].Count == 0) { emptyCols++; } }
            EmptyColumnsLabel.Content = $"Empty Columns: {emptyCols}";
        }

        private void DisplayGame()
        {
            UpdateStockPile();
            UpdateWastePile();
            UpdateFreeCells();
            // Foundations
            for (int i = 0; i < foundationControls.Count; i++)
            {
                List<Card> pile = solitaireRules.FoundationPiles[i];
                if (pile.Count > 0)
                {
                    foundationControls[i].SetupCard(pile[^1]);
                    foundationControls[i].IsFaceUp = true;
                }
                else { foundationControls[i].Card = null; }
            }
            // Tableau
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                RefreshTableauColumn(col);
            }
            if (currentGameType == "Freecell") { UpdateFreecellStatus(); }
        }

        private void EnsureTableauUiCapacity(int columnIndex, int requiredCount)
        {
            if (columnIndex < 0 || columnIndex >= tableauControls.Count) { return; }
            List<CardUserControl> controls = tableauControls[columnIndex];
            Canvas canvas = tableauCanvases[columnIndex];
            while (controls.Count < requiredCount)
            {
                CardUserControl ctrl = new CardUserControl();
                ctrl.CardDragStarted += OnCardDragStarted;
                ctrl.CardDropped += OnCardDropped;
                ctrl.ValidateDrop += OnValidateDrop;
                ctrl.CardClicked += OnCardClicked;
                ctrl.DebugLog += OnCardDebugLog;
                ctrl.IsDebugMode = dragDropDebugMode;
                canvas.Children.Add(ctrl);
                controls.Add(ctrl);
            }
        }

        private void RefreshTableauColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= solitaireRules.TableauColumns.Count) { return; }
            List<Card> data = solitaireRules.TableauColumns[columnIndex];
            EnsureTableauUiCapacity(columnIndex, data.Count == 0 ? 1 : data.Count);
            EnsureFaceUpStateCapacity(columnIndex, data.Count);
            List<CardUserControl> controls = tableauControls[columnIndex];
            for (int r = 0; r < controls.Count; r++)
            {
                CardUserControl ctrl = controls[r];
                if (r < data.Count)
                {
                    Card card = data[r];
                    ctrl.SetupCard(card);
                    bool faceUp = (r < tableauFaceUpStates[columnIndex].Count && tableauFaceUpStates[columnIndex][r]) || r == data.Count - 1;
                    ctrl.IsFaceUp = faceUp;
                    ctrl.Visibility = Visibility.Visible;
                    Canvas.SetTop(ctrl, r * CardVisualConstants.TableauVerticalOffset);
                    Panel.SetZIndex(ctrl, r);
                    ctrl.VisibleHeight = (r == data.Count - 1) ? CardVisualConstants.CardHeight : CardVisualConstants.TableauVerticalOffset;
                    ctrl.StackPosition = r;
                }
                else
                {
                    if (data.Count == 0 && r == 0)
                    {
                        ctrl.Card = null; ctrl.IsFaceUp = false; ctrl.Visibility = Visibility.Visible; ctrl.VisibleHeight = CardVisualConstants.CardHeight;
                        Canvas.SetTop(ctrl, 0); Panel.SetZIndex(ctrl, 0);
                    }
                    else
                    {
                        ctrl.Card = null; ctrl.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void OnCardDragStarted(object sender, Card card)
        {
            dragSourceControl = sender as CardUserControl;
            StatusLabel.Content = $"Dragging {card.Number} of {card.Suite}s";
            DebugLog($"DragStarted from {DescribeControl(dragSourceControl)} with {DescribeCard(card)}");
        }

        private void OnValidateDrop(object sender, ValidateDropEventArgs e)
        {
            string result = ValidateMoveDetailed(e.DraggedCard, e.TargetControl);
            e.IsValid = result == "Valid";
            DebugLog($"ValidateDrop: {DescribeCard(e.DraggedCard)} from {DescribeControl(dragSourceControl)} to {DescribeControl(e.TargetControl)} -> {result}");
        }

        private void OnCardDropped(object sender, CardDropEventArgs e)
        {
            string result = ValidateMoveDetailed(e.DroppedCard, e.TargetControl);
            DebugLog($"Drop: {DescribeCard(e.DroppedCard)} to {DescribeControl(e.TargetControl)} -> {result}");
            if (result == "Valid")
            {
                ExecuteMove(dragSourceControl, e.TargetControl, e.DroppedCard);
                if (solitaireRules.IsGameWon()) { StatusLabel.Content = "Congratulations! You won the game!"; }
                else { StatusLabel.Content = $"Moved {e.DroppedCard.Number} of {e.DroppedCard.Suite}s successfully"; }
            }
            else { StatusLabel.Content = $"Invalid move: {result}"; }
            dragSourceControl = null;
        }

        private void OnCardClicked(object sender, Card clickedCard)
        {
            CardUserControl src = sender as CardUserControl;
            if (src == null || clickedCard == null) { return; }
            DebugLog($"CardClicked on {DescribeControl(src)} -> {DescribeCard(clickedCard)}");
            int foundationIndex = solitaireRules.FindAvailableFoundationPile(clickedCard);
            if (foundationIndex >= 0)
            {
                ExecuteMove(src, foundationControls[foundationIndex], clickedCard);
                if (solitaireRules.IsGameWon()) { StatusLabel.Content = "Congratulations! You won the game!"; }
                else { StatusLabel.Content = $"Auto-moved {clickedCard.Number} of {clickedCard.Suite}s to foundation"; }
            }
            else { StatusLabel.Content = $"{clickedCard.Number} of {clickedCard.Suite}s cannot be moved to foundation"; }
        }

        private void ExecuteMove(CardUserControl sourceControl, CardUserControl targetControl, Card card)
        {
            DebugLog($"ExecuteMove: {DescribeCard(card)} from {DescribeControl(sourceControl)} to {DescribeControl(targetControl)}");
            List<Card> sequence = GetCardSequenceToMove(sourceControl, card);
            // Free cell
            if (freeCellControls.Contains(targetControl))
            {
                if (sequence.Count > 1) { sequence = new List<Card> { card }; }
                RemoveCardFromSource(sourceControl, card);
                int idx = freeCellControls.IndexOf(targetControl);
                solitaireRules.PlaceCardInFreeCell(card, idx);
                UpdateFreeCells(); UpdateFreecellStatus();
                return;
            }
            // Foundation
            if (foundationControls.Contains(targetControl))
            {
                if (sequence.Count > 1) { sequence = new List<Card> { card }; }
                RemoveCardSequenceFromSource(sourceControl, sequence);
                int fIdx = foundationControls.IndexOf(targetControl);
                solitaireRules.FoundationPiles[fIdx].Add(card);
                targetControl.SetupCard(card); targetControl.IsFaceUp = true;
                return;
            }
            // Tableau
            int targetCol = GetTableauColumnIndex(targetControl);
            if (targetCol >= 0)
            {
                RemoveCardSequenceFromSource(sourceControl, sequence);
                AddCardSequenceToTableau(targetCol, sequence);
                return;
            }
            // Other
            if (targetControl.Card == null)
            {
                if (sequence.Count > 1) { sequence = new List<Card> { card }; }
                targetControl.SetupCard(card); targetControl.IsFaceUp = true; sourceControl.Card = null;
            }
        }

        private void RemoveCardFromSource(CardUserControl sourceControl, Card card)
        {
            if (sourceControl == null || card == null) { return; }
            // From free cell
            if (freeCellControls.Contains(sourceControl))
            {
                int idx = freeCellControls.IndexOf(sourceControl);
                if (solitaireRules.GetCardFromFreeCell(idx) == card)
                {
                    solitaireRules.RemoveCardFromFreeCell(idx);
                }
                UpdateFreeCells(); UpdateFreecellStatus();
                return;
            }
            // From tableau
            int col = GetTableauColumnIndex(sourceControl);
            if (col >= 0)
            {
                List<Card> data = solitaireRules.TableauColumns[col];
                if (data.Count > 0 && data[^1] == card)
                {
                    data.RemoveAt(data.Count - 1);
                    if (data.Count > 0)
                    {
                        EnsureFaceUpStateCapacity(col, data.Count);
                        tableauFaceUpStates[col][data.Count - 1] = true;
                    }
                    RefreshTableauColumn(col);
                }
                return;
            }
            // Foundation
            if (foundationControls.Contains(sourceControl))
            {
                int f = foundationControls.IndexOf(sourceControl);
                List<Card> pile = solitaireRules.FoundationPiles[f];
                if (pile.Count > 0 && pile[^1] == card) { pile.RemoveAt(pile.Count - 1); }
                if (pile.Count > 0) { sourceControl.SetupCard(pile[^1]); sourceControl.IsFaceUp = true; } else { sourceControl.Card = null; }
                return;
            }
            // Waste
            if (ReferenceEquals(sourceControl, WastePile))
            {
                if (solitaireRules.WastePile.Count > 0 && solitaireRules.WastePile[^1] == card)
                {
                    solitaireRules.WastePile.RemoveAt(solitaireRules.WastePile.Count - 1);
                    UpdateWastePile();
                }
                return;
            }
            sourceControl.Card = null;
        }

        private void OnStockPileClicked(object sender, EventArgs e)
        {
            DrawCardFromStock();
        }

        private void DrawCardFromStock()
        {
            if (solitaireRules.StockPile.Count > 0)
            {
                bool drawn = solitaireRules.DrawFromStock();
                if (drawn && solitaireRules.WastePile.Count > 0)
                {
                    UpdateStockPile();
                    UpdateWastePile();
                }
            }
            else if (solitaireRules.WastePile.Count > 0)
            {
                solitaireRules.ResetStock();
                UpdateStockPile();
                UpdateWastePile();
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            DebugLog("NewGameButton_Click: invoked");
            try
            {
                GameSelectionWindow win = new GameSelectionWindow();
                win.Owner = this;
                bool? result = win.ShowDialog();
                DebugLog($"NewGameButton_Click: dialog result={result}, selected='{win.SelectedGameName}'");
                if (result == true && !string.IsNullOrWhiteSpace(win.SelectedGameName))
                {
                    StartNewGame(win.SelectedGameName);
                }
                else
                {
                    DebugLog("NewGameButton_Click: dialog cancelled -> no action taken");
                    // Do nothing when cancelled - don't restart the current game
                }
            }
            catch (Exception ex)
            {
                DebugLog($"NewGameButton_Click: exception {ex.Message} -> restarting current game");
                StartNewGame(currentGameType);
            }
        }

        private void RecycleButton_Click(object sender, RoutedEventArgs e)
        {
            DrawCardFromStock();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ToggleDragDropDebugMode();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Starts a new game of the specified type.
        /// </summary>
        private void StartNewGame(string gameType)
        {
            currentGameType = gameType;
            InitializeGame();
            ClearAllCards();
            DisplayGame();
            StatusLabel.Content = $"New game started: {gameType}";
        }

        /// <summary>
        /// Ensures the tableauFaceUpStates list for a column has at least the required count.
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
    }
}
