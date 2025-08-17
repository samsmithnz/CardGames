using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to verify the drag validation logic specifically handles empty tableau columns correctly
    /// This simulates the scenario where GetTableauColumnIndex fails but the fallback logic should succeed
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class EmptyTableauDragFallbackTest
    {
        [TestMethod]
        public void EmptyTableauFallbackLogic_KingOnEmptyColumn_ShouldBeValid()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            
            // Ensure all tableau columns are empty
            for (int col = 0; col < 7; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Test that Kings can be placed on all empty tableau columns
            for (int col = 0; col < 7; col++)
            {
                // Act
                bool canPlace = rules.CanPlaceCardOnTableau(kingOfSpades, col);
                
                // Assert
                Assert.IsTrue(canPlace, $"King should be placeable on empty tableau column {col}");
            }
        }
        
        [TestMethod]
        public void EmptyTableauFallbackLogic_NonKingOnEmptyColumn_ShouldBeInvalid()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card jackOfClubs = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };
            Card aceOfDiamonds = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Diamond };
            
            Card[] nonKings = { queenOfHearts, jackOfClubs, aceOfDiamonds };
            
            // Ensure all tableau columns are empty
            for (int col = 0; col < 7; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Test that non-Kings cannot be placed on empty tableau columns
            foreach (Card card in nonKings)
            {
                for (int col = 0; col < 7; col++)
                {
                    // Act
                    bool canPlace = rules.CanPlaceCardOnTableau(card, col);
                    
                    // Assert
                    Assert.IsFalse(canPlace, $"{card.Number} of {card.Suite} should NOT be placeable on empty tableau column {col}");
                }
            }
        }
        
        [TestMethod]
        public void EmptyTableauFallbackLogic_MultipleEmptyColumns_ShouldAllowKingsOnAny()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card kingOfClubs = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Club };
            Card kingOfDiamonds = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Diamond };
            
            // Clear all columns to start with all empty
            for (int col = 0; col < 7; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Test placing each King on a different column
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfSpades, 0), "King of Spades should be placeable on column 0");
            rules.TableauColumns[0].Add(kingOfSpades);
            
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfHearts, 1), "King of Hearts should be placeable on column 1");
            rules.TableauColumns[1].Add(kingOfHearts);
            
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfClubs, 2), "King of Clubs should be placeable on column 2");
            rules.TableauColumns[2].Add(kingOfClubs);
            
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfDiamonds, 3), "King of Diamonds should be placeable on column 3");
            rules.TableauColumns[3].Add(kingOfDiamonds);
            
            // Verify that columns 4, 5, 6 are still empty and can accept Kings
            Card anotherKing = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Assert.IsTrue(rules.CanPlaceCardOnTableau(anotherKing, 4), "Another King should be placeable on empty column 4");
            Assert.IsTrue(rules.CanPlaceCardOnTableau(anotherKing, 5), "Another King should be placeable on empty column 5");
            Assert.IsTrue(rules.CanPlaceCardOnTableau(anotherKing, 6), "Another King should be placeable on empty column 6");
        }
    }
}