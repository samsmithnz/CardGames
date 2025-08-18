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