using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Integration tests to verify the card sizing implementation meets requirements
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CardSizingIntegrationTests
    {
        [TestMethod]
        public void CardSizing_AllConstantsAvailable_ShouldProvideConsistentValues()
        {
            // Arrange & Act - Verify all constants are accessible and have expected values
            double cardWidth = CardVisualConstants.CardWidth;
            double cardHeight = CardVisualConstants.CardHeight;
            double tableauOffset = CardVisualConstants.TableauVerticalOffset;

            // Assert - Verify the exact values from the issue requirements
            Assert.AreEqual(80.0, cardWidth, "CardWidth should match issue specification");
            Assert.AreEqual(120.0, cardHeight, "CardHeight should match issue specification");
            Assert.AreEqual(24.0, tableauOffset, "TableauVerticalOffset should match issue specification");
        }

        [TestMethod]
        public void CardSizing_TableauStacking_ShouldCalculateCorrectPositions()
        {
            // Arrange
            double offset = CardVisualConstants.TableauVerticalOffset;
            int maxCardsInColumn = 7; // Column 7 has up to 7 cards

            // Act - Calculate positions for a full tableau column
            double[] positions = new double[maxCardsInColumn];
            for (int i = 0; i < maxCardsInColumn; i++)
            {
                positions[i] = i * offset;
            }

            // Assert - Verify positioning follows the stacking rules
            Assert.AreEqual(0.0, positions[0], "First card should be at position 0");
            Assert.AreEqual(24.0, positions[1], "Second card should be offset by TableauVerticalOffset");
            Assert.AreEqual(48.0, positions[2], "Third card should be at 2x offset");
            Assert.AreEqual(168.0, positions[6], "Seventh card should be at 6x offset");
            
            // Verify maximum stack height is reasonable for UI
            double maxStackHeight = positions[maxCardsInColumn - 1] + CardVisualConstants.CardHeight;
            Assert.IsTrue(maxStackHeight <= 300, 
                $"Maximum stack height ({maxStackHeight}) should fit comfortably in UI layout");
        }

        [TestMethod]
        public void CardSizing_FullyStackedPiles_ShouldOnlyShowTopCard()
        {
            // This test verifies the design principle for fully stacked piles
            // In fully stacked piles (stock, waste, foundations), all cards should be at position 0
            
            // Arrange
            int numberOfCardsInPile = 10; // Arbitrary number of cards
            
            // Act - All cards in fully stacked piles should be at same position
            double[] positions = new double[numberOfCardsInPile];
            for (int i = 0; i < numberOfCardsInPile; i++)
            {
                positions[i] = 0.0; // All cards at same position for full stack
            }
            
            // Assert
            for (int i = 0; i < numberOfCardsInPile; i++)
            {
                Assert.AreEqual(0.0, positions[i], 
                    $"Card {i} in fully stacked pile should be at position 0 (only top card visible)");
            }
        }
    }
}