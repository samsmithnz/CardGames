using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CardGames.Core
{
    /// <summary>
    /// Configuration structure for solitaire game rules and setup.
    /// Supports multiple game variants through a flexible configuration system.
    /// </summary>
    public class SolitaireGameConfig
    {
        /// <summary>
        /// Collection of supported game configurations
        /// </summary>
        [JsonPropertyName("games")]
        public List<GameDefinition> Games { get; set; } = new List<GameDefinition>();

        /// <summary>
        /// Loads configuration from JSON string
        /// </summary>
        /// <param name="json">JSON configuration string</param>
        /// <returns>Parsed configuration object</returns>
        public static SolitaireGameConfig FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON content is empty", nameof(json));
            }

            SolitaireGameConfig config = JsonSerializer.Deserialize<SolitaireGameConfig>(json);
            if (config == null)
            {
                throw new InvalidOperationException("Failed to deserialize SolitaireGameConfig");
            }

            return config;
        }

        /// <summary>
        /// Serializes configuration to JSON string
        /// </summary>
        /// <param name="indented">Whether to format JSON with indentation</param>
        /// <returns>JSON configuration string</returns>
        public string ToJson(bool indented = true)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = indented
            };
            return JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// Finds a specific game configuration by name
        /// </summary>
        /// <param name="gameName">Name of the game to find</param>
        /// <returns>Game definition or null if not found</returns>
        public GameDefinition FindGame(string gameName)
        {
            return Games.Find(g => string.Equals(g.GameName, gameName, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Complete definition of a specific solitaire game variant
    /// </summary>
    public class GameDefinition
    {
        /// <summary>
        /// Name of the game variant (e.g., "Klondike Solitaire", "Freecell")
        /// </summary>
        [JsonPropertyName("gameName")]
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// Number of standard decks used in the game
        /// </summary>
        [JsonPropertyName("decks")]
        public int Decks { get; set; } = 1;

        /// <summary>
        /// Configuration of different pile types in the game
        /// </summary>
        [JsonPropertyName("piles")]
        public PileConfiguration Piles { get; set; } = new PileConfiguration();

        /// <summary>
        /// How cards are initially distributed across piles
        /// </summary>
        [JsonPropertyName("initialLayout")]
        public InitialLayout InitialLayout { get; set; } = new InitialLayout();

        /// <summary>
        /// Rules governing how cards can be moved between piles
        /// </summary>
        [JsonPropertyName("movementRules")]
        public MovementRules MovementRules { get; set; } = new MovementRules();

        /// <summary>
        /// Rules for drawing cards from stock pile (if applicable)
        /// </summary>
        [JsonPropertyName("drawRules")]
        public DrawRules DrawRules { get; set; } = new DrawRules();

        /// <summary>
        /// Condition that must be met to win the game
        /// </summary>
        [JsonPropertyName("winCondition")]
        public string WinCondition { get; set; } = "all cards in foundations";

        /// <summary>
        /// Scoring method for the game
        /// </summary>
        [JsonPropertyName("scoring")]
        public string Scoring { get; set; } = "standard";

        /// <summary>
        /// Metadata about the game configuration
        /// </summary>
        [JsonPropertyName("metadata")]
        public GameMetadata Metadata { get; set; } = new GameMetadata();
    }

    /// <summary>
    /// Configuration of pile types and quantities
    /// </summary>
    public class PileConfiguration
    {
        /// <summary>
        /// Number of tableau columns
        /// </summary>
        [JsonPropertyName("tableau")]
        public int Tableau { get; set; } = 7;

        /// <summary>
        /// Number of foundation piles
        /// </summary>
        [JsonPropertyName("foundation")]
        public int Foundation { get; set; } = 4;

        /// <summary>
        /// Number of waste piles
        /// </summary>
        [JsonPropertyName("waste")]
        public int Waste { get; set; } = 1;

        /// <summary>
        /// Number of free cells (for games like Freecell)
        /// </summary>
        [JsonPropertyName("freecells")]
        public int Freecells { get; set; } = 0;
    }

    /// <summary>
    /// Configuration for initial card distribution
    /// </summary>
    public class InitialLayout
    {
        /// <summary>
        /// Number of cards dealt to each tableau column
        /// </summary>
        [JsonPropertyName("tableau")]
        public List<int> Tableau { get; set; } = new List<int> { 1, 2, 3, 4, 5, 6, 7 };

        /// <summary>
        /// Face-up state for cards in each tableau column (true = face-up, false = face-down)
        /// For Klondike: only the last card in each column is face-up initially
        /// </summary>
        [JsonPropertyName("faceUp")]
        public List<bool> FaceUp { get; set; } = new List<bool> { false, false, false, false, false, false, true };
    }

    /// <summary>
    /// Rules governing card movements between different pile types
    /// </summary>
    public class MovementRules
    {
        /// <summary>
        /// Rules for tableau to tableau moves (e.g., "descending, alternating colors")
        /// </summary>
        [JsonPropertyName("tableauToTableau")]
        public string TableauToTableau { get; set; } = "descending, alternating colors";

        /// <summary>
        /// Rules for tableau to foundation moves (e.g., "ascending, same suit")
        /// </summary>
        [JsonPropertyName("tableauToFoundation")]
        public string TableauToFoundation { get; set; } = "ascending, same suit";

        /// <summary>
        /// Whether waste to tableau moves are allowed
        /// </summary>
        [JsonPropertyName("wasteToTableau")]
        public string WasteToTableau { get; set; } = "allowed";

        /// <summary>
        /// Whether waste to foundation moves are allowed
        /// </summary>
        [JsonPropertyName("wasteToFoundation")]
        public string WasteToFoundation { get; set; } = "allowed";

        /// <summary>
        /// What cards can be placed on empty tableau columns (e.g., "kings only")
        /// </summary>
        [JsonPropertyName("emptyTableau")]
        public string EmptyTableau { get; set; } = "kings only";

        /// <summary>
        /// What cards can be placed on empty foundation piles (e.g., "aces only")
        /// </summary>
        [JsonPropertyName("emptyFoundation")]
        public string EmptyFoundation { get; set; } = "aces only";

        /// <summary>
        /// Rules for freecell moves (if applicable)
        /// </summary>
        [JsonPropertyName("freecellToTableau")]
        public string FreecellToTableau { get; set; } = "not applicable";

        /// <summary>
        /// Rules for freecell to foundation moves (if applicable)
        /// </summary>
        [JsonPropertyName("freecellToFoundation")]
        public string FreecellToFoundation { get; set; } = "not applicable";
    }

    /// <summary>
    /// Rules for drawing cards from stock pile
    /// </summary>
    public class DrawRules
    {
        /// <summary>
        /// Number of cards drawn at once (1 or 3 for Klondike)
        /// </summary>
        [JsonPropertyName("drawCount")]
        public int DrawCount { get; set; } = 1;

        /// <summary>
        /// Whether redeals are allowed and how many ("unlimited", "none", or a number)
        /// </summary>
        [JsonPropertyName("redeals")]
        public string Redeals { get; set; } = "unlimited";
    }

    /// <summary>
    /// Metadata about the game configuration
    /// </summary>
    public class GameMetadata
    {
        /// <summary>
        /// Author of the configuration
        /// </summary>
        [JsonPropertyName("author")]
        public string Author { get; set; } = "samsmithnz";

        /// <summary>
        /// Version of the configuration
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Date the configuration was created or last updated
        /// </summary>
        [JsonPropertyName("dateCreated")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Description of the game variant
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}