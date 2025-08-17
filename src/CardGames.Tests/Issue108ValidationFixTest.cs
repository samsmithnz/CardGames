using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    /// <summary>
    /// Test to validate the fix for issue #108 - first empty tableau column validation
    /// These tests ensure that the UI validation logic specifically handles the first empty tableau column correctly
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class Issue108ValidationFixTest
    {
        [TestMethod]
        public void Issue108Fix_FirstEmptyTableauColumn_KingValidation()
        {
            // Arrange - Test the specific scenario from issue #108
            SolitaireRules rules = new SolitaireRules();
            
            // Create different kings to test
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card kingOfClubs = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Club };
            Card kingOfDiamonds = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Diamond };
            
            Card[] kings = { kingOfSpades, kingOfHearts, kingOfClubs, kingOfDiamonds };
            
            // Test each king on the first empty tableau column specifically
            foreach (Card king in kings)
            {
                // Clear all tableau columns
                for (int col = 0; col < 7; col++)
                {
                    rules.TableauColumns[col].Clear();
                }
                
                // Focus specifically on the FIRST tableau column (index 0)
                int firstColumn = 0;
                
                // Act
                bool canPlaceOnFirst = rules.CanPlaceCardOnTableau(king, firstColumn);
                
                // Assert
                Assert.IsTrue(canPlaceOnFirst, 
                    $"Issue #108 Fix: {king.Number} of {king.Suite}s should be valid on the FIRST empty tableau column");
                Assert.AreEqual(0, rules.TableauColumns[firstColumn].Count, 
                    "First tableau column should be empty");
            }
        }
        
        [TestMethod]
        public void Issue108Fix_FirstEmptyTableauColumn_NonKingRejection()
        {
            // Arrange - Ensure non-Kings are still properly rejected on first empty column
            SolitaireRules rules = new SolitaireRules();
            
            Card queenOfSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };
            Card jackOfHearts = new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Heart };
            Card aceOfClubs = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Club };
            Card tenOfDiamonds = new Card { Number = Card.CardNumber._10, Suite = Card.CardSuite.Diamond };
            
            Card[] nonKings = { queenOfSpades, jackOfHearts, aceOfClubs, tenOfDiamonds };
            
            // Clear all tableau columns
            for (int col = 0; col < 7; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            int firstColumn = 0;
            
            // Test each non-King card
            foreach (Card card in nonKings)
            {
                // Act
                bool canPlaceOnFirst = rules.CanPlaceCardOnTableau(card, firstColumn);
                
                // Assert
                Assert.IsFalse(canPlaceOnFirst, 
                    $"Issue #108 Fix: {card.Number} of {card.Suite}s should NOT be valid on the first empty tableau column");
            }
        }
        
        [TestMethod]
        public void Issue108Fix_CompareBehaviorAcrossAllColumns()
        {
            // Arrange - Ensure the fix doesn't create inconsistencies between columns
            SolitaireRules rules = new SolitaireRules();
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };
            Card queenOfSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };
            
            // Test all 7 tableau columns when empty
            for (int targetCol = 0; targetCol < 7; targetCol++)
            {
                // Clear all columns
                for (int col = 0; col < 7; col++)
                {
                    rules.TableauColumns[col].Clear();
                }
                
                // Act
                bool kingCanPlace = rules.CanPlaceCardOnTableau(kingOfHearts, targetCol);
                bool queenCanPlace = rules.CanPlaceCardOnTableau(queenOfSpades, targetCol);
                
                // Assert
                Assert.IsTrue(kingCanPlace, 
                    $"Issue #108 Fix: King should be valid on empty column {targetCol} (consistency check)");
                Assert.IsFalse(queenCanPlace, 
                    $"Issue #108 Fix: Queen should NOT be valid on empty column {targetCol} (consistency check)");
                    
                // Special attention to the first column
                if (targetCol == 0)
                {
                    Assert.IsTrue(kingCanPlace, 
                        "CRITICAL: King should be valid on first empty tableau column - this is the main issue #108 fix");
                }
            }
        }
        
        [TestMethod]
        public void Issue108Fix_EmptyFirstColumnWithCardsInOtherColumns()
        {
            // Arrange - Test the specific scenario where only the first column is empty
            SolitaireRules rules = new SolitaireRules();
            Card kingOfDiamonds = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Diamond };
            
            // Clear all columns first
            for (int col = 0; col < 7; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            
            // Add some cards to other columns (not the first)
            rules.TableauColumns[1].Add(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade });
            rules.TableauColumns[2].Add(new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart });
            rules.TableauColumns[3].Add(new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Club });
            
            // Ensure first column is empty
            int firstColumn = 0;
            Assert.AreEqual(0, rules.TableauColumns[firstColumn].Count, "First column should be empty");
            
            // Act
            bool canPlaceOnFirst = rules.CanPlaceCardOnTableau(kingOfDiamonds, firstColumn);
            
            // Assert
            Assert.IsTrue(canPlaceOnFirst, 
                "Issue #108 Fix: King should be valid on first empty tableau column even when other columns have cards");
        }
    }
}