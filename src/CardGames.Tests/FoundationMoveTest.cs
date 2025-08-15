using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
public class FoundationMoveTest
{
    [TestMethod]
    public void TestAceCanMoveToEmptyFoundation()
    {
        // Arrange
        SolitaireRules rules = new SolitaireRules();
        Card aceSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
        
        // Act
        int foundationIndex = rules.FindAvailableFoundationPile(aceSpades);
        
        // Assert
        Assert.AreEqual(3, foundationIndex); // Spade should map to foundation 3
        Assert.IsTrue(rules.CanPlaceCardOnFoundation(aceSpades, foundationIndex));
    }
    
    [TestMethod]
    public void TestSequenceOfMovesToFoundation()
    {
        // Arrange
        SolitaireRules rules = new SolitaireRules();
        Card aceSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
        Card twoSpades = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Spade };
        
        // Act & Assert - First move ace to foundation
        int foundationIndex = rules.FindAvailableFoundationPile(aceSpades);
        Assert.AreEqual(3, foundationIndex);
        rules.FoundationPiles[foundationIndex].Add(aceSpades);
        
        // Now try to move two of spades
        foundationIndex = rules.FindAvailableFoundationPile(twoSpades);
        Assert.AreEqual(3, foundationIndex);
        Assert.IsTrue(rules.CanPlaceCardOnFoundation(twoSpades, foundationIndex));
    }

    [TestMethod]
    public void TestAceOnTopOfTableauCanMoveToFoundation()
    {
        // Arrange - Simulate user's scenario: Ace of Spades on top of a tableau column
        SolitaireRules rules = new SolitaireRules();
        Card aceSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
        Card otherCard = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
        
        // Add some cards to a tableau column, with Ace of Spades on top
        rules.TableauColumns[0].Add(otherCard);
        rules.TableauColumns[0].Add(aceSpades);
        
        // Act - Check if Ace can move to foundation
        int foundationIndex = rules.FindAvailableFoundationPile(aceSpades);
        
        // Assert
        Assert.AreEqual(3, foundationIndex, "Ace of Spades should be able to move to foundation pile 3");
        Assert.IsTrue(rules.CanPlaceCardOnFoundation(aceSpades, foundationIndex), "Ace should be placeable on empty foundation");
    }

    [TestMethod]
    public void TestEntireAutoMoveFlow()
    {
        // Arrange - Simulate the complete auto-move flow
        SolitaireRules rules = new SolitaireRules();
        Card aceSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
        Card kingHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
        
        // Set up tableau column with Ace on top
        rules.TableauColumns[0].Add(kingHearts);
        rules.TableauColumns[0].Add(aceSpades);
        
        // Verify initial state
        Assert.AreEqual(2, rules.TableauColumns[0].Count, "Tableau should have 2 cards initially");
        Assert.AreEqual(0, rules.FoundationPiles[3].Count, "Foundation should be empty initially");
        
        // Act - Simulate the complete auto-move process
        // 1. User clicks on Ace of Spades (top card of tableau column 0)
        Card clickedCard = rules.TableauColumns[0][rules.TableauColumns[0].Count - 1];
        Assert.AreEqual(aceSpades, clickedCard, "Clicked card should be Ace of Spades");
        
        // 2. System finds available foundation pile
        int foundationIndex = rules.FindAvailableFoundationPile(clickedCard);
        Assert.AreEqual(3, foundationIndex, "Should find Spades foundation (index 3)");
        
        // 3. System validates the move
        Assert.IsTrue(rules.CanPlaceCardOnFoundation(clickedCard, foundationIndex), "Move should be valid");
        
        // 4. System removes card from tableau (equivalent to RemoveCardFromSource)
        Card removedCard = rules.TableauColumns[0][rules.TableauColumns[0].Count - 1];
        Assert.AreEqual(aceSpades, removedCard, "Top card should match clicked card");
        rules.TableauColumns[0].RemoveAt(rules.TableauColumns[0].Count - 1);
        
        // 5. System adds card to foundation (equivalent to ExecuteMove)
        rules.FoundationPiles[foundationIndex].Add(clickedCard);
        
        // Assert final state
        Assert.AreEqual(1, rules.TableauColumns[0].Count, "Tableau should have 1 card after move");
        Assert.AreEqual(kingHearts, rules.TableauColumns[0][0], "King should now be on top of tableau");
        Assert.AreEqual(1, rules.FoundationPiles[3].Count, "Foundation should have 1 card after move");
        Assert.AreEqual(aceSpades, rules.FoundationPiles[3][0], "Foundation should contain Ace of Spades");
    }
}
}