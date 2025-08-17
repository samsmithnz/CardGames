using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class Issue108ReproductionTest
    {
        [TestMethod]
        public void Issue108_KingToFirstEmptyTableauColumn_ShouldBeValid()
        {
            // Arrange - specifically test the FIRST empty tableau column (index 0)
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            
            // Ensure the FIRST tableau column (index 0) is empty
            rules.TableauColumns[0].Clear();
            
            // Act - Test the core validation logic
            bool canPlace = rules.CanPlaceCardOnTableau(kingOfSpades, 0);
            
            // Assert
            Assert.IsTrue(canPlace, "King should be able to be placed on the FIRST empty tableau column (index 0)");
            Assert.AreEqual(0, rules.TableauColumns[0].Count, "First tableau column should be empty");
        }
        
        [TestMethod]
        public void Issue108_KingToFirstEmptyTableauColumn_CompareAllColumns()
        {
            // Arrange - test all columns but focus on first column
            SolitaireRules rules = new SolitaireRules();
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            
            // Clear all tableau columns
            for (int col = 0; col < 7; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Act & Assert - Test each column, paying special attention to column 0
            for (int col = 0; col < 7; col++)
            {
                bool canPlace = rules.CanPlaceCardOnTableau(kingOfHearts, col);
                if (col == 0)
                {
                    Assert.IsTrue(canPlace, "King should be placeable on FIRST empty tableau column (this is the specific issue)");
                }
                else
                {
                    Assert.IsTrue(canPlace, $"King should be placeable on empty tableau column {col} (should work normally)");
                }
            }
        }
    }
}