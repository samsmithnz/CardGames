using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to verify that Kings can be dragged to empty tableau columns
    /// This tests the drag validation logic that should allow Kings on all empty tableau columns
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class KingDragValidationTest
    {
        [TestMethod]
        public void KingDragValidation_AllEmptyTableauColumns_ShouldBeValid()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card kingOfClubs = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Club };
            Card kingOfDiamonds = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Diamond };
            
            Card[] kings = { kingOfSpades, kingOfHearts, kingOfClubs, kingOfDiamonds };
            
            // Test each King on each empty tableau column
            for (int kingIndex = 0; kingIndex < kings.Length; kingIndex++)
            {
                Card king = kings[kingIndex];
                
                for (int column = 0; column < 7; column++)
                {
                    // Ensure all columns are empty
                    for (int clearCol = 0; clearCol < 7; clearCol++)
                    {
                        rules.TableauColumns[clearCol].Clear();
                    }
                    
                    // Act - Test if the King can be placed on this empty tableau column
                    bool canPlace = rules.CanPlaceCardOnTableau(king, column);
                    
                    // Assert
                    Assert.IsTrue(canPlace, 
                        $"King of {king.Suite} should be able to be placed on empty tableau column {column}");
                }
            }
        }
        
        [TestMethod]
        public void NonKingDragValidation_AllEmptyTableauColumns_ShouldBeInvalid()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card queenOfSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };
            Card jackOfHearts = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Heart };
            Card aceOfClubs = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Club };
            
            Card[] nonKings = { queenOfSpades, jackOfHearts, aceOfClubs };
            
            // Test each non-King on each empty tableau column
            for (int cardIndex = 0; cardIndex < nonKings.Length; cardIndex++)
            {
                Card card = nonKings[cardIndex];
                
                for (int column = 0; column < 7; column++)
                {
                    // Ensure all columns are empty
                    for (int clearCol = 0; clearCol < 7; clearCol++)
                    {
                        rules.TableauColumns[clearCol].Clear();
                    }
                    
                    // Act - Test if the non-King can be placed on this empty tableau column
                    bool canPlace = rules.CanPlaceCardOnTableau(card, column);
                    
                    // Assert
                    Assert.IsFalse(canPlace, 
                        $"{card.Number} of {card.Suite} should NOT be able to be placed on empty tableau column {column}");
                }
            }
        }
    }
}