using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CardGames.Core
{
    /// <summary>
    /// Simple DTO to represent a card value in serialized state.
    /// </summary>
    public class CardDto
    {
        public Card.CardNumber Number { get; set; }
        public Card.CardSuite Suite { get; set; }
    }

    /// <summary>
    /// Serializable game state for Solitaire. Captures all piles/columns.
    /// Includes optional UI hints like TableauFaceUpStates to preserve UI state.
    /// </summary>
    public class SolitaireState
    {
        public List<CardDto> StockPile { get; set; } = new List<CardDto>();
        public List<CardDto> WastePile { get; set; } = new List<CardDto>();
        public List<List<CardDto>> FoundationPiles { get; set; } = new List<List<CardDto>>();
        public List<List<CardDto>> TableauColumns { get; set; } = new List<List<CardDto>>();

        /// <summary>
        /// Free cells for temporary card storage (used in games like Freecell)
        /// Null values represent empty free cells
        /// </summary>
        public List<CardDto?> FreeCells { get; set; } = new List<CardDto?>();

        /// <summary>
        /// Optional: UI face-up flags for each tableau column (parallel to TableauColumns).
        /// Each inner list should have the same length as the corresponding TableauColumns entry.
        /// </summary>
        public List<List<bool>> TableauFaceUpStates { get; set; } = new List<List<bool>>();

        /// <summary>
        /// The name of the game configuration that was used when this state was saved.
        /// This allows the loader to set up the correct game board configuration.
        /// </summary>
        public string? GameName { get; set; }

        public string? Note { get; set; }

        public string ToJson(bool indented = true)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = indented
            };
            return JsonSerializer.Serialize(this, options);
        }

        public static SolitaireState FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON content is empty", nameof(json));
            }

            SolitaireState? state = JsonSerializer.Deserialize<SolitaireState>(json);
            if (state == null)
            {
                throw new InvalidOperationException("Failed to deserialize SolitaireState");
            }

            return state;
        }
    }

    /// <summary>
    /// Extension methods to convert between SolitaireRules and a serializable state.
    /// </summary>
    public static class SolitaireStateExtensions
    {
        private static CardDto ToDto(this Card card)
        {
            return new CardDto { Number = card.Number, Suite = card.Suite };
        }

        private static Card FromDto(this CardDto dto)
        {
            return new Card { Number = dto.Number, Suite = dto.Suite };
        }

        /// <summary>
        /// Export current rules data into a serializable state. Order of cards in each list is preserved.
        /// </summary>
        public static SolitaireState ExportState(this SolitaireRules rules, string? note = null)
        {
            SolitaireState state = new SolitaireState();

            // Stock pile
            foreach (Card c in rules.StockPile)
            {
                state.StockPile.Add(c.ToDto());
            }

            // Waste pile
            foreach (Card c in rules.WastePile)
            {
                state.WastePile.Add(c.ToDto());
            }

            // Foundations (4 piles)
            for (int i = 0; i < rules.FoundationPiles.Count; i++)
            {
                List<CardDto> pile = new List<CardDto>();
                foreach (Card c in rules.FoundationPiles[i])
                {
                    pile.Add(c.ToDto());
                }
                state.FoundationPiles.Add(pile);
            }

            // Tableau columns
            for (int col = 0; col < rules.TableauColumns.Count; col++)
            {
                List<CardDto> column = new List<CardDto>();
                foreach (Card c in rules.TableauColumns[col])
                {
                    column.Add(c.ToDto());
                }
                state.TableauColumns.Add(column);
                state.TableauFaceUpStates.Add(new List<bool>()); // UI will populate if needed
            }

            // Free cells
            for (int i = 0; i < rules.FreeCells.Count; i++)
            {
                Card c = rules.FreeCells[i];
                if (c != null)
                {
                    state.FreeCells.Add(c.ToDto());
                }
                else
                {
                    state.FreeCells.Add(null);
                }
            }

            // Store the game configuration name for proper loading
            state.GameName = rules.GameConfig?.GameName;
            state.Note = note;
            return state;
        }

        /// <summary>
        /// Import the given state into the rules instance. All piles/columns are replaced.
        /// </summary>
        public static void ImportState(this SolitaireRules rules, SolitaireState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            // Clear existing
            rules.StockPile.Clear();
            rules.WastePile.Clear();
            for (int i = 0; i < rules.FoundationPiles.Count; i++)
            {
                rules.FoundationPiles[i].Clear();
            }
            for (int col = 0; col < rules.TableauColumns.Count; col++)
            {
                rules.TableauColumns[col].Clear();
            }
            for (int i = 0; i < rules.FreeCells.Count; i++)
            {
                rules.FreeCells[i] = null;
            }

            // Load stock
            foreach (CardDto dto in state.StockPile)
            {
                rules.StockPile.Add(dto.FromDto());
            }

            // Load waste
            foreach (CardDto dto in state.WastePile)
            {
                rules.WastePile.Add(dto.FromDto());
            }

            // Load foundations
            for (int i = 0; i < state.FoundationPiles.Count && i < rules.FoundationPiles.Count; i++)
            {
                foreach (CardDto dto in state.FoundationPiles[i])
                {
                    rules.FoundationPiles[i].Add(dto.FromDto());
                }
            }

            // Load tableau
            for (int col = 0; col < state.TableauColumns.Count && col < rules.TableauColumns.Count; col++)
            {
                foreach (CardDto dto in state.TableauColumns[col])
                {
                    rules.TableauColumns[col].Add(dto.FromDto());
                }
            }

            // Load free cells
            for (int i = 0; i < state.FreeCells.Count && i < rules.FreeCells.Count; i++)
            {
                CardDto? dto = state.FreeCells[i];
                if (dto != null)
                {
                    rules.FreeCells[i] = dto.FromDto();
                }
                else
                {
                    rules.FreeCells[i] = null;
                }
            }
        }
    }
}
