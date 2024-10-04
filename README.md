Card Matching Game

This is a card-matching game developed in Unity. The player must flip two cards to reveal them and try to match pairs. If two cards match, they are removed from the board. The game continues until all cards are matched, or the player runs out of attempts.

Table of Contents
- [Game Overview](game-overview)
- [Gameplay](gameplay)
- [Features](features)
- [Design Patterns Used](design-patterns-used)
- [Game States](game-states)
- [Installation](installation)
- [How to Play](how-to-play)
- [Saving and Loading](saving-and-loading)
- [Note](note)
- [Credits](credits)

---

Game Overview

The card-matching game includes:
- Dynamic card grids with varying difficulties
- Card flipping animations
- Sound effects
- Score tracking
- A save/load system to continue gameplay from where you left off
- Different game states: Playing, Game Over, and Win
- A UI system that tracks the player's score and matches

Gameplay

- The objective is to find all matching pairs of cards on the grid.
- Players click on two cards to flip them over.
  - If the cards match, they stay face-up and are removed from the board.
  - If they do not match, the cards flip back over after a short delay.
- The game is won when all cards have been successfully matched.

Features

- Dynamic Grid Layout**: The card grid adjusts based on the selected difficulty level:
  - Easy: 2x2 grid
  - Medium: 2x3 grid
  - Hard: 5x5 grid
- **Score and Match Count**: Your score increases for each successful match, and the total match count is displayed.
- **Card Flipping Animations**: Smooth animations for card flipping and removal.
- **Save/Load System**: You can save your progress and resume later.
- **Sound Manager**: Plays sounds for matching cards, game over, and win states.
- **Game Over and Win States**: The game transitions to "Game Over" when no matches are left or "Win" when all cards are matched.

 Design Patterns Used

 State Pattern

The **State Pattern** is used to manage the different game states (e.g., Playing, Game Over, and Win). This makes it easier to handle the transitions between these states in a clean, maintainable way. The `GameStateManager` handles switching between states, and each state (e.g., `PlayingState`, `GameOverState`, `GameWinState`) has its own behavior.

- **Playing State**: Manages the card flipping and matching logic during gameplay.
- **Game Over State**: Handles when the player runs out of moves.
- **Win State**: Handles when all cards have been matched.

 Factory Pattern

The **Factory Pattern** is used for generating the card objects in the game. The `CardFactory` class creates cards dynamically based on the selected difficulty level. This approach simplifies the card creation logic, making it more flexible and easier to maintain.

- The factory generates different sets of cards depending on the grid size (easy, medium, or hard).
- It also handles assigning textures (sprites) to the cards based on game logic.

 Generic Singleton Class

The project uses a **Generic Singleton** class to ensure that certain classes (like `UIManager`, `CardGameManager`, and `SaveLoadSystem`) have only one instance throughout the game. This pattern ensures centralized control for managing the game's core systems without creating multiple instances.

- **UIManager**: Manages UI elements like score, match count, game over, and win screens.
- **CardGameManager**: Handles game logic such as card matching, score tracking, and game state transitions.
- **SaveLoadSystem**: Manages saving and loading the game data using JSON.

 Game States

The game transitions through several states:
1. **Playing**: The player is actively matching cards.
2. **Game Over**: When the player fails to match all cards.
3. **Win**: When all cards are successfully matched.

 Installation

1. Clone or download the project repository.
2. Open the project in Unity 2021 LTS or higher.
3. Make sure all required assets and dependencies are included (e.g., Unity UI, TextMeshPro).
4. Run the project by pressing the Play button in Unity Editor.

 How to Play

1. Start the game by selecting a difficulty level from the dropdown menu.
2. Click the "Continue" button to either load a previously saved game or start a new one.
3. Flip two cards by clicking on them.
4. Try to match pairs of cards. Matched cards are removed, and unmatched cards are flipped back after a delay.
5. The score and match count are updated with each pair matched.
6. Save your game at any time by pressing the Save button.

 Controls

- **Mouse**: Click on cards to flip them.

 Saving and Loading

- Save Game: Your current game state, including score, match count, card positions, and flipped states, will be saved to a JSON file.
- Load Game: You can continue from where you left off, restoring the score, match count, card grid, and flipped cards.

 Note

- If you encounter any issues when trying to save the game, create a **new save file**. Restart the game using the **Restart** button in the menu and try saving again. This ensures that the game is reset properly, avoiding any corruption or persistence issues.

 Credits

- Developed by [Arshad Ansari]

---

This version includes the use of design patterns in your game development. You can replace "[Your Name]" with your actual name. Let me know if you need further modifications!
