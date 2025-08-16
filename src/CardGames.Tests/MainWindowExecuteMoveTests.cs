using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to reproduce the MainWindow ExecuteMove bug where cards disappear during stacking
    /// These tests simulate the exact conditions that cause the bug
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class MainWindowExecuteMoveTests
    {
        /// <summary>
        /// Mock class to simulate the MainWindow logic with buggy tableau detection
        /// This helps reproduce the exact bug condition
        /// </summary>
        private class MockMainWindowLogicWithBuggyDetection : MockMainWindowLogic
        {
            /// <summary>
            /// Simulate the bug where GetTableauColumnIndex sometimes fails for tableau cards
            /// </summary>
            protected override int GetTableauColumnIndexInternal(MockCardControl control)
            {
                // Always return -1 to simulate detection failure
                return -1;
            }
        }

        /// <summary>
        /// Mock class to simulate the MainWindow logic without actual UI controls
        /// This helps isolate the ExecuteMove logic bug
        /// </summary>
        private class MockMainWindowLogic
        {
            public SolitaireRules solitaireRules;
            public List<List<MockCardControl>> tableauControls;
            public List<MockCardControl> foundationControls;
            public MockCardControl WastePile;

            public MockMainWindowLogic()
            {
                solitaireRules = new SolitaireRules();
                foundationControls = new List<MockCardControl>();
                
                // Create mock tableau controls similar to real MainWindow
                tableauControls = new List<List<MockCardControl>>
                {
                    new List<MockCardControl> { new MockCardControl("Tableau1_1") },
                    new List<MockCardControl> { new MockCardControl("Tableau2_1"), new MockCardControl("Tableau2_2") },
                    new List<MockCardControl> { new MockCardControl("Tableau3_1"), new MockCardControl("Tableau3_2"), new MockCardControl("Tableau3_3") }
                };
                
                WastePile = new MockCardControl("WastePile");
            }

            /// <summary>
            /// Simplified version of ExecuteMove logic that demonstrates the bug
            /// </summary>
            public void ExecuteMove(MockCardControl sourceControl, MockCardControl targetControl, Card card)
            {
                // Check if moving to foundation pile
                if (foundationControls.Contains(targetControl))
                {
                    int foundationIndex = foundationControls.IndexOf(targetControl);
                    RemoveCardFromSource(sourceControl, card);
                    solitaireRules.FoundationPiles[foundationIndex].Add(card);
                    targetControl.Card = card;
                    return;
                }
                
                // Check if moving to tableau
                int targetColumnIndex = GetTableauColumnIndex(targetControl);
                if (targetColumnIndex >= 0)
                {
                    RemoveCardFromSource(sourceControl, card);
                    solitaireRules.TableauColumns[targetColumnIndex].Add(card);
                    return;
                }
                
                // Handle other pile types (waste, stock, etc.)
                if (targetControl.Card == null)
                {
                    // Move to empty space
                    targetControl.Card = card;
                    sourceControl.Card = null;
                }
                else
                {
                    // Move to existing card (should stack)
                    // First check if this is actually a tableau move that wasn't caught above
                    int targetColumnIndexForExistingCard = GetTableauColumnIndex(targetControl);
                    if (targetColumnIndexForExistingCard >= 0)
                    {
                        // This is a tableau move - handle it properly
                        RemoveCardFromSource(sourceControl, card);
                        solitaireRules.TableauColumns[targetColumnIndexForExistingCard].Add(card);
                    }
                    else
                    {
                        // Non-tableau move - replace the card (for waste pile, etc.)
                        // THIS IS THE BUG! It replaces instead of stacking
                        targetControl.Card = card;
                        sourceControl.Card = null;
                    }
                }
            }

            private int GetTableauColumnIndex(MockCardControl control)
            {
                return GetTableauColumnIndexInternal(control);
            }

            protected virtual int GetTableauColumnIndexInternal(MockCardControl control)
            {
                for (int col = 0; col < tableauControls.Count; col++)
                {
                    if (tableauControls[col].Contains(control))
                    {
                        return col;
                    }
                }
                return -1;
            }

            private void RemoveCardFromSource(MockCardControl sourceControl, Card card)
            {
                int sourceColumnIndex = GetTableauColumnIndex(sourceControl);
                if (sourceColumnIndex >= 0)
                {
                    List<Card> sourceColumn = solitaireRules.TableauColumns[sourceColumnIndex];
                    if (sourceColumn.Count > 0 && sourceColumn[sourceColumn.Count - 1] == card)
                    {
                        sourceColumn.RemoveAt(sourceColumn.Count - 1);
                    }
                    return;
                }
                
                if (sourceControl == WastePile)
                {
                    if (solitaireRules.WastePile.Count > 0 && solitaireRules.WastePile[solitaireRules.WastePile.Count - 1] == card)
                    {
                        solitaireRules.WastePile.RemoveAt(solitaireRules.WastePile.Count - 1);
                    }
                    return;
                }
                
                sourceControl.Card = null;
            }
        }

        private class MockCardControl
        {
            public string Name { get; }
            public Card Card { get; set; }

            public MockCardControl(string name)
            {
                Name = name;
            }
        }

        [TestMethod]
        public void ExecuteMove_WhenDroppingOnTableauCard_ShouldStackNotReplace()
        {
            // Arrange
            MockMainWindowLogic logic = new MockMainWindowLogic();
            
            // Set up tableau column 0 with a 4 of spades
            Card fourOfSpades = new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Spade };
            logic.solitaireRules.TableauColumns[0].Add(fourOfSpades);
            logic.tableauControls[0][0].Card = fourOfSpades;
            
            // Set up waste pile with a 3 of hearts (valid tableau move)
            Card threeOfHearts = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart };
            logic.solitaireRules.WastePile.Add(threeOfHearts);
            logic.WastePile.Card = threeOfHearts;
            
            MockCardControl sourceControl = logic.WastePile;
            MockCardControl targetControl = logic.tableauControls[0][0]; // The card we're dropping onto
            
            int initialTableauCount = logic.solitaireRules.TableauColumns[0].Count;
            Card originalTargetCard = targetControl.Card; // Keep reference to original card
            
            // Act - execute the move
            logic.ExecuteMove(sourceControl, targetControl, threeOfHearts);
            
            // Assert - the tableau should have both cards, not just the new one
            Assert.AreEqual(initialTableauCount + 1, logic.solitaireRules.TableauColumns[0].Count, 
                "Tableau column should have one more card after stacking");
            Assert.AreEqual(fourOfSpades, logic.solitaireRules.TableauColumns[0][0], 
                "Original card (4 of spades) should still be in the column");
            Assert.AreEqual(threeOfHearts, logic.solitaireRules.TableauColumns[0][1], 
                "New card (3 of hearts) should be on top of the stack");
            
            // CRITICAL: The target control should NOT have its card replaced
            // If the bug exists, targetControl.Card will be threeOfHearts instead of fourOfSpades
            Assert.AreEqual(originalTargetCard, targetControl.Card, 
                "Target control should still show the original card, not the dropped card");
        }

        /// <summary>
        /// Test what happens when GetTableauColumnIndex fails but we're still working with tableau cards
        /// This simulates the potential bug condition
        /// </summary>
        [TestMethod]
        public void ExecuteMove_WhenTableauDetectionFails_ShouldNotReplaceCard()
        {
            // Arrange
            MockMainWindowLogicWithBuggyDetection logic = new MockMainWindowLogicWithBuggyDetection();
            
            // Set up tableau column 0 with a 4 of spades
            Card fourOfSpades = new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Spade };
            logic.solitaireRules.TableauColumns[0].Add(fourOfSpades);
            logic.tableauControls[0][0].Card = fourOfSpades;
            
            // Set up waste pile with a 3 of hearts
            Card threeOfHearts = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart };
            logic.solitaireRules.WastePile.Add(threeOfHearts);
            logic.WastePile.Card = threeOfHearts;
            
            MockCardControl sourceControl = logic.WastePile;
            MockCardControl targetControl = logic.tableauControls[0][0];
            
            // Act - execute the move with buggy detection
            logic.ExecuteMove(sourceControl, targetControl, threeOfHearts);
            
            // Assert - this test should fail with current logic, demonstrating the bug
            // The tableau should have both cards
            Assert.AreEqual(2, logic.solitaireRules.TableauColumns[0].Count, 
                "Tableau column should have both cards even when detection fails");
            Assert.AreEqual(fourOfSpades, logic.solitaireRules.TableauColumns[0][0], 
                "Original card should not disappear when detection fails");
        }
    }
}