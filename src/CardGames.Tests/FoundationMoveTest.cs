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
}
}