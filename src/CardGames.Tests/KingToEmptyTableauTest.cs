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
        
        [TestMethod]
        public void TableauSequencePlacement_DescendingAlternatingColors()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };    // Black
            Card queenHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };   // Red
            Card jackClubs = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };      // Black
            Card tenDiamonds = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Diamond }; // Red
            
            // Clear column 0 and place King
            rules.TableauColumns[0].Clear();
            rules.TableauColumns[0].Add(kingSpades);
            
            // Act & Assert
            Assert.IsTrue(rules.CanPlaceCardOnTableau(queenHearts, 0), "Red Queen should be placeable on Black King");
            
            rules.TableauColumns[0].Add(queenHearts);
            Assert.IsTrue(rules.CanPlaceCardOnTableau(jackClubs, 0), "Black Jack should be placeable on Red Queen");
            
            rules.TableauColumns[0].Add(jackClubs);
            Assert.IsTrue(rules.CanPlaceCardOnTableau(tenDiamonds, 0), "Red 10 should be placeable on Black Jack");
            
            // Test invalid moves
            Card queenSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };  // Black Queen
            Card kingHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };   // Red King
            
            rules.TableauColumns[0].Clear();
            rules.TableauColumns[0].Add(kingSpades);
            
            Assert.IsFalse(rules.CanPlaceCardOnTableau(queenSpades, 0), "Black Queen should not be placeable on Black King (same color)");
            Assert.IsFalse(rules.CanPlaceCardOnTableau(kingHearts, 0), "King should not be placeable on King (same rank)");
        }
    }
}