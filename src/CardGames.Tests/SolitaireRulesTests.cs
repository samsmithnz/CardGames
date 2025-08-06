using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SolitaireRulesTests
    {
        [TestMethod]
        public void CanPlaceOnPlayingArea_KingOnEmptyPile_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.K };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(kingOfHearts, null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnPlayingArea_NonKingOnEmptyPile_ReturnsFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card queenOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.Q };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(queenOfHearts, null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanPlaceOnPlayingArea_RedOnBlackDescending_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card redJack = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.J };
            Card blackQueen = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber.Q };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(redJack, blackQueen);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnPlayingArea_SameColorCards_ReturnsFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card redJack = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.J };
            Card redQueen = new Card { Suite = Card.CardSuite.Diamond, Number = Card.CardNumber.Q };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(redJack, redQueen);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanPlaceOnFoundation_AceOnEmptyPile_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card aceOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };

            // Act
            bool result = rules.CanPlaceOnFoundation(aceOfHearts, null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnFoundation_TwoOnAceSameSuit_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card twoOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber._2 };
            Card aceOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };

            // Act
            bool result = rules.CanPlaceOnFoundation(twoOfHearts, aceOfHearts);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnFoundation_DifferentSuit_ReturnsFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card twoOfSpades = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber._2 };
            Card aceOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };

            // Act
            bool result = rules.CanPlaceOnFoundation(twoOfSpades, aceOfHearts);

            // Assert
            Assert.IsFalse(result);
        }
    }
}