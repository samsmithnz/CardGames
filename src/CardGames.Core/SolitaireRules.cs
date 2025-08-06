using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGames.Core
{
    /// <summary>
    /// Implements the rules and logic for Klondike Solitaire card game
    /// </summary>
    public class SolitaireRules
    {
        /// <summary>
        /// The seven tableau columns where cards are initially dealt and can be built in descending order with alternating colors
        /// </summary>
        public List<List<Card>> TableauColumns { get; private set; }

        /// <summary>
        /// The four foundation piles where cards are built up by suit from Ace to King
        /// </summary>
        public List<List<Card>> FoundationPiles { get; private set; }

        /// <summary>
        /// The stock pile containing face-down cards that can be drawn
        /// </summary>
        public List<Card> StockPile { get; private set; }

        /// <summary>
        /// The waste pile containing face-up cards drawn from the stock
        /// </summary>
        public List<Card> WastePile { get; private set; }

        /// <summary>
        /// Initializes a new Solitaire game with empty game state
        /// </summary>
        public SolitaireRules()
        {
            TableauColumns = new List<List<Card>>();
            for (int i = 0; i < 7; i++)
            {
                TableauColumns.Add(new List<Card>());
            }

            FoundationPiles = new List<List<Card>>();
            for (int i = 0; i < 4; i++)
            {
                FoundationPiles.Add(new List<Card>());
            }

            StockPile = new List<Card>();
            WastePile = new List<Card>();
        }

        /// <summary>
        /// Sets up a new Solitaire game with the provided deck
        /// </summary>
        /// <param name="deck">The shuffled deck to deal from</param>
        public void DealCards(Deck deck)
        {
            if (deck == null || deck.Cards.Count != 52)
            {
                throw new ArgumentException("Deck must contain exactly 52 cards");
            }

            // Clear existing game state
            foreach (List<Card> column in TableauColumns)
            {
                column.Clear();
            }
            foreach (List<Card> foundation in FoundationPiles)
            {
                foundation.Clear();
            }
            StockPile.Clear();
            WastePile.Clear();

            int cardIndex = 0;

            // Deal cards to tableau columns (1 to first column, 2 to second, etc.)
            for (int column = 0; column < 7; column++)
            {
                for (int card = 0; card <= column; card++)
                {
                    TableauColumns[column].Add(deck.Cards[cardIndex]);
                    cardIndex++;
                }
            }

            // Remaining cards go to stock pile
            for (int i = cardIndex; i < deck.Cards.Count; i++)
            {
                StockPile.Add(deck.Cards[i]);
            }
        }

        /// <summary>
        /// Checks if a card can be placed on a tableau column
        /// </summary>
        /// <param name="card">The card to place</param>
        /// <param name="columnIndex">The tableau column index (0-6)</param>
        /// <returns>True if the move is valid, false otherwise</returns>
        public bool CanPlaceCardOnTableau(Card card, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= 7 || card == null)
            {
                return false;
            }

            List<Card> column = TableauColumns[columnIndex];

            // Empty column can only accept Kings
            if (column.Count == 0)
            {
                return card.Number == Card.CardNumber.K;
            }

            // Get the top card of the column
            Card topCard = column[column.Count - 1];

            // Check if card is one rank lower and opposite color
            return IsOneRankLower(card.Number, topCard.Number) && IsOppositeColor(card, topCard);
        }

        /// <summary>
        /// Checks if a card can be placed on a foundation pile
        /// </summary>
        /// <param name="card">The card to place</param>
        /// <param name="foundationIndex">The foundation pile index (0-3)</param>
        /// <returns>True if the move is valid, false otherwise</returns>
        public bool CanPlaceCardOnFoundation(Card card, int foundationIndex)
        {
            if (foundationIndex < 0 || foundationIndex >= 4 || card == null)
            {
                return false;
            }

            List<Card> foundation = FoundationPiles[foundationIndex];

            // Empty foundation can only accept Aces
            if (foundation.Count == 0)
            {
                return card.Number == Card.CardNumber.A;
            }

            // Get the top card of the foundation
            Card topCard = foundation[foundation.Count - 1];

            // Check if card is same suit and one rank higher
            return card.Suite == topCard.Suite && IsOneRankHigher(card.Number, topCard.Number);
        }

        /// <summary>
        /// Checks if the game has been won (all cards moved to foundations)
        /// </summary>
        /// <returns>True if the game is won, false otherwise</returns>
        public bool IsGameWon()
        {
            foreach (List<Card> foundation in FoundationPiles)
            {
                if (foundation.Count != 13)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Draws a card from the stock pile to the waste pile
        /// </summary>
        /// <returns>True if a card was drawn, false if stock pile is empty</returns>
        public bool DrawFromStock()
        {
            if (StockPile.Count == 0)
            {
                return false;
            }

            Card drawnCard = StockPile[StockPile.Count - 1];
            StockPile.RemoveAt(StockPile.Count - 1);
            WastePile.Add(drawnCard);
            return true;
        }

        /// <summary>
        /// Resets the waste pile back to the stock pile when stock is empty
        /// </summary>
        public void ResetStock()
        {
            if (StockPile.Count == 0 && WastePile.Count > 0)
            {
                // Move all waste cards back to stock in reverse order
                for (int i = WastePile.Count - 1; i >= 0; i--)
                {
                    StockPile.Add(WastePile[i]);
                }
                WastePile.Clear();
            }
        }

        /// <summary>
        /// Checks if one card number is exactly one rank lower than another
        /// </summary>
        private bool IsOneRankLower(Card.CardNumber lower, Card.CardNumber higher)
        {
            return (int)lower == (int)higher - 1;
        }

        /// <summary>
        /// Checks if one card number is exactly one rank higher than another
        /// </summary>
        private bool IsOneRankHigher(Card.CardNumber higher, Card.CardNumber lower)
        {
            return (int)higher == (int)lower + 1;
        }

        /// <summary>
        /// Checks if two cards are opposite colors (red vs black)
        /// </summary>
        private bool IsOppositeColor(Card card1, Card card2)
        {
            bool card1IsRed = card1.Suite == Card.CardSuite.Heart || card1.Suite == Card.CardSuite.Diamond;
            bool card2IsRed = card2.Suite == Card.CardSuite.Heart || card2.Suite == Card.CardSuite.Diamond;
            return card1IsRed != card2IsRed;
        }
    }
}
