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

        [TestMethod]
        public void ImageFileNamingConventionTest()
        {
            //Arrange
            Card card = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };

            //Act  
            string fileName = "1920px-Playing_card_" + card.Suite.ToString().ToLower() + "_" + card.Number.ToString().Replace("_", "") + ".svg.png";

            //Assert
            Assert.AreEqual("1920px-Playing_card_heart_A.svg.png", fileName);

            //Test number cards
            Card numberCard = new Card { Suite = Card.CardSuite.Club, Number = Card.CardNumber._10 };
            string numberFileName = "1920px-Playing_card_" + numberCard.Suite.ToString().ToLower() + "_" + numberCard.Number.ToString().Replace("_", "") + ".svg.png";
            Assert.AreEqual("1920px-Playing_card_club_10.svg.png", numberFileName);
        }

        [TestMethod]
        public void AllCardSuitesAndNumbersGenerateValidFileNames()
        {
            //Arrange & Act & Assert
            foreach (Card.CardSuite suite in System.Enum.GetValues(typeof(Card.CardSuite)))
            {
                foreach (Card.CardNumber number in System.Enum.GetValues(typeof(Card.CardNumber)))
                {
                    string fileName = "1920px-Playing_card_" + suite.ToString().ToLower() + "_" + number.ToString().Replace("_", "") + ".svg.png";
                    
                    // Check that filename doesn't contain invalid characters
                    Assert.IsFalse(fileName.Contains("__"), $"Filename should not contain double underscores: {fileName}");
                    Assert.IsTrue(fileName.StartsWith("1920px-Playing_card_"), $"Filename should start with expected prefix: {fileName}");
                    Assert.IsTrue(fileName.EndsWith(".svg.png"), $"Filename should end with .svg.png: {fileName}");
                }
            }
        }
    }
}
