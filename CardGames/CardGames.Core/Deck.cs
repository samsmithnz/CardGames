using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Core
{
    public class Deck
    {
        public List<Card> Cards;
        private Random rng = new Random();

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
