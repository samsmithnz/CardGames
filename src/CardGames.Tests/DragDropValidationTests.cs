using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests for drag and drop validation logic
    /// </summary>
    [TestClass]
    public class DragDropValidationTests
    {
        /// <summary>
        /// Test that validates basic solitaire-like move rules
        /// </summary>
        [TestMethod]
        public void ValidateMove_KingOnEmptySpace_ShouldReturnTrue()
        {
            // Arrange
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card targetCard = null; // Empty space

            // Act
            bool result = IsValidMove(kingOfHearts, targetCard);

            // Assert
            Assert.IsTrue(result, "King should be allowed on empty space");
        }

        [TestMethod]
        public void ValidateMove_NonKingOnEmptySpace_ShouldReturnFalse()
        {
            // Arrange
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card targetCard = null; // Empty space

            // Act
            bool result = IsValidMove(queenOfHearts, targetCard);

            // Assert
            Assert.IsFalse(result, "Only King should be allowed on empty space");
        }

        [TestMethod]
        public void ValidateMove_RedOnBlack_OneRankLower_ShouldReturnTrue()
        {
            // Arrange
            Card redSix = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Heart };
            Card blackSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Spade };

            // Act
            bool result = IsValidMove(redSix, blackSeven);

            // Assert
            Assert.IsTrue(result, "Red 6 should be allowed on Black 7");
        }

        [TestMethod]
        public void ValidateMove_BlackOnRed_OneRankLower_ShouldReturnTrue()
        {
            // Arrange
            Card blackJack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };
            Card redQueen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Diamond };

            // Act
            bool result = IsValidMove(blackJack, redQueen);

            // Assert
            Assert.IsTrue(result, "Black Jack should be allowed on Red Queen");
        }

        [TestMethod]
        public void ValidateMove_SameColor_ShouldReturnFalse()
        {
            // Arrange
            Card redSix = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Heart };
            Card redSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Diamond };

            // Act
            bool result = IsValidMove(redSix, redSeven);

            // Assert
            Assert.IsFalse(result, "Same color cards should not be allowed");
        }

        [TestMethod]
        public void ValidateMove_WrongRank_ShouldReturnFalse()
        {
            // Arrange
            Card blackFive = new Card { Number = Card.CardNumber._5, Suite = Card.CardSuite.Spade };
            Card redSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Heart };

            // Act
            bool result = IsValidMove(blackFive, redSeven);

            // Assert
            Assert.IsFalse(result, "Cards not one rank apart should not be allowed");
        }

        [TestMethod]
        public void ValidateMoveDetailed_EmptySpace_NonKing_ShouldReturnErrorMessage()
        {
            // Arrange
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card targetCard = null; // Empty space

            // Act
            string result = ValidateMoveDetailed(queenOfHearts, targetCard);

            // Assert
            Assert.AreEqual("Only Kings can be placed on empty spaces", result);
        }

        [TestMethod]
        public void ValidateMoveDetailed_WrongRank_ShouldReturnErrorMessage()
        {
            // Arrange
            Card blackFive = new Card { Number = Card.CardNumber._5, Suite = Card.CardSuite.Spade };
            Card redSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Heart };

            // Act
            string result = ValidateMoveDetailed(blackFive, redSeven);

            // Assert
            Assert.AreEqual("_5 cannot be placed on _7 - must be one rank lower", result);
        }

        [TestMethod]
        public void ValidateMoveDetailed_SameColor_ShouldReturnErrorMessage()
        {
            // Arrange
            Card redSix = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Heart };
            Card redSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Diamond };

            // Act
            string result = ValidateMoveDetailed(redSix, redSeven);

            // Assert
            Assert.AreEqual("Red _6 cannot be placed on Red _7 - must be opposite color", result);
        }

        [TestMethod]
        public void ValidateMoveDetailed_ValidMove_ShouldReturnValid()
        {
            // Arrange
            Card blackSix = new Card { Number = Card.CardNumber._6, Suite = Card.CardSuite.Spade };
            Card redSeven = new Card { Number = Card.CardNumber._7, Suite = Card.CardSuite.Heart };

            // Act
            string result = ValidateMoveDetailed(blackSix, redSeven);

            // Assert
            Assert.AreEqual("Valid", result);
        }

        /// <summary>
        /// Helper method that implements the detailed validation logic from MainWindow
        /// </summary>
        private string ValidateMoveDetailed(Card card, Card targetCard)
        {
            // For demonstration purposes, implement basic solitaire-like rules
            if (targetCard == null)
            {
                // Empty space - only allow Kings
                if (card.Number == Card.CardNumber.K)
                {
                    return "Valid";
                }
                else
                {
                    return "Only Kings can be placed on empty spaces";
                }
            }
            
            // Check rank
            if (!IsOneRankLower(card.Number, targetCard.Number))
            {
                return $"{card.Number} cannot be placed on {targetCard.Number} - must be one rank lower";
            }
            
            // Check color
            if (!IsOppositeColor(card, targetCard))
            {
                return $"{GetColorName(card)} {card.Number} cannot be placed on {GetColorName(targetCard)} {targetCard.Number} - must be opposite color";
            }
            
            return "Valid";
        }

        private string GetColorName(Card card)
        {
            bool isRed = card.Suite == Card.CardSuite.Heart || card.Suite == Card.CardSuite.Diamond;
            return isRed ? "Red" : "Black";
        }

        /// <summary>
        /// Helper method that implements the same logic as in MainWindow
        /// </summary>
        private bool IsValidMove(Card card, Card targetCard)
        {
            // For demonstration purposes, implement basic solitaire-like rules
            if (targetCard == null)
            {
                // Empty space - only allow Kings
                return card.Number == Card.CardNumber.K;
            }
            
            // Check if card is one rank lower and opposite color
            return IsOneRankLower(card.Number, targetCard.Number) && IsOppositeColor(card, targetCard);
        }

        private bool IsOneRankLower(Card.CardNumber lower, Card.CardNumber higher)
        {
            return (int)lower == (int)higher - 1;
        }

        private bool IsOppositeColor(Card card1, Card card2)
        {
            bool card1IsRed = card1.Suite == Card.CardSuite.Heart || card1.Suite == Card.CardSuite.Diamond;
            bool card2IsRed = card2.Suite == Card.CardSuite.Heart || card2.Suite == Card.CardSuite.Diamond;
            return card1IsRed != card2IsRed;
        }
    }
}