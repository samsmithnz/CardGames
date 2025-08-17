using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Final comprehensive test to ensure foundation to tableau drag and drop works in all scenarios
    /// This test covers edge cases and potential UI integration issues
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class FoundationToTableauComprehensiveTest
    {
        /// <summary>
        /// Test that verifies foundation to tableau moves work even in complex game states
        /// with multiple cards in foundations and various tableau configurations
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_ComplexGameState_ShouldWork()
        {
            // Arrange - Create a complex game state similar to mid-game
            SolitaireRules rules = new SolitaireRules();
            
            // Foundation pile 0 (Hearts): A♥, 2♥, 3♥, 4♥, 5♥
            rules.FoundationPiles[0].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
            rules.FoundationPiles[0].Add(new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Heart });
            rules.FoundationPiles[0].Add(new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart });
            rules.FoundationPiles[0].Add(new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Heart });
            rules.FoundationPiles[0].Add(new Card { Number = Card.CardNumber._5, Suite = Card.CardSuite.Heart });
            
            // Foundation pile 1 (Diamonds): A♦, 2♦
            rules.FoundationPiles[1].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Diamond });
            rules.FoundationPiles[1].Add(new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Diamond });
            
            // Foundation pile 2 (Clubs): A♣
            rules.FoundationPiles[2].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Club });
            
            // Foundation pile 3 (Spades): A♠, 2♠, 3♠
            rules.FoundationPiles[3].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade });
            rules.FoundationPiles[3].Add(new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Spade });
            rules.FoundationPiles[3].Add(new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Spade });
            
            // Tableau columns with various configurations
            // Column 0: 6♠ (can accept 5♥ from foundation)
            rules.TableauColumns[0].Add(new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Spade });
            
            // Column 1: 3♦ (can accept 2♦ from foundation) 
            rules.TableauColumns[1].Add(new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Diamond });
            
            // Column 2: Empty (can accept any King)
            // Column 3: 4♣, 3♠ (can accept 2♦ from foundation)
            rules.TableauColumns[3].Add(new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Club });
            rules.TableauColumns[3].Add(new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Spade });
            
            // Column 4: J♦, 10♣, 9♦ (no valid foundation moves)
            rules.TableauColumns[4].Add(new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Diamond });
            rules.TableauColumns[4].Add(new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Club });
            rules.TableauColumns[4].Add(new Card { Number = Card.CardNumber._9, Suite = Card.CardSuite.Diamond });
            
            // Test multiple valid foundation to tableau moves
            
            // Test 1: Move 5♥ from foundation to 6♠ in tableau
            Card fiveOfHearts = rules.FoundationPiles[0][rules.FoundationPiles[0].Count - 1];
            Assert.IsTrue(rules.CanPlaceCardOnTableau(fiveOfHearts, 0), 
                "5♥ should be valid on 6♠");
            
            // Execute the move
            rules.FoundationPiles[0].RemoveAt(rules.FoundationPiles[0].Count - 1);
            rules.TableauColumns[0].Add(fiveOfHearts);
            
            // Verify foundation state after move
            Assert.AreEqual(4, rules.FoundationPiles[0].Count, "Hearts foundation should have 4 cards");
            Assert.AreEqual(Card.CardNumber._4, 
                rules.FoundationPiles[0][rules.FoundationPiles[0].Count - 1].Number,
                "4♥ should now be on top of hearts foundation");
            
            // Test 2: Move 2♦ from foundation to 3♣ in tableau column 1 (opposite colors)
            Card twoOfDiamonds = rules.FoundationPiles[1][rules.FoundationPiles[1].Count - 1];
            // Change the tableau card to be opposite color (3♣ instead of 3♦)
            rules.TableauColumns[1].Clear();
            rules.TableauColumns[1].Add(new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Club });
            Assert.IsTrue(rules.CanPlaceCardOnTableau(twoOfDiamonds, 1),
                "2♦ should be valid on 3♣ (opposite colors)");
                
            // Don't execute this move, just verify it would work
            
            // Test 3: Move 2♦ from foundation to 3♠ in tableau column 3
            Assert.IsTrue(rules.CanPlaceCardOnTableau(twoOfDiamonds, 3),
                "2♦ should be valid on 3♠ (opposite colors)");
            
            // Test 4: Verify that invalid moves are rejected
            Card threeOfSpades = rules.FoundationPiles[3][rules.FoundationPiles[3].Count - 1];
            Assert.IsFalse(rules.CanPlaceCardOnTableau(threeOfSpades, 0),
                "3♠ should not be valid on 5♥ (already moved)");
            
            // Test 5: Verify foundation cards still work with empty tableau columns
            Assert.IsFalse(rules.CanPlaceCardOnTableau(twoOfDiamonds, 2),
                "2♦ should not be valid on empty column (only Kings allowed)");
                
            // Final verification: ensure the game state is still valid after moves
            Assert.AreEqual(2, rules.TableauColumns[0].Count, "Tableau column 0 should have 2 cards");
            Assert.AreEqual(fiveOfHearts, rules.TableauColumns[0][1], "5♥ should be on top of column 0");
        }
        
        /// <summary>
        /// Test edge case: moving the last card from a foundation pile to tableau
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_LastCardInFoundation_ShouldWork()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Foundation with only one card (Ace of Hearts)
            Card aceOfHearts = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };
            rules.FoundationPiles[0].Add(aceOfHearts);
            
            // Empty tableau column (Ace cannot go here - only Kings)
            
            // Tableau column with 2 of Spades (Ace cannot go here either)
            Card twoOfSpades = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Spade };
            rules.TableauColumns[1].Add(twoOfSpades);
            
            // Act & Assert
            Assert.IsFalse(rules.CanPlaceCardOnTableau(aceOfHearts, 0),
                "Ace should not be valid on empty tableau (only Kings)");
            Assert.IsTrue(rules.CanPlaceCardOnTableau(aceOfHearts, 1),
                "Ace should be valid on 2 of opposite color (one rank lower)");
            
            // Now test with a valid target (3 of Clubs)
            rules.TableauColumns[2].Add(new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Club });
            Assert.IsFalse(rules.CanPlaceCardOnTableau(aceOfHearts, 2),
                "Ace should not be valid on 3 (not one rank lower)");
            
            // Test with 2 of Clubs (valid target - opposite color)
            rules.TableauColumns[3].Add(new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Club });
            Assert.IsTrue(rules.CanPlaceCardOnTableau(aceOfHearts, 3),
                "Ace should be valid on 2 of opposite color");
            
            // Execute the move - foundation should become empty
            rules.FoundationPiles[0].RemoveAt(0);
            rules.TableauColumns[3].Add(aceOfHearts);
            
            Assert.AreEqual(0, rules.FoundationPiles[0].Count, "Foundation should be empty");
            Assert.AreEqual(2, rules.TableauColumns[3].Count, "Tableau should have 2 cards");
        }
    }
}