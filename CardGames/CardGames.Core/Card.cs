using System;

namespace CardGames.Core
{
    public class Card
    {
        public CardNumber Number { get; set; }
        public CardSuite Suite { get; set; }

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

        public enum CardSuite
        {
            Hearts,
            Clubs,
            Diamonds,
            Spades
        }

        //TODO: Add jokers
    }
}
