using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests for FreeCell sequence move validation based on available free cells and empty tableau columns
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class FreeCellSequenceMoveTests
    {
        [TestMethod]
        public void GetEmptyTableauColumnCount_WithAllColumnsHavingCards_ShouldReturnZero()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Ensure all tableau columns have at least one card
            for (int i = 0; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act
            int emptyCount = rules.GetEmptyTableauColumnCount();
            
            // Assert
            Assert.AreEqual(0, emptyCount, "Should return 0 when no tableau columns are empty");
        }

        [TestMethod]
        public void GetEmptyTableauColumnCount_WithSomeEmptyColumns_ShouldReturnCorrectCount()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Clear first two columns
            rules.TableauColumns[0].Clear();
            rules.TableauColumns[1].Clear();
            
            // Ensure other columns have cards
            for (int i = 2; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act
            int emptyCount = rules.GetEmptyTableauColumnCount();
            
            // Assert
            Assert.AreEqual(2, emptyCount, "Should return 2 when two tableau columns are empty");
        }

        [TestMethod]
        public void CalculateMaxSequenceMoveSize_WithNoEmptySpaces_ShouldReturnOne()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Fill all free cells
            for (int i = 0; i < 4; i++)
            {
                rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, i);
            }
            
            // Ensure all tableau columns have cards
            for (int i = 0; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act
            int maxSize = rules.CalculateMaxSequenceMoveSize();
            
            // Assert
            // Formula: C = 2^0 × (0+1) = 1 × 1 = 1
            Assert.AreEqual(1, maxSize, "Should return 1 when no free cells or empty columns available");
        }

        [TestMethod]
        public void CalculateMaxSequenceMoveSize_WithTwoFreeCellsOneEmptyColumn_ShouldReturnSix()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Fill 2 free cells, leave 2 empty
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, 0);
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade }, 1);
            
            // Clear one tableau column
            rules.TableauColumns[0].Clear();
            
            // Ensure other columns have cards
            for (int i = 1; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act
            int maxSize = rules.CalculateMaxSequenceMoveSize();
            
            // Assert
            // Formula: C = 2^1 × (2+1) = 2 × 3 = 6
            Assert.AreEqual(6, maxSize, "Should return 6 with 2 empty free cells and 1 empty tableau column");
        }

        [TestMethod]
        public void CalculateMaxSequenceMoveSize_WithAllFreeCellsAndTwoEmptyColumns_ShouldReturnTwenty()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Keep all free cells empty
            
            // Clear two tableau columns
            rules.TableauColumns[0].Clear();
            rules.TableauColumns[1].Clear();
            
            // Ensure other columns have cards
            for (int i = 2; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act
            int maxSize = rules.CalculateMaxSequenceMoveSize();
            
            // Assert
            // Formula: C = 2^2 × (4+1) = 4 × 5 = 20
            Assert.AreEqual(20, maxSize, "Should return 20 with 4 empty free cells and 2 empty tableau columns");
        }

        [TestMethod]
        public void CanMoveCardSequence_ForNonFreeCellGame_ShouldAlwaysReturnTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Klondike Solitaire");
            
            // Act & Assert
            Assert.IsTrue(rules.CanMoveCardSequence(1), "Single card should be allowed");
            Assert.IsTrue(rules.CanMoveCardSequence(5), "Multiple cards should be allowed for non-FreeCell games");
            Assert.IsTrue(rules.CanMoveCardSequence(13), "Large sequences should be allowed for non-FreeCell games");
        }

        [TestMethod]
        public void CanMoveCardSequence_ForFreeCellWithSufficientSpace_ShouldReturnTrue()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Keep all free cells empty and clear one tableau column for max space
            rules.TableauColumns[0].Clear();
            
            // Act & Assert
            // With 4 free cells and 1 empty column: max = 2^1 × (4+1) = 10
            Assert.IsTrue(rules.CanMoveCardSequence(1), "Single card should always be allowed");
            Assert.IsTrue(rules.CanMoveCardSequence(5), "5 cards should be allowed with sufficient space");
            Assert.IsTrue(rules.CanMoveCardSequence(10), "10 cards should be allowed at the limit");
        }

        [TestMethod]
        public void CanMoveCardSequence_ForFreeCellWithInsufficientSpace_ShouldReturnFalse()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Fill all free cells and ensure all columns have cards (minimal space)
            for (int i = 0; i < 4; i++)
            {
                rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, i);
            }
            for (int i = 0; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act & Assert
            // With 0 free cells and 0 empty columns: max = 2^0 × (0+1) = 1
            Assert.IsTrue(rules.CanMoveCardSequence(1), "Single card should always be allowed");
            Assert.IsFalse(rules.CanMoveCardSequence(2), "2 cards should not be allowed with no free space");
            Assert.IsFalse(rules.CanMoveCardSequence(5), "5 cards should not be allowed with no free space");
        }

        [TestMethod]
        public void CanMoveCardSequence_ToEmptyTableauColumn_ShouldReduceMaxByHalf()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Set up: 2 empty free cells, 1 empty tableau column
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, 0);
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade }, 1);
            rules.TableauColumns[0].Clear(); // This is the target column
            
            // Ensure other columns have cards
            for (int i = 1; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act & Assert
            // Normal max: C = 2^1 × (2+1) = 6
            // To empty column: max = 6 / 2 = 3
            Assert.IsTrue(rules.CanMoveCardSequence(3, 0), "3 cards should be allowed to empty column");
            Assert.IsFalse(rules.CanMoveCardSequence(4, 0), "4 cards should not be allowed to empty column");
            
            // To non-empty column should use full calculation
            Assert.IsTrue(rules.CanMoveCardSequence(6, 1), "6 cards should be allowed to non-empty column");
        }
    }
}