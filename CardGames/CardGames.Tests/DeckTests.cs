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
            Assert.AreEqual(Card.CardSuite.Hearts, deck.Cards[0].Suite);
            Assert.AreEqual(Card.CardNumber.A, deck.Cards[0].Number);
            //Look at last item
            Assert.AreEqual(Card.CardSuite.Spades, deck.Cards[^1].Suite);
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
            Card card1 = new Card { Suite = Card.CardSuite.Hearts, Number = Card.CardNumber.A };
            Assert.AreNotEqual(card1, deck.Cards[0]);
            //Look at last item
            Card cardn = new Card { Suite = Card.CardSuite.Spades, Number = Card.CardNumber.K };
            Assert.AreNotEqual(cardn, deck.Cards[^1]);
        }
    }
}
