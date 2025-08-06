using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Core
{
    public class SolitaireRules
    {
        /// <summary>
        /// Validates if a card can be placed on another card in playing areas (descending, alternating colors)
        /// </summary>
        /// <param name="cardToPlace">Card being moved</param>
        /// <param name="targetCard">Card to place on (null if empty pile)</param>
        /// <returns>True if move is valid</returns>
        public bool CanPlaceOnPlayingArea(Card cardToPlace, Card targetCard)
        {
            if (cardToPlace == null)
            {
                return false;
            }

            // If no target card, only Kings can be placed on empty playing areas
            if (targetCard == null)
            {
                return cardToPlace.Number == Card.CardNumber.K;
            }

            // Check if card is one rank lower than target
            if (!IsOneRankLower(cardToPlace.Number, targetCard.Number))
            {
                return false;
            }

            // Check if card is opposite color from target
            return IsOppositeColor(cardToPlace.Suite, targetCard.Suite);
        }

        /// <summary>
        /// Validates if a card can be placed on foundation piles (Ace piles - ascending, same suit)
        /// </summary>
        /// <param name="cardToPlace">Card being moved</param>
        /// <param name="targetCard">Top card of foundation pile (null if empty)</param>
        /// <returns>True if move is valid</returns>
        public bool CanPlaceOnFoundation(Card cardToPlace, Card targetCard)
        {
            if (cardToPlace == null)
            {
                return false;
            }

            // If no target card, only Aces can be placed on empty foundations
            if (targetCard == null)
            {
                return cardToPlace.Number == Card.CardNumber.A;
            }

            // Must be same suit and one rank higher
            return cardToPlace.Suite == targetCard.Suite && 
                   IsOneRankHigher(cardToPlace.Number, targetCard.Number);
        }

        /// <summary>
        /// Checks if card1 is one rank lower than card2
        /// </summary>
        private bool IsOneRankLower(Card.CardNumber card1, Card.CardNumber card2)
        {
            return (int)card1 == (int)card2 - 1;
        }

        /// <summary>
        /// Checks if card1 is one rank higher than card2
        /// </summary>
        private bool IsOneRankHigher(Card.CardNumber card1, Card.CardNumber card2)
        {
            return (int)card1 == (int)card2 + 1;
        }

        /// <summary>
        /// Checks if two card suits are opposite colors (Red vs Black)
        /// </summary>
        private bool IsOppositeColor(Card.CardSuite suite1, Card.CardSuite suite2)
        {
            bool suite1IsRed = (suite1 == Card.CardSuite.Heart || suite1 == Card.CardSuite.Diamond);
            bool suite2IsRed = (suite2 == Card.CardSuite.Heart || suite2 == Card.CardSuite.Diamond);
            return suite1IsRed != suite2IsRed;
        }
    }
}
