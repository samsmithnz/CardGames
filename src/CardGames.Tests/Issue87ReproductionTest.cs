using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class Issue87ReproductionTest
    {
        [TestMethod]
        public void Issue87_KingFromWastePileToEmptyTableau_ShouldBeValid()
        {
            // Arrange - Reproduce the exact scenario from issue #87
            SolitaireRules rules = new SolitaireRules();
            
            // Create a King in the waste pile (simulate drawing from stock)
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            rules.WastePile.Add(kingOfSpades);
            
            // Ensure we have an empty tableau column (simulate game state)
            int emptyColumnIndex = 0;
            rules.TableauColumns[emptyColumnIndex].Clear();
            
            // Act - Test the core validation logic that the UI should use
            bool canPlaceKing = rules.CanPlaceCardOnTableau(kingOfSpades, emptyColumnIndex);
            
            // Assert
            Assert.IsTrue(canPlaceKing, "King from waste pile should be able to be placed on empty tableau column");
            Assert.AreEqual(0, rules.TableauColumns[emptyColumnIndex].Count, "Target tableau column should be empty");
            Assert.AreEqual(1, rules.WastePile.Count, "Waste pile should contain the King");
            Assert.AreEqual(kingOfSpades, rules.WastePile[0], "Waste pile should contain the King of Spades");
        }

        [TestMethod]
        public void Issue87_SimulateCompleteMove_WastePileToEmptyTableau()
        {
            // Arrange - Full move simulation
            SolitaireRules rules = new SolitaireRules();
            
            // Set up waste pile with some cards
            Card jackOfHearts = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Heart };
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            
            rules.WastePile.Add(jackOfHearts);
            rules.WastePile.Add(kingOfSpades); // King is on top
            
            // Ensure column 2 is empty
            int targetColumn = 2;
            rules.TableauColumns[targetColumn].Clear();
            
            // Pre-move validation
            Card topWasteCard = rules.WastePile[rules.WastePile.Count - 1];
            Assert.AreEqual(kingOfSpades, topWasteCard, "Top waste card should be King");
            Assert.IsTrue(rules.CanPlaceCardOnTableau(topWasteCard, targetColumn), "Move should be valid");
            
            // Simulate the move
            rules.WastePile.RemoveAt(rules.WastePile.Count - 1); // Remove from waste
            rules.TableauColumns[targetColumn].Add(kingOfSpades); // Add to tableau
            
            // Post-move verification
            Assert.AreEqual(1, rules.WastePile.Count, "Waste pile should have 1 card left");
            Assert.AreEqual(jackOfHearts, rules.WastePile[0], "Jack should now be on top of waste pile");
            Assert.AreEqual(1, rules.TableauColumns[targetColumn].Count, "Tableau column should have 1 card");
            Assert.AreEqual(kingOfSpades, rules.TableauColumns[targetColumn][0], "King should be in tableau column");
        }

        [TestMethod]
        public void Issue87_VerifyAllEmptyColumnsAcceptKings()
        {
            // Arrange - Test all 7 tableau columns
            SolitaireRules rules = new SolitaireRules();
            Card[] kings = new Card[]
            {
                new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade },
                new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart },
                new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Club },
                new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Diamond }
            };
            
            // Clear all tableau columns
            for (int col = 0; col < 7; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Act & Assert - Each king should be valid on each empty column
            foreach (Card king in kings)
            {
                for (int col = 0; col < 7; col++)
                {
                    bool canPlace = rules.CanPlaceCardOnTableau(king, col);
                    Assert.IsTrue(canPlace, $"{king.Number} of {king.Suite}s should be valid on empty column {col}");
                }
            }
        }
    }
}