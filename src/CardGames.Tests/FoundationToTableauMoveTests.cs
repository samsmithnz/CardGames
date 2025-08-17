using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to verify that cards can be moved from foundation piles back to tableau columns
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class FoundationToTableauMoveTests
    {
        /// <summary>
        /// Test that a card from a foundation pile can be validly placed on a tableau column
        /// </summary>
        [TestMethod]
        public void CanPlaceCardOnTableau_FromFoundation_ShouldReturnTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a foundation pile with a 3 of hearts
            Card threeOfHearts = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(threeOfHearts);
            
            // Set up a tableau column with a 4 of clubs (can accept 3 of hearts - one rank lower, opposite color)
            Card fourOfClubs = new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Club };
            rules.TableauColumns[1].Add(fourOfClubs);
            
            // Act - Test if the foundation card can be placed on the tableau
            bool canPlace = rules.CanPlaceCardOnTableau(threeOfHearts, 1);
            
            // Assert
            Assert.IsTrue(canPlace, "3 of hearts from foundation should be able to be placed on 4 of clubs in tableau");
        }
        
        /// <summary>
        /// Test that a card from a foundation pile cannot be placed on an invalid tableau column
        /// </summary>
        [TestMethod]
        public void CanPlaceCardOnTableau_FromFoundation_InvalidMove_ShouldReturnFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a foundation pile with a 3 of hearts
            Card threeOfHearts = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(threeOfHearts);
            
            // Set up a tableau column with a 5 of clubs (cannot accept 3 of hearts - not one rank lower)
            Card fiveOfClubs = new Card { Number = Card.CardNumber._5, Suite = Card.CardSuite.Club };
            rules.TableauColumns[1].Add(fiveOfClubs);
            
            // Act - Test if the foundation card can be placed on the tableau
            bool canPlace = rules.CanPlaceCardOnTableau(threeOfHearts, 1);
            
            // Assert
            Assert.IsFalse(canPlace, "3 of hearts from foundation should not be able to be placed on 5 of clubs in tableau");
        }
        
        /// <summary>
        /// Test that a King from foundation can be placed on an empty tableau column
        /// </summary>
        [TestMethod]
        public void CanPlaceCardOnTableau_KingFromFoundation_EmptyTableau_ShouldReturnTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a foundation pile with a King of spades
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            rules.FoundationPiles[3].Add(kingOfSpades);
            
            // Leave tableau column 2 empty
            
            // Act - Test if the King can be placed on the empty tableau
            bool canPlace = rules.CanPlaceCardOnTableau(kingOfSpades, 2);
            
            // Assert
            Assert.IsTrue(canPlace, "King of spades from foundation should be able to be placed on empty tableau column");
        }
        
        /// <summary>
        /// Test that a non-King from foundation cannot be placed on an empty tableau column
        /// </summary>
        [TestMethod]
        public void CanPlaceCardOnTableau_NonKingFromFoundation_EmptyTableau_ShouldReturnFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a foundation pile with a Queen of hearts
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(queenOfHearts);
            
            // Leave tableau column 2 empty
            
            // Act - Test if the Queen can be placed on the empty tableau
            bool canPlace = rules.CanPlaceCardOnTableau(queenOfHearts, 2);
            
            // Assert
            Assert.IsFalse(canPlace, "Queen of hearts from foundation should not be able to be placed on empty tableau column - only Kings allowed");
        }
        
        /// <summary>
        /// Test the complete foundation to tableau move workflow by simulating the actual sequence
        /// </summary>
        [TestMethod]
        public void FoundationToTableauMove_CompleteWorkflow_ShouldSucceed()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a foundation pile with Ace and 2 of hearts
            Card aceOfHearts = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };
            Card twoOfHearts = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(aceOfHearts);
            rules.FoundationPiles[0].Add(twoOfHearts);
            
            // Set up a tableau column with 3 of spades (can accept 2 of hearts)
            Card threeOfSpades = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Spade };
            rules.TableauColumns[1].Add(threeOfSpades);
            
            // Initial counts
            int initialFoundationCount = rules.FoundationPiles[0].Count;
            int initialTableauCount = rules.TableauColumns[1].Count;
            
            // Act - Simulate the move workflow
            
            // 1. Validate the move (this should be what ValidateMoveDetailed does)
            bool canPlace = rules.CanPlaceCardOnTableau(twoOfHearts, 1);
            Assert.IsTrue(canPlace, "2 of hearts should be valid to place on 3 of spades");
            
            // 2. Remove from source (foundation)
            rules.FoundationPiles[0].RemoveAt(rules.FoundationPiles[0].Count - 1);
            
            // 3. Add to target (tableau)
            rules.TableauColumns[1].Add(twoOfHearts);
            
            // Assert - Verify the move completed successfully
            Assert.AreEqual(initialFoundationCount - 1, rules.FoundationPiles[0].Count, 
                "Foundation should have one less card");
            Assert.AreEqual(initialTableauCount + 1, rules.TableauColumns[1].Count, 
                "Tableau should have one more card");
            
            // Verify the foundation pile now shows the ace on top
            Assert.AreEqual(aceOfHearts, rules.FoundationPiles[0][rules.FoundationPiles[0].Count - 1], 
                "Ace of hearts should now be on top of foundation pile");
            
            // Verify the tableau has both cards in correct order
            Assert.AreEqual(threeOfSpades, rules.TableauColumns[1][0], 
                "3 of spades should be at bottom of tableau column");
            Assert.AreEqual(twoOfHearts, rules.TableauColumns[1][1], 
                "2 of hearts should be on top of tableau column");
        }
    }
}