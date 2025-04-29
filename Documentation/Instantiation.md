# **LibUR.Instantiation** — Centralized Instantiation Framework for Unity

LibUR.Instantiation provides a structured way to instantiate and initialize all scene objects through a **single factory class**, ensuring full control over object creation and setup.

---

## **Core Components**

### **1. AInstantiatorFactory**

> _Located in `AInstantiatorFactory.cs`_

An abstract base class designed to centralize and standardize the creation of objects in a Unity scene.

**Responsibilities:**

- Automatically groups all created objects under a common `Transform` (`Objects_Container`).
- Instantiates GameObjects at a given position.
- Automatically initializes any object implementing `IInitializable` with custom arguments.

**Main Methods:**

```csharp
protected void Init();
protected GameObject CreateObject(GameObject reference, Vector3 position, params object[] args);
```

#### Method Details:

- **Init()**

  - Creates an empty `GameObject` named `"Objects_Container"`.
  - All future instantiated objects will be children of this container.
  - Should be called once during your main spawner/manager's initialization phase (e.g., `Awake` or `Start`).

- **CreateObject(GameObject reference, Vector3 position, params object[] args)**
  - Instantiates the `reference` prefab at the specified `position`.
  - If the instantiated prefab has a component implementing `IInitializable`, it automatically calls `Initialize(args)`.
  - Ensures that initialization arguments are passed directly at the point of creation.

---

### **2. IInitializable**

> _Located in `IInitializable.cs`_

An interface to be implemented by any component that requires **initialization** or **termination** logic after instantiation.

**Methods:**

```csharp
void Initialize(params object[] args);
void Terminate();
```

#### Purpose:

- **Initialize(params object[] args)**:

  - Called right after the object is instantiated.
  - Allows passing setup parameters dynamically.

- **Terminate()**:
  - Reserved for controlled cleanup or recycling logic when an object is no longer needed.

---

## **Typical Usage Flow**

1. **Create a Manager/Spawner Class**
   - Derive from `AInstantiatorFactory`.
2. **Call `Init()`**
   - Setup the container.
3. **Use `CreateObject()`**
   - Instantiate prefabs and initialize components on creation.

---

## **Example**

```csharp
public class SceneSpawner : AInstantiatorFactory
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject itemPrefab;

    private void Start()
    {
        Init(); // Create the Objects_Container

        // Instantiate an enemy at a specific position with initialization arguments
        CreateObject(enemyPrefab, new Vector3(0, 0, 0), "EnemyTypeA", 100);

        // Instantiate an item with different arguments
        CreateObject(itemPrefab, new Vector3(5, 0, 0), "HealthPotion", 50);
    }
}
```

**Prefab Setup Example:**

```csharp
public class Enemy : MonoBehaviour, IInitializable
{
    private string _enemyType;
    private int _health;

    public void Initialize(params object[] args)
    {
        _enemyType = (string)args[0];
        _health = (int)args[1];
    }

    public void Terminate()
    {
        // Handle death, return to pool, etc.
    }
}
```

---

## **Design Philosophy**

| Feature                 | Benefit                                         |
| ----------------------- | ----------------------------------------------- |
| **Single Entry Point**  | Centralizes all object creation logic.          |
| **Grouped Objects**     | Keeps hierarchy clean and manageable.           |
| **Auto-Initialization** | Decouples prefab logic from instantiation code. |
| **Flexible Parameters** | Pass any needed setup data dynamically.         |

---

# **Summary**

LibUR.Instantiation ensures:

- **Clean** project hierarchy through automatic containerization.
- **Decoupled** logic by separating prefab creation from setup.
- **Scalable** scene setup via a single responsible factory.
