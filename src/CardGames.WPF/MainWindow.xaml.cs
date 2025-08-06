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

namespace CardGames.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Deck Deck = null;
        private SolitaireRules SolitaireRules = null;

        public MainWindow()
        {
            InitializeComponent();
            Deck = new Deck();
            SolitaireRules = new SolitaireRules();
            Deck.Shuffle();
            LoadCards(Deck);
        }

        private void LoadCards(Deck deck)
        {
            int cardIndex = 0;
            
            // Deal cards to playing areas (7 columns)
            for (int column = 0; column < 7; column++)
            {
                Canvas playingArea = GetPlayingAreaByIndex(column);
                if (playingArea != null)
                {
                    for (int row = 0; row <= column && cardIndex < deck.Cards.Count; row++)
                    {
                        CardUserControl cardUserControl = new CardUserControl
                        {
                            Width = 100,
                            Height = 140
                        };
                        cardUserControl.SetupCard(deck.Cards[cardIndex]);
                        
                        // Only show the top card face up
                        cardUserControl.IsFaceUp = (row == column);
                        
                        Canvas.SetLeft(cardUserControl, 0);
                        Canvas.SetTop(cardUserControl, row * 20); // Cascade effect
                        Canvas.SetZIndex(cardUserControl, row);
                        
                        playingArea.Children.Add(cardUserControl);
                        cardIndex++;
                    }
                }
            }

            // Put remaining cards in deck pile
            Canvas deckArea = DeckPile as Canvas;
            if (deckArea == null)
            {
                // Create a canvas inside the border if it doesn't exist
                deckArea = new Canvas();
                DeckPile.Child = deckArea;
            }

            for (int i = cardIndex; i < deck.Cards.Count; i++)
            {
                CardUserControl cardUserControl = new CardUserControl
                {
                    Width = 100,
                    Height = 140
                };
                cardUserControl.SetupCard(deck.Cards[i]);
                cardUserControl.IsFaceUp = false;
                
                Canvas.SetLeft(cardUserControl, 0);
                Canvas.SetTop(cardUserControl, 0);
                Canvas.SetZIndex(cardUserControl, i - cardIndex);
                
                deckArea.Children.Add(cardUserControl);
            }
        }

        private Canvas GetPlayingAreaByIndex(int index)
        {
            switch (index)
            {
                case 0: return PlayingArea1;
                case 1: return PlayingArea2;
                case 2: return PlayingArea3;
                case 3: return PlayingArea4;
                case 4: return PlayingArea5;
                case 5: return PlayingArea6;
                case 6: return PlayingArea7;
                default: return null;
            }
        }

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                CardUserControl draggedCard = e.Data.GetData(typeof(CardUserControl)) as CardUserControl;
                FrameworkElement targetPanel = sender as FrameworkElement;
                
                if (draggedCard != null && targetPanel != null && IsValidMove(draggedCard, targetPanel))
                {
                    e.Effects = DragDropEffects.Move;
                    targetPanel.Background = new SolidColorBrush(Colors.LightBlue);
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    targetPanel.Background = new SolidColorBrush(Colors.LightCoral);
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Panel_DragOver(object sender, DragEventArgs e)
        {
            // Maintain DragEnter behavior during drag over
            Panel_DragEnter(sender, e);
        }

        private void Panel_Drop(object sender, DragEventArgs e)
        {
            FrameworkElement targetPanel = sender as FrameworkElement;
            
            // Reset background color
            if (targetPanel != null)
            {
                SetOriginalPanelBackground(targetPanel);
            }

            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                CardUserControl draggedCard = e.Data.GetData(typeof(CardUserControl)) as CardUserControl;

                if (draggedCard != null && targetPanel != null && IsValidMove(draggedCard, targetPanel))
                {
                    // Remove card from its current parent
                    Panel sourcePanel = draggedCard.Parent as Panel;
                    sourcePanel?.Children.Remove(draggedCard);

                    // Add card to target panel
                    Panel targetPanelContainer = GetPanelContainer(targetPanel);
                    if (targetPanelContainer != null)
                    {
                        PositionCardInPanel(draggedCard, targetPanelContainer, targetPanel.Name);
                        targetPanelContainer.Children.Add(draggedCard);
                    }
                }
            }
        }

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CardUserControl)))
            {
                CardUserControl draggedCard = e.Data.GetData(typeof(CardUserControl)) as CardUserControl;
                if (draggedCard != null)
                {
                    // Remove from current parent
                    Panel sourcePanel = draggedCard.Parent as Panel;
                    sourcePanel?.Children.Remove(draggedCard);

                    // Add to main grid for free positioning
                    Point dropPosition = e.GetPosition(this);
                    draggedCard.Margin = new Thickness(dropPosition.X, dropPosition.Y, 0, 0);
                    ((Grid)this.Content).Children.Add(draggedCard);
                }
            }
        }

        private bool IsValidMove(CardUserControl draggedCard, FrameworkElement targetPanel)
        {
            if (draggedCard?.Card == null || targetPanel == null)
            {
                return false;
            }

            string panelName = targetPanel.Name;
            
            // Foundation piles (Ace piles)
            if (panelName.StartsWith("FoundationPile"))
            {
                Card topCard = GetTopCardInPanel(targetPanel);
                return SolitaireRules.CanPlaceOnFoundation(draggedCard.Card, topCard);
            }
            
            // Playing areas
            if (panelName.StartsWith("PlayingArea"))
            {
                Card topCard = GetTopCardInPanel(targetPanel);
                return SolitaireRules.CanPlaceOnPlayingArea(draggedCard.Card, topCard);
            }
            
            // Discard pile accepts any card
            if (panelName == "DiscardPile")
            {
                return true;
            }
            
            // Deck pile rejects all drops
            if (panelName == "DeckPile")
            {
                return false;
            }

            return false;
        }

        private Card GetTopCardInPanel(FrameworkElement panel)
        {
            Panel panelContainer = GetPanelContainer(panel);
            if (panelContainer?.Children.Count > 0)
            {
                // Get the card with highest Z-index
                CardUserControl topCard = null;
                int highestZIndex = -1;
                
                foreach (UIElement child in panelContainer.Children)
                {
                    if (child is CardUserControl card)
                    {
                        int zIndex = Canvas.GetZIndex(card);
                        if (zIndex > highestZIndex)
                        {
                            highestZIndex = zIndex;
                            topCard = card;
                        }
                    }
                }
                
                return topCard?.Card;
            }
            return null;
        }

        private Panel GetPanelContainer(FrameworkElement panel)
        {
            if (panel is Canvas canvas)
            {
                return canvas;
            }
            
            if (panel is Border border)
            {
                if (border.Child is Canvas existingCanvas)
                {
                    return existingCanvas;
                }
                else
                {
                    // Create a canvas inside the border
                    Canvas newCanvas = new Canvas();
                    border.Child = newCanvas;
                    return newCanvas;
                }
            }
            
            return panel as Panel;
        }

        private void PositionCardInPanel(CardUserControl card, Panel targetPanel, string panelName)
        {
            if (targetPanel is Canvas canvas)
            {
                Canvas.SetLeft(card, 0);
                
                if (panelName.StartsWith("FoundationPile"))
                {
                    // Stack cards in foundation piles
                    Canvas.SetTop(card, 0);
                    Canvas.SetZIndex(card, canvas.Children.Count);
                }
                else if (panelName.StartsWith("PlayingArea"))
                {
                    // Cascade cards in playing areas
                    Canvas.SetTop(card, canvas.Children.Count * 20);
                    Canvas.SetZIndex(card, canvas.Children.Count);
                }
                else
                {
                    // Default positioning
                    Canvas.SetTop(card, 0);
                    Canvas.SetZIndex(card, canvas.Children.Count);
                }
            }
        }

        private void SetOriginalPanelBackground(FrameworkElement panel)
        {
            string panelName = panel.Name;
            
            if (panelName.StartsWith("FoundationPile") || panelName == "DeckPile")
            {
                panel.Background = new SolidColorBrush(Colors.LightGray);
            }
            else if (panelName.StartsWith("PlayingArea"))
            {
                panel.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else if (panelName == "DiscardPile")
            {
                panel.Background = new SolidColorBrush(Colors.LightYellow);
            }
            else if (panelName == "DeckPile")
            {
                panel.Background = new SolidColorBrush(Colors.DarkGray);
            }
        }
    }
}
