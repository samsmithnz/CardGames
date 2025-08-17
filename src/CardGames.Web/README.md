# CardGames.Web

A modern TypeScript/React implementation of classic card games, featuring clean architecture and comprehensive test coverage.

## Overview

This project provides a complete JavaScript/TypeScript translation of the C# CardGames.Core library, maintaining the same clean architecture principles while adding web-specific features.

## Features

- ✅ **Complete TypeScript Game Logic**: Full translation of Card, Deck, and SolitaireRules from C#
- ✅ **Comprehensive Testing**: 58+ unit tests with 93%+ test coverage  
- ✅ **React Integration**: Demonstrates integration with React components
- ✅ **Type Safety**: Full TypeScript support with strict type checking
- ✅ **Clean Architecture**: Separation of game logic from UI components
- ✅ **CI/CD Integration**: Automated testing in GitHub Actions

## Project Structure

```
src/
├── core/                   # Core game logic (TypeScript)
│   ├── Card.ts            # Playing card implementation
│   ├── Deck.ts            # Deck management and shuffling
│   └── SolitaireRules.ts  # Solitaire game rules and logic
├── components/            # React UI components
│   └── SolitaireGame.tsx  # Demo Solitaire component
├── __tests__/            # Unit tests
│   ├── Card.test.ts
│   ├── Deck.test.ts
│   ├── SolitaireRules.test.ts
│   └── SolitaireGame.test.tsx
└── index.ts              # Main exports
```

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm

### Installation

```bash
npm install
```

### Running Tests

```bash
# Run all tests
npm test

# Run tests with coverage
npm run test:coverage

# Run tests in watch mode
npm run test:watch
```

### Building

```bash
# Build TypeScript to JavaScript
npm run build
```

## Core Game Logic

The core game logic is implemented in TypeScript and provides the same functionality as the C# version:

### Card Class
- Represents playing cards with number/rank and suit
- Provides color checking (red/black) and numeric value methods
- Full equality comparison support

### Deck Class  
- Standard 52-card deck implementation
- Fisher-Yates shuffle algorithm
- Deal/add card functionality

### SolitaireRules Class
- Complete Klondike Solitaire rule implementation
- Card placement validation for tableau and foundation piles
- Stock/waste pile mechanics with auto-reset
- Win condition detection

## Example Usage

```typescript
import { SolitaireRules, Deck, Card, CardNumber, CardSuite } from './core';

// Create and start a new game
const game = new SolitaireRules();
const deck = new Deck();
deck.shuffle();
game.dealCards(deck);

// Check if a move is valid
const card = new Card(CardNumber._7, CardSuite.Heart);
const canPlace = game.canPlaceCardOnTableau(card, 0);

// Draw from stock pile
const success = game.drawFromStock();

// Check win condition
const isWon = game.isGameWon();
```

## Testing

The project includes comprehensive unit tests covering:

- **Card functionality**: Creation, equality, color checking, numeric values
- **Deck operations**: Shuffling, dealing, card management
- **Solitaire rules**: All game logic, move validation, win conditions  
- **React components**: UI interaction and state management

### Test Coverage
- **Overall**: 93.68% statement coverage
- **Core Logic**: 100% function coverage
- **All Tests**: 58+ test cases covering edge cases and normal operations

## React Integration

The `SolitaireGame` component demonstrates how to integrate the TypeScript game engine with React:

- State management with React hooks
- Real-time game statistics display
- Interactive game controls
- Type-safe component props and state

## CI/CD Integration

JavaScript tests are automatically run in GitHub Actions alongside the C# tests, ensuring both implementations remain synchronized and functional.

## Architecture

This implementation follows the same clean architecture principles as the C# version:

- **Separation of Concerns**: Game logic is completely independent of UI
- **Testability**: All game logic is unit tested without UI dependencies  
- **Type Safety**: TypeScript provides compile-time type checking
- **Immutability**: Game state changes are handled safely
- **Single Responsibility**: Each class has a focused, single purpose

## Future Enhancements

Potential improvements for a full implementation:

- Complete visual card rendering with CSS/SVG
- Drag and drop functionality
- Animations and transitions
- Multiple game variants (Spider, FreeCell, etc.)
- Multiplayer support
- Save/load game state
- Undo/redo functionality

## License

MIT License - see the main project LICENSE file for details.