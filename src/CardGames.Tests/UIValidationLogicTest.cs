using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;
using System.Collections.Generic;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class UIValidationLogicTest
    {
        [TestMethod]
        public void TestValidationLogic_KingToEmptyTableauColumn()
        {
            // Arrange - Simulate the validation logic that was fixed
            SolitaireRules rules = new SolitaireRules();
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            
            // Clear a tableau column to simulate empty space
            rules.TableauColumns[0].Clear();
            
            // Simulate the tableau column index detection (this would return 0 for first tableau column)
            int targetColumnIndex = 0; // This simulates GetTableauColumnIndex returning 0
            
            // Act - Test the core validation logic that was fixed
            bool canPlace = rules.CanPlaceCardOnTableau(kingOfHearts, targetColumnIndex);
            
            // Assert - This should now work correctly
            Assert.IsTrue(canPlace, "King should be placeable on empty tableau column");
        }
        
        [TestMethod]
        public void TestValidationLogic_NonKingToEmptyTableauColumn()
        {
            // Arrange - Test the error case
            SolitaireRules rules = new SolitaireRules();
            Card aceOfSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
            
            // Clear a tableau column to simulate empty space
            rules.TableauColumns[1].Clear();
            
            // Simulate the tableau column index detection
            int targetColumnIndex = 1;
            
            // Act - Test that non-Kings are rejected
            bool canPlace = rules.CanPlaceCardOnTableau(aceOfSpades, targetColumnIndex);
            
            // Assert - This should correctly reject non-Kings
            Assert.IsFalse(canPlace, "Non-King should not be placeable on empty tableau column");
        }
        
        [TestMethod]
        public void TestValidationLogic_CardSequencesOnTableau()
        {
            // Arrange - Test valid sequences
            SolitaireRules rules = new SolitaireRules();
            Card kingSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };     // Black King
            Card queenHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };    // Red Queen
            Card jackClubs = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };       // Black Jack
            
            // Set up tableau column with King
            rules.TableauColumns[2].Clear();
            rules.TableauColumns[2].Add(kingSpades);
            
            // Act & Assert - Test valid sequence
            Assert.IsTrue(rules.CanPlaceCardOnTableau(queenHearts, 2), "Red Queen should be placeable on Black King");
            
            // Add Queen and test next card
            rules.TableauColumns[2].Add(queenHearts);
            Assert.IsTrue(rules.CanPlaceCardOnTableau(jackClubs, 2), "Black Jack should be placeable on Red Queen");
            
            // Test invalid moves
            Card queenSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };    // Black Queen
            Card aceHearts = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };      // Red Ace
            
            // Reset to just King
            rules.TableauColumns[2].Clear();
            rules.TableauColumns[2].Add(kingSpades);
            
            Assert.IsFalse(rules.CanPlaceCardOnTableau(queenSpades, 2), "Black Queen should not be placeable on Black King (same color)");
            Assert.IsFalse(rules.CanPlaceCardOnTableau(aceHearts, 2), "Ace should not be placeable on King (wrong rank sequence)");
        }
        
        [TestMethod]
        public void TestMultipleEmptyTableauColumns()
        {
            // Arrange - Test that Kings can be placed on any empty tableau column
            SolitaireRules rules = new SolitaireRules();
            
            Card kingHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card kingSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card kingDiamonds = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Diamond };
            Card kingClubs = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Club };
            
            // Clear multiple tableau columns
            for (int i = 0; i < 4; i++)
            {
                rules.TableauColumns[i].Clear();
            }
            
            // Act & Assert - All Kings should be placeable on empty columns
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingHearts, 0), "King of Hearts should be placeable on empty column 0");
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingSpades, 1), "King of Spades should be placeable on empty column 1");
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingDiamonds, 2), "King of Diamonds should be placeable on empty column 2");
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingClubs, 3), "King of Clubs should be placeable on empty column 3");
        }
    }
}