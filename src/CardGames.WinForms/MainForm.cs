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
            solitaireRules = new SolitaireRules();
            solitaireRules.NewGame();

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
                wastePileControl.Visible = false;
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
                    foundationControls[i].Visible = false;
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
        }

        private void Foundation_CardDropped(object sender, CardDropEventArgs e)
        {
            // Handle card dropped on foundation
            int foundationIndex = foundationControls.IndexOf(e.TargetControl);
            if (foundationIndex >= 0)
            {
                // Try to move card to foundation
                // This would need more complex logic to handle different source locations
                statusLabel.Text = $"Card dropped on foundation {foundationIndex + 1}";
                UpdateAllUI();
            }
        }

        private void Foundation_ValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Validate if card can be dropped on foundation
            int foundationIndex = foundationControls.IndexOf(e.TargetControl);
            if (foundationIndex >= 0)
            {
                // Simple validation - would need more logic for actual game rules
                e.IsValid = true; // Placeholder
            }
        }

        private void Tableau_CardDragStarted(object sender, Card card)
        {
            // Tableau card is being dragged
        }

        private void Tableau_CardDropped(object sender, CardDropEventArgs e)
        {
            // Handle card dropped on tableau
            statusLabel.Text = "Card dropped on tableau";
            UpdateAllUI();
        }

        private void Tableau_ValidateDrop(object sender, ValidateDropEventArgs e)
        {
            // Validate if card can be dropped on tableau
            e.IsValid = true; // Placeholder
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
                    statusLabel.Text = $"Clicked {card.Number} of {card.Suite}s";
                }
            }
        }

        #endregion
    }
}