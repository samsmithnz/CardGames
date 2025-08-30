# Solitaire Game Configuration Usage

## Overview

The CardGames project now supports configurable game rules through JSON configuration files. This allows for:

- Easy modification of game rules without code changes
- Support for multiple solitaire variants (Klondike, Freecell, etc.)
- Extensible architecture for future game types

## Basic Usage

### Default Klondike Solitaire
```csharp
// Creates a Klondike Solitaire game with default rules
SolitaireRules rules = new SolitaireRules();
```

### Specific Game Variant
```csharp
// Load Klondike Solitaire explicitly
SolitaireRules klondike = new SolitaireRules("Klondike Solitaire");

// Load Freecell variant
SolitaireRules freecell = new SolitaireRules("Freecell");
```

### Custom Configuration
```csharp
// Create custom game configuration
GameDefinition customGame = new GameDefinition
{
    GameName = "My Custom Solitaire",
    Piles = new PileConfiguration { Tableau = 5, Foundation = 4 },
    InitialLayout = new InitialLayout 
    { 
        Tableau = new List<int> { 1, 2, 3, 4, 5 }
    },
    MovementRules = new MovementRules
    {
        EmptyTableau = "any card",
        TableauToTableau = "descending, same suit"
    }
};

SolitaireRules customRules = new SolitaireRules(customGame);
```

## Configuration Structure

The JSON configuration file defines complete game rules:

```json
{
  "games": [
    {
      "gameName": "Klondike Solitaire",
      "decks": 1,
      "piles": {
        "tableau": 7,
        "foundation": 4,
        "waste": 1,
        "freecells": 0
      },
      "initialLayout": {
        "tableau": [1, 2, 3, 4, 5, 6, 7],
        "faceUp": [false, false, false, false, false, false, true]
      },
      "movementRules": {
        "tableauToTableau": "descending, alternating colors",
        "tableauToFoundation": "ascending, same suit",
        "emptyTableau": "kings only",
        "emptyFoundation": "aces only"
      },
      "drawRules": {
        "drawCount": 1,
        "redeals": "unlimited"
      },
      "winCondition": "all cards in foundations",
      "scoring": "standard"
    }
  ]
}
```

## Game Configuration Properties

### Game Identification
- `gameName`: Unique identifier for the game variant
- `decks`: Number of standard 52-card decks used
- `metadata`: Author, version, description information

### Pile Configuration
- `tableau`: Number of tableau columns (main playing area)
- `foundation`: Number of foundation piles (goal piles)
- `waste`: Number of waste piles (for stock draws)
- `freecells`: Number of free cells (for temporary storage)

### Initial Layout
- `tableau`: Array defining how many cards go in each tableau column
- `faceUp`: Array defining which cards start face-up

### Movement Rules
- `tableauToTableau`: How cards move between tableau columns
- `tableauToFoundation`: How cards move to foundation piles
- `emptyTableau`: What cards can be placed on empty tableau spots
- `emptyFoundation`: What cards can be placed on empty foundations

### Draw Rules
- `drawCount`: How many cards drawn from stock at once
- `redeals`: Whether stock can be recycled ("unlimited", "none", or number)

## Supported Game Variants

### Klondike Solitaire (Default)
- 7 tableau columns (1,2,3,4,5,6,7 cards)
- 4 foundation piles
- 1 waste pile
- Draw 1 card, unlimited redeals
- Kings only on empty tableau
- Aces only on empty foundations

### Freecell
- 8 tableau columns (7,7,7,7,6,6,6,6 cards)
- 4 foundation piles
- 4 free cells
- No stock/waste mechanics
- Any card on empty tableau
- Aces only on empty foundations

## Adding New Game Variants

To add a new solitaire variant:

1. Define the game rules in the JSON configuration file
2. Add the new game object to the "games" array
3. The game will automatically be available using its `gameName`

Example for Spider Solitaire:
```json
{
  "gameName": "Spider Solitaire",
  "decks": 2,
  "piles": {
    "tableau": 10,
    "foundation": 8,
    "waste": 0
  },
  "movementRules": {
    "tableauToTableau": "descending, any suit",
    "emptyTableau": "any card"
  }
}
```

## Backward Compatibility

All existing code continues to work unchanged:
- `new SolitaireRules()` still creates standard Klondike Solitaire
- All existing methods and properties work exactly as before
- No breaking changes to the API

The configuration system is additive - it extends functionality without changing existing behavior.