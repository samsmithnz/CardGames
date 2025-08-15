using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class AutoMoveSimulationTest
    {
        [TestMethod]
        public void SimulateUserScenario_AceOfSpadesOnTableauTop()
        {
            // Arrange - Create a game state like the user described
            SolitaireRules rules = new SolitaireRules();
            
            // Create cards: some base cards and an Ace of Spades on top
            Card kingHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card aceSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
            
            // Set up tableau column 0 with King of Hearts at bottom, Ace of Spades on top
            rules.TableauColumns[0].Add(kingHearts);
            rules.TableauColumns[0].Add(aceSpades);
            
            // Verify initial state
            Assert.AreEqual(2, rules.TableauColumns[0].Count);
            Assert.AreEqual(aceSpades, rules.TableauColumns[0][1], "Ace should be on top");
            Assert.AreEqual(0, rules.FoundationPiles[3].Count, "Spades foundation should be empty");
            
            // Act - Simulate what should happen when user clicks Ace of Spades
            int foundationIndex = rules.FindAvailableFoundationPile(aceSpades);
            
            // Assert foundation logic works
            Assert.AreEqual(3, foundationIndex, "Ace of Spades should target foundation 3");
            Assert.IsTrue(rules.CanPlaceCardOnFoundation(aceSpades, foundationIndex), "Move should be valid");
            
            // Act - Simulate the actual move
            Card topCard = rules.TableauColumns[0][rules.TableauColumns[0].Count - 1];
            Assert.AreEqual(aceSpades, topCard, "Top card should be Ace of Spades");
            
            // Remove from tableau and add to foundation (like ExecuteMove would do)
            rules.TableauColumns[0].RemoveAt(rules.TableauColumns[0].Count - 1);
            rules.FoundationPiles[foundationIndex].Add(aceSpades);
            
            // Assert final state
            Assert.AreEqual(1, rules.TableauColumns[0].Count, "Tableau should have one less card");
            Assert.AreEqual(kingHearts, rules.TableauColumns[0][0], "King should now be on top");
            Assert.AreEqual(1, rules.FoundationPiles[3].Count, "Spades foundation should have one card");
            Assert.AreEqual(aceSpades, rules.FoundationPiles[3][0], "Foundation should contain the Ace");
        }
    }
}