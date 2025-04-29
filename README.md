# **LibUR - Core Systems Overview**

LibUR is a modular set of libraries designed to improve and organize Unity project architecture.  
Each library focuses on solving a specific problem cleanly, efficiently, and with minimal dependencies.

Below are the major systems included:

---

## **1. Pooling System (LibUR.Pooling)**

- **Purpose:**  
  Manage object reuse to avoid costly instantiation and destruction during runtime.
  
- **Highlights:**
  - Supports fixed-size, multiple object types, and flexible (expandable) pools.
  - Works with ordered or randomized retrieval (queue strategies).
  - Customizable object initialization and reactivation behaviors.

- **Use Cases:**  
  Bullets, enemies, particles, collectibles — any frequently spawned objects.

📚 [ObjectPooling Documentation](Documentation/Pooling.md)

---

## **2. Game State System (LibUR.GameStates)**

- **Purpose:**  
  Organize the game's lifecycle into clearly defined states like Loading, Playing, GameOver, etc.

- **Highlights:**
  - Enum-based state definitions.
  - Lightweight observer for subscribing to state changes.
  - Designed for modular, event-driven state transitions.

- **Use Cases:**  
  Handling menus, pausing, loading screens, gameplay loops, debugging modes.

📚 [GameState Documentation](Documentation/GameState.md)

---

## **3. Centralized Instantiation System (LibUR.Instantiation)**

- **Purpose:**  
  Manage object creation through a single factory to maintain clean project structure.

- **Highlights:**
  - All instantiated objects are grouped under a central container.
  - Automatic initialization for components implementing `IInitializable`.
  - Clean separation of instantiation and initialization logic.

- **Use Cases:**  
  Spawning managers, dynamic object generators, controlled scene setup.

📚 [Instantiation Documentation](Documentation/Instantiation.md)

---

## **4. Safe Delegate Wrappers (LibUR.Delegates)**

- **Purpose:**  
  Provide safe, single-assignment wrappers for Actions and Funcs to prevent delegate misuse.

- **Highlights:**
  - Strongly typed wrappers for Actions (void returns) and Funcs (value returns).
  - Only allows a delegate to be registered once.
  - Safe invocation without risk of null references.

- **Use Cases:**  
  Single-shot callbacks, initialization handlers, safe lifecycle events.

📚 [Delegates Documentation](Documentation/Delegates.md)

---

# **Summary**

LibUR offers:
- **Performance Optimization** (Pooling)
- **Lifecycle Management** (Game States)
- **Centralized Control** (Instantiation)
- **Safe, Modular Code** (Delegates)

Each system is **independent** but **designed to work together** for a clean, scalable Unity project.
