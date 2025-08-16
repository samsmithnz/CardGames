using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SolitaireGameplaySimulationTest
    {
        [TestMethod]
        public void SimulateKingMoveToEmptyTableauColumn_ShouldSucceed()
        {
            // Arrange - Create a scenario where a tableau column becomes empty
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a King that needs to be moved
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            
            // Place the King in one column initially
            rules.TableauColumns[0].Add(kingOfHearts);
            
            // Make another column empty (this could happen when all cards are moved to foundations)
            rules.TableauColumns[1].Clear();
            
            // Verify initial state
            Assert.AreEqual(1, rules.TableauColumns[0].Count, "Column 0 should have the King");
            Assert.AreEqual(0, rules.TableauColumns[1].Count, "Column 1 should be empty");
            
            // Act - Try to move the King to the empty column
            bool canMoveToEmpty = rules.CanPlaceCardOnTableau(kingOfHearts, 1);
            
            // Assert - King should be able to move to empty column
            Assert.IsTrue(canMoveToEmpty, "King should be able to move to empty tableau column");
            
            // Simulate the actual move
            if (canMoveToEmpty)
            {
                // Remove from source column
                rules.TableauColumns[0].RemoveAt(0);
                
                // Add to target column
                rules.TableauColumns[1].Add(kingOfHearts);
            }
            
            // Verify final state
            Assert.AreEqual(0, rules.TableauColumns[0].Count, "Column 0 should now be empty");
            Assert.AreEqual(1, rules.TableauColumns[1].Count, "Column 1 should now have the King");
            Assert.AreEqual(kingOfHearts, rules.TableauColumns[1][0], "Column 1 should contain the King of Hearts");
        }
        
        [TestMethod]
        public void SimulateNonKingMoveToEmptyTableauColumn_ShouldFail()
        {
            // Arrange - Create a scenario with an empty tableau column
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a non-King card
            Card queenOfSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };
            
            // Place the Queen in one column initially
            rules.TableauColumns[0].Add(queenOfSpades);
            
            // Make another column empty
            rules.TableauColumns[1].Clear();
            
            // Act - Try to move the Queen to the empty column
            bool canMoveToEmpty = rules.CanPlaceCardOnTableau(queenOfSpades, 1);
            
            // Assert - Queen should NOT be able to move to empty column
            Assert.IsFalse(canMoveToEmpty, "Non-King cards should not be able to move to empty tableau column");
        }
        
        [TestMethod]
        public void SimulateSequentialCardMoves_KingToEmptyThenQueenOnKing()
        {
            // Arrange - Create a realistic gameplay scenario
            SolitaireRules rules = new SolitaireRules();
            
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };    // Black
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };   // Red
            
            // Start with cards in different columns
            rules.TableauColumns[0].Add(kingOfSpades);
            rules.TableauColumns[1].Add(queenOfHearts);
            
            // Make column 2 empty
            rules.TableauColumns[2].Clear();
            
            // Act & Assert - Move King to empty column
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfSpades, 2), "King should be able to move to empty column");
            
            // Simulate moving the King
            rules.TableauColumns[0].RemoveAt(0);
            rules.TableauColumns[2].Add(kingOfSpades);
            
            // Now try to place Queen on the King (should work - red on black, one rank lower)
            Assert.IsTrue(rules.CanPlaceCardOnTableau(queenOfHearts, 2), "Red Queen should be placeable on Black King");
            
            // Simulate moving the Queen
            rules.TableauColumns[1].RemoveAt(0);
            rules.TableauColumns[2].Add(queenOfHearts);
            
            // Verify final state
            Assert.AreEqual(2, rules.TableauColumns[2].Count, "Column 2 should have 2 cards");
            Assert.AreEqual(kingOfSpades, rules.TableauColumns[2][0], "King should be at bottom");
            Assert.AreEqual(queenOfHearts, rules.TableauColumns[2][1], "Queen should be on top");
        }
    }
}