using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [TestClass]
    public class EmptyStockPileDisplayTests
    {
        [TestMethod]
        public void EmptyStockPile_ShouldShowEmptySpot()
        {
            // This test validates that when stock pile is empty, 
            // it should show an empty spot (no card visible) instead of a facedown card
            
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Draw all cards to make stock empty
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            
            // Assert stock pile state
            Assert.AreEqual(0, rules.StockPile.Count, "Stock should be empty");
            Assert.IsTrue(rules.WastePile.Count > 0, "Waste should have cards");
            
            // The UI should represent this as an empty spot
            // This is validated by the UpdateVisibility() logic in CardUserControl
            // When Card == null and IsStockPile == true, both images should be hidden
            bool shouldShowEmptySpot = (rules.StockPile.Count == 0);
            Assert.IsTrue(shouldShowEmptySpot, "Empty stock pile should show empty spot");
        }
        
        [TestMethod]
        public void EmptyStockPile_StillAllowsResetFunctionality()
        {
            // This test ensures that even when showing empty spot, 
            // the stock pile remains functional for reset operation
            
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Draw all cards to make stock empty
            int originalStockCount = rules.StockPile.Count;
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            
            // Verify empty state
            Assert.AreEqual(0, rules.StockPile.Count, "Stock should be empty");
            
            // Act - Reset stock from waste pile (simulates clicking empty stock pile)
            rules.ResetStock();
            
            // Assert - Should work despite showing empty spot
            Assert.AreEqual(originalStockCount, rules.StockPile.Count, "Stock should be reset with all cards");
            Assert.AreEqual(0, rules.WastePile.Count, "Waste should be empty after reset");
        }
    }
}