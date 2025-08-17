using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Test to verify that foundation to tableau moves are fully functional
    /// This test simulates a complete scenario where the user would want to move a card back from foundation to tableau
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class FoundationToTableauIntegrationTest
    {
        /// <summary>
        /// Test a realistic game scenario where moving a card from foundation back to tableau is beneficial
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_StrategicMove_ShouldWork()
        {
            // Arrange - Set up a realistic game state where moving from foundation to tableau makes sense
            SolitaireRules rules = new SolitaireRules();
            
            // Foundation has been built up: A♠, 2♠, 3♠
            Card aceOfSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
            Card twoOfSpades = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Spade };
            Card threeOfSpades = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Spade };
            
            rules.FoundationPiles[3].Add(aceOfSpades);
            rules.FoundationPiles[3].Add(twoOfSpades);
            rules.FoundationPiles[3].Add(threeOfSpades);
            
            // Tableau has a 4♥ that could accept the 3♠
            Card fourOfHearts = new Card { Number = Card.CardNumber._4, Suite = Card.CardSuite.Heart };
            rules.TableauColumns[2].Add(fourOfHearts);
            
            // Another tableau column has face-down cards with 2♥ revealed on top 
            // Player wants to move 3♠ from foundation to 4♥ to then place 2♥ on 3♠
            Card twoOfHearts = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Heart };
            rules.TableauColumns[5].Add(twoOfHearts);
            
            // Act - Test the strategic move: 3♠ from foundation to 4♥ in tableau
            
            // 1. Validate that 3♠ can be placed on 4♥
            bool canPlace = rules.CanPlaceCardOnTableau(threeOfSpades, 2);
            Assert.IsTrue(canPlace, "3 of spades should be valid to place on 4 of hearts (one rank lower, opposite color)");
            
            // 2. Execute the move
            Card removedCard = rules.FoundationPiles[3][rules.FoundationPiles[3].Count - 1];
            Assert.AreEqual(threeOfSpades, removedCard, "Top card of foundation should be 3 of spades");
            
            rules.FoundationPiles[3].RemoveAt(rules.FoundationPiles[3].Count - 1);
            rules.TableauColumns[2].Add(threeOfSpades);
            
            // Assert - Verify the move was successful
            Assert.AreEqual(2, rules.FoundationPiles[3].Count, "Foundation should have 2 cards remaining");
            Assert.AreEqual(twoOfSpades, rules.FoundationPiles[3][rules.FoundationPiles[3].Count - 1], 
                "2 of spades should now be on top of foundation");
            
            Assert.AreEqual(2, rules.TableauColumns[2].Count, "Tableau should have 2 cards");
            Assert.AreEqual(fourOfHearts, rules.TableauColumns[2][0], "4 of hearts should be at bottom");
            Assert.AreEqual(threeOfSpades, rules.TableauColumns[2][1], "3 of spades should be on top");
            
            // 3. Now test the follow-up move: 2♥ onto 3♠
            bool canPlaceFollowUp = rules.CanPlaceCardOnTableau(twoOfHearts, 2);
            Assert.IsTrue(canPlaceFollowUp, "2 of hearts should be valid to place on 3 of spades");
            
            // This demonstrates why foundation-to-tableau moves are strategically important in Solitaire
        }
        
        /// <summary>
        /// Test various valid foundation to tableau moves to ensure comprehensive coverage
        /// </summary>
        [TestMethod]
        public void FoundationToTableau_VariousValidMoves_ShouldAllWork()
        {
            // Test multiple suit combinations and ranks
            
            // Test 1: Hearts (red) on Clubs (black)
            SolitaireRules rules1 = new SolitaireRules();
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card kingOfClubs = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Club };
            
            rules1.FoundationPiles[0].Add(queenOfHearts);
            rules1.TableauColumns[0].Add(kingOfClubs);
            
            Assert.IsTrue(rules1.CanPlaceCardOnTableau(queenOfHearts, 0), 
                "Queen of hearts should be valid on King of clubs");
            
            // Test 2: Spades (black) on Diamonds (red)
            SolitaireRules rules2 = new SolitaireRules();
            Card jackOfSpades = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Spade };
            Card queenOfDiamonds = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Diamond };
            
            rules2.FoundationPiles[3].Add(jackOfSpades);
            rules2.TableauColumns[1].Add(queenOfDiamonds);
            
            Assert.IsTrue(rules2.CanPlaceCardOnTableau(jackOfSpades, 1), 
                "Jack of spades should be valid on Queen of diamonds");
            
            // Test 3: Clubs (black) on Hearts (red)
            SolitaireRules rules3 = new SolitaireRules();
            Card tenOfClubs = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Club };
            Card jackOfHearts = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Heart };
            
            rules3.FoundationPiles[1].Add(tenOfClubs);
            rules3.TableauColumns[3].Add(jackOfHearts);
            
            Assert.IsTrue(rules3.CanPlaceCardOnTableau(tenOfClubs, 3), 
                "10 of clubs should be valid on Jack of hearts");
            
            // Test 4: Diamonds (red) on Spades (black)
            SolitaireRules rules4 = new SolitaireRules();
            Card nineOfDiamonds = new Card { Number = Card.CardNumber._9, Suite = Card.CardSuite.Diamond };
            Card tenOfSpades = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Spade };
            
            rules4.FoundationPiles[2].Add(nineOfDiamonds);
            rules4.TableauColumns[4].Add(tenOfSpades);
            
            Assert.IsTrue(rules4.CanPlaceCardOnTableau(nineOfDiamonds, 4), 
                "9 of diamonds should be valid on 10 of spades");
        }
    }
}