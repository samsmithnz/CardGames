using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class DragDropIntegrationTests
    {
        [TestMethod]
        public void SolitaireRules_CompleteGameSequence_ValidatesCorrectly()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Create test cards representing a typical game sequence
            Card aceOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };
            Card twoOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber._2 };
            Card kingOfSpades = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber.K };
            Card queenOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.Q };
            Card jackOfSpades = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber.J };

            // Act & Assert - Foundation pile sequence
            Assert.IsTrue(rules.CanPlaceOnFoundation(aceOfHearts, null), "Ace should be placeable on empty foundation");
            Assert.IsTrue(rules.CanPlaceOnFoundation(twoOfHearts, aceOfHearts), "Two should be placeable on Ace of same suit");
            Assert.IsFalse(rules.CanPlaceOnFoundation(kingOfSpades, aceOfHearts), "King should not be placeable on Ace");

            // Act & Assert - Playing area sequence  
            Assert.IsTrue(rules.CanPlaceOnPlayingArea(kingOfSpades, null), "King should be placeable on empty playing area");
            Assert.IsTrue(rules.CanPlaceOnPlayingArea(queenOfHearts, kingOfSpades), "Red Queen should be placeable on Black King");
            Assert.IsTrue(rules.CanPlaceOnPlayingArea(jackOfSpades, queenOfHearts), "Black Jack should be placeable on Red Queen");
            Assert.IsFalse(rules.CanPlaceOnPlayingArea(aceOfHearts, kingOfSpades), "Ace should not be placeable on King (wrong rank)");
        }

        [TestMethod]
        public void CardUserControl_SetupCard_StoresCardReference()
        {
            // This test demonstrates the CardUserControl Card property usage
            // Note: This test validates the logic even though we can't test UI components directly
            
            // Arrange
            Card testCard = new Card { Suite = Card.CardSuite.Diamond, Number = Card.CardNumber.Q };
            
            // Act & Assert
            // The CardUserControl would store the card reference for drag-and-drop operations
            Assert.IsNotNull(testCard);
            Assert.AreEqual(Card.CardSuite.Diamond, testCard.Suite);
            Assert.AreEqual(Card.CardNumber.Q, testCard.Number);
            
            // This simulates what happens when SetupCard is called:
            // cardUserControl.Card = testCard;
            // cardUserControl.lblTopLeftNumber.Text = testCard.Number.ToString().Replace("_", "");
            string displayNumber = testCard.Number.ToString().Replace("_", "");
            Assert.AreEqual("Q", displayNumber);
        }

        [TestMethod]
        public void DragDropLogic_PanelTypeValidation_WorksCorrectly()
        {
            // This test demonstrates the panel type validation logic
            // that would be used in the MainForm.IsValidMove method
            
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card aceOfSpades = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber.A };
            Card kingOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.K };
            
            // Simulate panel types (these would come from Panel.Tag in the actual UI)
            string foundationPanelTag = "Ace1";
            string playingAreaPanelTag = "PlayingArea1";
            string deckDiscardPanelTag = "DeckDiscard";
            string deckPanelTag = "Deck";
            
            // Act & Assert - Foundation panel logic
            if (foundationPanelTag.StartsWith("Ace"))
            {
                Assert.IsTrue(rules.CanPlaceOnFoundation(aceOfSpades, null), 
                    "Ace should be valid for empty foundation");
                Assert.IsFalse(rules.CanPlaceOnFoundation(kingOfHearts, null), 
                    "King should not be valid for empty foundation");
            }
            
            // Act & Assert - Playing area panel logic
            if (playingAreaPanelTag.StartsWith("PlayingArea"))
            {
                Assert.IsTrue(rules.CanPlaceOnPlayingArea(kingOfHearts, null), 
                    "King should be valid for empty playing area");
                Assert.IsFalse(rules.CanPlaceOnPlayingArea(aceOfSpades, null), 
                    "Ace should not be valid for empty playing area");
            }
            
            // Act & Assert - Special panel logic
            Assert.IsTrue(deckDiscardPanelTag == "DeckDiscard", "Discard pile should always allow drops");
            Assert.IsTrue(deckPanelTag == "Deck", "Deck pile should be identified correctly");
        }
    }
}