using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [TestClass]
    public class StockPileClickTests
    {
        [TestMethod]
        public void StockPile_SingleClick_DrawsCard_WhenStockHasCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            int initialStockCount = rules.StockPile.Count;
            int initialWasteCount = rules.WastePile.Count;
            
            // Act - Simulate single-clicking on stock pile (drawing a card)
            bool drawn = rules.DrawFromStock();
            
            // Assert
            Assert.IsTrue(drawn, "Should be able to draw from stock when it has cards");
            Assert.AreEqual(initialStockCount - 1, rules.StockPile.Count, "Stock pile should have one less card");
            Assert.AreEqual(initialWasteCount + 1, rules.WastePile.Count, "Waste pile should have one more card");
        }
        
        [TestMethod]
        public void StockPile_ClickDrawsCard_WhenStockHasCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            int initialStockCount = rules.StockPile.Count;
            int initialWasteCount = rules.WastePile.Count;
            
            // Act - Simulate clicking on stock pile (drawing a card)
            bool drawn = rules.DrawFromStock();
            
            // Assert
            Assert.IsTrue(drawn, "Should be able to draw from stock when it has cards");
            Assert.AreEqual(initialStockCount - 1, rules.StockPile.Count, "Stock pile should have one less card");
            Assert.AreEqual(initialWasteCount + 1, rules.WastePile.Count, "Waste pile should have one more card");
        }
        
        [TestMethod]
        public void StockPile_ClickResetsStock_WhenStockEmptyAndWasteHasCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Draw all cards from stock to waste
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            
            Assert.AreEqual(0, rules.StockPile.Count, "Stock should be empty");
            Assert.IsTrue(rules.WastePile.Count > 0, "Waste should have cards");
            
            int wasteCount = rules.WastePile.Count;
            
            // Act - Simulate clicking on empty stock pile (should reset from waste)
            rules.ResetStock();
            
            // Assert
            Assert.AreEqual(wasteCount, rules.StockPile.Count, "All waste cards should move back to stock");
            Assert.AreEqual(0, rules.WastePile.Count, "Waste pile should be empty after reset");
        }
        
        [TestMethod]
        public void StockPile_ClickAfterReset_CanDrawAgain()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Draw all cards from stock to waste
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            
            // Reset stock from waste
            rules.ResetStock();
            
            int stockCountAfterReset = rules.StockPile.Count;
            
            // Act - Try drawing again after reset
            bool drawn = rules.DrawFromStock();
            
            // Assert
            Assert.IsTrue(drawn, "Should be able to draw after reset");
            Assert.AreEqual(stockCountAfterReset - 1, rules.StockPile.Count, "Stock should have one less card");
            Assert.AreEqual(1, rules.WastePile.Count, "Waste should have one card");
        }
    }
}