# LibUR.GameStates — Game State Management for Unity

`LibUR.GameStates` is a modular and testable game state management framework for Unity. It enables systems to register and react to specific states in the game's lifecycle in a clean, decoupled way. The system supports multiple subscribers per state, enforces subscriber identity, and is compatible with NUnit testing.

---

## Core Concepts

### GameState (enum)

> _Defined in `GameState.cs`_

This enumeration defines all valid states in the game lifecycle.

| State         | Description                                  |
| ------------- | -------------------------------------------- |
| Preloading    | Resource initialization (e.g. splash, bundles). |
| Startup       | System setup and logo display.               |
| MainMenu      | Main menu active.                            |
| Settings      | Configuration or options menu.               |
| Loading       | Scene/level loading in progress.             |
| Playing       | Gameplay loop running.                       |
| Cutscene      | Non-interactive scripted events.             |
| Dialogue      | Narrative/dialogue sequences.                |
| Pause         | Gameplay is paused.                          |
| GameOver      | Displaying results or retry screen.          |
| Cleanup       | Reset or shutdown state.                     |
| Error         | Error handling or fallback logic.            |
| Debug         | Developer/testing state.                     |
| CustomState1–3| User-defined additional states.              |

---

## Interfaces and Implementations

### IGameStateObserver

> Defined in `IGameStateObserver.cs`

Interface that defines the observer contract for managing game state transitions.

```csharp
Dictionary<GameState, List<SubscriberData>> Subscribers { get; }
void Subscribe(GameState state, Action action, string subscriberName);
void Unsubscribe(GameState state, string subscriberName);
void Unsubscribe(string subscriberName);
void Fire(GameState state);
void Clear();
````

* `Subscribe` — Register an action for a specific state with a unique subscriber name.
* `Unsubscribe(state, name)` — Remove a subscription by state and name.
* `Unsubscribe(name)` — Remove a subscriber from all states.
* `Fire` — Trigger all actions subscribed to a given state.
* `Clear` — Remove all subscribers from all states.

---

### GameStateObserver

> *Defined in `GameStateObserver.cs`*

Concrete implementation of `IGameStateObserver`. Manages internal state-action mappings and enforces subscriber uniqueness.

#### Key Features:

* Stores subscriptions in a `Dictionary<GameState, List<SubscriberData>>`.
* Allows multiple subscribers per state.
* Logs a warning if a subscriber with the same name tries to subscribe to the same state again.
* Provides a `Clear()` method for clean-up, ideal for test setup/teardown scenarios.

#### Example Usage:

```csharp
var observer = new GameStateObserver();

// Subscribe to Playing state
observer.Subscribe(GameState.Playing, () => Debug.Log("Game is now playing!"), "PlayerSystem");

// Trigger the state
observer.Fire(GameState.Playing); // Output: Game is now playing!

// Unsubscribe
observer.Unsubscribe(GameState.Playing, "PlayerSystem");
```

---

### SubscriberData

> *Defined in `SubscriberData.cs`*

Lightweight data structure representing a named subscriber and its associated action.

```csharp
public struct SubscriberData
{
    internal string Name;
    internal Action Action;

    public SubscriberData(string name, Action action);
}
```

Each subscription is uniquely identified by a `Name`, ensuring no duplicate subscriptions per state.

---

## Usage Flow

1. **Create the Observer**

   * Instantiate a `GameStateObserver` at game initialization or inject via a service locator.

2. **Register Subscriptions**

   * Different systems (UI, audio, logic) register for states they care about using `Subscribe`.

3. **Fire State Transitions**

   * Use `Fire(GameState)` to notify all registered systems when a state is entered.

4. **Unsubscribe and Clean Up**

   * Remove subscriptions as needed, especially in destructors or scene unloads.

---

## Testing Support

The `GameStateObserver` class is **fully testable with NUnit**. Its clean interface and use of standard delegates (`Action`) make it easy to write isolated unit tests without requiring Unity-specific components.

### Example NUnit Test

```csharp
[Test]
public void Subscribe_And_Fire_Invokes_Correct_Action()
{
    var observer = new GameStateObserver();
    bool wasCalled = false;

    observer.Subscribe(GameState.Pause, () => wasCalled = true, "TestSystem");
    observer.Fire(GameState.Pause);

    Assert.IsTrue(wasCalled);
}
```

---

## Design Notes

* **Multiple Subscribers per State:**
  Unlike earlier versions, each state can now have multiple named subscribers.

* **Subscriber Uniqueness:**
  Re-subscribing with the same name to the same state logs a warning and does not overwrite the existing entry.

* **Test-Ready:**
  Decoupled from Unity's MonoBehaviour and lifecycle. Ideal for headless test execution.

* **Custom States:**
  Three flexible slots (`CustomState1–3`) are reserved for project-specific extensions.

---

## Summary

LibUR.GameStates provides a robust, extensible, and testable foundation for managing game state transitions in Unity. Its clean architecture promotes decoupling, modularity, and scalability across various gameplay systems.

```

---

Would you like this saved as a downloadable `.md` file?
```
