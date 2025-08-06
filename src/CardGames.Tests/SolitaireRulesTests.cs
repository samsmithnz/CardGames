using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SolitaireRulesTests
    {
        [TestMethod]
        public void CreateSolitaireRulesTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();

            //Act

            //Assert
            Assert.AreEqual(7, rules.TableauColumns.Count);
            Assert.AreEqual(4, rules.FoundationPiles.Count);
            Assert.AreEqual(0, rules.StockPile.Count);
            Assert.AreEqual(0, rules.WastePile.Count);

            // Check that all collections are initially empty
            foreach (var column in rules.TableauColumns)
            {
                Assert.AreEqual(0, column.Count);
            }
            foreach (var foundation in rules.FoundationPiles)
            {
                Assert.AreEqual(0, foundation.Count);
            }
        }

        [TestMethod]
        public void DealCardsTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();

            //Act
            rules.DealCards(deck);

            //Assert
            // Check tableau columns have correct number of cards
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(i + 1, rules.TableauColumns[i].Count);
            }

            // Check stock pile has remaining cards (52 - 28 = 24)
            int expectedStockCount = 52 - (1 + 2 + 3 + 4 + 5 + 6 + 7);
            Assert.AreEqual(expectedStockCount, rules.StockPile.Count);

            // Check waste pile is empty
            Assert.AreEqual(0, rules.WastePile.Count);

            // Check foundations are empty
            foreach (var foundation in rules.FoundationPiles)
            {
                Assert.AreEqual(0, foundation.Count);
            }
        }

        [TestMethod]
        public void DealCardsWithNullDeckTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();

            //Act & Assert
            Assert.ThrowsException<ArgumentException>(() => rules.DealCards(null));
        }

        [TestMethod]
        public void CanPlaceCardOnTableauEmptyColumnTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Card king = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };

            //Act & Assert
            // Only Kings can be placed on empty tableau columns
            Assert.IsTrue(rules.CanPlaceCardOnTableau(king, 0));
            Assert.IsFalse(rules.CanPlaceCardOnTableau(queen, 0));
        }

        [TestMethod]
        public void CanPlaceCardOnTableauDescendingAlternatingTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Card redKing = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card blackQueen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };
            Card redQueen = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Diamond };
            Card blackJack = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club };

            // Set up a column with a red King
            rules.TableauColumns[0].Add(redKing);

            //Act & Assert
            // Black Queen can be placed on red King
            Assert.IsTrue(rules.CanPlaceCardOnTableau(blackQueen, 0));
            
            // Red Queen cannot be placed on red King (same color)
            Assert.IsFalse(rules.CanPlaceCardOnTableau(redQueen, 0));
            
            // Black Jack cannot be placed on red King (wrong rank)
            Assert.IsFalse(rules.CanPlaceCardOnTableau(blackJack, 0));
        }

        [TestMethod]
        public void CanPlaceCardOnFoundationEmptyTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Card ace = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };
            Card two = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Heart };

            //Act & Assert
            // Only Aces can be placed on empty foundation piles
            Assert.IsTrue(rules.CanPlaceCardOnFoundation(ace, 0));
            Assert.IsFalse(rules.CanPlaceCardOnFoundation(two, 0));
        }

        [TestMethod]
        public void CanPlaceCardOnFoundationAscendingSameSuitTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Card aceHearts = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };
            Card twoHearts = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Heart };
            Card twoSpades = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Spade };
            Card threeHearts = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Heart };

            // Set up a foundation with Ace of Hearts
            rules.FoundationPiles[0].Add(aceHearts);

            //Act & Assert
            // Two of Hearts can be placed on Ace of Hearts
            Assert.IsTrue(rules.CanPlaceCardOnFoundation(twoHearts, 0));
            
            // Two of Spades cannot be placed on Ace of Hearts (different suit)
            Assert.IsFalse(rules.CanPlaceCardOnFoundation(twoSpades, 0));
            
            // Three of Hearts cannot be placed on Ace of Hearts (wrong rank)
            Assert.IsFalse(rules.CanPlaceCardOnFoundation(threeHearts, 0));
        }

        [TestMethod]
        public void IsGameWonTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();

            //Act & Assert - Initial state should not be won
            Assert.IsFalse(rules.IsGameWon());

            // Fill all foundation piles with 13 cards each to simulate winning
            for (int foundationIndex = 0; foundationIndex < 4; foundationIndex++)
            {
                for (int cardValue = 0; cardValue < 13; cardValue++)
                {
                    Card card = new Card 
                    { 
                        Number = (Card.CardNumber)cardValue, 
                        Suite = (Card.CardSuite)foundationIndex 
                    };
                    rules.FoundationPiles[foundationIndex].Add(card);
                }
            }

            // Now the game should be won
            Assert.IsTrue(rules.IsGameWon());
        }

        [TestMethod]
        public void DrawFromStockTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Deck deck = new Deck();
            rules.DealCards(deck);

            int initialStockCount = rules.StockPile.Count;
            int initialWasteCount = rules.WastePile.Count;

            //Act
            bool result = rules.DrawFromStock();

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(initialStockCount - 1, rules.StockPile.Count);
            Assert.AreEqual(initialWasteCount + 1, rules.WastePile.Count);
        }

        [TestMethod]
        public void DrawFromEmptyStockTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            // Stock pile is empty by default

            //Act
            bool result = rules.DrawFromStock();

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, rules.StockPile.Count);
            Assert.AreEqual(0, rules.WastePile.Count);
        }

        [TestMethod]
        public void ResetStockTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Card card1 = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };
            Card card2 = new Card { Number = Card.CardNumber._2, Suite = Card.CardSuite.Spade };
            Card card3 = new Card { Number = Card.CardNumber._3, Suite = Card.CardSuite.Diamond };

            // Set up waste pile with some cards and empty stock
            rules.WastePile.Add(card1);
            rules.WastePile.Add(card2);
            rules.WastePile.Add(card3);

            //Act
            rules.ResetStock();

            //Assert
            Assert.AreEqual(3, rules.StockPile.Count);
            Assert.AreEqual(0, rules.WastePile.Count);
            
            // Cards should be in reverse order in stock
            Assert.AreEqual(card3, rules.StockPile[0]);
            Assert.AreEqual(card2, rules.StockPile[1]);
            Assert.AreEqual(card1, rules.StockPile[2]);
        }

        [TestMethod]
        public void InvalidTableauColumnIndexTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Card card = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };

            //Act & Assert
            Assert.IsFalse(rules.CanPlaceCardOnTableau(card, -1));
            Assert.IsFalse(rules.CanPlaceCardOnTableau(card, 7));
            Assert.IsFalse(rules.CanPlaceCardOnTableau(null, 0));
        }

        [TestMethod]
        public void InvalidFoundationIndexTest()
        {
            //Arrange
            SolitaireRules rules = new SolitaireRules();
            Card card = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart };

            //Act & Assert
            Assert.IsFalse(rules.CanPlaceCardOnFoundation(card, -1));
            Assert.IsFalse(rules.CanPlaceCardOnFoundation(card, 4));
            Assert.IsFalse(rules.CanPlaceCardOnFoundation(null, 0));
        }

        // Drag-and-drop validation tests
        [TestMethod]
        public void CanPlaceOnPlayingArea_KingOnEmptyPile_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card kingOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.K };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(kingOfHearts, null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnPlayingArea_NonKingOnEmptyPile_ReturnsFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card queenOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.Q };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(queenOfHearts, null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanPlaceOnPlayingArea_RedOnBlackDescending_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card redJack = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.J };
            Card blackQueen = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber.Q };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(redJack, blackQueen);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnPlayingArea_SameColorCards_ReturnsFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card redJack = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.J };
            Card redQueen = new Card { Suite = Card.CardSuite.Diamond, Number = Card.CardNumber.Q };

            // Act
            bool result = rules.CanPlaceOnPlayingArea(redJack, redQueen);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanPlaceOnFoundation_AceOnEmptyPile_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card aceOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };

            // Act
            bool result = rules.CanPlaceOnFoundation(aceOfHearts, null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnFoundation_TwoOnAceSameSuit_ReturnsTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card twoOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber._2 };
            Card aceOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };

            // Act
            bool result = rules.CanPlaceOnFoundation(twoOfHearts, aceOfHearts);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPlaceOnFoundation_DifferentSuit_ReturnsFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();
            Card twoOfSpades = new Card { Suite = Card.CardSuite.Spade, Number = Card.CardNumber._2 };
            Card aceOfHearts = new Card { Suite = Card.CardSuite.Heart, Number = Card.CardNumber.A };

            // Act
            bool result = rules.CanPlaceOnFoundation(twoOfSpades, aceOfHearts);

            // Assert
            Assert.IsFalse(result);
        }
    }
}