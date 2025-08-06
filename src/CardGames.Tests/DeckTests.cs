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
