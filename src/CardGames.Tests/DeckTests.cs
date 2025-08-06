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
            // Note: We can't assert that specific cards are not in their original positions
            // because there's a small chance they could randomly end up there after shuffling.
            // The other tests (ShuffleChangesCardOrderTest and ShufflePreservesAllCardsTest) 
            // provide better verification of the shuffle functionality.
        }

        [TestMethod]
        public void ShuffleChangesCardOrderTest()
        {
            //Arrange
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();

            //Act
            deck2.Shuffle();

            //Assert - After shuffling, the order should be different
            Assert.AreEqual(52, deck1.Cards.Count);
            Assert.AreEqual(52, deck2.Cards.Count);
            
            // Check that at least some cards are in different positions
            int differentPositions = 0;
            for (int i = 0; i < deck1.Cards.Count; i++)
            {
                if (!deck1.Cards[i].Equals(deck2.Cards[i]))
                {
                    differentPositions++;
                }
            }
            
            // Statistical test: expect that most cards are in different positions after shuffle
            // Using a conservative threshold - at least 20 cards should be in different positions
            Assert.IsTrue(differentPositions >= 20, $"Only {differentPositions} cards changed position, expected at least 20");
        }

        [TestMethod]
        public void ShufflePreservesAllCardsTest()
        {
            //Arrange
            Deck originalDeck = new Deck();
            Deck shuffledDeck = new Deck();

            //Act
            shuffledDeck.Shuffle();

            //Assert - All original cards should still be present after shuffle
            Assert.AreEqual(52, originalDeck.Cards.Count);
            Assert.AreEqual(52, shuffledDeck.Cards.Count);
            
            // Every card from original deck should exist in shuffled deck
            foreach (Card originalCard in originalDeck.Cards)
            {
                Assert.IsTrue(shuffledDeck.Cards.Contains(originalCard), 
                    $"Card {originalCard.Number} of {originalCard.Suite} is missing after shuffle");
            }
            
            // Every card from shuffled deck should exist in original deck
            foreach (Card shuffledCard in shuffledDeck.Cards)
            {
                Assert.IsTrue(originalDeck.Cards.Contains(shuffledCard), 
                    $"Card {shuffledCard.Number} of {shuffledCard.Suite} was added during shuffle");
            }
        }
    }
}
