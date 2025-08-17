using CardGames.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CardGames.WinForms
{
    /// <summary>
    /// Main form for the Solitaire game using Windows Forms
    /// </summary>
    public partial class MainForm : Form
    {
        private Deck deck;
        private SolitaireRules solitaireRules;
        private List<List<CardUserControl>> tableauControls;
        private List<List<bool>> tableauFaceUpStates;
        private List<Panel> tableauPanels;
        private CardUserControl stockPileControl;
        private CardUserControl wastePileControl;
        private List<CardUserControl> foundationControls;
        private Label statusLabel;
        private Button newGameButton;
        private Button drawCardButton;

        public MainForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        /// <summary>
        /// Initialize the form components and layout
        /// </summary>
        private void InitializeComponent()
        {
            Text = "Solitaire - Card Games (Windows Forms)";
            Size = new Size(1200, 700);
            MinimumSize = new Size(1000, 600);
            BackColor = Color.DarkGreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterScreen;

            // Create status bar
            statusLabel = new Label
            {
                Text = "Welcome to Solitaire!",
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            Controls.Add(statusLabel);

            // Create button panel
            Panel buttonPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom,
                BackColor = Color.LightGray
            };

            newGameButton = new Button
            {
                Text = "New Game",
                Size = new Size(100, 30),
                Location = new Point(10, 5)
            };
            newGameButton.Click += NewGameButton_Click;
            buttonPanel.Controls.Add(newGameButton);

            drawCardButton = new Button
            {
                Text = "Draw Card",
                Size = new Size(100, 30),
                Location = new Point(120, 5)
            };
            drawCardButton.Click += DrawCardButton_Click;
            buttonPanel.Controls.Add(drawCardButton);

            Controls.Add(buttonPanel);

            // Create game area panel
            Panel gamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGreen
            };
            Controls.Add(gamePanel);

            SetupGameArea(gamePanel);
        }

        /// <summary>
        /// Set up the game area with all card positions
        /// </summary>
        private void SetupGameArea(Panel gamePanel)
        {
            int cardWidth = (int)CardVisualConstants.CardWidth;
            int cardHeight = (int)CardVisualConstants.CardHeight;
            int margin = 20;

            // Stock pile (top-left)
            stockPileControl = new CardUserControl
            {
                Location = new Point(margin, margin),
                Size = new Size(cardWidth, cardHeight),
                IsFaceUp = false
            };
            stockPileControl.CardClicked += StockPile_CardClicked;
            gamePanel.Controls.Add(stockPileControl);

            Label stockLabel = new Label
            {
                Text = "Stock",
                Location = new Point(margin, margin + cardHeight + 5),
                Size = new Size(cardWidth, 20),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter
            };
            gamePanel.Controls.Add(stockLabel);

            // Waste pile (next to stock)
            wastePileControl = new CardUserControl
            {
                Location = new Point(margin * 2 + cardWidth, margin),
                Size = new Size(cardWidth, cardHeight),
                IsFaceUp = true
            };
            wastePileControl.CardDragStarted += WastePile_CardDragStarted;
            wastePileControl.CardClicked += WastePile_CardClicked;
            wastePileControl.CardDropped += WastePile_CardDropped;
            wastePileControl.ValidateDrop += WastePile_ValidateDrop;
            gamePanel.Controls.Add(wastePileControl);

            Label wasteLabel = new Label
            {
                Text = "Waste",
                Location = new Point(margin * 2 + cardWidth, margin + cardHeight + 5),
                Size = new Size(cardWidth, 20),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter
            };
            gamePanel.Controls.Add(wasteLabel);

            // Foundation piles (top-right, 4 piles)
            foundationControls = new List<CardUserControl>();
            string[] suitSymbols = { "♥", "♣", "♦", "♠" };
            Color[] suitColors = { Color.Red, Color.Black, Color.Red, Color.Black };

            for (int i = 0; i < 4; i++)
            {
                CardUserControl foundationControl = new CardUserControl
                {
                    Location = new Point(gamePanel.Width - (4 - i) * (cardWidth + margin), margin),
                    Size = new Size(cardWidth, cardHeight),
                    IsFaceUp = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                foundationControl.CardDropped += Foundation_CardDropped;
                foundationControl.ValidateDrop += Foundation_ValidateDrop;
                foundationControl.CardClicked += Foundation_CardClicked;
                foundationControls.Add(foundationControl);
                gamePanel.Controls.Add(foundationControl);

                Label foundationLabel = new Label
                {
                    Text = suitSymbols[i],
                    Location = new Point(gamePanel.Width - (4 - i) * (cardWidth + margin), margin + cardHeight + 5),
                    Size = new Size(cardWidth, 20),
                    ForeColor = suitColors[i],
                    TextAlign = ContentAlignment.TopCenter,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                gamePanel.Controls.Add(foundationLabel);
            }

            // Tableau columns (7 columns in the middle-bottom area)
            int tableauStartY = margin * 2 + cardHeight + 40;
            int columnWidth = cardWidth + 10;
            int tableauStartX = (gamePanel.Width - (7 * columnWidth)) / 2;

            tableauPanels = new List<Panel>();
            tableauControls = new List<List<CardUserControl>>();

            for (int col = 0; col < 7; col++)
            {
                Panel columnPanel = new Panel
                {
                    Location = new Point(tableauStartX + col * columnWidth, tableauStartY),
                    Size = new Size(cardWidth, 400),
                    BackColor = Color.Transparent,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                };
                
                // Add column border
                columnPanel.Paint += (s, e) =>
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, columnPanel.Width - 1, Math.Min(cardHeight, columnPanel.Height - 1));
                };

                tableauPanels.Add(columnPanel);
                gamePanel.Controls.Add(columnPanel);

                // Initialize card controls for this column (start with 7 controls per column)
                List<CardUserControl> columnControls = new List<CardUserControl>();
                for (int row = 0; row < 7; row++)
                {
                    CardUserControl cardControl = new CardUserControl
                    {
                        Location = new Point(0, row * 20), // Overlap cards
                        Size = new Size(cardWidth, cardHeight),
                        Visible = false
                    };
                    cardControl.CardDragStarted += Tableau_CardDragStarted;
                    cardControl.CardDropped += Tableau_CardDropped;
                    cardControl.ValidateDrop += Tableau_ValidateDrop;
                    cardControl.CardClicked += Tableau_CardClicked;
                    
                    columnControls.Add(cardControl);
                    columnPanel.Controls.Add(cardControl);
                }
                tableauControls.Add(columnControls);
            }

            // Handle resize to reposition foundation piles
            gamePanel.Resize += (s, e) =>
            {
                for (int i = 0; i < foundationControls.Count; i++)
                {
                    foundationControls[i].Location = new Point(gamePanel.Width - (4 - i) * (cardWidth + margin), margin);
                }
                
                // Reposition tableau columns
                int newTableauStartX = (gamePanel.Width - (7 * columnWidth)) / 2;
                for (int col = 0; col < tableauPanels.Count; col++)
                {
                    tableauPanels[col].Location = new Point(newTableauStartX + col * columnWidth, tableauStartY);
                }
            };
        }

        /// <summary>
        /// Initialize the game components and logic
        /// </summary>
        private void InitializeGame()
        {
            // Initialize face-up state tracking for tableau columns
            tableauFaceUpStates = new List<List<bool>>();
            for (int i = 0; i < 7; i++)
            {
                tableauFaceUpStates.Add(new List<bool>());
            }

            // Automatically start a new game
            StartNewGame();
        }

        /// <summary>
        /// Start a new game of Solitaire
        /// </summary>
        private void StartNewGame()
        {
            deck = new Deck();
            solitaireRules = new SolitaireRules();
            
            // Shuffle deck and deal new game
            deck.Shuffle();
            solitaireRules.DealCards(deck);

            // Clear face-up states
            foreach (List<bool> columnStates in tableauFaceUpStates)
            {
                columnStates.Clear();
            }

            // Initialize face-up states for tableau based on standard Solitaire rules
            InitializeTableauFaceUpStates();

            // Update all UI elements
            UpdateAllUI();

            statusLabel.Text = "New game started! Good luck!";
        }

        /// <summary>
        /// Initialize face-up states for tableau columns based on standard Solitaire rules
        /// </summary>
        private void InitializeTableauFaceUpStates()
        {
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                List<Card> column = solitaireRules.TableauColumns[col];
                List<bool> faceUpStates = tableauFaceUpStates[col];

                faceUpStates.Clear();
                for (int row = 0; row < column.Count; row++)
                {
                    // Only the last card in each column should be face-up initially
                    faceUpStates.Add(row == column.Count - 1);
                }
            }
        }

        /// <summary>
        /// Update all UI elements to reflect current game state
        /// </summary>
        private void UpdateAllUI()
        {
            UpdateStockPile();
            UpdateWastePile();
            UpdateFoundationPiles();
            UpdateTableauColumns();
        }

        /// <summary>
        /// Update the stock pile display
        /// </summary>
        private void UpdateStockPile()
        {
            if (solitaireRules.StockPile.Count > 0)
            {
                stockPileControl.Card = solitaireRules.StockPile[0]; // Just show we have cards
                stockPileControl.IsFaceUp = false;
                stockPileControl.Visible = true;
            }
            else
            {
                stockPileControl.Visible = false;
            }
        }

        /// <summary>
        /// Update the waste pile display
        /// </summary>
        private void UpdateWastePile()
        {
            if (solitaireRules.WastePile.Count > 0)
            {
                wastePileControl.Card = solitaireRules.WastePile[solitaireRules.WastePile.Count - 1];
                wastePileControl.IsFaceUp = true;
                wastePileControl.Visible = true;
            }
            else
            {
                wastePileControl.Card = null;
                wastePileControl.IsFaceUp = false;
                wastePileControl.Visible = true; // Keep visible for game interaction
            }
        }

        /// <summary>
        /// Update the foundation piles display
        /// </summary>
        private void UpdateFoundationPiles()
        {
            for (int i = 0; i < foundationControls.Count; i++)
            {
                if (i < solitaireRules.FoundationPiles.Count && solitaireRules.FoundationPiles[i].Count > 0)
                {
                    foundationControls[i].Card = solitaireRules.FoundationPiles[i][solitaireRules.FoundationPiles[i].Count - 1];
                    foundationControls[i].IsFaceUp = true;
                    foundationControls[i].Visible = true;
                }
                else
                {
                    foundationControls[i].Card = null;
                    foundationControls[i].IsFaceUp = false;
                    foundationControls[i].Visible = true; // Keep visible to accept drops
                }
            }
        }

        /// <summary>
        /// Update the tableau columns display
        /// </summary>
        private void UpdateTableauColumns()
        {
            for (int col = 0; col < tableauControls.Count && col < solitaireRules.TableauColumns.Count; col++)
            {
                List<Card> column = solitaireRules.TableauColumns[col];
                List<CardUserControl> controls = tableauControls[col];
                List<bool> faceUpStates = tableauFaceUpStates[col];

                // Hide all controls first
                foreach (CardUserControl control in controls)
                {
                    control.Visible = false;
                }

                // Show controls for actual cards
                for (int row = 0; row < column.Count && row < controls.Count; row++)
                {
                    CardUserControl control = controls[row];
                    control.Card = column[row];
                    control.IsFaceUp = row < faceUpStates.Count ? faceUpStates[row] : false;
                    control.Location = new Point(0, row * 20); // Stack with 20px offset
                    control.Visible = true;
                    control.BringToFront();
                }
            }
        }

        #region Event Handlers

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void DrawCardButton_Click(object sender, EventArgs e)
        {
            DrawCardFromStock();
        }

        private void StockPile_CardClicked(object sender, Card card)
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
                    Card drawnCard = solitaireRules.WastePile[solitaireRules.WastePile.Count - 1];
                    UpdateStockPile();
                    UpdateWastePile();
                    statusLabel.Text = $"Drew {drawnCard.Number} of {drawnCard.Suite}s";
                }
            }
            else if (solitaireRules.WastePile.Count > 0)
            {
                solitaireRules.ResetStock();
                UpdateStockPile();
                UpdateWastePile();
                statusLabel.Text = "Stock pile reset";
            }
            else
            {
                statusLabel.Text = "No more cards to draw";
            }
        }

        private void WastePile_CardDragStarted(object sender, Card card)
        {
            // Waste pile card is being dragged
            statusLabel.Text = $"Dragging {card.Number} of {card.Suite}s from waste pile";
        }

        private void WastePile_CardDropped(object sender, CardDropEventArgs e)
        {
            // Cards can't normally be dropped on waste pile in Solitaire
            statusLabel.Text = "Cannot drop cards on waste pile";
        }

        private void WastePile_ValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Cards can't normally be dropped on waste pile in Solitaire
            e.IsValid = false;
        }

        private void Foundation_CardDropped(object sender, CardDropEventArgs e)
        {
            // Handle card dropped on foundation
            int foundationIndex = foundationControls.IndexOf(e.TargetControl);
            if (foundationIndex >= 0)
            {
                ExecuteMove(e.SourceCard, e.TargetControl, foundationIndex);
            }
        }

        private void Foundation_ValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Validate if card can be dropped on foundation
            int foundationIndex = foundationControls.IndexOf(e.TargetControl);
            if (foundationIndex >= 0)
            {
                e.IsValid = solitaireRules.CanPlaceCardOnFoundation(e.SourceCard, foundationIndex);
            }
        }

        private void Tableau_CardDragStarted(object sender, Card card)
        {
            // Tableau card is being dragged
        }

        private void Tableau_CardDropped(object sender, CardDropEventArgs e)
        {
            // Handle card dropped on tableau
            int targetColumnIndex = GetTableauColumnIndex(e.TargetControl);
            if (targetColumnIndex >= 0)
            {
                ExecuteMove(e.SourceCard, e.TargetControl, targetColumnIndex);
            }
        }

        private void Tableau_ValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Validate if card can be dropped on tableau
            int targetColumnIndex = GetTableauColumnIndex(e.TargetControl);
            if (targetColumnIndex >= 0)
            {
                e.IsValid = solitaireRules.CanPlaceCardOnTableau(e.SourceCard, targetColumnIndex);
            }
        }

        private void Tableau_CardClicked(object sender, Card card)
        {
            // Handle tableau card clicked - flip face-up cards or move to foundation
            int colIndex = -1;
            int rowIndex = -1;

            // Find which column and row was clicked
            for (int col = 0; col < tableauControls.Count; col++)
            {
                for (int row = 0; row < tableauControls[col].Count; row++)
                {
                    if (tableauControls[col][row].Card != null && 
                        tableauControls[col][row].Card.Equals(card) &&
                        tableauControls[col][row] == sender)
                    {
                        colIndex = col;
                        rowIndex = row;
                        break;
                    }
                }
                if (colIndex >= 0) break;
            }

            if (colIndex >= 0 && rowIndex >= 0)
            {
                // If card is face down, flip it
                if (rowIndex < tableauFaceUpStates[colIndex].Count && !tableauFaceUpStates[colIndex][rowIndex])
                {
                    tableauFaceUpStates[colIndex][rowIndex] = true;
                    UpdateTableauColumns();
                    statusLabel.Text = $"Flipped {card.Number} of {card.Suite}s face up";
                }
                else
                {
                    // Try to auto-move to foundation if card is face up and at the top of its column
                    if (rowIndex == solitaireRules.TableauColumns[colIndex].Count - 1)
                    {
                        int foundationIndex = solitaireRules.FindAvailableFoundationPile(card);
                        if (foundationIndex >= 0)
                        {
                            // Execute the move to foundation
                            CardUserControl sourceControl = (CardUserControl)sender;
                            CardUserControl targetControl = foundationControls[foundationIndex];
                            ExecuteMove(card, targetControl, foundationIndex);
                        }
                        else
                        {
                            statusLabel.Text = $"{card.Number} of {card.Suite}s cannot be moved to foundation";
                        }
                    }
                    else
                    {
                        statusLabel.Text = $"Clicked {card.Number} of {card.Suite}s";
                    }
                }
            }
        }

        /// <summary>
        /// Handle foundation card clicked - for future functionality
        /// </summary>
        private void Foundation_CardClicked(object sender, Card card)
        {
            // Foundation cards are generally not moved once placed
            // This could be used for other functionality in the future
            statusLabel.Text = $"Clicked {card.Number} of {card.Suite}s on foundation";
        }

        /// <summary>
        /// Handle waste pile card clicked - try to auto-move to foundation
        /// </summary>
        private void WastePile_CardClicked(object sender, Card card)
        {
            if (card != null && solitaireRules.WastePile.Count > 0 && card.Equals(solitaireRules.WastePile[solitaireRules.WastePile.Count - 1]))
            {
                int foundationIndex = solitaireRules.FindAvailableFoundationPile(card);
                if (foundationIndex >= 0)
                {
                    CardUserControl targetControl = foundationControls[foundationIndex];
                    ExecuteMove(card, targetControl, foundationIndex);
                }
                else
                {
                    statusLabel.Text = $"{card.Number} of {card.Suite}s cannot be moved to foundation";
                }
            }
        }

        /// <summary>
        /// Execute a move between card piles
        /// </summary>
        private void ExecuteMove(Card card, CardUserControl targetControl, int targetIndex)
        {
            CardUserControl sourceControl = FindSourceControl(card);
            if (sourceControl == null)
            {
                statusLabel.Text = "Cannot find source of card";
                return;
            }

            // Determine source pile type and remove card
            bool cardRemoved = false;
            
            // Check if source is waste pile
            if (sourceControl == wastePileControl && solitaireRules.WastePile.Count > 0 && 
                card.Equals(solitaireRules.WastePile[solitaireRules.WastePile.Count - 1]))
            {
                solitaireRules.WastePile.RemoveAt(solitaireRules.WastePile.Count - 1);
                cardRemoved = true;
            }
            
            // Check if source is tableau
            int sourceTableauColumn = GetTableauColumnIndex(sourceControl);
            if (sourceTableauColumn >= 0)
            {
                List<Card> column = solitaireRules.TableauColumns[sourceTableauColumn];
                if (column.Count > 0 && card.Equals(column[column.Count - 1]))
                {
                    column.RemoveAt(column.Count - 1);
                    
                    // Remove face-up state
                    if (tableauFaceUpStates[sourceTableauColumn].Count > 0)
                    {
                        tableauFaceUpStates[sourceTableauColumn].RemoveAt(tableauFaceUpStates[sourceTableauColumn].Count - 1);
                    }
                    
                    // If there are remaining cards and the top one is face down, flip it
                    if (column.Count > 0 && tableauFaceUpStates[sourceTableauColumn].Count > 0 && 
                        !tableauFaceUpStates[sourceTableauColumn][tableauFaceUpStates[sourceTableauColumn].Count - 1])
                    {
                        tableauFaceUpStates[sourceTableauColumn][tableauFaceUpStates[sourceTableauColumn].Count - 1] = true;
                    }
                    
                    cardRemoved = true;
                }
            }

            if (!cardRemoved)
            {
                statusLabel.Text = "Cannot remove card from source";
                return;
            }

            // Add card to destination
            if (foundationControls.Contains(targetControl))
            {
                // Moving to foundation
                solitaireRules.FoundationPiles[targetIndex].Add(card);
                statusLabel.Text = $"Moved {card.Number} of {card.Suite}s to foundation";
                
                // Check for win condition
                if (solitaireRules.IsGameWon())
                {
                    statusLabel.Text = "Congratulations! You won the game!";
                }
            }
            else
            {
                // Moving to tableau
                solitaireRules.TableauColumns[targetIndex].Add(card);
                tableauFaceUpStates[targetIndex].Add(true); // Cards moved to tableau are always face up
                statusLabel.Text = $"Moved {card.Number} of {card.Suite}s to tableau";
            }

            // Update all UI
            UpdateAllUI();
        }

        /// <summary>
        /// Find the source control for a given card
        /// </summary>
        private CardUserControl FindSourceControl(Card card)
        {
            // Check waste pile
            if (solitaireRules.WastePile.Count > 0 && card.Equals(solitaireRules.WastePile[solitaireRules.WastePile.Count - 1]))
            {
                return wastePileControl;
            }

            // Check tableau columns
            for (int col = 0; col < solitaireRules.TableauColumns.Count; col++)
            {
                List<Card> column = solitaireRules.TableauColumns[col];
                if (column.Count > 0 && card.Equals(column[column.Count - 1]))
                {
                    // Find the control for the top card in this column
                    if (col < tableauControls.Count)
                    {
                        List<CardUserControl> controls = tableauControls[col];
                        for (int row = controls.Count - 1; row >= 0; row--)
                        {
                            if (controls[row].Visible && controls[row].Card != null && controls[row].Card.Equals(card))
                            {
                                return controls[row];
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the tableau column index for a given CardUserControl
        /// </summary>
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

        #endregion
    }
}