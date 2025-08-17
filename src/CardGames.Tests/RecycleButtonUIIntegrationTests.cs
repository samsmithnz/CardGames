using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [TestClass]
    public class RecycleButtonUIIntegrationTests
    {
        [TestMethod]
        public void RecycleButton_UILogic_WorksEndToEnd()
        {
            // This test validates the complete UI integration for the recycle button
            // It simulates the exact sequence that would happen in the UI
            
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Simulate initial game state - button should be hidden
            bool initialButtonHidden = ShouldRecycleButtonBeVisible(rules);
            Assert.IsFalse(initialButtonHidden, "Initially, recycle button should be hidden when stock has cards");
            
            // Act 1: Draw all cards from stock to waste
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
                // After each draw, check button visibility
                bool buttonVisibilityDuringDraw = ShouldRecycleButtonBeVisible(rules);
                if (rules.StockPile.Count > 0)
                {
                    Assert.IsFalse(buttonVisibilityDuringDraw, "Button should remain hidden while stock has cards");
                }
            }
            
            // Verify final state after drawing all cards - button should be visible
            bool buttonVisibleAfterAllDrawn = ShouldRecycleButtonBeVisible(rules);
            Assert.IsTrue(buttonVisibleAfterAllDrawn, "Button should be visible when stock is empty and waste has cards");
            
            // Act 2: Reset stock from waste (simulate clicking recycle button)
            rules.ResetStock();
            
            // Verify button hidden after reset
            bool buttonHiddenAfterReset = ShouldRecycleButtonBeVisible(rules);
            Assert.IsFalse(buttonHiddenAfterReset, "Button should be hidden after stock is reset");
            
            // Act 3: Draw one more card to verify normal operation continues
            bool canDrawAfterReset = rules.DrawFromStock();
            Assert.IsTrue(canDrawAfterReset, "Should be able to draw normally after reset");
            
            // Final verification - button should still be hidden
            bool finalButtonState = ShouldRecycleButtonBeVisible(rules);
            Assert.IsFalse(finalButtonState, "Button should remain hidden after normal draw operation");
        }
        
        [TestMethod]
        public void RecycleButton_EdgeCase_EmptyWastePileAfterMove()
        {
            // Test edge case where waste pile becomes empty due to card moves
            
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);
            
            // Draw a few cards to waste
            for (int i = 0; i < 3 && rules.StockPile.Count > 0; i++)
            {
                rules.DrawFromStock();
            }
            
            // Simulate moving all waste cards to tableaux (not implementing full move logic, just clearing waste)
            // This represents the scenario where user moves all waste cards elsewhere
            rules.WastePile.Clear();
            
            // Verify button visibility when stock has cards but waste is empty
            bool buttonVisibility = ShouldRecycleButtonBeVisible(rules);
            Assert.IsFalse(buttonVisibility, "Button should be hidden when waste is empty, regardless of stock state");
            
            // Empty stock completely
            while (rules.StockPile.Count > 0)
            {
                rules.DrawFromStock();
            }
            
            // Clear waste again (simulating all cards moved elsewhere)
            rules.WastePile.Clear();
            
            // Verify button still hidden when both piles are empty
            bool buttonVisibilityBothEmpty = ShouldRecycleButtonBeVisible(rules);
            Assert.IsFalse(buttonVisibilityBothEmpty, "Button should be hidden when both stock and waste are empty");
        }
        
        /// <summary>
        /// Helper method that mimics the UI logic for determining recycle button visibility
        /// </summary>
        /// <param name="rules">The solitaire rules containing the current game state</param>
        /// <returns>True if the recycle button should be visible</returns>
        private bool ShouldRecycleButtonBeVisible(SolitaireRules rules)
        {
            // This matches the logic in UpdateRecycleButtonVisibility() method
            return rules.StockPile.Count == 0 && rules.WastePile.Count > 0;
        }
    }
}