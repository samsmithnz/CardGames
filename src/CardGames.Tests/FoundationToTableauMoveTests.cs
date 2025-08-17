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
    }
}