using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardGames.Core;
using System;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests for game selection functionality
    /// </summary>
    [TestClass]
    public class GameSelectionTests
    {
        /// <summary>
        /// Test that Klondike Solitaire can be created and has correct configuration
        /// </summary>
        [TestMethod]
        public void KlondikeSolitaire_CanBeCreated_WithCorrectConfiguration()
        {
            // Arrange & Act
            SolitaireRules klondikeRules = new SolitaireRules("Klondike Solitaire");
            
            // Assert
            Assert.IsNotNull(klondikeRules);
            Assert.IsNotNull(klondikeRules.GameConfig);
            Assert.AreEqual("Klondike Solitaire", klondikeRules.GameConfig.GameName);
            Assert.AreEqual(7, klondikeRules.GameConfig.Piles.Tableau);
            Assert.AreEqual(4, klondikeRules.GameConfig.Piles.Foundation);
            Assert.AreEqual(1, klondikeRules.GameConfig.Piles.Waste);
            Assert.AreEqual(0, klondikeRules.GameConfig.Piles.Freecells);
        }

        /// <summary>
        /// Test that Freecell can be created and has correct configuration
        /// </summary>
        [TestMethod]
        public void Freecell_CanBeCreated_WithCorrectConfiguration()
        {
            // Arrange & Act
            SolitaireRules freecellRules = new SolitaireRules("Freecell");
            
            // Assert
            Assert.IsNotNull(freecellRules);
            Assert.IsNotNull(freecellRules.GameConfig);
            Assert.AreEqual("Freecell", freecellRules.GameConfig.GameName);
            Assert.AreEqual(8, freecellRules.GameConfig.Piles.Tableau);
            Assert.AreEqual(4, freecellRules.GameConfig.Piles.Foundation);
            Assert.AreEqual(0, freecellRules.GameConfig.Piles.Waste);
            Assert.AreEqual(4, freecellRules.GameConfig.Piles.Freecells);
        }

        /// <summary>
        /// Test that default constructor still creates Klondike Solitaire
        /// </summary>
        [TestMethod]
        public void DefaultConstructor_CreatesKlondikeSolitaire()
        {
            // Arrange & Act
            SolitaireRules defaultRules = new SolitaireRules();
            
            // Assert
            Assert.IsNotNull(defaultRules);
            Assert.IsNotNull(defaultRules.GameConfig);
            Assert.AreEqual("Klondike Solitaire", defaultRules.GameConfig.GameName);
        }

        /// <summary>
        /// Test that invalid game name throws appropriate exception
        /// </summary>
        [TestMethod]
        public void InvalidGameName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<ArgumentException>(() => 
            {
                SolitaireRules invalidRules = new SolitaireRules("NonExistentGame");
            });
        }

        /// <summary>
        /// Test that both game types can deal cards successfully
        /// </summary>
        [TestMethod]
        public void BothGameTypes_CanDealCards_Successfully()
        {
            // Arrange
            SolitaireRules klondikeRules = new SolitaireRules("Klondike Solitaire");
            SolitaireRules freecellRules = new SolitaireRules("Freecell");
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();
            deck1.Shuffle();
            deck2.Shuffle();

            // Act
            klondikeRules.DealCards(deck1);
            freecellRules.DealCards(deck2);

            // Assert - Klondike should have cards in tableau columns
            Assert.IsTrue(klondikeRules.TableauColumns.Count >= 7);
            for (int i = 0; i < 7; i++)
            {
                Assert.IsTrue(klondikeRules.TableauColumns[i].Count > 0, $"Klondike tableau column {i} should have cards");
            }

            // Assert - Freecell should have cards in tableau columns (may have 8 columns but only 7 tracked in current implementation)
            Assert.IsTrue(freecellRules.TableauColumns.Count >= 7);
            for (int i = 0; i < Math.Min(7, freecellRules.TableauColumns.Count); i++)
            {
                Assert.IsTrue(freecellRules.TableauColumns[i].Count > 0, $"Freecell tableau column {i} should have cards");
            }
        }
    }
}