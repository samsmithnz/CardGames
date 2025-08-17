# CardGames

[![CI/CD](https://github.com/samsmithnz/CardGames/actions/workflows/dotnet.yml/badge.svg)](https://github.com/samsmithnz/CardGames/actions/workflows/dotnet.yml)
[![Coverage Status](https://coveralls.io/repos/github/samsmithnz/CardGames/badge.svg?branch=main)](https://coveralls.io/github/samsmithnz/CardGames?branch=main)
![Current Release](https://img.shields.io/github/release/samsmithnz/CardGames/all.svg)

A C# implementation of classic card games featuring clean architecture, testable logic, and a desktop user interface. This project demonstrates modern software development practices while providing entertaining and familiar card game experiences.

## Project Overview

CardGames is designed to implement various classic card games (such as Solitaire, Poker, and others) with a focus on:

- **Clean Architecture**: Core game logic is completely separated from UI code
- **Testable Design**: All game logic is unit tested and easily verifiable
- **Modular Structure**: Each game variant is modular and reusable
- **Multi-Platform**: Both desktop (WPF) and web (React/TypeScript) interfaces available

## Project Structure

The solution is organized into four main projects:

- **CardGames.Core** (.NET Standard 2.1): Contains the core game logic, card definitions, deck management, and game rules
- **CardGames.Tests** (.NET 8.0): Comprehensive unit tests for all core functionality  
- **CardGames.WPF** (.NET 8.0 Windows): Windows Presentation Foundation UI using MVVM pattern
- **CardGames.Web** (React/TypeScript): Cross-platform web implementation with complete JavaScript test coverage

## Current Features

- **Card System**: Complete playing card implementation with suits and numbers
- **Deck Management**: Standard 52-card deck with shuffling capabilities
- **Solitaire Game**: Full implementation of Klondike Solitaire with complete rule validation
- **Unit Testing**: Comprehensive test coverage for both C# and JavaScript implementations
- **Web Implementation**: TypeScript/React version of core game logic with full test coverage

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Node.js 18+](https://nodejs.org/) (for web project)
- Windows (for WPF UI components)

### Building the Project

```bash
# Clone the repository
git clone https://github.com/samsmithnz/CardGames.git
cd CardGames

# Build the .NET solution
dotnet build src/CardGames.sln

# Install JavaScript dependencies and run tests
cd src/CardGames.Web
npm install
npm test

# Run .NET tests
cd ../..
dotnet test src/CardGames.Tests/CardGames.Tests.csproj
```

### Running the Applications

```bash
# Run the WPF application (Windows only)
dotnet run --project src/CardGames.WPF/CardGames.WPF.csproj

# Run JavaScript tests with coverage
cd src/CardGames.Web
npm run test:coverage

# Build TypeScript for production
npm run build
```

## Game Rules

### Solitaire (Klondike)
The repository includes a complete implementation of Klondike Solitaire. For detailed rules, setup instructions, and strategy tips, see the [Solitaire Rules Documentation](docs/SOLITAIRE_RULES.md).

Key features:
- Traditional 7-column tableau with alternating color placement
- 4 foundation piles building from Ace to King by suit
- Stock and waste pile mechanics with automatic reset
- Complete move validation and win condition checking

## Technology Stack

- **Languages**: C#, TypeScript/JavaScript
- **Frameworks**: .NET 8.0 / .NET Standard 2.1, React
- **UI Frameworks**: Windows Presentation Foundation (WPF), React with TypeScript
- **Testing**: MSTest
- **CI/CD**: GitHub Actions
- **Architecture Pattern**: MVVM (for UI), Clean Architecture (overall)

## Contributing

Contributions are welcome! Please ensure that:

- All new public methods and classes include XML documentation
- Unit tests are written for new functionality
- Code follows the established C# conventions
- UI logic remains separate from core game logic

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.