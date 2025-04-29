# **LibUR.GameStates** — Game State Management System

LibUR.GameStates provides a simple, extensible state management framework for Unity projects.  
It allows systems to react dynamically to changes in the game's lifecycle by subscribing to specific game states.

---

## **Core Components**

### **1. GameState (enum)**

> _Located in `GameState.cs`_

Defines all possible states of the game lifecycle.

| State              | Description                                  |
| ------------------ | -------------------------------------------- |
| **Preloading**     | Loading resources (splash screens, bundles). |
| **Startup**        | System initialization and logos.             |
| **MainMenu**       | Player is in the main menu.                  |
| **Settings**       | Configurations screen.                       |
| **Loading**        | Scene or level transition loading.           |
| **Playing**        | Main gameplay loop active.                   |
| **Cutscene**       | Scripted non-interactive sequences.          |
| **Dialogue**       | Narrative/dialog scenes.                     |
| **Pause**          | Gameplay paused.                             |
| **GameOver**       | End-game result screen or retry prompt.      |
| **Cleanup**        | Internal reset/shutdown.                     |
| **Error**          | Error handling/failsafe mode.                |
| **Debug**          | Developer debugging/testing mode.            |
| **CustomState1–3** | Reserved for user-defined states.            |

---

### **2. IGameStateObserver**

> _Located in `IGameStateObserver.cs`_

Interface defining the contract for game state observers.

```csharp
void Subscribe(GameState state, Action action);
void Unsubscribe(GameState state);
void Fire(GameState state);
```

- **Subscribe** — Register a callback to a specific state.
- **Unsubscribe** — Remove a callback from a state.
- **Fire** — Invoke all callbacks associated with a specific state.

---

### **3. GameStateObserver**

> _Located in `GameStateObserver.cs`_

Concrete implementation of `IGameStateObserver`.  
Internally manages a dictionary mapping `GameState` values to `Action` delegates.

**How it Works:**

- **Subscribe** adds a new callback for a given state.
- **Unsubscribe** removes the callback.
- **Fire** triggers all callbacks registered to the specified state.

**Example:**

```csharp
GameStateObserver gameStateObserver = new GameStateObserver();

// Subscribing to a state
gameStateObserver.Subscribe(GameState.Playing, () => Debug.Log("Game Started!"));

// Triggering a state
gameStateObserver.Fire(GameState.Playing); // Output: Game Started!
```

---

## **Typical Usage Flow**

1. **Initialization**
   - Create a `GameStateObserver` instance at game start (or via a service locator / singleton).
2. **Subscription**
   - Different systems subscribe to the states they care about.
3. **Triggering**
   - When the game's state changes (e.g., entering gameplay), `Fire()` the corresponding state to notify all subscribers.

---

## **Important Notes**

- **One Action per State**:  
  Each state can have **one action** associated with it.  
  If you need multiple listeners per state, you would need to extend `GameStateObserver` to handle a `List<Action>`.
- **Custom States**:  
  `CustomState1`, `CustomState2`, and `CustomState3` are placeholders intended for specific gameplay needs outside of the default flow.

- **Error Handling**:  
  Ensure that you avoid subscribing twice to the same `GameState` (this will throw an exception due to duplicate dictionary keys).

---

# **Summary**

LibUR.GameStates gives your Unity project:

- Clear, organized management of the game flow.
- Decoupled and modular state-dependent behaviors.
- Easy expansion with custom states.
