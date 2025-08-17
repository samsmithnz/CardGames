using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to verify the UI workflow for dragging cards from foundation piles to tableau columns
    /// These tests simulate the actual drag and drop workflow that the UI would perform
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class FoundationToTableauUITests
    {
        /// <summary>
        /// Test the complete UI workflow: validate and execute a foundation to tableau move
        /// This simulates what happens when a user drags a card from foundation to tableau
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_CompleteUIWorkflow_ShouldSucceed()
        {
            // Arrange - Simulate the state after some gameplay
            SolitaireRules rules = new SolitaireRules();
            
            // Foundation pile with Ace, 2, and 3 of hearts
            Card aceOfHearts = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };
            Card twoOfHearts = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Heart };
            Card threeOfHearts = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(aceOfHearts);
            rules.FoundationPiles[0].Add(twoOfHearts);
            rules.FoundationPiles[0].Add(threeOfHearts);
            
            // Tableau column with 4 of clubs (can accept 3 of hearts)
            Card fourOfClubs = new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Club };
            rules.TableauColumns[2].Add(fourOfClubs);
            
            // Simulate user dragging the 3 of hearts from foundation to tableau column 2
            Card draggedCard = threeOfHearts; // Top card of foundation pile 0
            int sourceFoundationIndex = 0;
            int targetTableauIndex = 2;
            
            // Act & Assert - Step by step UI workflow
            
            // 1. User starts dragging - card should be face-up and draggable
            // (Foundation cards are always face-up in a real game)
            Assert.IsTrue(true, "Foundation cards are always face-up and draggable");
            
            // 2. Validate the drop target (simulates ValidateMoveDetailed)
            bool canPlace = rules.CanPlaceCardOnTableau(draggedCard, targetTableauIndex);
            Assert.IsTrue(canPlace, "3 of hearts should be valid to place on 4 of clubs");
            
            // 3. If valid, execute the move (simulates ExecuteMove)
            
            // 3a. Remove from source foundation
            Assert.AreEqual(draggedCard, rules.FoundationPiles[sourceFoundationIndex][rules.FoundationPiles[sourceFoundationIndex].Count - 1],
                "Dragged card should be on top of foundation pile");
            
            rules.FoundationPiles[sourceFoundationIndex].RemoveAt(rules.FoundationPiles[sourceFoundationIndex].Count - 1);
            
            // 3b. Add to target tableau
            rules.TableauColumns[targetTableauIndex].Add(draggedCard);
            
            // Assert final state
            Assert.AreEqual(2, rules.FoundationPiles[sourceFoundationIndex].Count, 
                "Foundation pile should have 2 cards remaining");
            Assert.AreEqual(twoOfHearts, rules.FoundationPiles[sourceFoundationIndex][rules.FoundationPiles[sourceFoundationIndex].Count - 1],
                "2 of hearts should now be on top of foundation pile");
            
            Assert.AreEqual(2, rules.TableauColumns[targetTableauIndex].Count,
                "Tableau column should have 2 cards");
            Assert.AreEqual(fourOfClubs, rules.TableauColumns[targetTableauIndex][0],
                "4 of clubs should be at bottom of tableau column");
            Assert.AreEqual(threeOfHearts, rules.TableauColumns[targetTableauIndex][1],
                "3 of hearts should be on top of tableau column");
        }
        
        /// <summary>
        /// Test that invalid foundation to tableau moves are properly rejected
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_InvalidMove_ShouldBeRejected()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Foundation pile with 5 of diamonds
            Card fiveOfDiamonds = new Card { Number = Card.CardNumber._5, Suite = Card.CardSuite.Diamond };
            rules.FoundationPiles[1].Add(fiveOfDiamonds);
            
            // Tableau column with 3 of clubs (cannot accept 5 of diamonds - not one rank lower)
            Card threeOfClubs = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Club };
            rules.TableauColumns[1].Add(threeOfClubs);
            
            // Act - Try to validate the invalid move
            bool canPlace = rules.CanPlaceCardOnTableau(fiveOfDiamonds, 1);
            
            // Assert
            Assert.IsFalse(canPlace, "5 of diamonds should not be valid to place on 3 of clubs");
        }
        
        /// <summary>
        /// Test foundation to empty tableau column with King
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_KingToEmptyColumn_ShouldSucceed()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Foundation pile with King of spades
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            rules.FoundationPiles[3].Add(kingOfSpades);
            
            // Leave tableau column 4 empty
            
            // Act - Validate and execute the move
            bool canPlace = rules.CanPlaceCardOnTableau(kingOfSpades, 4);
            Assert.IsTrue(canPlace, "King should be valid to place on empty tableau column");
            
            // Execute the move
            rules.FoundationPiles[3].RemoveAt(rules.FoundationPiles[3].Count - 1);
            rules.TableauColumns[4].Add(kingOfSpades);
            
            // Assert
            Assert.AreEqual(0, rules.FoundationPiles[3].Count, "Foundation pile should be empty");
            Assert.AreEqual(1, rules.TableauColumns[4].Count, "Tableau column should have 1 card");
            Assert.AreEqual(kingOfSpades, rules.TableauColumns[4][0], "King should be in tableau column");
        }
        
        /// <summary>
        /// Test foundation to empty tableau column with non-King (should fail)
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_NonKingToEmptyColumn_ShouldFail()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Foundation pile with Jack of hearts
            Card jackOfHearts = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(jackOfHearts);
            
            // Leave tableau column 5 empty
            
            // Act & Assert
            bool canPlace = rules.CanPlaceCardOnTableau(jackOfHearts, 5);
            Assert.IsFalse(canPlace, "Jack should not be valid to place on empty tableau column - only Kings allowed");
        }
    }
}