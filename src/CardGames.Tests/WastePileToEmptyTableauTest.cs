using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class WastePileToEmptyTableauTest
    {
        [TestMethod]
        public void CanMoveKingFromWastePileToEmptyTableauColumn_ShouldReturnTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            
            // Add king to waste pile
            rules.WastePile.Add(kingOfSpades);
            
            // Ensure tableau column 0 is empty
            rules.TableauColumns[0].Clear();
            
            // Act
            bool canPlace = rules.CanPlaceCardOnTableau(kingOfSpades, 0);
            
            // Assert
            Assert.IsTrue(canPlace, "King from waste pile should be able to be placed on empty tableau column");
        }

        [TestMethod]
        public void CannotMoveNonKingFromWastePileToEmptyTableauColumn_ShouldReturnFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            
            // Add queen to waste pile
            rules.WastePile.Add(queenOfHearts);
            
            // Ensure tableau column 0 is empty
            rules.TableauColumns[0].Clear();
            
            // Act
            bool canPlace = rules.CanPlaceCardOnTableau(queenOfHearts, 0);
            
            // Assert
            Assert.IsFalse(canPlace, "Non-King cards from waste pile should not be able to be placed on empty tableau column");
        }

        [TestMethod]
        public void WastePileToTableauMoveSequence_ShouldWorkCorrectly()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            
            // Simulate moving king from waste pile to empty tableau column
            rules.WastePile.Add(kingOfSpades);
            rules.TableauColumns[0].Clear();
            
            // Verify king can be placed
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfSpades, 0), "King should be placeable on empty column");
            
            // Simulate the move
            rules.WastePile.RemoveAt(rules.WastePile.Count - 1); // Remove from waste
            rules.TableauColumns[0].Add(kingOfSpades); // Add to tableau
            
            // Now test placing a queen on the king
            rules.WastePile.Add(queenOfHearts);
            bool canPlaceQueen = rules.CanPlaceCardOnTableau(queenOfHearts, 0);
            
            // Assert
            Assert.IsTrue(canPlaceQueen, "Red Queen should be placeable on Black King");
        }
    }
}