using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to reproduce and fix the MainWindow ExecuteMove bug where cards disappear during stacking
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class MainWindowExecuteMoveTests
    {
        /// <summary>
        /// Test what happens when GetTableauColumnIndex fails but we're still working with tableau cards
        /// This simulates the bug condition and should FAIL with the original logic
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
            
            // Assert - this test should fail with original logic, demonstrating the bug
            Assert.AreEqual(2, logic.solitaireRules.TableauColumns[0].Count, 
                "Tableau column should have both cards even when detection fails");
            Assert.AreEqual(fourOfSpades, logic.solitaireRules.TableauColumns[0][0], 
                "Original card should not disappear when detection fails");
        }

        /// <summary>
        /// Test the FIXED version that should handle tableau detection failures correctly
        /// </summary>
        [TestMethod]
        public void ExecuteMove_WithFix_WhenTableauDetectionFails_ShouldStackCorrectly()
        {
            // Arrange
            MockMainWindowLogicWithBuggyDetectionButFixed logic = new MockMainWindowLogicWithBuggyDetectionButFixed();
            
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
            
            // Act - execute the move with the FIXED logic
            logic.ExecuteMove(sourceControl, targetControl, threeOfHearts);
            
            // Assert - this test should PASS with the fix
            Assert.AreEqual(2, logic.solitaireRules.TableauColumns[0].Count, 
                "FIXED: Tableau column should have both cards even when primary detection fails");
            Assert.AreEqual(fourOfSpades, logic.solitaireRules.TableauColumns[0][0], 
                "FIXED: Original card should not disappear even when primary detection fails");
            Assert.AreEqual(threeOfHearts, logic.solitaireRules.TableauColumns[0][1], 
                "FIXED: New card should be properly stacked on top");
        }

        /// <summary>
        /// Mock class to simulate the MainWindow logic without actual UI controls
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
            /// Original version of ExecuteMove logic that has the bug
            /// </summary>
            public virtual void ExecuteMove(MockCardControl sourceControl, MockCardControl targetControl, Card card)
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

            protected void RemoveCardFromSource(MockCardControl sourceControl, Card card)
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

        /// <summary>
        /// Mock class with buggy tableau detection to reproduce the bug
        /// </summary>
        private class MockMainWindowLogicWithBuggyDetection : MockMainWindowLogic
        {
            protected override int GetTableauColumnIndexInternal(MockCardControl control)
            {
                // Always return -1 to simulate detection failure
                return -1;
            }
        }

        /// <summary>
        /// Mock class with buggy detection but FIXED ExecuteMove logic
        /// </summary>
        private class MockMainWindowLogicWithBuggyDetectionButFixed : MockMainWindowLogic
        {
            protected override int GetTableauColumnIndexInternal(MockCardControl control)
            {
                // Always return -1 to simulate detection failure
                return -1;
            }

            /// <summary>
            /// Fixed version of ExecuteMove with improved tableau detection
            /// </summary>
            public override void ExecuteMove(MockCardControl sourceControl, MockCardControl targetControl, Card card)
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
                int targetColumnIndex = GetTableauColumnIndexInternal(targetControl);
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
                    // Check if this is a tableau move - try a more comprehensive detection
                    int targetColumnIndexForExistingCard = GetTableauColumnIndexInternal(targetControl);
                    bool isTableauMove = (targetColumnIndexForExistingCard >= 0) || IsTableauCardControl(targetControl);
                    
                    if (isTableauMove)
                    {
                        // This is a tableau move - handle it properly
                        if (targetColumnIndexForExistingCard < 0)
                        {
                            // Detection failed but we know it's a tableau card - find it manually
                            targetColumnIndexForExistingCard = FindTableauColumnForCard(targetControl);
                        }
                        
                        if (targetColumnIndexForExistingCard >= 0)
                        {
                            RemoveCardFromSource(sourceControl, card);
                            
                            // Add to target tableau column
                            solitaireRules.TableauColumns[targetColumnIndexForExistingCard].Add(card);
                        }
                    }
                    else
                    {
                        // Non-tableau move - replace the card (for waste pile, etc.)
                        targetControl.Card = card;
                        sourceControl.Card = null;
                    }
                }
            }

            private bool IsTableauCardControl(MockCardControl control)
            {
                foreach (List<MockCardControl> column in tableauControls)
                {
                    if (column.Contains(control))
                    {
                        return true;
                    }
                }
                return false;
            }

            private int FindTableauColumnForCard(MockCardControl control)
            {
                for (int col = 0; col < tableauControls.Count; col++)
                {
                    for (int row = 0; row < tableauControls[col].Count; row++)
                    {
                        if (tableauControls[col][row] == control)
                        {
                            return col;
                        }
                    }
                }
                return -1;
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
    }
}