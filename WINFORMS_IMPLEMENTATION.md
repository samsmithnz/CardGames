# CardGames.WinForms Implementation

## Overview

The CardGames.WinForms project provides a Windows Forms-based user interface for playing card games, specifically implementing Klondike Solitaire. This project follows the same clean architecture principles as the WPF version, separating UI concerns from game logic.

## Project Structure

```
CardGames.WinForms/
├── CardGames.WinForms.csproj    # Project file with Windows Forms configuration
├── Program.cs                   # Application entry point
├── MainForm.cs                  # Main game form with solitaire layout
├── CardUserControl.cs           # Windows Forms card display control
├── Images/                      # Playing card images (53 files)
│   ├── 1920px-Playing_card_*.png  # 52 individual card face images
│   └── cardback1.jpg              # Card back image
└── playingcards.ico            # Application icon
```

## Key Features

### CardUserControl
- Custom Windows Forms UserControl for displaying playing cards
- Supports both face-up and face-down display
- Implements drag-and-drop functionality for card movement
- Embedded resource loading for card images
- Fallback text rendering when images are unavailable
- Events for card interactions (click, drag start, drop validation)

### MainForm
- Complete solitaire game layout with proper positioning
- Stock and waste pile controls
- 4 foundation piles (one per suit)
- 7 tableau columns with overlapping card display
- New Game and Draw Card buttons
- Status bar for game feedback
- Responsive layout that adjusts to window resizing

### Game Integration
- Uses CardGames.Core for all game logic and rules
- Maintains face-up/face-down state for tableau cards
- Implements proper solitaire rule validation
- Handles card movement between different game areas

## User Interface Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│  Solitaire - Card Games (Windows Forms)                    [_][□][X] │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  [Stock]   [Waste]                         [♥][♣][♦][♠]            │
│                                            Foundation Piles          │
│                                                                     │
│                                                                     │
│    [1]      [2]      [3]      [4]      [5]      [6]      [7]      │
│     □        □        □        □        □        □        □        │
│              □        □        □        □        □        □        │
│                       □        □        □        □        □        │
│                                □        □        □        □        │
│                                         □        □        □        │
│                                                  □        □        │
│                                                           □        │
│                         Tableau Columns                            │
│                                                                     │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│ [New Game] [Draw Card]                                             │
├─────────────────────────────────────────────────────────────────────┤
│ Welcome to Solitaire!                                              │
└─────────────────────────────────────────────────────────────────────┘
```

## Technical Implementation

### Architecture Compliance
- Follows clean architecture with UI separated from game logic
- Uses dependency injection pattern for game rules
- Maintains consistent coding standards (explicit types, curly braces)
- Implements proper event-driven architecture

### Card Display
- Uses embedded resources for reliable image loading
- Implements fallback rendering for missing images
- Maintains consistent card sizing using CardVisualConstants
- Supports both programmatic and user-initiated card flipping

### Interaction Model
- Click-to-flip for face-down tableau cards
- Drag-and-drop for moving cards between piles
- Validation events for legal move checking
- Status feedback for all user actions

## Building and Running

```bash
# Build the Windows Forms project
dotnet build src/CardGames.WinForms/CardGames.WinForms.csproj

# Run the application (Windows only)
dotnet run --project src/CardGames.WinForms/CardGames.WinForms.csproj
```

## Comparison with WPF Version

| Feature | WPF | Windows Forms |
|---------|-----|---------------|
| Framework | WPF/XAML | Windows Forms |
| Card Display | XAML-based | Custom Paint |
| Drag/Drop | WPF DragDrop | Windows Forms DragDrop |
| Layout | Canvas/Grid | Panel positioning |
| Styling | XAML Styles | Code-based styling |
| Resource Loading | Pack URIs | Embedded Resources |
| Performance | GPU-accelerated | GDI+ rendering |

Both implementations provide the same game functionality while using different UI technologies, allowing users to choose their preferred Windows desktop experience.