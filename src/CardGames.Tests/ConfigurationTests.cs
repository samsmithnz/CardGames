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
        public void SolitaireRules_FreecellConfiguration_ShouldCreateCorrectPileStructure()
        {
            // Arrange & Act
            SolitaireRules rules = new SolitaireRules("Freecell");

            // Assert
            Assert.AreEqual(8, rules.TableauColumns.Count, "Freecell should have 8 tableau columns");
            Assert.AreEqual(4, rules.FoundationPiles.Count, "Freecell should have 4 foundation piles");
            Assert.IsNotNull(rules.StockPile, "Stock pile should exist");
            Assert.IsNotNull(rules.WastePile, "Waste pile should exist");
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
    }
}