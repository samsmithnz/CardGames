using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests for card visual constants and sizing consistency
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CardVisualConstantsTests
    {
        [TestMethod]
        public void CardVisualConstants_ShouldHaveCorrectCardWidth()
        {
            // Arrange & Act
            double width = CardVisualConstants.CardWidth;

            // Assert
            Assert.AreEqual(80.0, width, "Card width should be 80.0 pixels");
        }

        [TestMethod]
        public void CardVisualConstants_ShouldHaveCorrectCardHeight()
        {
            // Arrange & Act
            double height = CardVisualConstants.CardHeight;

            // Assert
            Assert.AreEqual(120.0, height, "Card height should be 120.0 pixels");
        }

        [TestMethod]
        public void CardVisualConstants_ShouldHaveCorrectTableauVerticalOffset()
        {
            // Arrange & Act
            double offset = CardVisualConstants.TableauVerticalOffset;

            // Assert
            Assert.AreEqual(24.0, offset, "Tableau vertical offset should be 20.0 pixels");
        }

        [TestMethod]
        public void CardVisualConstants_ShouldMaintainProperAspectRatio()
        {
            // Arrange
            double width = CardVisualConstants.CardWidth;
            double height = CardVisualConstants.CardHeight;

            // Act
            double aspectRatio = height / width;

            // Assert
            Assert.AreEqual(1.5, aspectRatio, 0.01, "Cards should maintain a 2:3 aspect ratio (height:width)");
        }

        [TestMethod]
        public void CardVisualConstants_TableauOffsetShouldAllowVisibleRankAndSuit()
        {
            // Arrange
            double offset = CardVisualConstants.TableauVerticalOffset;
            double cardHeight = CardVisualConstants.CardHeight;

            // Act
            double visiblePortion = offset / cardHeight;

            // Assert
            // Offset should be at least 15% of card height to show rank and suit
            Assert.IsTrue(visiblePortion >= 0.15, 
                $"Tableau offset ({offset}) should be at least 15% of card height ({cardHeight}) to show rank and suit");
            
            // But not more than 25% to avoid wasting space
            Assert.IsTrue(visiblePortion <= 0.25,
                $"Tableau offset ({offset}) should not exceed 25% of card height ({cardHeight}) to avoid wasting space");
        }
    }
}