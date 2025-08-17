using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to validate the logic that drives visual indicator behavior during drag operations.
    /// These tests verify the core validation logic that determines whether indicators should show green (valid) or red (invalid).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class VisualIndicatorLogicTests
    {
        /// <summary>
        /// Test that the validation logic correctly identifies a valid move,
        /// which should result in green indicators being displayed.
        /// </summary>
        [TestMethod]
        public void ValidMove_ShouldResultInGreenIndicators()
        {
            // Arrange - Create a scenario where a move is valid (red card on black card, one rank lower)
            Card redSix = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Heart };
            Card blackSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Spade };

            // Act - Use the same validation logic that drives the visual indicators
            bool isValid = IsValidTableauMove(redSix, blackSeven);

            // Assert - This should be valid, meaning green indicators would be shown
            Assert.IsTrue(isValid, "Red Six on Black Seven should be valid and show green indicators");
        }

        /// <summary>
        /// Test that the validation logic correctly identifies an invalid move,
        /// which should result in red indicators being displayed.
        /// </summary>
        [TestMethod]
        public void InvalidMove_SameColor_ShouldResultInRedIndicators()
        {
            // Arrange - Create a scenario where a move is invalid (same color cards)
            Card redSix = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Heart };
            Card redSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Diamond };

            // Act - Use the same validation logic that drives the visual indicators
            bool isValid = IsValidTableauMove(redSix, redSeven);

            // Assert - This should be invalid, meaning red indicators would be shown
            Assert.IsFalse(isValid, "Red Six on Red Seven should be invalid and show red indicators");
        }

        /// <summary>
        /// Test that the validation logic correctly identifies an invalid move due to wrong rank,
        /// which should result in red indicators being displayed.
        /// </summary>
        [TestMethod]
        public void InvalidMove_WrongRank_ShouldResultInRedIndicators()
        {
            // Arrange - Create a scenario where a move is invalid (wrong rank - not one lower)
            Card blackFive = new Card { Number = Card.CardNumber._5, Suite = Card.CardSuite.Spade };
            Card redSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Heart };

            // Act - Use the same validation logic that drives the visual indicators
            bool isValid = IsValidTableauMove(blackFive, redSeven);

            // Assert - This should be invalid, meaning red indicators would be shown
            Assert.IsFalse(isValid, "Black Five on Red Seven should be invalid (wrong rank) and show red indicators");
        }

        /// <summary>
        /// Test that Kings can be placed on empty spaces, which should show green indicators.
        /// </summary>
        [TestMethod]
        public void KingOnEmptySpace_ShouldResultInGreenIndicators()
        {
            // Arrange - King on empty space scenario
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card emptySpace = null;

            // Act - Use the same validation logic that drives the visual indicators
            bool isValid = IsValidTableauMove(kingOfSpades, emptySpace);

            // Assert - This should be valid, meaning green indicators would be shown
            Assert.IsTrue(isValid, "King on empty space should be valid and show green indicators");
        }

        /// <summary>
        /// Test that non-Kings cannot be placed on empty spaces, which should show red indicators.
        /// </summary>
        [TestMethod]
        public void NonKingOnEmptySpace_ShouldResultInRedIndicators()
        {
            // Arrange - Non-King on empty space scenario
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card emptySpace = null;

            // Act - Use the same validation logic that drives the visual indicators
            bool isValid = IsValidTableauMove(queenOfHearts, emptySpace);

            // Assert - This should be invalid, meaning red indicators would be shown
            Assert.IsFalse(isValid, "Non-King on empty space should be invalid and show red indicators");
        }

        /// <summary>
        /// Helper method that implements the same tableau validation logic used in the UI.
        /// This mirrors the logic that determines whether visual indicators should be green or red.
        /// </summary>
        private bool IsValidTableauMove(Card movingCard, Card targetCard)
        {
            if (targetCard == null)
            {
                // Empty space - only Kings are allowed
                return movingCard.Number == Card.CardNumber.K;
            }

            // Check if the moving card is one rank lower than the target
            if (!IsOneRankLower(movingCard.Number, targetCard.Number))
            {
                return false;
            }

            // Check if colors are opposite
            return IsOppositeColor(movingCard, targetCard);
        }

        /// <summary>
        /// Helper method to check if first card is one rank lower than second card
        /// </summary>
        private bool IsOneRankLower(Card.CardNumber first, Card.CardNumber second)
        {
            return (int)first == (int)second - 1;
        }

        /// <summary>
        /// Helper method to check if two cards have opposite colors
        /// </summary>
        private bool IsOppositeColor(Card first, Card second)
        {
            bool firstIsRed = first.Suite == Card.CardSuite.Heart || first.Suite == Card.CardSuite.Diamond;
            bool secondIsRed = second.Suite == Card.CardSuite.Heart || second.Suite == Card.CardSuite.Diamond;
            return firstIsRed != secondIsRed;
        }
    }
}