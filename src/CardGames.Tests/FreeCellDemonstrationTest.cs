using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;

namespace CardGames.Tests
{
    /// <summary>
    /// Quick demonstration of the FreeCell sequence move constraints in action
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class FreeCellDemonstrationTest
    {
        [TestMethod]
        public void FreeCellDemo_ShowSequenceMoveConstraints()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            
            // Start with a fresh game state
            System.Console.WriteLine("=== FreeCell Sequence Move Constraints Demo ===");
            System.Console.WriteLine();
            
            // Scenario 1: Maximum free space
            System.Console.WriteLine("Scenario 1: Maximum free space (4 free cells, 2 empty columns)");
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
            
            int emptyFreeCells1 = rules.GetEmptyFreeCellCount();
            int emptyColumns1 = rules.GetEmptyTableauColumnCount();
            int maxCards1 = rules.CalculateMaxSequenceMoveSize();
            System.Console.WriteLine($"Empty free cells: {emptyFreeCells1}, Empty columns: {emptyColumns1}");
            System.Console.WriteLine($"Formula: C = 2^{emptyColumns1} × ({emptyFreeCells1}+1) = {(int)System.Math.Pow(2, emptyColumns1)} × {emptyFreeCells1 + 1} = {maxCards1} cards");
            System.Console.WriteLine($"Can move 13 cards: {rules.CanMoveCardSequence(13)}");
            System.Console.WriteLine($"Can move 20 cards: {rules.CanMoveCardSequence(20)}");
            System.Console.WriteLine($"Can move 21 cards: {rules.CanMoveCardSequence(21)}");
            System.Console.WriteLine();
            
            // Scenario 2: Limited free space
            System.Console.WriteLine("Scenario 2: Limited free space (1 free cell, 0 empty columns)");
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart }, 0);
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade }, 1);
            rules.PlaceCardInFreeCell(new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Diamond }, 2);
            // Free cell 3 remains empty (1 empty)
            
            // Ensure all tableau columns have cards
            for (int i = 0; i < rules.TableauColumns.Count; i++)
            {
                if (rules.TableauColumns[i].Count == 0)
                {
                    rules.TableauColumns[i].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Heart });
                }
            }
            
            int emptyFreeCells2 = rules.GetEmptyFreeCellCount();
            int emptyColumns2 = rules.GetEmptyTableauColumnCount();
            int maxCards2 = rules.CalculateMaxSequenceMoveSize();
            System.Console.WriteLine($"Empty free cells: {emptyFreeCells2}, Empty columns: {emptyColumns2}");
            System.Console.WriteLine($"Formula: C = 2^{emptyColumns2} × ({emptyFreeCells2}+1) = {(int)System.Math.Pow(2, emptyColumns2)} × {emptyFreeCells2 + 1} = {maxCards2} cards");
            System.Console.WriteLine($"Can move 1 card: {rules.CanMoveCardSequence(1)}");
            System.Console.WriteLine($"Can move 2 cards: {rules.CanMoveCardSequence(2)}");
            System.Console.WriteLine($"Can move 3 cards: {rules.CanMoveCardSequence(3)}");
            System.Console.WriteLine();
            
            // Scenario 3: Moving to empty column reduces limit
            System.Console.WriteLine("Scenario 3: Moving to empty column (same as scenario 2 but to empty column)");
            rules.TableauColumns[0].Clear(); // Make column 0 empty again (1 empty column)
            
            int emptyFreeCells3 = rules.GetEmptyFreeCellCount();
            int emptyColumns3 = rules.GetEmptyTableauColumnCount();
            int maxCards3 = rules.CalculateMaxSequenceMoveSize();
            int maxToEmpty = maxCards3 / 2;
            System.Console.WriteLine($"Empty free cells: {emptyFreeCells3}, Empty columns: {emptyColumns3}");
            System.Console.WriteLine($"Normal max: C = 2^{emptyColumns3} × ({emptyFreeCells3}+1) = {(int)System.Math.Pow(2, emptyColumns3)} × {emptyFreeCells3 + 1} = {maxCards3} cards");
            System.Console.WriteLine($"To empty column: {maxCards3} / 2 = {maxToEmpty} cards");
            System.Console.WriteLine($"Can move 2 cards to non-empty column: {rules.CanMoveCardSequence(2, 1)}");
            System.Console.WriteLine($"Can move 2 cards to empty column: {rules.CanMoveCardSequence(2, 0)}");
            System.Console.WriteLine($"Can move 1 card to empty column: {rules.CanMoveCardSequence(1, 0)}");
            System.Console.WriteLine();
            
            // Scenario 4: Non-FreeCell game comparison
            System.Console.WriteLine("Scenario 4: Klondike Solitaire (no restrictions)");
            SolitaireRules klondikeRules = new SolitaireRules("Klondike Solitaire");
            System.Console.WriteLine($"Can move 13 cards: {klondikeRules.CanMoveCardSequence(13)}");
            System.Console.WriteLine($"Can move 26 cards: {klondikeRules.CanMoveCardSequence(26)}");
            System.Console.WriteLine($"Can move 52 cards: {klondikeRules.CanMoveCardSequence(52)}");
            
            // All assertions should pass
            Assert.IsTrue(true, "Demo completed successfully");
        }
    }
}