# Card Sequence Movement Fix for Issue #85

## Problem Description

When dragging a card from a tableau column in Solitaire, only that single card would move instead of the entire sequence of face-up cards below it. For example:

- Column 3 has: 6♥, 5♠, 4♥ (all face-up)
- Column 7 has: 7♠ (face-up)
- Dragging the 6♥ to the 7♠ should move all three cards (6♥, 5♠, 4♥) as a sequence

**Symptoms observed:**
- Sometimes nothing moves
- Sometimes the red 6 is duplicated

## Root Cause

The original `ExecuteMove` and `RemoveCardFromSource` methods only handled individual cards:

```csharp
// Old logic - only moves single card
RemoveCardFromSource(sourceControl, card);
solitaireRules.TableauColumns[targetColumnIndex].Add(card);
```

## Solution Overview

The fix implements sequence detection and movement logic:

1. **Sequence Detection**: `GetCardSequenceToMove()` identifies valid card sequences
2. **Sequence Removal**: `RemoveCardSequenceFromSource()` removes multiple cards properly  
3. **Sequence Addition**: `AddCardSequenceToTableau()` adds multiple cards with face-up state tracking
4. **Enhanced ExecuteMove**: Uses sequence logic for tableau moves, single-card logic for others

## Key Implementation Details

### Sequence Detection Logic

```csharp
private List<Card> GetCardSequenceToMove(CardUserControl sourceControl, Card draggedCard)
{
    // 1. Find the dragged card's position in the tableau column
    // 2. Check all cards from that position to the end
    // 3. Verify all cards are face-up
    // 4. Verify they form a valid descending sequence with alternating colors
    // 5. Return full sequence if valid, otherwise return single card
}
```

### Valid Sequence Requirements

A valid sequence must have:
- All cards face-up
- Descending rank order (6, 5, 4)
- Alternating colors (Red, Black, Red)

### Removal Order

Cards are removed from the source column in the correct order (top to bottom):
1. Remove 4♥ (top card)
2. Remove 5♠ (now top card) 
3. Remove 6♥ (now top card)

### Addition Order  

Cards are added to the target column in the correct order (bottom to top):
1. Add 6♥ (goes on 7♠)
2. Add 5♠ (goes on 6♥)
3. Add 4♥ (goes on 5♠)

## Backward Compatibility

The fix maintains full backward compatibility:
- Single card moves work exactly as before
- Foundation moves remain single-card only
- Non-tableau moves remain single-card only
- Only tableau-to-tableau moves gain sequence capability

## Test Coverage

Added comprehensive tests covering:
- Single card moves
- Valid multi-card sequences
- Invalid sequences (fall back to single card)
- Mixed face-up/face-down scenarios
- Correct removal and addition order

## Files Modified

1. **MainWindow.xaml.cs**: 
   - Added `GetCardSequenceToMove()`
   - Added `RemoveCardSequenceFromSource()`
   - Added `AddCardSequenceToTableau()`
   - Modified `ExecuteMove()` to use sequence logic
   - Enhanced `ValidateMoveDetailed()` for debugging

2. **CardSequenceMoveTests.cs**: New test file for sequence logic

3. **CardSequenceRemovalTests.cs**: New test file for removal/addition order