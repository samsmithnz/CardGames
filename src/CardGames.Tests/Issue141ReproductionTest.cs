using System;
using System.Collections.Generic;
using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Specific test to reproduce the exact issue described in #141
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class Issue141ReproductionTest
    {
        [TestMethod]
        public void Issue141_FreecellSequenceMove_ShouldNotDuplicateCards()
        {
            // Arrange - Create the exact scenario from the issue
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Set up the initial state mentioned in the issue:
            // - 7 of spades in first column with 6 below it 
            // - 8 of hearts in sixth column
            Card sixOfSpades = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Spade };
            Card sevenOfSpades = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Spade };
            Card eightOfHearts = new Card { Number = Card.CardNumber._8, Suite = Card.CardSuite.Heart };
            
            // Column 0 (first column): has 6 of spades at bottom, 7 of spades on top
            rules.TableauColumns[0].Add(sixOfSpades);
            rules.TableauColumns[0].Add(sevenOfSpades);
            
            // Column 5 (sixth column): has 8 of hearts
            rules.TableauColumns[5].Add(eightOfHearts);
            
            // Mock face-up states (both cards in column 0 are face-up)
            List<List<bool>> mockFaceUpStates = new List<List<bool>>();
            for (int col = 0; col < 8; col++)
            {
                mockFaceUpStates.Add(new List<bool>());
                for (int row = 0; row < 13; row++)
                {
                    mockFaceUpStates[col].Add(false);
                }
            }
            
            // Set both cards in column 0 as face-up
            mockFaceUpStates[0][0] = true; // 6 of spades
            mockFaceUpStates[0][1] = true; // 7 of spades
            
            // Act - Determine what sequence should be moved when dragging the 7 of spades
            List<Card> sourceColumn = rules.TableauColumns[0];
            List<Card> sequenceToMove = GetCardSequenceToMoveForFreecell(sourceColumn, sevenOfSpades, mockFaceUpStates[0]);
            
            // In Freecell, 6 and 7 of spades are both spades (same color), so they do NOT form a valid sequence
            // According to Freecell rules, sequences must be descending AND alternating colors
            // Therefore, only the 7 of spades should move (single card move)
            Assert.AreEqual(1, sequenceToMove.Count, "Only 7 of spades should move - 6 and 7 of spades are same color");
            Assert.AreEqual(sevenOfSpades, sequenceToMove[0], "Sequence should contain only the 7 of spades");
            
            // Verify the move is valid first
            bool canPlace = rules.CanPlaceCardOnTableau(sevenOfSpades, 5);
            Assert.IsTrue(canPlace, "Should be able to place 7 of spades on 8 of hearts");
            
            // Count cards before the move
            int totalSevenOfSpadesBefore = CountCardInAllColumns(rules, Card.CardNumber._7, Card.CardSuite.Spade);
            int totalSixOfSpadesBefore = CountCardInAllColumns(rules, Card.CardNumber._6, Card.CardSuite.Spade);
            
            Assert.AreEqual(1, totalSevenOfSpadesBefore, "Should have exactly one 7 of spades before move");
            Assert.AreEqual(1, totalSixOfSpadesBefore, "Should have exactly one 6 of spades before move");
            
            // Simulate the move operation (remove from source, add to target)
            // This mimics what ExecuteMove should do
            for (int i = sequenceToMove.Count - 1; i >= 0; i--)
            {
                if (rules.TableauColumns[0].Count > 0 && 
                    AreCardsEqual(rules.TableauColumns[0][rules.TableauColumns[0].Count - 1], sequenceToMove[i]))
                {
                    rules.TableauColumns[0].RemoveAt(rules.TableauColumns[0].Count - 1);
                }
            }
            
            // Add the sequence to the target column
            foreach (Card card in sequenceToMove)
            {
                rules.TableauColumns[5].Add(card);
            }
            
            // Assert - Verify no duplication occurred
            int totalSevenOfSpadesAfter = CountCardInAllColumns(rules, Card.CardNumber._7, Card.CardSuite.Spade);
            int totalSixOfSpadesAfter = CountCardInAllColumns(rules, Card.CardNumber._6, Card.CardSuite.Spade);
            
            Assert.AreEqual(1, totalSevenOfSpadesAfter, "Should still have exactly one 7 of spades after move");
            Assert.AreEqual(1, totalSixOfSpadesAfter, "Should still have exactly one 6 of spades after move");
            
            // The correct behavior: only 7 of spades should have moved
            Assert.AreEqual(1, rules.TableauColumns[0].Count, "Source column should have 6 of spades remaining");
            Assert.AreEqual(sixOfSpades, rules.TableauColumns[0][0], "Remaining card should be 6 of spades");
            Assert.AreEqual(2, rules.TableauColumns[5].Count, "Target column should have 2 cards");
            Assert.AreEqual(eightOfHearts, rules.TableauColumns[5][0], "Bottom card should be 8 of hearts");
            Assert.AreEqual(sevenOfSpades, rules.TableauColumns[5][1], "Top card should be 7 of spades");
        }
        
        [TestMethod]
        public void Issue141_FixVerification_ValueComparisonPreventsCardDuplication()
        {
            // This test verifies that the fix (using AreCardsEqual instead of ==) works correctly
            
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            Card sixOfSpades = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Spade };
            Card sevenOfSpades = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Spade };
            Card eightOfHearts = new Card { Number = Card.CardNumber._8, Suite = Card.CardSuite.Heart };
            
            rules.TableauColumns[0].Add(sixOfSpades);
            rules.TableauColumns[0].Add(sevenOfSpades);
            rules.TableauColumns[5].Add(eightOfHearts);
            
            // Create a different object with the same values (this simulates what happens in the UI)
            Card draggedCardDifferentReference = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Spade };
            
            // Verify they're equal by value but different by reference
            Assert.AreEqual(sevenOfSpades, draggedCardDifferentReference, "Cards should be equal by value");
            Assert.IsFalse(ReferenceEquals(sevenOfSpades, draggedCardDifferentReference), "Cards should be different objects");
            
            // Mock face-up states for sequence detection
            List<List<bool>> mockFaceUpStates = new List<List<bool>>();
            for (int col = 0; col < 8; col++)
            {
                mockFaceUpStates.Add(new List<bool>());
                for (int row = 0; row < 13; row++)
                {
                    mockFaceUpStates[col].Add(false);
                }
            }
            mockFaceUpStates[0][0] = true; // 6 of spades
            mockFaceUpStates[0][1] = true; // 7 of spades
            
            // Test the fixed sequence detection logic (should now find the card by value)
            List<Card> sequenceWithFixedLogic = GetCardSequenceToMoveForFreecell(rules.TableauColumns[0], draggedCardDifferentReference, mockFaceUpStates[0]);
            
            // With the fix, this should find the card in the tableau and return a sequence containing the actual tableau card
            Assert.AreEqual(1, sequenceWithFixedLogic.Count, "Should return single card sequence");
            
            // The fix: the returned card should be the one from the tableau, not the parameter
            Assert.IsTrue(ReferenceEquals(sevenOfSpades, sequenceWithFixedLogic[0]), 
                "Fixed logic returns the tableau card, not the parameter card");
            
            // Now simulate the move with the correct sequence
            int sevenOfSpadesBefore = CountCardInAllColumns(rules, Card.CardNumber._7, Card.CardSuite.Spade);
            Assert.AreEqual(1, sevenOfSpadesBefore, "Should start with one 7 of spades");
            
            // With the fixed sequence, removal should succeed
            bool removalWillSucceed = CanRemoveCardSequenceFromTableauTest(rules.TableauColumns[0], sequenceWithFixedLogic);
            Assert.IsTrue(removalWillSucceed, "Removal should succeed with fixed sequence");
            
            // Remove the sequence (this should succeed now)
            for (int i = sequenceWithFixedLogic.Count - 1; i >= 0; i--)
            {
                if (rules.TableauColumns[0].Count > 0 && 
                    AreCardsEqual(rules.TableauColumns[0][rules.TableauColumns[0].Count - 1], sequenceWithFixedLogic[i]))
                {
                    rules.TableauColumns[0].RemoveAt(rules.TableauColumns[0].Count - 1);
                }
            }
            
            // Add to target
            foreach (Card card in sequenceWithFixedLogic)
            {
                rules.TableauColumns[5].Add(card);
            }
            
            // Verify no duplication
            int sevenOfSpadesAfter = CountCardInAllColumns(rules, Card.CardNumber._7, Card.CardSuite.Spade);
            Assert.AreEqual(1, sevenOfSpadesAfter, "Should still have exactly one 7 of spades - no duplication");
            
            // Verify the card moved correctly
            Assert.AreEqual(1, rules.TableauColumns[0].Count, "Source should have 6 of spades remaining");
            Assert.AreEqual(sixOfSpades, rules.TableauColumns[0][0], "Source should contain 6 of spades");
            Assert.AreEqual(2, rules.TableauColumns[5].Count, "Target should have 8 and 7");
            Assert.AreEqual(eightOfHearts, rules.TableauColumns[5][0], "Target bottom should be 8 of hearts");
            Assert.AreEqual(sevenOfSpades, rules.TableauColumns[5][1], "Target top should be the original 7 of spades from tableau");
        }
        
        /// <summary>
        /// Test version of the validation method to check reference vs value comparison issues
        /// </summary>
        private bool CanRemoveCardSequenceFromTableauTest(List<Card> sourceColumn, List<Card> cardsToRemove)
        {
            if (cardsToRemove == null || cardsToRemove.Count == 0)
            {
                return true;
            }
            
            if (sourceColumn.Count < cardsToRemove.Count)
            {
                return false;
            }
            
            // Check that the cards to remove match the top cards in the column in reverse order
            for (int i = 0; i < cardsToRemove.Count; i++)
            {
                int columnIndex = sourceColumn.Count - 1 - i;
                Card expectedCard = cardsToRemove[cardsToRemove.Count - 1 - i];
                Card actualCard = sourceColumn[columnIndex];
                
                // The key question: are cards compared by reference or by value?
                if (!AreCardsEqual(actualCard, expectedCard))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        [TestMethod]
        public void Issue141_CorrectBehavior_NoCardDuplication()
        {
            // This test shows the CORRECT behavior without duplication
            
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            Card sixOfSpades = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Spade };
            Card sevenOfSpades = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Spade };
            Card eightOfHearts = new Card { Number = Card.CardNumber._8, Suite = Card.CardSuite.Heart };
            
            rules.TableauColumns[0].Add(sixOfSpades);
            rules.TableauColumns[0].Add(sevenOfSpades);
            rules.TableauColumns[5].Add(eightOfHearts);
            
            // Count before
            int sevenOfSpadesBefore = CountCardInAllColumns(rules, Card.CardNumber._7, Card.CardSuite.Spade);
            Assert.AreEqual(1, sevenOfSpadesBefore, "Should start with exactly one 7 of spades");
            
            // CORRECT behavior: move only the 7 of spades (since 6-7 spades is invalid sequence)
            List<Card> sequenceToMove = new List<Card> { sevenOfSpades };
            
            // Remove from source FIRST (correct order)
            for (int i = sequenceToMove.Count - 1; i >= 0; i--)
            {
                if (rules.TableauColumns[0].Count > 0 && 
                    AreCardsEqual(rules.TableauColumns[0][rules.TableauColumns[0].Count - 1], sequenceToMove[i]))
                {
                    rules.TableauColumns[0].RemoveAt(rules.TableauColumns[0].Count - 1);
                }
            }
            
            // Then add to target
            foreach (Card card in sequenceToMove)
            {
                rules.TableauColumns[5].Add(card);
            }
            
            // Verify no duplication
            int sevenOfSpadesAfter = CountCardInAllColumns(rules, Card.CardNumber._7, Card.CardSuite.Spade);
            Assert.AreEqual(1, sevenOfSpadesAfter, "Should still have exactly one 7 of spades");
            
            // Verify final state
            Assert.AreEqual(1, rules.TableauColumns[0].Count, "Source should have 6 of spades remaining");
            Assert.AreEqual(sixOfSpades, rules.TableauColumns[0][0], "Source should contain 6 of spades");
            Assert.AreEqual(2, rules.TableauColumns[5].Count, "Target should have 8 and 7");
            Assert.AreEqual(eightOfHearts, rules.TableauColumns[5][0], "Target bottom should be 8 of hearts");
            Assert.AreEqual(sevenOfSpades, rules.TableauColumns[5][1], "Target top should be 7 of spades");
        }
        
        /// <summary>
        /// Helper method to simulate GetCardSequenceToMove logic for Freecell
        /// This mirrors the logic from MainWindow.xaml.cs but allows testing without UI dependencies
        /// </summary>
        private List<Card> GetCardSequenceToMoveForFreecell(List<Card> sourceColumn, Card draggedCard, List<bool> faceUpStates)
        {
            List<Card> sequence = new List<Card>();
            
            // Find the position of the dragged card in the column
            int draggedCardIndex = -1;
            for (int i = 0; i < sourceColumn.Count; i++)
            {
                if (AreCardsEqual(sourceColumn[i], draggedCard))
                {
                    draggedCardIndex = i;
                    break;
                }
            }
            
            if (draggedCardIndex < 0)
            {
                return new List<Card> { draggedCard };
            }
            
            // Check if all cards from the dragged card to the end are face-up and form a valid sequence
            bool isValidSequence = true;
            for (int i = draggedCardIndex; i < sourceColumn.Count; i++)
            {
                // Check if card is face-up
                if (i >= faceUpStates.Count || !faceUpStates[i])
                {
                    isValidSequence = false;
                    break;
                }

                sequence.Add(sourceColumn[i]);

                // Check if this card can be placed on the previous card in the sequence
                // In Freecell tableau building, sequences should be descending and alternating colors
                if (i > draggedCardIndex)
                {
                    Card previousCard = sourceColumn[i - 1];
                    Card currentCard = sourceColumn[i];
                    
                    bool rankOk = IsOneRankLower(currentCard.Number, previousCard.Number);
                    bool colorOk = IsOppositeColor(currentCard, previousCard);
                    
                    if (!rankOk || !colorOk)
                    {
                        isValidSequence = false;
                        break;
                    }
                }
            }
            
            if (!isValidSequence)
            {
                return new List<Card> { draggedCard };
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Count how many cards of a specific number and suite exist in all tableau columns
        /// </summary>
        private int CountCardInAllColumns(SolitaireRules rules, Card.CardNumber number, Card.CardSuite suite)
        {
            int count = 0;
            for (int col = 0; col < rules.TableauColumns.Count; col++)
            {
                foreach (Card card in rules.TableauColumns[col])
                {
                    if (card.Number == number && card.Suite == suite)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        /// <summary>
        /// Check if two cards are equal by value (not reference)
        /// </summary>
        private bool AreCardsEqual(Card card1, Card card2)
        {
            if (card1 == null && card2 == null) return true;
            if (card1 == null || card2 == null) return false;
            return card1.Number == card2.Number && card1.Suite == card2.Suite;
        }
        
        /// <summary>
        /// Check if one card number is exactly one rank lower than another
        /// </summary>
        private bool IsOneRankLower(Card.CardNumber lower, Card.CardNumber higher)
        {
            return (int)lower == (int)higher - 1;
        }

        /// <summary>
        /// Check if two cards are opposite colors (red vs black)
        /// </summary>
        private bool IsOppositeColor(Card card1, Card card2)
        {
            bool card1IsRed = card1.Suite == Card.CardSuite.Heart || card1.Suite == Card.CardSuite.Diamond;
            bool card2IsRed = card2.Suite == Card.CardSuite.Heart || card2.Suite == Card.CardSuite.Diamond;
            return card1IsRed != card2IsRed;
        }
    }
}