# Freecell Requirements

## Game Overview

Freecell is a solitaire card game where nearly every deal is solvable with perfect play. Unlike Klondike Solitaire, all cards are dealt face-up from the beginning, and there is no stock pile to draw from. The challenge lies in strategic planning and using the limited free cells effectively.

## Game Setup

### Initial Layout
- **Deck**: One standard 52-card deck
- **Tableau**: 8 columns of cards
  - Columns 1-4: 7 cards each (28 cards total)
  - Columns 5-8: 6 cards each (24 cards total)
  - All cards are dealt face-up
- **Foundation Piles**: 4 empty piles (one for each suit)
- **Free Cells**: 4 empty cells where individual cards can be temporarily stored
- **Stock/Waste Piles**: Not used in Freecell

### Pile Configuration
```
[Foundation] [Foundation] [Foundation] [Foundation]
[Free Cell]  [Free Cell]  [Free Cell]  [Free Cell]

[Tableau 1] [Tableau 2] [Tableau 3] [Tableau 4] [Tableau 5] [Tableau 6] [Tableau 7] [Tableau 8]
```

## Game Rules

### Card Movement Rules

#### Tableau to Tableau
- Cards can be moved from one tableau column to another
- Cards must be placed in descending rank order
- Cards must alternate colors (red on black, black on red)
- Only one card can be moved at a time UNLESS there are empty free cells or empty tableau columns that allow for sequence moves
- The number of cards that can be moved as a sequence is limited by: `(1 + number of empty free cells) × 2^(number of empty tableau columns)`

#### Tableau to Foundation
- Cards can be moved from tableau to foundation piles
- Foundation piles are built up by suit from Ace to King (A, 2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K)
- Only the top card of a tableau column can be moved to foundation

#### Free Cell Operations
- **Tableau to Free Cell**: Any single top card from a tableau column can be moved to an empty free cell
- **Free Cell to Tableau**: A card in a free cell can be moved to an appropriate tableau column following standard tableau rules
- **Free Cell to Foundation**: A card in a free cell can be moved to the appropriate foundation pile if it's the next card in sequence

#### Empty Tableau Columns
- Any single card (of any rank) can be placed on an empty tableau column
- Empty tableau columns are valuable as they double the number of cards that can be moved as a sequence

#### Empty Foundation Piles
- Only Aces can be placed on empty foundation piles

### Auto-Play Rules
Many Freecell implementations include auto-play features that automatically move cards to foundations when safe:
- A card can be auto-moved to foundation if both cards of the opposite color that are one rank lower are already in foundations
- Example: The 5 of Hearts can be auto-moved if both the 4 of Spades and 4 of Clubs are in foundations

## Winning Conditions

### Primary Win Condition
- All 52 cards are moved to the foundation piles
- Each foundation pile contains 13 cards of the same suit in ascending order (A through K)

### Game Over Conditions
- **Victory**: All cards successfully moved to foundations
- **Defeat**: No more legal moves available (very rare in Freecell - most deals are solvable)

## User Interactions

### Card Selection and Movement
1. **Single Click**: Select a card (highlight it)
2. **Single Click on Valid Target**: Move selected card to target location
3. **Double Click**: Attempt to auto-move card to foundation or best valid location
4. **Drag and Drop**: Alternative method for moving cards

### Valid Move Indicators
- Highlight valid target locations when a card is selected
- Show visual feedback for invalid moves
- Provide clear indication of available free cells

### Undo Functionality
- Allow unlimited undo operations
- Maintain move history for the entire game
- Undo should reverse the last move completely

## Scoring System

### Standard Scoring (Optional)
- **No Scoring**: Many Freecell implementations don't use scoring, focusing on completion
- **Move Count**: Track the number of moves taken to complete the game
- **Time**: Track the time taken to complete the game

### Statistics Tracking
- Games won/lost
- Win percentage
- Average completion time
- Best completion time
- Current winning/losing streak

## Special Features

### Deal Numbers
- Each game should have a unique deal number that can be replayed
- Players should be able to input a specific deal number to replay the same card arrangement
- Deal numbers ensure reproducible games for comparison and sharing

### Hints System (Optional)
- Suggest next possible moves
- Highlight cards that can be safely moved to foundations
- Warn about potentially blocking moves

### Game Configuration Options
- Auto-move cards to foundation when safe
- Right-click to send cards to foundation
- Animation speed settings
- Card back designs
- Sound effects on/off

## Technical Implementation Requirements

### Game State Management
- Track all card positions (tableau, foundations, free cells)
- Maintain move history for undo functionality
- Validate all moves according to Freecell rules
- Detect win/lose conditions

### Configuration Integration
- Use existing `SolitaireGameConfig` system
- Freecell configuration already exists in `solitaire-config.json`:
  - 8 tableau columns
  - 4 foundation piles
  - 4 free cells
  - No stock/waste piles
  - Initial layout: [7,7,7,7,6,6,6,6] cards per column
  - All cards face-up initially

### Movement Validation
- Implement sequence move calculation based on available free cells and empty columns
- Validate color alternation and descending rank for tableau moves
- Validate ascending rank and same suit for foundation moves
- Enforce free cell capacity limits (maximum 4 cards)

### User Interface Requirements
- Clear visual distinction between different pile types
- Intuitive drag-and-drop or click-to-move interface
- Status indicators showing:
  - Number of free cells available
  - Number of empty tableau columns
  - Move count
  - Game timer
- Menu options for new game, restart, undo, and settings

## Differences from Klondike Solitaire

| Feature | Klondike | Freecell |
|---------|----------|----------|
| Initial Setup | 7 columns, some face-down | 8 columns, all face-up |
| Stock Pile | Yes, draw cards | No stock pile |
| Waste Pile | Yes | No waste pile |
| Free Cells | None | 4 free cells |
| Sequence Moves | Multiple cards always allowed | Limited by free cells/empty columns |
| Empty Tableau | Kings only | Any card |
| Solvability | ~79% of deals | ~99.99% of deals |

## Future Enhancements

### Variants
- **Eight Off**: 8 free cells instead of 4
- **Baker's Game**: Building down by suit instead of alternating colors
- **Forecell**: Only 3 free cells

### Advanced Features
- Online multiplayer comparisons using deal numbers
- Tournament mode with preset deal sequences
- Advanced statistics and analytics
- Custom card themes and animations