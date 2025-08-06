# Copilot Instructions for samsmithnz/CardGames

Welcome to the CardGames repository! This document provides guidelines for GitHub Copilot and contributors to ensure consistent, high-quality code and helpful suggestions.

---

## Project Overview

- **Language:** C#
- **Purpose:** Implement classic card games (e.g., Solitaire, Poker) with a focus on clean architecture, testable logic, and a desktop user interface (WPF, WinForms, or similar).
- **Structure:** Core game logic is separate from UI code. Each game variant should be modular and reusable.

---

## Coding Guidelines

- Use standard C# conventions: PascalCase for types/methods, camelCase for local variables, UPPER_CASE for constants.
- Organize code into logical namespaces (e.g., CardGames.Core, CardGames.WPF).
- Prefer composition over inheritance except for clearly shared base classes (such as Card, Deck, Player).
- Public methods and classes should include XML documentation comments.
- Write unit tests for all new public methods and classes.
- Use async/await for asynchronous operations where appropriate.
- **Do not use `var`—always use explicit types.**
- **Always use curly braces `{ }` for all blocks, even for single-line statements, loops, or conditionals.**

---

## Project-Specific Practices

- Put card and deck logic in the `Core` project/folder.
- UI logic goes in UI projects/folders (e.g., `CardGames.WPF`).
- Game rules for each card game should be in their own class or namespace (e.g., `SolitaireRules`).
- Use the MVVM pattern for any WPF UI.
- Avoid hardcoding file paths; use paths relative to the project directory.

---

## What to Avoid

- Do not use external card game logic libraries; implement from scratch.
- Do not mix UI and core game logic.
- Avoid global/static state unless necessary (e.g., for singletons).
- Avoid code duplication—prefer reusable functions and classes.
- **Do not use `var` for variable declarations.**
- **Never omit curly braces `{ }` for any code block.**

---

## Example Prompts for Copilot

- "Add a method to shuffle the deck in CardGames.Core/Deck.cs."
- "Implement a WPF UserControl to display a playing card."
- "Write unit tests for CardGames.Core/Deck."
- "Add a new game variant: War, as a class in CardGames.Core.Games."
- "Implement drag-and-drop support for moving cards in the WPF UI."

---

## Good Output Example

```csharp
public void Shuffle()
{
    Random rng = new Random();
    int n = Cards.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);
        Card value = Cards[k];
        Cards[k] = Cards[n];
        Cards[n] = value;
    }
}
```

---

Thank you for contributing!
