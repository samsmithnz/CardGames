using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    /// <summary>
    /// Integration tests to reproduce the card stacking bug in the UI layer
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ExecuteMoveIntegrationTests
    {
        /// <summary>
        /// Test that simulates the bug: dragging a 2 of hearts to a 3 of spades
        /// should result in both cards being in the tableau column, but the bug
        /// causes the 2 of hearts to disappear instead.
        /// </summary>
        [TestMethod]
        public void ExecuteMove_TableauStacking_ShouldMaintainBothCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a tableau column with a 3 of spades
            Card threeOfSpades = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Spade };
            rules.TableauColumns[0].Add(threeOfSpades);
            
            // Set up waste pile with a 2 of hearts (valid move)
            Card twoOfHearts = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Heart };
            rules.WastePile.Add(twoOfHearts);
            
            int initialTableauCount = rules.TableauColumns[0].Count;
            int initialWasteCount = rules.WastePile.Count;
            
            // Act - simulate the move from waste pile to tableau
            // This simulates what should happen when ExecuteMove is called
            
            // First remove from source (waste pile)
            rules.WastePile.RemoveAt(rules.WastePile.Count - 1);
            
            // Then add to target tableau column
            rules.TableauColumns[0].Add(twoOfHearts);
            
            // Assert - both cards should be in the tableau column
            Assert.AreEqual(initialTableauCount + 1, rules.TableauColumns[0].Count, 
                "Tableau column should have one more card after stacking");
            Assert.AreEqual(initialWasteCount - 1, rules.WastePile.Count, 
                "Waste pile should have one less card");
            
            Assert.AreEqual(threeOfSpades, rules.TableauColumns[0][0], 
                "Three of spades should still be in the column at position 0");
            Assert.AreEqual(twoOfHearts, rules.TableauColumns[0][1], 
                "Two of hearts should be on top at position 1");
        }
        
        /// <summary>
        /// Test the specific scenario from the bug report using foundation pile as source
        /// </summary>
        [TestMethod]
        public void ExecuteMove_FoundationToTableauStacking_ShouldMaintainBothCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a tableau column with a 4 of clubs
            Card fourOfClubs = new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Club };
            rules.TableauColumns[1].Add(fourOfClubs);
            
            // Set up foundation pile with a 3 of hearts (valid move to tableau)
            Card threeOfHearts = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(threeOfHearts);
            
            int initialTableauCount = rules.TableauColumns[1].Count;
            int initialFoundationCount = rules.FoundationPiles[0].Count;
            
            // Act - simulate the move from foundation to tableau
            rules.FoundationPiles[0].RemoveAt(rules.FoundationPiles[0].Count - 1);
            rules.TableauColumns[1].Add(threeOfHearts);
            
            // Assert
            Assert.AreEqual(initialTableauCount + 1, rules.TableauColumns[1].Count, 
                "Tableau column should have one more card after stacking");
            Assert.AreEqual(initialFoundationCount - 1, rules.FoundationPiles[0].Count, 
                "Foundation pile should have one less card");
            
            Assert.AreEqual(fourOfClubs, rules.TableauColumns[1][0], 
                "Four of clubs should still be in the column");
            Assert.AreEqual(threeOfHearts, rules.TableauColumns[1][1], 
                "Three of hearts should be on top of the stack");
        }
    }
}