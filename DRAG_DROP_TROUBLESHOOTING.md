# Drag and Drop Troubleshooting Guide

## Overview

This enhancement provides comprehensive troubleshooting capabilities for drag and drop operations in the CardGames Solitaire application. If you're experiencing issues where certain cards can't be grabbed or dragged (especially cards that are partially covered by other cards), these features will help you understand what's happening.

## Common Issues

### Problem: "I can't grab cards that have other cards partially stacked on it"
This was caused by:
1. **Hit Testing Overlap**: The full 80×120 pixel card areas overlapped, even when only 20px was visible
2. **Incorrect Hit Area**: Cards responded to clicks anywhere in their full area, not just the visible portion
3. **Z-Index Priority**: WPF prioritized the topmost visual element for mouse events

**✅ FIXED**: The system now restricts hit testing to only the visible portion of each card:
- **Stacked cards**: Only respond to clicks in their visible 20px strip  
- **Top cards**: Respond to clicks in their full 80×120px area
- **Debug mode**: Shows the correct, clipped hit areas with yellow borders

### Problem: "I can always move cards with no stacks"
This is expected behavior - cards without stacks have their full 80×120 pixel area available for mouse interaction.

## Troubleshooting Features

### 1. Debug Mode Toggle

**How to Enable**: Press `Ctrl+D` while the game window has focus

**What it does**:
- Adds colored borders around cards to show their **actual clickable boundaries** (not full card size)
- Shows visual indicators for draggable vs non-draggable cards:
  - **Light Green Background**: Face-up cards (draggable)
  - **Light Red Background**: Face-down cards (non-draggable)
- **Yellow Border with Clipping**: Shows the exact hit area for each card
  - **Stacked cards**: Clipped to show only the visible 20px strip
  - **Top cards**: Shows full 80×120px area
  - **Foundation/Stock/Waste**: Shows full card area

### 2. Enhanced Debug Logging

When debug mode is enabled, detailed log messages are written to the Visual Studio Output window (Debug category). These logs show:

**Mouse Events**:
```
CardControl(Tableau[col=2, row=1]): MouseDown - Button: Pressed, ClickCount: 1, IsFaceUp: True, Position: (15.2, 8.5)
CardControl(Tableau[col=2, row=1]): Drag start point recorded: (15.2, 8.5)
CardControl(Tableau[col=2, row=1]): Debug: Visible hit area clipped to 80.0 × 20.0 (was 80.0 × 120.0)
CardControl(Tableau[col=2, row=1]): MouseMove - Current: (18.4, 18.0), Distance: (3.2, 9.5), MinRequired: (4.0, 4.0)
CardControl(Tableau[col=2, row=1]): Drag threshold exceeded - starting drag operation
```

**Hit Area Validation**:
```
CardControl(Tableau[col=2, row=2]): MouseDown - Button: Pressed, Position: (25.0, 95.0)
CardControl(Tableau[col=2, row=2]): MouseDown ignored - Click outside visible area (visible height: 20.0)
```

**Drag and Drop Events**:
```
CardControl(Tableau[col=4, row=1]): DragEnter with card: 6 of Spades
CardControl(Tableau[col=4, row=1]): Drop validation result: True
CardControl(Tableau[col=4, row=1]): Drop with card: 6 of Spades
```

### 3. Visual Debugging Information

**Card Boundaries**: Yellow borders show the exact clickable area of each card
**Draggable State**: Background colors indicate whether a card can be dragged
**Hit Area Visualization**: See exactly which pixels are responsive to mouse events

## Interpreting Debug Information

### Understanding Mouse Event Logs

1. **MouseDown Event**: Shows if the initial click was registered correctly
   - Check if `IsFaceUp: True` for cards you expect to be draggable
   - Verify the button state is `Pressed`

2. **Drag Threshold Calculation**: Shows the mouse movement distance vs required threshold
   - `Distance: (X, Y)` - how far the mouse moved
   - `MinRequired: (X, Y)` - system minimum for drag detection (usually 4px each direction)

3. **Drag Operation Flow**: Follow the complete sequence from MouseDown to Drop

### Common Debug Scenarios

**Scenario 1: Card won't start dragging**
Look for:
- `MouseDown ignored - Card is face down` → Card needs to be flipped
- `MouseMove ignored - No card present` → Empty control clicked
- Missing "Drag threshold exceeded" → Mouse didn't move far enough

**Scenario 2: Drag starts but drop fails**
Look for:
- `Drop validation result: False` → Move violates game rules
- Check the validation details in the main debug log

**Scenario 3: Wrong card responds to mouse**
- Use visual borders to see which card is actually receiving events
- Check Z-index values in the logs to understand layering

## Best Practices for Grabbing Stacked Cards

1. **Target the Visible Area**: Aim for the 20px strip that's visible at the bottom of partially covered cards
2. **Use Precise Movements**: Small, deliberate mouse movements work better than quick gestures
3. **Enable Debug Mode**: Use `Ctrl+D` to see exactly which areas are clickable
4. **Check Card State**: Ensure the card you're trying to grab is face-up (green background in debug mode)

## Technical Details

### Card Stacking Layout
- **Card Dimensions**: 80px wide × 120px high
- **Stacking Offset**: 20px vertical offset between stacked cards
- **Visibility Ratio**: 16.7% of underlying cards remain visible
- **Hit Area**: The visible 20px strip is the clickable area for partially covered cards

### Event Priority
1. WPF processes mouse events based on visual hierarchy (Z-index)
2. Cards higher in the stack have higher Z-index values
3. The hit testing system prioritizes the topmost visual element
4. Debug mode helps visualize this hierarchy

## Troubleshooting Workflow

1. **Enable Debug Mode**: Press `Ctrl+D` to activate visual debugging
2. **Identify the Problem**: Look at the colored borders and backgrounds
3. **Check the Logs**: Monitor the Output window for detailed event information
4. **Verify Card State**: Ensure target cards are draggable (face-up, green background)
5. **Test Mouse Targeting**: Try clicking different areas of the visible card portion
6. **Review Event Flow**: Follow the complete mouse event sequence in the logs

## Disabling Debug Mode

- Press `Ctrl+D` again to toggle debug mode off
- All visual indicators will be hidden
- Debug logging will stop
- Normal game appearance is restored

---

This troubleshooting system provides comprehensive visibility into the drag and drop mechanics, helping users understand why certain cards might be difficult to grab and how to work around these limitations.