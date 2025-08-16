using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class KingToEmptyTableauTest
    {
        [TestMethod]
        public void CanPlaceKingOnEmptyTableauColumn_ShouldReturnTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            
            // Ensure column 0 is empty
            rules.TableauColumns[0].Clear();
            
            // Act
            bool canPlace = rules.CanPlaceCardOnTableau(kingOfSpades, 0);
            
            // Assert
            Assert.IsTrue(canPlace, "King should be able to be placed on empty tableau column");
        }
        
        [TestMethod]
        public void CannotPlaceNonKingOnEmptyTableauColumn_ShouldReturnFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            
            // Ensure column 0 is empty
            rules.TableauColumns[0].Clear();
            
            // Act
            bool canPlace = rules.CanPlaceCardOnTableau(queenOfHearts, 0);
            
            // Assert
            Assert.IsFalse(canPlace, "Non-King cards should not be able to be placed on empty tableau column");
        }
        
        [TestMethod]
        public void KingToEmptyTableau_AllColumnsTest()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            
            // Test all 7 tableau columns
            for (int column = 0; column < 7; column++)
            {
                // Ensure column is empty
                rules.TableauColumns[column].Clear();
                
                // Act
                bool canPlace = rules.CanPlaceCardOnTableau(kingOfHearts, column);
                
                // Assert
                Assert.IsTrue(canPlace, $"King should be able to be placed on empty tableau column {column}");
            }
        }
    }
}