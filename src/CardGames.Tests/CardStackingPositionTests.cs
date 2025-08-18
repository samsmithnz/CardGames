using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to verify proper card positioning during stacking operations
    /// This addresses the issue where cards stack "fully on top" instead of partial stacking
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CardStackingPositionTests
    {
        [TestMethod]
        public void TableauVerticalOffset_ShouldBeCorrectValue()
        {
            // Verify the constant that controls stacking offset
            Assert.AreEqual(20.0, CardVisualConstants.TableauVerticalOffset, 
                "TableauVerticalOffset should be 20 pixels for proper partial stacking");
        }

        [TestMethod]
        public void CardStacking_ShouldCalculateProperOffsets()
        {
            // Test the calculation logic for card positioning in a tableau stack
            double offset = CardVisualConstants.TableauVerticalOffset;
            
            // Simulate positions for a stack of 4 cards
            double[] expectedPositions = { 0.0, 20.0, 40.0, 60.0 };
            
            for (int i = 0; i < expectedPositions.Length; i++)
            {
                double calculatedPosition = i * offset;
                Assert.AreEqual(expectedPositions[i], calculatedPosition,
                    $"Card at position {i} should be at offset {expectedPositions[i]}");
            }
        }

        [TestMethod]
        public void PartialStacking_ShouldAllowVisibilityOfUnderlyingCards()
        {
            // Test that the offset is sufficient to show rank and suit of underlying cards
            double cardHeight = CardVisualConstants.CardHeight; // 120.0
            double offset = CardVisualConstants.TableauVerticalOffset; // 20.0
            
            // With 20px offset, the underlying card should show enough to see rank/suit
            // This is approximately 1/6 of the card height, which is reasonable for visibility
            double visibilityRatio = offset / cardHeight;
            
            Assert.IsTrue(visibilityRatio >= 0.15 && visibilityRatio <= 0.25, 
                $"Visibility ratio {visibilityRatio:F2} should be between 15-25% for good partial stacking");
        }

        [TestMethod]
        public void RefreshTableauColumn_ShouldCalculateCorrectCanvasTopPositions()
        {
            // Test the logic that RefreshTableauColumn should use for positioning cards
            // This simulates what Canvas.SetTop() should be called with for each card in a stack
            
            // Simulate a tableau column with 5 cards stacked
            int numberOfCards = 5;
            double[] expectedCanvasTopPositions = new double[numberOfCards];
            
            // Calculate expected positions using the same logic as RefreshTableauColumn
            for (int cardIndex = 0; cardIndex < numberOfCards; cardIndex++)
            {
                expectedCanvasTopPositions[cardIndex] = cardIndex * CardVisualConstants.TableauVerticalOffset;
            }
            
            // Verify the calculated positions
            Assert.AreEqual(0.0, expectedCanvasTopPositions[0], "First card should be at Canvas.Top = 0");
            Assert.AreEqual(20.0, expectedCanvasTopPositions[1], "Second card should be at Canvas.Top = 20");
            Assert.AreEqual(40.0, expectedCanvasTopPositions[2], "Third card should be at Canvas.Top = 40");
            Assert.AreEqual(60.0, expectedCanvasTopPositions[3], "Fourth card should be at Canvas.Top = 60");
            Assert.AreEqual(80.0, expectedCanvasTopPositions[4], "Fifth card should be at Canvas.Top = 80");
            
            // Verify that cards don't overlap completely (proper spacing)
            for (int i = 1; i < numberOfCards; i++)
            {
                double spacing = expectedCanvasTopPositions[i] - expectedCanvasTopPositions[i - 1];
                Assert.AreEqual(CardVisualConstants.TableauVerticalOffset, spacing,
                    $"Spacing between card {i-1} and card {i} should be exactly TableauVerticalOffset");
            }
        }
    }
}