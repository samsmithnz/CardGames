using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EmptyTableauColumnValidationTest
    {
        [TestMethod]
        public void EmptyTableauColumn_ShouldAcceptKing()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            
            // Ensure all tableau columns are empty
            for (int col = 0; col < rules.TableauColumns.Count; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Act & Assert - test all columns
            for (int col = 0; col < rules.TableauColumns.Count; col++)
            {
                bool canPlace = rules.CanPlaceCardOnTableau(kingOfSpades, col);
                Assert.IsTrue(canPlace, $"King should be placeable on empty tableau column {col}");
                
                // Verify the column is actually empty
                Assert.AreEqual(0, rules.TableauColumns[col].Count, $"Column {col} should be empty");
            }
        }

        [TestMethod]
        public void EmptyTableauColumn_ShouldRejectNonKing()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card aceOfSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
            Card tenOfDiamonds = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Diamond };
            
            // Test various non-King cards
            Card[] nonKings = { queenOfHearts, aceOfSpades, tenOfDiamonds };
            
            // Ensure all tableau columns are empty
            for (int col = 0; col < rules.TableauColumns.Count; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Act & Assert
            foreach (Card card in nonKings)
            {
                for (int col = 0; col < rules.TableauColumns.Count; col++)
                {
                    bool canPlace = rules.CanPlaceCardOnTableau(card, col);
                    Assert.IsFalse(canPlace, $"{card.Number} should NOT be placeable on empty tableau column {col}");
                }
            }
        }

        [TestMethod]
        public void EmptyTableauColumnScenario_WastePileToTableau()
        {
            // Arrange - simulate a game state where waste pile has a King
            SolitaireRules rules = new SolitaireRules();
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card queenOfSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };
            
            // Set up waste pile with King on top
            rules.WastePile.Clear();
            rules.WastePile.Add(queenOfSpades); // Bottom card
            rules.WastePile.Add(kingOfHearts);  // Top card (can be moved)
            
            // Ensure tableau column 3 is empty (arbitrary choice)
            rules.TableauColumns[3].Clear();
            
            // Act
            Card topWasteCard = rules.WastePile[rules.WastePile.Count - 1];
            bool canMoveToEmptyColumn = rules.CanPlaceCardOnTableau(topWasteCard, 3);
            
            // Assert
            Assert.AreEqual(kingOfHearts, topWasteCard, "Top waste card should be King of Hearts");
            Assert.IsTrue(canMoveToEmptyColumn, "King from waste pile should be moveable to empty tableau column");
            Assert.AreEqual(0, rules.TableauColumns[3].Count, "Target tableau column should be empty");
        }

        [TestMethod]
        public void TableauColumnWithCards_ShouldFollowNormalRules()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };   // Black
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };  // Red
            Card jackOfClubs = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };     // Black
            Card queenOfSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };  // Black
            
            // Set up tableau column 0 with a King
            rules.TableauColumns[0].Clear();
            rules.TableauColumns[0].Add(kingOfSpades);
            
            // Act & Assert
            // Red Queen should be valid on Black King
            Assert.IsTrue(rules.CanPlaceCardOnTableau(queenOfHearts, 0), "Red Queen should be valid on Black King");
            
            // Black Queen should NOT be valid on Black King (same color)
            Assert.IsFalse(rules.CanPlaceCardOnTableau(queenOfSpades, 0), "Black Queen should NOT be valid on Black King (same color)");
            
            // Jack should NOT be valid on King (wrong rank)
            Assert.IsFalse(rules.CanPlaceCardOnTableau(jackOfClubs, 0), "Jack should NOT be valid on King (wrong rank)");
            
            // Add the Red Queen and test placing Black Jack
            rules.TableauColumns[0].Add(queenOfHearts);
            Assert.IsTrue(rules.CanPlaceCardOnTableau(jackOfClubs, 0), "Black Jack should be valid on Red Queen");
        }
    }
}