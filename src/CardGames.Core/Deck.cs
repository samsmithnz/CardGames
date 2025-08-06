using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Core
{
    /// <summary>
    /// Represents a deck of playing cards
    /// </summary>
    public class Deck
    {
        /// <summary>
        /// Gets the list of cards in the deck
        /// </summary>
        public List<Card> Cards { get; private set; }
        
        private Random rng = new Random();

        /// <summary>
        /// Initializes a new instance of the Deck class with a standard 52-card deck
        /// </summary>
        public Deck()
        {
            //Add all of the cards to the deck
            Cards = new List<Card>();
            foreach (Card.CardSuite suite in Enum.GetValues(typeof(Card.CardSuite)))
            {
                foreach (Card.CardNumber number in Enum.GetValues(typeof(Card.CardNumber)))
                {
                    Cards.Add(new Card
                    {
                        Suite = suite,
                        Number = number
                    });
                }
            }
        }

        /// <summary>
        /// Shuffles the cards in the deck using the Fisher-Yates shuffle algorithm
        /// </summary>
        public void Shuffle()
        {
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = value;
            }
        }

    }
}
