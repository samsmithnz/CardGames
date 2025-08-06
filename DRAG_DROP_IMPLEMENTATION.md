# Drag-and-Drop Card Movement Implementation

This document describes the implementation of drag-and-drop functionality for card movement in the CardGames WinForms application.

## Overview

The drag-and-drop system provides intuitive card movement with visual feedback and rule validation based on Solitaire game rules.

## Features

### 1. **Visual Feedback**
- **Hand Cursor**: Appears when starting a drag operation
- **Drop Zone Colors**: 
  - Light Blue: Valid drop location
  - Light Coral: Invalid drop location  
  - Transparent: Default state

### 2. **Rule Validation**
- **Foundation Piles (Ace Piles)**: 
  - Only Aces can be placed on empty piles
  - Cards must be placed in ascending order (A, 2, 3, ..., K)
  - Cards must be of the same suit
- **Playing Areas**: 
  - Only Kings can be placed on empty piles
  - Cards must be placed in descending order (K, Q, J, ..., A)
  - Cards must alternate colors (Red on Black, Black on Red)

### 3. **Panel-Specific Behavior**
- **Foundation Piles**: Cards stack on top of each other
- **Playing Areas**: Cards cascade with visible offsets
- **Discard Pile**: Accepts any card
- **Deck**: Does not accept dropped cards

## Implementation Details

### Core Classes

#### SolitaireRules
```csharp
public bool CanPlaceOnFoundation(Card cardToPlace, Card targetCard)
public bool CanPlaceOnPlayingArea(Card cardToPlace, Card targetCard)
```

#### CardUserControl
- Stores reference to Card object
- Initiates drag operations on mouse down
- Provides visual feedback during drag

#### MainForm
- Manages drag-and-drop for all game panels
- Validates moves using SolitaireRules
- Positions cards appropriately in target panels

### Key Methods

#### Move Validation
```csharp
private bool IsValidMove(CardUserControl draggedCard, Panel targetPanel)
```
Validates moves based on panel type and Solitaire rules.

#### Visual Feedback
```csharp
private void Panel_DragEnter(object sender, DragEventArgs e)
private void Panel_DragLeave(object sender, EventArgs e)
```
Provides color-coded feedback for valid/invalid drop zones.

#### Card Positioning
```csharp
private void PositionCardInPanel(CardUserControl card, Panel panel)
```
Positions cards with appropriate stacking or cascading based on panel type.

## Usage

### For Players
1. **Click and drag** any visible card to move it
2. **Observe color feedback** while dragging to see valid drop zones
3. **Drop on valid locations** to complete the move
4. **Invalid moves will be rejected** and the card returns to its original position

### For Developers

#### Adding New Game Rules
1. Extend the `SolitaireRules` class with new validation methods
2. Update `IsValidMove()` in MainForm to handle new panel types
3. Add appropriate positioning logic in `PositionCardInPanel()`

#### Customizing Visual Feedback
Modify the colors in `Panel_DragEnter()`:
```csharp
targetPanel.BackColor = Color.YourColor; // For valid/invalid feedback
```

#### Adding New Panel Types
1. Add panels to the `gamePanels` array in `SetupDragDropForPanels()`
2. Set appropriate `Tag` values for panel identification
3. Update validation logic in `IsValidMove()`

## Testing

The implementation includes comprehensive unit tests:
- **SolitaireRulesTests**: Validates all rule logic
- **DragDropIntegrationTests**: Tests complete game scenarios
- **DeckTests**: Validates basic card deck functionality

Run tests with:
```bash
cd src/CardGames.Tests
dotnet test
```

## Technical Notes

### Resolved Issues
- **Fixed data passing**: DoDragDrop now passes CardUserControl instead of sender
- **Fixed file paths**: Removed hardcoded paths, using relative paths
- **Added null checking**: Comprehensive validation of card and panel references
- **Improved error handling**: Graceful handling of invalid operations

### Performance Considerations
- Drag operations are initiated only on left mouse button
- Visual feedback is applied/removed efficiently
- Card positioning calculations are optimized for each panel type

### Compatibility
- Compatible with .NET Framework and .NET Core
- Follows WinForms best practices
- Maintains existing application functionality

## Future Enhancements

Potential improvements:
- **Animated card movement** between panels
- **Sound effects** for valid/invalid moves
- **Undo/Redo functionality** for moves
- **Multi-card selection** and movement
- **Keyboard shortcuts** for common operations