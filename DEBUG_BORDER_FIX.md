# Debug Border Color Fix - Resolving Overlapping Visual Indicators

## Problem
The debug mode yellow borders were overlapping and indistinguishable when cards were stacked in tableau columns, making it impossible to see which border belonged to which card. This occurred because:

1. **Stacked cards naturally overlap** - Card controls are positioned with 20px vertical offset but have 120px height
2. **All borders were the same color** - Every debug border was yellow, making overlapping borders look like one large border
3. **User confusion** - Overlapping yellow borders made it appear as if hit boxes were incorrect

## Root Cause
```
Card 0: Y=0-120px   (Debug border: Yellow 80×20px at top)
Card 1: Y=20-140px  (Debug border: Yellow 80×20px at top) 
Card 2: Y=40-160px  (Debug border: Yellow 80×20px at top)
```
Result: Multiple identical yellow borders overlapping = visual confusion

## Solution: Color-Coded Debug Borders

### Implementation
1. **Added `StackPosition` property** to `CardUserControl` to track position in stack
2. **Color array for different stack positions**:
   - Position 0 (bottom): Yellow
   - Position 1: Orange  
   - Position 2: Red
   - Position 3: Magenta
   - Position 4: Blue
   - Position 5: Cyan
   - Position 6: LimeGreen
   - Position 7+: Purple (cycles)

3. **Updated `UpdateDebugVisuals()`** to use different colors based on stack position
4. **Set `StackPosition`** during tableau layout in `RefreshTableauColumn()`

### Code Changes

**CardUserControl.xaml.cs:**
```csharp
// New property to track position in stack
public int StackPosition { get; set; } = 0;

// Color array for debug borders  
Color[] debugColors = new Color[]
{
    Colors.Yellow, Colors.Orange, Colors.Red, Colors.Magenta,
    Colors.Blue, Colors.Cyan, Colors.LimeGreen, Colors.Purple
};

// Select color based on stack position
Color borderColor = debugColors[StackPosition % debugColors.Length];
DebugBorder.Stroke = new SolidColorBrush(borderColor);
```

**MainWindow.xaml.cs:**
```csharp
// Set stack position for debug border coloring
control.StackPosition = row;
```

## Result
- **Distinct colored borders** for each stacked card
- **Clear visual separation** even when cards overlap
- **Accurate hit area representation** - borders still show correct 80×20px clickable areas
- **Easy debugging** - users can immediately see which border belongs to which card

## Visual Example
```
Before: All yellow borders overlapping → confusing blob
After:  Yellow(bottom) → Orange → Red → Magenta(top) → clearly distinguishable
```

The debug visualization now accurately represents the stacked card layout while making it easy to distinguish between overlapping hit areas.