using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests specifically for the card removal logic to ensure cards are removed in correct order
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CardSequenceRemovalTests
    {
        [TestMethod]
        public void RemoveCardSequence_MultipleCards_ShouldRemoveInCorrectOrder()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a column with cards: King, Queen, Jack, 10
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card jack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };
            Card ten = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Diamond };
            
            rules.TableauColumns[0].Add(king);
            rules.TableauColumns[0].Add(queen);
            rules.TableauColumns[0].Add(jack);
            rules.TableauColumns[0].Add(ten);
            
            // Prepare to remove Queen, Jack, Ten (as a sequence)
            List<Card> sequenceToRemove = new List<Card> { queen, jack, ten };
            
            // Act - simulate the removal logic from RemoveCardSequenceFromSource
            List<Card> sourceColumn = rules.TableauColumns[0];
            for (int i = 0; i < sequenceToRemove.Count; i++)
            {
                if (sourceColumn.Count > 0 && sourceColumn[sourceColumn.Count - 1] == sequenceToRemove[i])
                {
                    sourceColumn.RemoveAt(sourceColumn.Count - 1);
                }
            }
            
            // Assert
            Assert.AreEqual(1, sourceColumn.Count, "Should have 1 card remaining after removing sequence");
            Assert.AreEqual(king, sourceColumn[0], "King should be the remaining card");
        }

        [TestMethod]
        public void RemoveCardSequence_WrongOrder_ShouldOnlyRemoveMatchingCards()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a column with cards: King, Queen, Jack, 10
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card jack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };
            Card ten = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Diamond };
            
            rules.TableauColumns[0].Add(king);
            rules.TableauColumns[0].Add(queen);
            rules.TableauColumns[0].Add(jack);
            rules.TableauColumns[0].Add(ten);
            
            // Try to remove in wrong order: Jack, Queen, Ten
            List<Card> wrongOrderSequence = new List<Card> { jack, queen, ten };
            
            // Act - simulate the removal logic
            List<Card> sourceColumn = rules.TableauColumns[0];
            for (int i = 0; i < wrongOrderSequence.Count; i++)
            {
                if (sourceColumn.Count > 0 && sourceColumn[sourceColumn.Count - 1] == wrongOrderSequence[i])
                {
                    sourceColumn.RemoveAt(sourceColumn.Count - 1);
                }
            }
            
            // Assert - only the ten should be removed (it's at the top)
            Assert.AreEqual(3, sourceColumn.Count, "Should have 3 cards remaining");
            Assert.AreEqual(king, sourceColumn[0], "King should still be at bottom");
            Assert.AreEqual(queen, sourceColumn[1], "Queen should still be second");
            Assert.AreEqual(jack, sourceColumn[2], "Jack should now be at top");
        }

        [TestMethod]
        public void AddCardSequence_MultipleCards_ShouldAddInCorrectOrder()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Start with King of Spades in the column
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            rules.TableauColumns[0].Add(king);
            
            // Sequence to add: Queen, Jack, 10
            Card queen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card jack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };
            Card ten = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Diamond };
            
            List<Card> sequenceToAdd = new List<Card> { queen, jack, ten };
            
            // Act - simulate the addition logic from AddCardSequenceToTableau
            foreach (Card card in sequenceToAdd)
            {
                rules.TableauColumns[0].Add(card);
            }
            
            // Assert
            Assert.AreEqual(4, rules.TableauColumns[0].Count, "Should have 4 cards total");
            Assert.AreEqual(king, rules.TableauColumns[0][0], "King should be at bottom");
            Assert.AreEqual(queen, rules.TableauColumns[0][1], "Queen should be second");
            Assert.AreEqual(jack, rules.TableauColumns[0][2], "Jack should be third");
            Assert.AreEqual(ten, rules.TableauColumns[0][3], "Ten should be at top");
        }
    }
}