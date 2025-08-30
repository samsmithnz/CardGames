using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;
using System.Collections.Generic;

namespace CardGames.Tests
{
    /// <summary>
    /// Integration tests to verify FreeCell sequence move constraints work correctly in real game scenarios
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class FreeCellSequenceMoveIntegrationTests
    {
        [TestMethod]
        public void FreeCellGame_WithLimitedFreeSpace_ShouldRestrictLargeSequenceMoves()
        {
            // Arrange - Create a FreeCell game with limited free space
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Fill 3 out of 4 free cells (leaving 1 empty)
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, 0);
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade }, 1);
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Diamond }, 2);
            
            // Fill all tableau columns except one (leaving 1 empty)
            for (int i = 1; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            rules.TableauColumns[0].Clear(); // Keep column 0 empty
            
            // Act & Assert
            // With 1 free cell and 1 empty column: max = 2^1 × (1+1) = 4 cards
            Assert.IsTrue(rules.CanMoveCardSequence(1), "1 card should be allowed");
            Assert.IsTrue(rules.CanMoveCardSequence(4), "4 cards should be allowed at the limit");
            Assert.IsFalse(rules.CanMoveCardSequence(5), "5 cards should not be allowed - exceeds limit");
            
            // Moving to empty column should reduce max to 2
            Assert.IsTrue(rules.CanMoveCardSequence(2, 0), "2 cards should be allowed to empty column");
            Assert.IsFalse(rules.CanMoveCardSequence(3, 0), "3 cards should not be allowed to empty column");
        }

        [TestMethod]
        public void FreeCellGame_WithMaximumFreeSpace_ShouldAllowLargeSequenceMoves()
        {
            // Arrange - Create a FreeCell game with maximum free space
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Keep all free cells empty (4 available)
            
            // Clear multiple tableau columns
            rules.TableauColumns[0].Clear();
            rules.TableauColumns[1].Clear();
            rules.TableauColumns[2].Clear(); // 3 empty columns
            
            // Ensure remaining columns have cards
            for (int i = 3; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Act & Assert
            // With 4 free cells and 3 empty columns: max = 2^3 × (4+1) = 8 × 5 = 40 cards
            Assert.IsTrue(rules.CanMoveCardSequence(13), "13 cards (full suit) should be allowed");
            Assert.IsTrue(rules.CanMoveCardSequence(20), "20 cards should be allowed");
            Assert.IsTrue(rules.CanMoveCardSequence(40), "40 cards should be allowed at the limit");
            
            // Moving to empty column should allow 20 cards (40 / 2)
            Assert.IsTrue(rules.CanMoveCardSequence(20, 0), "20 cards should be allowed to empty column");
            Assert.IsFalse(rules.CanMoveCardSequence(21, 0), "21 cards should not be allowed to empty column");
        }

        [TestMethod]
        public void FreeCellGame_ProgressiveFreeSpaceReduction_ShouldUpdateLimitsCorrectly()
        {
            // Arrange - Start with maximum free space and progressively reduce it
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Clear all tableau columns and keep all free cells empty
            for (int i = 0; i < rules.TableauColumns.Count; i++)
            {
                rules.TableauColumns[i].Clear();
            }
            
            // Act & Assert - Test progressive reduction
            
            // Step 1: All free (4 free cells, 8 empty columns): max = 2^8 × 5 = 1280
            int maxMove1 = rules.CalculateMaxSequenceMoveSize();
            Assert.AreEqual(1280, maxMove1, "Should allow 1280 cards with maximum free space");
            Assert.IsTrue(rules.CanMoveCardSequence(52), "Should allow moving entire deck");
            
            // Step 2: Fill one free cell (3 free cells, 8 empty columns): max = 2^8 × 4 = 1024
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, 0);
            int maxMove2 = rules.CalculateMaxSequenceMoveSize();
            Assert.AreEqual(1024, maxMove2, "Should reduce to 1024 with one less free cell");
            
            // Step 3: Fill one column (3 free cells, 7 empty columns): max = 2^7 × 4 = 512
            rules.TableauColumns[0].Add(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade });
            int maxMove3 = rules.CalculateMaxSequenceMoveSize();
            Assert.AreEqual(512, maxMove3, "Should reduce to 512 with one less empty column");
            
            // Step 4: Test minimum case (0 free cells, 0 empty columns): max = 2^0 × 1 = 1
            for (int i = 1; i < 4; i++)
            {
                rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, i);
            }
            for (int i = 1; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            int maxMove4 = rules.CalculateMaxSequenceMoveSize();
            Assert.AreEqual(1, maxMove4, "Should be 1 with no free space");
            Assert.IsTrue(rules.CanMoveCardSequence(1), "Should still allow single card moves");
            Assert.IsFalse(rules.CanMoveCardSequence(2), "Should not allow multi-card moves with no free space");
        }

        [TestMethod]
        public void KlondikeGame_ShouldNotBeAffectedByFreeCellConstraints()
        {
            // Arrange - Create a Klondike game (non-FreeCell)
            SolitaireRules rules = new SolitaireRules("Klondike Solitaire");
            
            // Fill all free cells (if any) and tableau columns to simulate worst case
            for (int i = 0; i < rules.FreeCells.Count && i < 4; i++)
            {
                if (rules.CanPlaceCardInFreeCell(i))
                {
                    rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, i);
                }
            }
            
            // Act & Assert - Klondike should not be restricted by FreeCell rules
            Assert.IsTrue(rules.CanMoveCardSequence(1), "Single card should be allowed");
            Assert.IsTrue(rules.CanMoveCardSequence(13), "13 cards should be allowed in Klondike");
            Assert.IsTrue(rules.CanMoveCardSequence(26), "26 cards should be allowed in Klondike");
            Assert.IsTrue(rules.CanMoveCardSequence(52), "Even unrealistic sequences should be allowed in Klondike");
            
            // Moving to empty column should also not be restricted
            Assert.IsTrue(rules.CanMoveCardSequence(52, 0), "Large sequences to empty columns should be allowed in Klondike");
        }

        [TestMethod]
        public void FreeCellGame_RealWorldScenario_MovingValidSequence()
        {
            // Arrange - Set up a realistic FreeCell game scenario
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Set up a scenario with 2 empty free cells and 1 empty tableau column
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, 0);
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade }, 1);
            // Free cells 2 and 3 remain empty
            
            // Clear one tableau column, fill others
            rules.TableauColumns[0].Clear(); // Empty column
            for (int i = 1; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            // Create a valid sequence in column 1: King(Red) -> Queen(Black) -> Jack(Red) -> 10(Black)
            rules.TableauColumns[1].Clear();
            rules.TableauColumns[1].Add(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart });   // Red King
            rules.TableauColumns[1].Add(new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade });    // Black Queen
            rules.TableauColumns[1].Add(new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Diamond });  // Red Jack
            rules.TableauColumns[1].Add(new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Club });   // Black 10
            
            // Act & Assert
            // With 2 free cells and 1 empty column: max = 2^1 × (2+1) = 6 cards
            Assert.AreEqual(6, rules.CalculateMaxSequenceMoveSize(), "Should calculate 6 as maximum sequence size");
            
            // Test moving the 4-card sequence from column 1
            Assert.IsTrue(rules.CanMoveCardSequence(4, 2), "Should allow moving 4-card sequence to non-empty column");
            Assert.IsTrue(rules.CanMoveCardSequence(3, 0), "Should allow moving 3-card sequence to empty column (6/2=3)");
            Assert.IsFalse(rules.CanMoveCardSequence(4, 0), "Should not allow moving 4-card sequence to empty column");
            
            // Verify single card placement is not affected
            Card blackTen = rules.TableauColumns[1][3]; // Black 10 at the top
            Card redNine = new Card { Number = Card.CardNumber._9, Suite = Card.CardSuite.Heart }; // Red 9 should be valid on Black 10
            Assert.IsTrue(rules.CanPlaceCardOnTableau(redNine, 1), "Basic card placement rules should still work - Red 9 on Black 10");
        }
    }
}