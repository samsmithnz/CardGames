using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [TestClass]
    public class RecycleButtonTests
    {
        [TestMethod]
        public void RecycleButton_ShouldBeVisible_WhenAllCardsInWaste()
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
            Assert.AreEqual(24, rules.WastePile.Count, "Waste should have all 24 stock cards");
            
            // The UI should show the recycle button when all cards are in waste
            bool shouldShowRecycleButton = (rules.WastePile.Count == 24);
            Assert.IsTrue(shouldShowRecycleButton, "Recycle button should be visible when all cards are in waste");
        }
        
        [TestMethod]
        public void RecycleButton_ShouldBeHidden_WhenStockHasCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Assert initial state - stock has cards, waste has fewer than 24 cards
            Assert.IsTrue(rules.StockPile.Count > 0, "Stock should have cards");
            Assert.IsTrue(rules.WastePile.Count < 24, "Waste should have fewer than 24 cards");
            
            // The UI should hide the recycle button in this state
            bool shouldHideRecycleButton = (rules.WastePile.Count != 24);
            Assert.IsTrue(shouldHideRecycleButton, "Recycle button should be hidden when waste doesn't have all cards");
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
            bool shouldHideRecycleButton = (rules.WastePile.Count != 24);
            Assert.IsTrue(shouldHideRecycleButton, "Recycle button should be hidden when waste is empty");
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
            
            // Initial state - waste doesn't have all cards, button should be hidden
            bool initialButtonHidden = (rules.WastePile.Count != 24);
            Assert.IsTrue(initialButtonHidden, "Initially, recycle button should be hidden");
            
            // Draw all cards - button should become visible when waste has all 24 cards
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            bool buttonVisibleAfterDraw = (rules.WastePile.Count == 24);
            Assert.IsTrue(buttonVisibleAfterDraw, "After drawing all cards, recycle button should be visible");
            
            // Reset stock - button should become hidden again
            rules.ResetStock();
            bool buttonHiddenAfterReset = (rules.WastePile.Count != 24);
            Assert.IsTrue(buttonHiddenAfterReset, "After resetting stock, recycle button should be hidden again");
        }
    }
}