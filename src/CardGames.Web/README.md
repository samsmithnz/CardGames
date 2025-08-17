# CardGames.Web

ASP.NET Core web application with React frontend for the CardGames project.

## Architecture

This project combines ASP.NET Core backend with a React frontend, providing:
- **Backend**: ASP.NET Core Web API providing game logic endpoints
- **Frontend**: React + TypeScript SPA with the same game implementation
- **Integration**: Seamless ASP.NET Core + React integration with development proxy

## Project Structure

```
CardGames.Web/
├── Controllers/              # ASP.NET Core API controllers
│   └── SolitaireController.cs
├── Program.cs               # ASP.NET Core application startup
├── CardGames.Web.csproj     # .NET project file
└── ClientApp/               # React frontend application
    ├── src/
    │   ├── core/           # TypeScript game logic (mirrors C# implementation)
    │   ├── components/     # React UI components
    │   ├── __tests__/      # Jest + React Testing Library tests
    │   ├── App.tsx         # Main React app component
    │   └── index.tsx       # React app entry point
    ├── public/             # Static assets
    ├── package.json        # npm dependencies and scripts
    └── tsconfig.json       # TypeScript configuration
```

## Development

### Prerequisites

- .NET 8.0 SDK
- Node.js 16+ with npm

### Building

```bash
# Build the entire project (includes npm install and React build)
dotnet build

# Build just the ASP.NET Core project
dotnet build --no-dependencies

# Build just the React frontend
cd ClientApp && npm run build
```

### Running

```bash
# Start the ASP.NET Core development server (includes React dev server proxy)
dotnet run

# Or start React development server independently
cd ClientApp && npm start
```

### Testing

```bash
# Run .NET tests
dotnet test

# Run React/TypeScript tests
cd ClientApp && npm test

# Run React tests with coverage
cd ClientApp && npm run test:coverage
```

## API Endpoints

- `GET /api/solitaire/test` - Health check endpoint
- `POST /api/solitaire/new-game` - Start a new Solitaire game

## Features

- **Type-safe implementation**: Both C# and TypeScript implementations with strict typing
- **Comprehensive testing**: 64 unit tests covering all game logic and UI components
- **Clean architecture**: Separation between game logic and UI components
- **Cross-platform**: Runs on any platform supporting .NET 8 and modern browsers
- **Development integration**: Hot reload for both backend and frontend during development

## Testing Coverage

The project includes comprehensive test coverage:
- **Card class**: 18 tests covering creation, equality, color checking, numeric values
- **Deck class**: 14 tests covering shuffling, dealing, card management  
- **SolitaireRules class**: 26 tests covering all game logic, move validation, win conditions
- **React components**: 6 tests covering UI interaction and state management

Total: **64 tests** ensuring reliability across both frontend and backend implementations.