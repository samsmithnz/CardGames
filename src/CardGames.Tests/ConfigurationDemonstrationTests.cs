using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CardGames.Tests
{
    /// <summary>
    /// Demonstration tests showing the configuration system in action
    /// These tests validate that different game configurations produce different behaviors
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ConfigurationDemonstrationTests
    {
        [TestMethod]
        public void CompareKlondikeVsFreecell_ShouldShowConfigurationDifferences()
        {
            // Arrange
            SolitaireRules klondike = new SolitaireRules("Klondike Solitaire");
            SolitaireRules freecell = new SolitaireRules("Freecell");

            // Act & Assert - Validate structural differences
            Assert.AreEqual(7, klondike.TableauColumns.Count, "Klondike should have 7 tableau columns");
            Assert.AreEqual(8, freecell.TableauColumns.Count, "Freecell should have 8 tableau columns");

            Assert.AreEqual(4, klondike.FoundationPiles.Count, "Both games should have 4 foundation piles");
            Assert.AreEqual(4, freecell.FoundationPiles.Count, "Both games should have 4 foundation piles");

            // Validate configuration properties
            Assert.AreEqual(1, klondike.GameConfig.Piles.Waste, "Klondike should have waste pile");
            Assert.AreEqual(0, freecell.GameConfig.Piles.Waste, "Freecell should not have waste pile");

            Assert.AreEqual(0, klondike.GameConfig.Piles.Freecells, "Klondike should not have free cells");
            Assert.AreEqual(4, freecell.GameConfig.Piles.Freecells, "Freecell should have 4 free cells");

            // Validate draw rules
            Assert.AreEqual(1, klondike.GameConfig.DrawRules.DrawCount, "Klondike draws 1 card");
            Assert.AreEqual(0, freecell.GameConfig.DrawRules.DrawCount, "Freecell has no draw mechanism");

            // Validate initial layout differences
            Assert.AreEqual(7, klondike.GameConfig.InitialLayout.Tableau.Count, "Klondike has 7 column layout");
            Assert.AreEqual(8, freecell.GameConfig.InitialLayout.Tableau.Count, "Freecell has 8 column layout");

            // Check Klondike layout: [1,2,3,4,5,6,7]
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(i + 1, klondike.GameConfig.InitialLayout.Tableau[i], 
                    $"Klondike column {i} should have {i + 1} cards");
            }

            // Check Freecell layout: [7,7,7,7,6,6,6,6]
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(7, freecell.GameConfig.InitialLayout.Tableau[i], 
                    $"Freecell column {i} should have 7 cards");
            }
            for (int i = 4; i < 8; i++)
            {
                Assert.AreEqual(6, freecell.GameConfig.InitialLayout.Tableau[i], 
                    $"Freecell column {i} should have 6 cards");
            }
        }

        [TestMethod]
        public void KlondikeVsFreecell_MovementRulesDifferences()
        {
            // Arrange
            SolitaireRules klondike = new SolitaireRules("Klondike Solitaire");
            SolitaireRules freecell = new SolitaireRules("Freecell");
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };

            // Act & Assert - Test empty tableau rules
            Assert.IsTrue(klondike.CanPlaceCardOnTableau(kingOfSpades, 0), 
                "Klondike: Kings only on empty tableau");
            Assert.IsFalse(klondike.CanPlaceCardOnTableau(queenOfHearts, 0), 
                "Klondike: Non-kings not allowed on empty tableau");

            Assert.IsTrue(freecell.CanPlaceCardOnTableau(kingOfSpades, 0), 
                "Freecell: Any card on empty tableau - King");
            Assert.IsTrue(freecell.CanPlaceCardOnTableau(queenOfHearts, 0), 
                "Freecell: Any card on empty tableau - Queen");

            // Validate configuration strings
            Assert.AreEqual("kings only", klondike.GameConfig.MovementRules.EmptyTableau);
            Assert.AreEqual("any card", freecell.GameConfig.MovementRules.EmptyTableau);
        }

        [TestMethod]
        public void ConfigurationSystem_ShowsCompleteGameDefinition()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Klondike Solitaire");

            // Act - Access all configuration aspects
            GameDefinition config = rules.GameConfig;

            // Assert - Demonstrate complete configuration coverage
            Console.WriteLine($"Game: {config.GameName}");
            Console.WriteLine($"Decks: {config.Decks}");
            Console.WriteLine($"Tableau Columns: {config.Piles.Tableau}");
            Console.WriteLine($"Foundation Piles: {config.Piles.Foundation}");
            Console.WriteLine($"Initial Layout: [{string.Join(",", config.InitialLayout.Tableau)}]");
            Console.WriteLine($"Empty Tableau Rule: {config.MovementRules.EmptyTableau}");
            Console.WriteLine($"Draw Count: {config.DrawRules.DrawCount}");
            Console.WriteLine($"Win Condition: {config.WinCondition}");
            Console.WriteLine($"Author: {config.Metadata.Author}");

            // All the hardcoded values are now configurable
            Assert.AreEqual("Klondike Solitaire", config.GameName);
            Assert.AreEqual(1, config.Decks);
            Assert.AreEqual(7, config.Piles.Tableau);
            Assert.AreEqual(4, config.Piles.Foundation);
            Assert.AreEqual("kings only", config.MovementRules.EmptyTableau);
            Assert.AreEqual("aces only", config.MovementRules.EmptyFoundation);
            Assert.AreEqual("descending, alternating colors", config.MovementRules.TableauToTableau);
            Assert.AreEqual("ascending, same suit", config.MovementRules.TableauToFoundation);
            Assert.AreEqual(1, config.DrawRules.DrawCount);
            Assert.AreEqual("unlimited", config.DrawRules.Redeals);
            Assert.AreEqual("all cards in foundations", config.WinCondition);
            Assert.AreEqual("samsmithnz", config.Metadata.Author);
        }

        [TestMethod]
        public void BackwardCompatibility_ExistingCodeUnchanged()
        {
            // This test demonstrates that existing code works exactly as before
            
            // Arrange & Act - Using the original constructor pattern
            SolitaireRules rules = new SolitaireRules();

            // Test movement rules BEFORE dealing cards (on empty tableau)
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card aceOfSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };

            // Test empty tableau behavior (before dealing)
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfSpades, 0), "Kings should be allowed on empty tableau");
            Assert.IsFalse(rules.CanPlaceCardOnTableau(queenOfHearts, 0), "Queens should not be allowed on empty tableau");
            Assert.IsTrue(rules.CanPlaceCardOnFoundation(aceOfSpades, 0), "Aces should be allowed on empty foundation");

            // Now deal cards and test the layout
            Deck deck = new Deck();
            rules.DealCards(deck);

            // Assert - All original behavior preserved
            Assert.AreEqual(7, rules.TableauColumns.Count);
            Assert.AreEqual(4, rules.FoundationPiles.Count);
            
            // Original tableau layout (1,2,3,4,5,6,7 cards)
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(i + 1, rules.TableauColumns[i].Count);
            }

            // Original stock pile calculation
            int expectedStock = 52 - (1 + 2 + 3 + 4 + 5 + 6 + 7);
            Assert.AreEqual(expectedStock, rules.StockPile.Count);

            // Verify configuration was loaded correctly
            Assert.AreEqual("Klondike Solitaire", rules.GameConfig.GameName);
            Assert.AreEqual("kings only", rules.GameConfig.MovementRules.EmptyTableau);
            Assert.AreEqual("aces only", rules.GameConfig.MovementRules.EmptyFoundation);

            // The behavior is identical, but now it's driven by configuration instead of hardcoded logic
        }
    }
}