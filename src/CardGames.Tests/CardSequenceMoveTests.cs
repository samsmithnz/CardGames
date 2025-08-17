using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests for card sequence movement functionality
    /// This validates that when dragging a card from a tableau column,
    /// all face-up cards below it move as a sequence
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class CardSequenceMoveTests
    {
        [TestMethod]
        public void CardSequenceMove_SingleCard_ShouldMoveOnlyThatCard()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a column with just one card
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            rules.TableauColumns[0].Add(king);
            
            // Set up face-up states (only the king is face-up)
            List<List<bool>> faceUpStates = new List<List<bool>>();
            for (int col = 0; col < 7; col++)
            {
                faceUpStates.Add(new List<bool>());
            }
            faceUpStates[0].Add(true); // King is face-up
            
            // Act - simulate detecting the sequence to move
            List<Card> sequence = GetCardSequenceToMove(rules, faceUpStates, 0, king);
            
            // Assert
            Assert.AreEqual(1, sequence.Count, "Single card should result in sequence of 1");
            Assert.AreEqual(king, sequence[0], "Sequence should contain the king");
        }

        [TestMethod]
        public void CardSequenceMove_ValidSequence_ShouldMoveAllCardsInSequence()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a column with a valid sequence: King of Spades, Queen of Hearts, Jack of Clubs
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card jack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };
            
            rules.TableauColumns[0].Add(king);
            rules.TableauColumns[0].Add(queen);
            rules.TableauColumns[0].Add(jack);
            
            // Set up face-up states (all cards are face-up)
            List<List<bool>> faceUpStates = new List<List<bool>>();
            for (int col = 0; col < 7; col++)
            {
                faceUpStates.Add(new List<bool>());
            }
            faceUpStates[0].Add(true); // King is face-up
            faceUpStates[0].Add(true); // Queen is face-up
            faceUpStates[0].Add(true); // Jack is face-up
            
            // Act - simulate dragging the queen (middle card)
            List<Card> sequence = GetCardSequenceToMove(rules, faceUpStates, 0, queen);
            
            // Assert
            Assert.AreEqual(2, sequence.Count, "Dragging queen should move queen and jack");
            Assert.AreEqual(queen, sequence[0], "First card in sequence should be the queen");
            Assert.AreEqual(jack, sequence[1], "Second card in sequence should be the jack");
        }

        [TestMethod]
        public void CardSequenceMove_PartialFaceDownCards_ShouldMoveOnlyFaceUpPortion()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a column with mixed face-up/face-down cards
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card jack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };
            Card ten = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Diamond };
            
            rules.TableauColumns[0].Add(king);
            rules.TableauColumns[0].Add(queen);
            rules.TableauColumns[0].Add(jack);
            rules.TableauColumns[0].Add(ten);
            
            // Set up face-up states (king face-down, others face-up)
            List<List<bool>> faceUpStates = new List<List<bool>>();
            for (int col = 0; col < 7; col++)
            {
                faceUpStates.Add(new List<bool>());
            }
            faceUpStates[0].Add(false); // King is face-down
            faceUpStates[0].Add(true);  // Queen is face-up
            faceUpStates[0].Add(true);  // Jack is face-up
            faceUpStates[0].Add(true);  // Ten is face-up
            
            // Act - simulate dragging the queen
            List<Card> sequence = GetCardSequenceToMove(rules, faceUpStates, 0, queen);
            
            // Assert
            Assert.AreEqual(3, sequence.Count, "Should move queen, jack, and ten");
            Assert.AreEqual(queen, sequence[0], "First card should be queen");
            Assert.AreEqual(jack, sequence[1], "Second card should be jack");
            Assert.AreEqual(ten, sequence[2], "Third card should be ten");
        }

        [TestMethod]
        public void CardSequenceMove_InvalidSequence_ShouldMoveOnlySingleCard()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            
            // Set up a column with an invalid sequence (same color cards)
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Club }; // Same color as king
            Card jack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Heart };
            
            rules.TableauColumns[0].Add(king);
            rules.TableauColumns[0].Add(queen);
            rules.TableauColumns[0].Add(jack);
            
            // Set up face-up states (all face-up)
            List<List<bool>> faceUpStates = new List<List<bool>>();
            for (int col = 0; col < 7; col++)
            {
                faceUpStates.Add(new List<bool>());
            }
            faceUpStates[0].Add(true); // King is face-up
            faceUpStates[0].Add(true); // Queen is face-up
            faceUpStates[0].Add(true); // Jack is face-up
            
            // Act - simulate dragging the king
            List<Card> sequence = GetCardSequenceToMove(rules, faceUpStates, 0, king);
            
            // Assert
            Assert.AreEqual(1, sequence.Count, "Invalid sequence should result in single card move");
            Assert.AreEqual(king, sequence[0], "Should only move the king");
        }

        /// <summary>
        /// Helper method to simulate the GetCardSequenceToMove logic from MainWindow
        /// This replicates the logic without requiring the full UI
        /// </summary>
        private List<Card> GetCardSequenceToMove(SolitaireRules rules, List<List<bool>> faceUpStates, int columnIndex, Card draggedCard)
        {
            List<Card> sourceColumn = rules.TableauColumns[columnIndex];
            List<Card> sequence = new List<Card>();

            // Find the position of the dragged card in the column
            int draggedCardIndex = -1;
            for (int i = 0; i < sourceColumn.Count; i++)
            {
                if (sourceColumn[i] == draggedCard)
                {
                    draggedCardIndex = i;
                    break;
                }
            }

            if (draggedCardIndex < 0)
            {
                // Card not found in column - return single card
                return new List<Card> { draggedCard };
            }

            // Check if all cards from the dragged card to the end are face-up and form a valid sequence
            bool isValidSequence = true;
            for (int i = draggedCardIndex; i < sourceColumn.Count; i++)
            {
                // Check if card is face-up
                if (i >= faceUpStates[columnIndex].Count || !faceUpStates[columnIndex][i])
                {
                    isValidSequence = false;
                    break;
                }

                sequence.Add(sourceColumn[i]);

                // Check if this card can be placed on the previous card in the sequence (descending order, alternating colors)
                if (i > draggedCardIndex)
                {
                    Card previousCard = sourceColumn[i - 1];
                    Card currentCard = sourceColumn[i];
                    
                    if (!IsOneRankLower(currentCard.Number, previousCard.Number) || !IsOppositeColor(currentCard, previousCard))
                    {
                        isValidSequence = false;
                        break;
                    }
                }
            }

            if (!isValidSequence)
            {
                // Not a valid sequence - return single card
                return new List<Card> { draggedCard };
            }

            return sequence;
        }

        private bool IsOneRankLower(Card.CardNumber lower, Card.CardNumber higher)
        {
            return (int)lower == (int)higher - 1;
        }

        private bool IsOppositeColor(Card card1, Card card2)
        {
            bool card1IsRed = (card1.Suite == Card.CardSuite.Heart || card1.Suite == Card.CardSuite.Diamond);
            bool card2IsRed = (card2.Suite == Card.CardSuite.Heart || card2.Suite == Card.CardSuite.Diamond);
            
            return card1IsRed != card2IsRed;
        }
    }
}