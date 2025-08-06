using System;

namespace CardGames.Core
{
    /// <summary>
    /// Represents a playing card with a number/rank and suit
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Gets or sets the number/rank of the card
        /// </summary>
        public CardNumber Number { get; set; }
        
        /// <summary>
        /// Gets or sets the suit of the card
        /// </summary>
        public CardSuite Suite { get; set; }

        /// <summary>
        /// Represents the rank/number of a playing card
        /// </summary>
        public enum CardNumber
        {
            A,
            _2,
            _3,
            _4,
            _5,
            _6,
            _7,
            _8,
            _9,
            _10,
            J,
            Q,
            K                
        }

        /// <summary>
        /// Represents the suit of a playing card
        /// </summary>
        public enum CardSuite
        {
            Heart,
            Club,
            Diamond,
            Spade
        }

        //TODO: Add jokers
    }
}
