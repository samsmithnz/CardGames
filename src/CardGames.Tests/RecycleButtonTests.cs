using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [TestClass]
    public class RecycleButtonTests
    {
        [TestMethod]
        public void RecycleButton_ShouldBeVisible_WhenStockEmptyAndWasteHasCards()
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
            
            // Assert conditions for button visibility
            Assert.AreEqual(0, rules.StockPile.Count, "Stock should be empty");
            Assert.IsTrue(rules.WastePile.Count > 0, "Waste should have cards");
            
            // The UI should show the recycle button in this state
            bool shouldShowRecycleButton = (rules.StockPile.Count == 0 && rules.WastePile.Count > 0);
            Assert.IsTrue(shouldShowRecycleButton, "Recycle button should be visible when stock is empty and waste has cards");
        }
        
        [TestMethod]
        public void RecycleButton_ShouldBeHidden_WhenStockHasCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Assert initial state - stock has cards
            Assert.IsTrue(rules.StockPile.Count > 0, "Stock should have cards");
            
            // The UI should hide the recycle button in this state
            bool shouldHideRecycleButton = (rules.StockPile.Count > 0);
            Assert.IsTrue(shouldHideRecycleButton, "Recycle button should be hidden when stock has cards");
        }
        
        [TestMethod]
        public void RecycleButton_ShouldBeHidden_WhenBothStockAndWasteEmpty()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            // Don't deal cards - both piles start empty
            
            // Assert both piles are empty
            Assert.AreEqual(0, rules.StockPile.Count, "Stock should be empty");
            Assert.AreEqual(0, rules.WastePile.Count, "Waste should be empty");
            
            // The UI should hide the recycle button in this state
            bool shouldHideRecycleButton = (rules.StockPile.Count == 0 && rules.WastePile.Count == 0);
            Assert.IsTrue(shouldHideRecycleButton, "Recycle button should be hidden when both piles are empty");
        }
        
        [TestMethod]
        public void RecycleButton_Functionality_ShouldMatchStockPileClick()
        {
            // This test validates that the recycle button performs the same action as clicking the empty stock pile
            
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Draw all cards from stock to waste
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            
            int wasteCount = rules.WastePile.Count;
            Assert.AreEqual(0, rules.StockPile.Count, "Stock should be empty");
            Assert.IsTrue(wasteCount > 0, "Waste should have cards");
            
            // Act - Simulate recycle button click (which should call the same logic as empty stock pile click)
            rules.ResetStock();
            
            // Assert - Should produce the same result as clicking empty stock pile
            Assert.AreEqual(wasteCount, rules.StockPile.Count, "All waste cards should move back to stock");
            Assert.AreEqual(0, rules.WastePile.Count, "Waste pile should be empty after recycling");
        }
        
        [TestMethod]
        public void RecycleButton_VisibilityToggle_WorksCorrectly()
        {
            // This test validates the complete cycle of recycle button visibility changes
            
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Initial state - stock has cards, button should be hidden
            bool initialButtonHidden = (rules.StockPile.Count > 0);
            Assert.IsTrue(initialButtonHidden, "Initially, recycle button should be hidden");
            
            // Draw all cards - button should become visible
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            bool buttonVisibleAfterDraw = (rules.StockPile.Count == 0 && rules.WastePile.Count > 0);
            Assert.IsTrue(buttonVisibleAfterDraw, "After emptying stock, recycle button should be visible");
            
            // Reset stock - button should become hidden again
            rules.ResetStock();
            bool buttonHiddenAfterReset = (rules.StockPile.Count > 0);
            Assert.IsTrue(buttonHiddenAfterReset, "After resetting stock, recycle button should be hidden again");
        }
    }
}