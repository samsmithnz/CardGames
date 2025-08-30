using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void LoadConfigurationFromJson_ShouldWork()
        {
            // Arrange
            string jsonConfig = @"{
  ""games"": [
    {
      ""gameName"": ""Klondike Solitaire"",
      ""decks"": 1,
      ""piles"": {
        ""tableau"": 7,
        ""foundation"": 4,
        ""waste"": 1,
        ""freecells"": 0
      },
      ""initialLayout"": {
        ""tableau"": [1, 2, 3, 4, 5, 6, 7],
        ""faceUp"": [false, false, false, false, false, false, true]
      },
      ""movementRules"": {
        ""tableauToTableau"": ""descending, alternating colors"",
        ""tableauToFoundation"": ""ascending, same suit"",
        ""wasteToTableau"": ""allowed"",
        ""wasteToFoundation"": ""allowed"",
        ""emptyTableau"": ""kings only"",
        ""emptyFoundation"": ""aces only"",
        ""freecellToTableau"": ""not applicable"",
        ""freecellToFoundation"": ""not applicable""
      },
      ""drawRules"": {
        ""drawCount"": 1,
        ""redeals"": ""unlimited""
      },
      ""winCondition"": ""all cards in foundations"",
      ""scoring"": ""standard"",
      ""metadata"": {
        ""author"": ""samsmithnz"",
        ""version"": ""1.0"",
        ""dateCreated"": ""2024-12-19T00:00:00Z"",
        ""description"": ""Traditional Klondike Solitaire with standard rules""
      }
    }
  ]
}";

            // Act
            SolitaireGameConfig config = SolitaireGameConfig.FromJson(jsonConfig);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(1, config.Games.Count);
            
            GameDefinition klondikeConfig = config.FindGame("Klondike Solitaire");
            Assert.IsNotNull(klondikeConfig);
            Assert.AreEqual("Klondike Solitaire", klondikeConfig.GameName);
            Assert.AreEqual(7, klondikeConfig.Piles.Tableau);
            Assert.AreEqual(4, klondikeConfig.Piles.Foundation);
        }

        [TestMethod]
        public void SolitaireRules_WithGameDefinition_ShouldInitializeCorrectly()
        {
            // Arrange
            GameDefinition config = new GameDefinition
            {
                GameName = "Test Klondike",
                Piles = new PileConfiguration { Tableau = 7, Foundation = 4, Waste = 1 },
                InitialLayout = new InitialLayout 
                { 
                    Tableau = new System.Collections.Generic.List<int> { 1, 2, 3, 4, 5, 6, 7 },
                    FaceUp = new System.Collections.Generic.List<bool> { false, false, false, false, false, false, true }
                },
                MovementRules = new MovementRules
                {
                    EmptyTableau = "kings only",
                    EmptyFoundation = "aces only",
                    TableauToTableau = "descending, alternating colors",
                    TableauToFoundation = "ascending, same suit"
                }
            };

            // Act
            SolitaireRules rules = new SolitaireRules(config);

            // Assert
            Assert.IsNotNull(rules);
            Assert.AreEqual(7, rules.TableauColumns.Count);
            Assert.AreEqual(4, rules.FoundationPiles.Count);
            Assert.IsNotNull(rules.StockPile);
            Assert.IsNotNull(rules.WastePile);
        }

        [TestMethod]
        public void SolitaireRules_DefaultConstructor_ShouldLoadKlondikeConfiguration()
        {
            // Act
            SolitaireRules rules = new SolitaireRules();

            // Assert
            Assert.IsNotNull(rules);
            Assert.AreEqual(7, rules.TableauColumns.Count);
            Assert.AreEqual(4, rules.FoundationPiles.Count);
            Assert.IsNotNull(rules.StockPile);
            Assert.IsNotNull(rules.WastePile);
            Assert.IsNotNull(rules.GameConfig);
            Assert.AreEqual("Klondike Solitaire", rules.GameConfig.GameName);
        }

        [TestMethod]
        public void SolitaireRules_KlondikeByName_ShouldLoadCorrectConfiguration()
        {
            // Act
            SolitaireRules rules = new SolitaireRules("Klondike Solitaire");

            // Assert
            Assert.IsNotNull(rules);
            Assert.AreEqual("Klondike Solitaire", rules.GameConfig.GameName);
            Assert.AreEqual(7, rules.GameConfig.Piles.Tableau);
            Assert.AreEqual(4, rules.GameConfig.Piles.Foundation);
            Assert.AreEqual(1, rules.GameConfig.Piles.Waste);
            Assert.AreEqual(0, rules.GameConfig.Piles.Freecells);
            Assert.AreEqual(1, rules.GameConfig.DrawRules.DrawCount);
            Assert.AreEqual("unlimited", rules.GameConfig.DrawRules.Redeals);
        }

        [TestMethod]
        public void SolitaireRules_FreecellByName_ShouldLoadCorrectConfiguration()
        {
            // Act
            SolitaireRules rules = new SolitaireRules("Freecell");

            // Assert
            Assert.IsNotNull(rules);
            Assert.AreEqual("Freecell", rules.GameConfig.GameName);
            Assert.AreEqual(8, rules.GameConfig.Piles.Tableau);
            Assert.AreEqual(4, rules.GameConfig.Piles.Foundation);
            Assert.AreEqual(0, rules.GameConfig.Piles.Waste);
            Assert.AreEqual(4, rules.GameConfig.Piles.Freecells);
            Assert.AreEqual(0, rules.GameConfig.DrawRules.DrawCount);
            Assert.AreEqual("not applicable", rules.GameConfig.DrawRules.Redeals);
        }

        [TestMethod]
        public void SolitaireRules_KlondikeConfiguration_ShouldDealCardsCorrectly()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Klondike Solitaire");
            Deck deck = new Deck();

            // Act
            rules.DealCards(deck);

            // Assert
            // Check tableau columns have correct number of cards (1, 2, 3, 4, 5, 6, 7)
            for (int i = 0; i < 7; i++)
            {
                Assert.AreEqual(i + 1, rules.TableauColumns[i].Count, $"Column {i} should have {i + 1} cards");
            }

            // Check stock pile has remaining cards (52 - 28 = 24)
            int expectedStockCount = 52 - (1 + 2 + 3 + 4 + 5 + 6 + 7);
            Assert.AreEqual(expectedStockCount, rules.StockPile.Count);

            // Check other piles are empty initially
            Assert.AreEqual(0, rules.WastePile.Count);
            foreach (var foundation in rules.FoundationPiles)
            {
                Assert.AreEqual(0, foundation.Count);
            }
        }

        [TestMethod]
        public void SolitaireRules_KlondikeMovementRules_ShouldBeEnforced()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Klondike Solitaire");
            Card kingOfSpades = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Spade };
            Card queenOfHearts = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Heart };
            Card aceOfSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };

            // Act & Assert

            // Test empty tableau (should only accept Kings)
            Assert.IsTrue(rules.CanPlaceCardOnTableau(kingOfSpades, 0), "Kings should be allowed on empty tableau");
            Assert.IsFalse(rules.CanPlaceCardOnTableau(queenOfHearts, 0), "Non-Kings should not be allowed on empty tableau");

            // Test empty foundation (should only accept Aces)
            Assert.IsTrue(rules.CanPlaceCardOnFoundation(aceOfSpades, 0), "Aces should be allowed on empty foundation");
            Assert.IsFalse(rules.CanPlaceCardOnFoundation(kingOfSpades, 0), "Non-Aces should not be allowed on empty foundation");

            // Test tableau alternating colors
            rules.TableauColumns[0].Add(kingOfSpades); // Black King
            Assert.IsTrue(rules.CanPlaceCardOnTableau(queenOfHearts, 0), "Red Queen should be allowed on Black King");
            
            Card queenOfSpades = new Card { Number = Card.CardNumber.Q, Suite = Card.CardSuite.Spade };
            Assert.IsFalse(rules.CanPlaceCardOnTableau(queenOfSpades, 0), "Black Queen should not be allowed on Black King");
        }

        [TestMethod]
        public void SolitaireRules_GameTypeSwitching_ShouldMaintainCorrectConfiguration()
        {
            // Test switching from Klondike to Freecell
            SolitaireRules rules = new SolitaireRules("Klondike Solitaire");
            
            // Verify initial Klondike state
            Assert.AreEqual(7, rules.TableauColumns.Count, "Klondike should have 7 tableau columns");
            Assert.AreEqual(4, rules.FoundationPiles.Count, "Klondike should have 4 foundation piles");
            Assert.AreEqual(0, rules.FreeCells.Count, "Klondike should have 0 free cells");
            Assert.AreEqual(1, rules.GameConfig.Piles.Waste, "Klondike should have 1 waste pile");
            
            // Switch to Freecell
            rules = new SolitaireRules("Freecell");
            
            // Verify Freecell state
            Assert.AreEqual(8, rules.TableauColumns.Count, "Freecell should have 8 tableau columns");
            Assert.AreEqual(4, rules.FoundationPiles.Count, "Freecell should have 4 foundation piles");
            Assert.AreEqual(4, rules.FreeCells.Count, "Freecell should have 4 free cells");
            Assert.AreEqual(0, rules.GameConfig.Piles.Waste, "Freecell should have 0 waste piles");
            
            // Verify all free cells are initially empty
            for (int i = 0; i < rules.FreeCells.Count; i++)
            {
                Assert.IsNull(rules.FreeCells[i], $"Free cell {i} should be initially empty");
            }
            
            // Switch back to Klondike
            rules = new SolitaireRules("Klondike Solitaire");
            
            // Verify back to Klondike state
            Assert.AreEqual(7, rules.TableauColumns.Count, "Klondike should have 7 tableau columns after switch back");
            Assert.AreEqual(0, rules.FreeCells.Count, "Klondike should have 0 free cells after switch back");
            Assert.AreEqual(1, rules.GameConfig.Piles.Waste, "Klondike should have 1 waste pile after switch back");
        }

        [TestMethod]
        public void SolitaireRules_FreecellUIConfiguration_ShouldHaveCorrectSettings()
        {
            // Arrange & Act - Create Freecell rules
            SolitaireRules freecellRules = new SolitaireRules("Freecell");

            // Assert - Verify Freecell-specific UI requirements
            Assert.AreEqual(4, freecellRules.FreeCells.Count, "Freecell should have 4 free cells for UI");
            Assert.AreEqual(8, freecellRules.TableauColumns.Count, "Freecell should have 8 tableau columns for UI");
            Assert.AreEqual(0, freecellRules.GameConfig.Piles.Waste, "Freecell should have 0 waste piles (hidden in UI)");
            Assert.AreEqual(4, freecellRules.GameConfig.Piles.Freecells, "Freecell config should specify 4 freecells");
            
            // Verify free cells are properly initialized as empty
            for (int i = 0; i < freecellRules.FreeCells.Count; i++)
            {
                Assert.IsNull(freecellRules.FreeCells[i], $"Free cell {i} should start empty");
                Assert.IsTrue(freecellRules.CanPlaceCardInFreeCell(i), $"Should be able to place card in empty free cell {i}");
            }
            
            // Verify that free cell functionality works
            Card testCard = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
            Assert.IsTrue(freecellRules.PlaceCardInFreeCell(testCard, 0), "Should be able to place card in free cell");
            Assert.IsFalse(freecellRules.CanPlaceCardInFreeCell(0), "Should not be able to place another card in occupied free cell");
            
            Card retrievedCard = freecellRules.GetCardFromFreeCell(0);
            Assert.IsNotNull(retrievedCard, "Should retrieve the placed card");
            Assert.AreEqual(Card.CardNumber.A, retrievedCard.Number, "Retrieved card should be ace");
            Assert.AreEqual(Card.CardSuite.Spade, retrievedCard.Suite, "Retrieved card should be spades");
        }

        [TestMethod]
        public void SolitaireRules_InvalidGameName_ShouldThrowException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => new SolitaireRules("NonExistentGame"));
        }

        [TestMethod]
        public void ListEmbeddedResources_ForDebugging()
        {
            // Arrange
            Assembly assembly = Assembly.GetAssembly(typeof(SolitaireRules));

            // Act
            string[] resourceNames = assembly.GetManifestResourceNames();

            // Assert & Debug Info
            Console.WriteLine($"Found {resourceNames.Length} embedded resources:");
            foreach (string resourceName in resourceNames)
            {
                Console.WriteLine($"  - {resourceName}");
            }
            
            // This test always passes, it's just for debugging
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SolitaireRules_FreecellMethods_ShouldWorkCorrectly()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            Card aceOfSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };
            Card kingOfHearts = new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Heart };

            // Act & Assert - Test placing cards in free cells
            Assert.IsTrue(rules.CanPlaceCardInFreeCell(0), "Should be able to place card in empty free cell");
            Assert.IsTrue(rules.PlaceCardInFreeCell(aceOfSpades, 0), "Should successfully place ace in free cell 0");
            Assert.IsFalse(rules.CanPlaceCardInFreeCell(0), "Should not be able to place card in occupied free cell");
            Assert.IsFalse(rules.PlaceCardInFreeCell(kingOfHearts, 0), "Should not be able to place second card in same free cell");

            // Test getting cards from free cells
            Card retrievedCard = rules.GetCardFromFreeCell(0);
            Assert.IsNotNull(retrievedCard, "Should retrieve card from free cell");
            Assert.AreEqual(Card.CardNumber.A, retrievedCard.Number, "Retrieved card should be the ace");
            Assert.AreEqual(Card.CardSuite.Spade, retrievedCard.Suite, "Retrieved card should be spades");

            // Test removing cards from free cells
            Card removedCard = rules.RemoveCardFromFreeCell(0);
            Assert.IsNotNull(removedCard, "Should remove card from free cell");
            Assert.AreEqual(Card.CardNumber.A, removedCard.Number, "Removed card should be the ace");
            Assert.IsNull(rules.GetCardFromFreeCell(0), "Free cell should be empty after removal");

            // Test empty free cell count
            Assert.AreEqual(4, rules.GetEmptyFreeCellCount(), "All free cells should be empty initially");
            rules.PlaceCardInFreeCell(aceOfSpades, 0);
            rules.PlaceCardInFreeCell(kingOfHearts, 1);
            Assert.AreEqual(2, rules.GetEmptyFreeCellCount(), "Should have 2 empty free cells after placing 2 cards");
        }

        [TestMethod]
        public void SolitaireRules_FreecellMethods_ShouldHandleInvalidIndices()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules("Freecell");
            Card aceOfSpades = new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade };

            // Act & Assert - Test invalid indices
            Assert.IsFalse(rules.CanPlaceCardInFreeCell(-1), "Negative index should be invalid");
            Assert.IsFalse(rules.CanPlaceCardInFreeCell(4), "Index beyond free cell count should be invalid");
            Assert.IsFalse(rules.PlaceCardInFreeCell(aceOfSpades, -1), "Should not place card at negative index");
            Assert.IsFalse(rules.PlaceCardInFreeCell(aceOfSpades, 4), "Should not place card beyond free cell count");
            Assert.IsNull(rules.GetCardFromFreeCell(-1), "Should return null for negative index");
            Assert.IsNull(rules.GetCardFromFreeCell(4), "Should return null for index beyond count");
            Assert.IsNull(rules.RemoveCardFromFreeCell(-1), "Should return null when removing from negative index");
            Assert.IsNull(rules.RemoveCardFromFreeCell(4), "Should return null when removing from index beyond count");
        }
    }
}