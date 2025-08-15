# Solitaire (Klondike) Rules

## Overview

Solitaire, also known as Klondike Solitaire, is a classic single-player card game where the objective is to move all cards to four foundation piles, building each suit from Ace to King. This implementation follows the traditional Klondike rules.

## Game Components

### Tableau Columns (7 columns)
- Seven columns where cards are initially dealt
- Cards are built in descending order with alternating colors
- In traditional play, only the top card of each column is visible and available for play
- Empty columns can only accept Kings

### Foundation Piles (4 piles)
- Four piles where cards are built up by suit from Ace to King
- Each foundation represents one suit (Hearts, Diamonds, Clubs, Spades)
- Cards must be placed in ascending order: A, 2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K
- Only Aces can be placed on empty foundation piles

### Stock Pile
- Contains cards that haven't been dealt to the tableau
- Cards can be drawn from the stock to the waste pile
- When the stock pile is empty, the waste pile can be reset back to the stock

### Waste Pile
- Contains cards drawn from the stock pile
- Only the top card is available for play
- Cards are drawn one at a time from the stock

## Game Setup

1. **Initial Deal**: Cards are dealt to the tableau columns as follows:
   - Column 1: 1 card
   - Column 2: 2 cards
   - Column 3: 3 cards
   - Column 4: 4 cards
   - Column 5: 5 cards
   - Column 6: 6 cards
   - Column 7: 7 cards
   
   *Note: In traditional Solitaire, only the top card of each column is face-up initially, with all others face-down. The face-up/face-down state is handled by the user interface.*

2. **Remaining Cards**: The remaining 24 cards form the stock pile

3. **Foundation Piles**: Start empty

4. **Waste Pile**: Starts empty

## How to Play

### Objective
Move all 52 cards to the four foundation piles, with each foundation containing one complete suit from Ace to King.

### Valid Moves

#### Tableau Rules
- **Building Down**: Cards can be placed on tableau columns in descending order
- **Alternating Colors**: Cards must alternate between red (Hearts, Diamonds) and black (Clubs, Spades)
- **Example**: A black 7 can be placed on a red 8
- **Empty Columns**: Only Kings can be placed on empty tableau columns

#### Foundation Rules
- **Building Up**: Cards must be placed in ascending order by suit
- **Same Suit**: Only cards of the same suit can be placed on each foundation
- **Starting Card**: Only Aces can be placed on empty foundation piles
- **Example**: The 5 of Hearts can only be placed on the 4 of Hearts

#### Stock and Waste Pile
- **Drawing Cards**: Click the stock pile to draw a card to the waste pile
- **Playing from Waste**: The top card of the waste pile is available for play
- **Resetting Stock**: When the stock pile is empty, the waste pile automatically resets back to the stock

### Winning the Game
The game is won when all four foundation piles are complete (each containing 13 cards from Ace to King in the same suit).

## Strategy Tips

1. **Uncover Hidden Cards**: In traditional Solitaire, prioritize moves that reveal face-down cards in the tableau
2. **Empty Columns**: Try to create empty tableau columns to place Kings
3. **Foundation Building**: Build foundations evenly rather than focusing on one suit
4. **Stock Management**: Be strategic about when to draw from the stock pile
5. **Color Awareness**: Pay attention to color patterns when building tableau sequences

## Implementation Details

This Solitaire implementation follows these specific rules:

- **Card Drawing**: Cards are drawn one at a time from the stock to the waste pile
- **Stock Reset**: When the stock is empty, all waste pile cards are moved back to the stock in reverse order
- **Move Validation**: All moves are validated according to traditional Klondike rules
- **Game State**: The game tracks all pile states and validates winning conditions

## Technical Notes

The game logic is implemented in the `SolitaireRules` class with the following key methods:

- `DealCards()`: Sets up the initial game state
- `CanPlaceCardOnTableau()`: Validates tableau moves
- `CanPlaceCardOnFoundation()`: Validates foundation moves
- `DrawFromStock()`: Handles stock pile card drawing
- `IsGameWon()`: Checks if the game has been completed

All moves follow the traditional Klondike Solitaire rules as implemented in the core game logic.