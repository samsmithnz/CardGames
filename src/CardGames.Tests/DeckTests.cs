using System.Linq;
using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DeckTests
    {
        [TestMethod]
        public void CreateDeckTest()
        {
            //Arrange
            Deck deck = new Deck();

            //Act

            //Assert
            Assert.AreEqual(52, deck.Cards.Count);
            //Look at first item
            Assert.AreEqual(Card.CardSuite.Heart, deck.Cards[0].Suite);
            Assert.AreEqual(Card.CardNumber.A, deck.Cards[0].Number);
            //Look at last item
            Assert.AreEqual(Card.CardSuite.Spade, deck.Cards[^1].Suite);
            Assert.AreEqual(Card.CardNumber.K, deck.Cards[^1].Number);
        }

        [TestMethod]
        public void CreateDeckAndShuffleTest()
        {
            //Arrange
            Deck deck = new Deck();

            //Act
            deck.Shuffle();

            //Assert
            Assert.AreEqual(52, deck.Cards.Count);
            //Look at first item
            Card card1 = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };
            Assert.AreNotEqual(card1, deck.Cards[0]);
            //Look at last item
            Card cardn = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber.K };
            Assert.AreNotEqual(cardn, deck.Cards[^1]);
        }

        [TestMethod]
        public void CreateDeckHasAllSuitsAndRanksTest()
        {
            //Arrange
            Deck deck = new Deck();

            //Act & Assert
            // Verify we have exactly 13 cards of each suit
            Assert.AreEqual(13, deck.Cards.Count(c => c.Suite == Card.CardSuite.Heart));
            Assert.AreEqual(13, deck.Cards.Count(c => c.Suite == Card.CardSuite.Club));
            Assert.AreEqual(13, deck.Cards.Count(c => c.Suite == Card.CardSuite.Diamond));
            Assert.AreEqual(13, deck.Cards.Count(c => c.Suite == Card.CardSuite.Spade));

            // Verify we have exactly 4 cards of each rank
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber.A));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._2));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._3));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._4));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._5));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._6));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._7));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._8));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._9));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber._10));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber.J));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber.Q));
            Assert.AreEqual(4, deck.Cards.Count(c => c.Number == Card.CardNumber.K));
        }
    }
}
