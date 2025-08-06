using CardGames.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardGames.WinForms
{
    public partial class MainForm : Form
    {
        private Deck Deck = null;
        private SolitaireRules SolitaireRules = null;
        
        public MainForm()
        {
            InitializeComponent();
            Deck = new Deck();
            SolitaireRules = new SolitaireRules();
            Deck.Shuffle();
            LoadCards(Deck);
            SetupDragDropForPanels();

            //button1.Text = new Rune(0x1F0DE).ToString();
            //this.button1.Font = new System.Drawing.Font("Segoe UI", 100F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            //this.button1.Location = new System.Drawing.Point(1089, 358);
            //this.button1.Name = "button1";
            //this.button1.Size = new System.Drawing.Size(300, 400);
            //this.button1.TabIndex = 3;
            //this.button1.Text = "0";
            //this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //this.button1.UseVisualStyleBackColor = true;
        }

        private void LoadCards(Deck deck)
        {
            int xOffset = 0;
            int yOffset = 0;
            foreach (Card card in deck.Cards)
            {
                CardUserControl cardUserControl = new CardUserControl
                {
                    Size = new System.Drawing.Size(200, 300),
                    Location = new Point(0 + xOffset, 0+ yOffset)
                };
                DeckPanel.Controls.Add(cardUserControl);
                cardUserControl.SetupCard(card);
                //xOffset += 4;
                //yOffset += 4;
            }
            statusStrip.Items[0].Text = "Total cards: " + deck.Cards.Count.ToString();
        }

        /// <summary>
        /// Setup drag and drop support for all game panels
        /// </summary>
        private void SetupDragDropForPanels()
        {
            // Enable drop for all game panels
            Panel[] gamePanels = { DeckPanel, panel1, panel2, panel3, panel5, 
                                   panel6, panel7, panel8, panel9, panel10, panel11, panel12 };
            
            foreach (Panel panel in gamePanels)
            {
                panel.AllowDrop = true;
                panel.DragEnter += Panel_DragEnter;
                panel.DragDrop += Panel_DragDrop;
                panel.DragOver += Panel_DragOver;
                panel.DragLeave += Panel_DragLeave;
            }
        }

        /// <summary>
        /// Handle drag enter for individual panels with visual feedback
        /// </summary>
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                Panel targetPanel = sender as Panel;
                CardUserControl draggedCard = (CardUserControl)e.Data.GetData(typeof(CardUserControl));
                
                if (IsValidMove(draggedCard, targetPanel))
                {
                    e.Effect = DragDropEffects.Move;
                    targetPanel.BackColor = Color.LightBlue; // Visual feedback for valid drop
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    targetPanel.BackColor = Color.LightCoral; // Visual feedback for invalid drop
                }
            }
        }

        /// <summary>
        /// Handle drag over to maintain visual feedback
        /// </summary>
        private void Panel_DragOver(object sender, DragEventArgs e)
        {
            // This ensures DragEnter behavior is maintained during drag over
            Panel_DragEnter(sender, e);
        }

        /// <summary>
        /// Handle drag leave to reset visual feedback
        /// </summary>
        private void Panel_DragLeave(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                panel.BackColor = Color.Transparent; // Reset background color
            }
        }

        /// <summary>
        /// Handle card drop on panels
        /// </summary>
        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            Panel targetPanel = sender as Panel;
            targetPanel.BackColor = Color.Transparent; // Reset background color
            
            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                CardUserControl draggedCard = (CardUserControl)e.Data.GetData(typeof(CardUserControl));
                
                if (IsValidMove(draggedCard, targetPanel))
                {
                    // Remove card from current parent
                    draggedCard.Parent.Controls.Remove(draggedCard);
                    
                    // Add to new panel
                    targetPanel.Controls.Add(draggedCard);
                    
                    // Position the card in the target panel
                    PositionCardInPanel(draggedCard, targetPanel);
                    
                    // Bring to front
                    draggedCard.BringToFront();
                    draggedCard.CardVisible = true;
                }
            }
        }

        /// <summary>
        /// Validates if a card can be moved to a specific panel based on Solitaire rules
        /// </summary>
        private bool IsValidMove(CardUserControl draggedCard, Panel targetPanel)
        {
            if (draggedCard?.Card == null || targetPanel == null)
            {
                return false;
            }

            string panelTag = targetPanel.Tag?.ToString() ?? "";
            
            // Get the top card of the target panel (if any)
            CardUserControl topCard = GetTopCardInPanel(targetPanel);
            
            // Apply rules based on panel type
            if (panelTag.StartsWith("Ace")) // Foundation piles
            {
                return SolitaireRules.CanPlaceOnFoundation(draggedCard.Card, topCard?.Card);
            }
            else if (panelTag.StartsWith("PlayingArea")) // Playing areas
            {
                return SolitaireRules.CanPlaceOnPlayingArea(draggedCard.Card, topCard?.Card);
            }
            else if (panelTag == "DeckDiscard") // Discard pile - always allow
            {
                return true;
            }
            else if (panelTag == "Deck") // Deck pile - don't allow drops
            {
                return false;
            }
            
            return false; // Unknown panel type
        }

        /// <summary>
        /// Gets the topmost card in a panel (the one that would be the drop target)
        /// </summary>
        private CardUserControl GetTopCardInPanel(Panel panel)
        {
            CardUserControl topCard = null;
            int highestZOrder = -1;
            
            foreach (Control control in panel.Controls)
            {
                if (control is CardUserControl cardControl)
                {
                    int zOrder = panel.Controls.GetChildIndex(cardControl);
                    if (zOrder > highestZOrder)
                    {
                        highestZOrder = zOrder;
                        topCard = cardControl;
                    }
                }
            }
            
            return topCard;
        }

        /// <summary>
        /// Positions a card appropriately within a panel based on panel type and existing cards
        /// </summary>
        private void PositionCardInPanel(CardUserControl card, Panel panel)
        {
            string panelTag = panel.Tag?.ToString() ?? "";
            
            if (panelTag.StartsWith("Ace")) // Foundation piles - stack cards
            {
                card.Location = new Point(5, 5);
            }
            else if (panelTag.StartsWith("PlayingArea")) // Playing areas - cascade cards
            {
                int cardCount = panel.Controls.OfType<CardUserControl>().Count();
                card.Location = new Point(5, 5 + (cardCount * 25)); // Cascade effect
            }
            else // Default positioning
            {
                card.Location = new Point(5, 5);
            }
        }

        //Fallback: Check if the card is allowed to be dropped on main form
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        //Fallback: Drop the card on main form (allows free positioning)
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                CardUserControl draggedCard = (CardUserControl)e.Data.GetData(typeof(CardUserControl));
                if (draggedCard != null)
                {
                    // Convert cursor position to form coordinates
                    Point dropLocation = this.PointToClient(Cursor.Position);
                    draggedCard.Location = dropLocation;
                    draggedCard.CardVisible = true;
                    draggedCard.BringToFront();
                }
            }
        }
    }
}
