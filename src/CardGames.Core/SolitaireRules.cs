using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CardGames.Core
{
    /// <summary>
    /// Implements the rules and logic for Klondike Solitaire card game
    /// </summary>
    public class SolitaireRules
    {
        /// <summary>
        /// The tableau columns where cards are initially dealt and can be built in descending order with alternating colors
        /// </summary>
        public List<List<Card>> TableauColumns { get; private set; }

        /// <summary>
        /// The foundation piles where cards are built up by suit from Ace to King
        /// </summary>
        public List<List<Card>> FoundationPiles { get; private set; }

        /// <summary>
        /// The stock pile containing face-down cards that can be drawn
        /// </summary>
        public List<Card> StockPile { get; private set; }

        /// <summary>
        /// The waste pile containing face-up cards drawn from the stock
        /// </summary>
        public List<Card> WastePile { get; private set; }

        /// <summary>
        /// The free cells used in games like Freecell for temporary card storage
        /// </summary>
        public List<Card> FreeCells { get; private set; }

        /// <summary>
        /// Configuration defining the rules and setup for this game variant
        /// </summary>
        public GameDefinition GameConfig { get; private set; }

        /// <summary>
        /// Initializes a new Solitaire game with empty game state using default Klondike Solitaire configuration
        /// </summary>
        public SolitaireRules() : this("Klondike Solitaire")
        {
        }

        /// <summary>
        /// Initializes a new Solitaire game with empty game state using the specified game configuration
        /// </summary>
        /// <param name="gameName">Name of the game variant to use (e.g., "Klondike Solitaire", "Freecell")</param>
        public SolitaireRules(string gameName)
        {
            GameConfig = LoadGameConfiguration(gameName);
            InitializePiles();
        }

        /// <summary>
        /// Initializes a new Solitaire game with empty game state using the provided game configuration
        /// </summary>
        /// <param name="gameConfig">Game configuration to use</param>
        public SolitaireRules(GameDefinition gameConfig)
        {
            GameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            InitializePiles();
        }

        /// <summary>
        /// Initializes the game piles based on the current configuration
        /// </summary>
        private void InitializePiles()
        {
            TableauColumns = new List<List<Card>>();
            for (int i = 0; i < GameConfig.Piles.Tableau; i++)
            {
                TableauColumns.Add(new List<Card>());
            }

            FoundationPiles = new List<List<Card>>();
            for (int i = 0; i < GameConfig.Piles.Foundation; i++)
            {
                FoundationPiles.Add(new List<Card>());
            }

            StockPile = new List<Card>();
            WastePile = new List<Card>();

            // Initialize free cells for games like Freecell
            FreeCells = new List<Card>();
            for (int i = 0; i < GameConfig.Piles.Freecells; i++)
            {
                FreeCells.Add(null); // null represents an empty free cell
            }
        }

        /// <summary>
        /// Loads game configuration from embedded JSON resource
        /// </summary>
        /// <param name="gameName">Name of the game variant to load</param>
        /// <returns>Game configuration</returns>
        private static GameDefinition LoadGameConfiguration(string gameName)
        {
            string configJson = LoadEmbeddedConfigurationJson();
            SolitaireGameConfig config = SolitaireGameConfig.FromJson(configJson);
            
            GameDefinition gameConfig = config.FindGame(gameName);
            if (gameConfig == null)
            {
                throw new ArgumentException($"Game configuration not found for: {gameName}", nameof(gameName));
            }

            return gameConfig;
        }

        /// <summary>
        /// Loads the embedded JSON configuration file
        /// </summary>
        /// <returns>JSON configuration content</returns>
        private static string LoadEmbeddedConfigurationJson()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "CardGames.Core.solitaire-config.json";
            
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Sets up a new Solitaire game with the provided deck
        /// </summary>
        /// <param name="deck">The shuffled deck to deal from</param>
        public void DealCards(Deck deck)
        {
            if (deck == null || deck.Cards.Count != 52)
            {
                throw new ArgumentException("Deck must contain exactly 52 cards");
            }

            // Clear existing game state
            foreach (List<Card> column in TableauColumns)
            {
                column.Clear();
            }
            foreach (List<Card> foundation in FoundationPiles)
            {
                foundation.Clear();
            }
            StockPile.Clear();
            WastePile.Clear();

            int cardIndex = 0;

            // Deal cards to tableau columns based on configuration
            List<int> tableauLayout = GameConfig.InitialLayout.Tableau;
            for (int column = 0; column < GameConfig.Piles.Tableau && column < tableauLayout.Count; column++)
            {
                int cardsInColumn = tableauLayout[column];
                for (int card = 0; card < cardsInColumn; card++)
                {
                    if (cardIndex < deck.Cards.Count)
                    {
                        TableauColumns[column].Add(deck.Cards[cardIndex]);
                        cardIndex++;
                    }
                }
            }

            // Remaining cards go to stock pile
            for (int i = cardIndex; i < deck.Cards.Count; i++)
            {
                StockPile.Add(deck.Cards[i]);
            }
        }

        /// <summary>
        /// Checks if a card can be placed on a tableau column
        /// </summary>
        /// <param name="card">The card to place</param>
        /// <param name="columnIndex">The tableau column index</param>
        /// <returns>True if the move is valid, false otherwise</returns>
        public bool CanPlaceCardOnTableau(Card card, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= GameConfig.Piles.Tableau || card == null)
            {
                return false;
            }

            List<Card> column = TableauColumns[columnIndex];

            // Empty column rules based on configuration
            if (column.Count == 0)
            {
                if (GameConfig.MovementRules.EmptyTableau.Contains("kings only"))
                {
                    return card.Number == Card.CardNumber.K;
                }
                else if (GameConfig.MovementRules.EmptyTableau.Contains("any card"))
                {
                    return true;
                }
                return false;
            }

            // Get the top card of the column
            Card topCard = column[column.Count - 1];

            // Check movement rules based on configuration
            if (GameConfig.MovementRules.TableauToTableau.Contains("descending") && 
                GameConfig.MovementRules.TableauToTableau.Contains("alternating colors"))
            {
                return IsOneRankLower(card.Number, topCard.Number) && IsOppositeColor(card, topCard);
            }

            return false;
        }

        /// <summary>
        /// Checks if a card can be placed on a foundation pile
        /// </summary>
        /// <param name="card">The card to place</param>
        /// <param name="foundationIndex">The foundation pile index</param>
        /// <returns>True if the move is valid, false otherwise</returns>
        public bool CanPlaceCardOnFoundation(Card card, int foundationIndex)
        {
            if (foundationIndex < 0 || foundationIndex >= GameConfig.Piles.Foundation || card == null)
            {
                return false;
            }

            List<Card> foundation = FoundationPiles[foundationIndex];

            // Empty foundation rules based on configuration
            if (foundation.Count == 0)
            {
                if (GameConfig.MovementRules.EmptyFoundation.Contains("aces only"))
                {
                    return card.Number == Card.CardNumber.A;
                }
                return false;
            }

            // Get the top card of the foundation
            Card topCard = foundation[foundation.Count - 1];

            // Check movement rules based on configuration
            if (GameConfig.MovementRules.TableauToFoundation.Contains("ascending") && 
                GameConfig.MovementRules.TableauToFoundation.Contains("same suit"))
            {
                return card.Suite == topCard.Suite && IsOneRankHigher(card.Number, topCard.Number);
            }

            return false;
        }

        /// <summary>
        /// Checks if the game has been won (all cards moved to foundations)
        /// </summary>
        /// <returns>True if the game is won, false otherwise</returns>
        public bool IsGameWon()
        {
            foreach (List<Card> foundation in FoundationPiles)
            {
                if (foundation.Count != 13)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds the foundation pile for a card's specific suite
        /// </summary>
        /// <param name="card">The card to place</param>
        /// <returns>The foundation index (0-3) if a valid placement is found, -1 otherwise</returns>
        public int FindAvailableFoundationPile(Card card)
        {
            if (card == null)
            {
                return -1;
            }

            // Each suite maps to a specific foundation pile based on the enum index
            int suiteFoundationIndex = (int)card.Suite;
            
            if (CanPlaceCardOnFoundation(card, suiteFoundationIndex))
            {
                return suiteFoundationIndex;
            }
            
            return -1;
        }

        /// <summary>
        /// Draws a card from the stock pile to the waste pile
        /// </summary>
        /// <returns>True if a card was drawn, false if stock pile is empty</returns>
        public bool DrawFromStock()
        {
            if (StockPile.Count == 0)
            {
                return false;
            }

            Card drawnCard = StockPile[StockPile.Count - 1];
            StockPile.RemoveAt(StockPile.Count - 1);
            WastePile.Add(drawnCard);
            return true;
        }

        /// <summary>
        /// Resets the waste pile back to the stock pile when stock is empty
        /// </summary>
        public void ResetStock()
        {
            if (StockPile.Count == 0 && WastePile.Count > 0)
            {
                // Move all waste cards back to stock in reverse order
                for (int i = WastePile.Count - 1; i >= 0; i--)
                {
                    StockPile.Add(WastePile[i]);
                }
                WastePile.Clear();
            }
        }

        /// <summary>
        /// Checks if one card number is exactly one rank lower than another
        /// </summary>
        private bool IsOneRankLower(Card.CardNumber lower, Card.CardNumber higher)
        {
            return (int)lower == (int)higher - 1;
        }

        /// <summary>
        /// Checks if one card number is exactly one rank higher than another
        /// </summary>
        private bool IsOneRankHigher(Card.CardNumber higher, Card.CardNumber lower)
        {
            return (int)higher == (int)lower + 1;
        }

        /// <summary>
        /// Checks if two cards are opposite colors (red vs black)
        /// </summary>
        private bool IsOppositeColor(Card card1, Card card2)
        {
            bool card1IsRed = card1.Suite == Card.CardSuite.Heart || card1.Suite == Card.CardSuite.Diamond;
            bool card2IsRed = card2.Suite == Card.CardSuite.Heart || card2.Suite == Card.CardSuite.Diamond;
            return card1IsRed != card2IsRed;
        }

        /// <summary>
        /// Checks if a card can be placed in a specific free cell
        /// </summary>
        /// <param name="freeCellIndex">Index of the free cell (0-3 for Freecell)</param>
        /// <returns>True if the free cell is empty and available</returns>
        public bool CanPlaceCardInFreeCell(int freeCellIndex)
        {
            if (freeCellIndex < 0 || freeCellIndex >= FreeCells.Count)
            {
                return false;
            }
            
            return FreeCells[freeCellIndex] == null;
        }

        /// <summary>
        /// Places a card in a specific free cell
        /// </summary>
        /// <param name="card">Card to place</param>
        /// <param name="freeCellIndex">Index of the free cell</param>
        /// <returns>True if successful, false if free cell is occupied or invalid</returns>
        public bool PlaceCardInFreeCell(Card card, int freeCellIndex)
        {
            if (!CanPlaceCardInFreeCell(freeCellIndex))
            {
                return false;
            }
            
            FreeCells[freeCellIndex] = card;
            return true;
        }

        /// <summary>
        /// Gets a card from a specific free cell
        /// </summary>
        /// <param name="freeCellIndex">Index of the free cell</param>
        /// <returns>The card in the free cell, or null if empty</returns>
        public Card GetCardFromFreeCell(int freeCellIndex)
        {
            if (freeCellIndex < 0 || freeCellIndex >= FreeCells.Count)
            {
                return null;
            }
            
            return FreeCells[freeCellIndex];
        }

        /// <summary>
        /// Removes a card from a specific free cell
        /// </summary>
        /// <param name="freeCellIndex">Index of the free cell</param>
        /// <returns>The card that was removed, or null if free cell was empty</returns>
        public Card RemoveCardFromFreeCell(int freeCellIndex)
        {
            if (freeCellIndex < 0 || freeCellIndex >= FreeCells.Count)
            {
                return null;
            }
            
            Card card = FreeCells[freeCellIndex];
            FreeCells[freeCellIndex] = null;
            return card;
        }

        /// <summary>
        /// Gets the number of empty free cells available
        /// </summary>
        /// <returns>Count of empty free cells</returns>
        public int GetEmptyFreeCellCount()
        {
            int count = 0;
            for (int i = 0; i < FreeCells.Count; i++)
            {
                if (FreeCells[i] == null)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
