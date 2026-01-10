# **LibUR.Pooling** — Complete Overview (v3)

LibUR.Pooling provides a **modular object pooling system for Unity** built around strong type-safety, optional expansion, and queue-based activation strategies.
It supports **fixed**, **multi-prefab**, **flexible (expandable)**, and **enum-based** pools with consistent usage patterns.

---

## 🧩 Core Architecture

### **IPool<T>**

Located in `IPool.cs`

Common contract for all standard pool types. Supports both single-step and two-step activation patterns.

```csharp
public interface IPool<T> where T : MonoBehaviour
{
    bool TryActivateObject(Vector3 position, out T obj);
    bool TwoStep_TrySelectAndDequeueObject(out T obj);
    void TwoStep_EnableObject(Vector3 position, T obj);
    T[] GetPool();
    void DestroyAll(bool alsoDestroyContainer = true);
}
```

**Methods:**

- **TryActivateObject** — Single-step activation: Returns an available object positioned at the given location. Returns `false` if the pool is empty.
- **TwoStep_TrySelectAndDequeueObject** — Two-step activation (step 1): Selects and dequeues an object without activating it. Use when you need to perform additional setup before activation.
- **TwoStep_EnableObject** — Two-step activation (step 2): Activates a previously selected object at the specified position. Call after `TwoStep_TrySelectAndDequeueObject`.
- **GetPool** — Access the internal array of pooled instances (includes both active and inactive objects).
- **DestroyAll** — Destroys all pooled instances and optionally the container GameObject.

---

### **IPoolByEnum<T, TEnum>**

Located in `IPoolByEnum.cs`

Interface for pools that support selection by enum value. Each enum value maps to its own separate pool of objects.

```csharp
public interface IPoolByEnum<T, TEnum> 
    where T : MonoBehaviour 
    where TEnum : Enum
{
    bool TryActivateObject(Vector3 position, TEnum key, out T obj);
    bool TwoStep_TrySelectObject(TEnum key, out T obj);
    void TwoStep_EnableObject(Vector3 position, T obj);
    Dictionary<TEnum, T[]> GetPool();
    void DestroyAll(bool alsoDestroyContainer = true);
}
```

**Key Differences from IPool<T>:**
- Activation methods require an enum `key` parameter to specify which pool to select from.
- `GetPool()` returns a `Dictionary<TEnum, T[]>` instead of a flat array.
- `TwoStep_TrySelectObject` uses enum selection instead of queue-based dequeue.

---

### **SPoolCreationData<T>**

Located in `SPoolCreationData.cs`

Immutable struct holding all pool configuration data:

| Field                | Description                                | Used By                    |
| -------------------- | ------------------------------------------ | -------------------------- |
| `PoolName`           | Unique pool name for hierarchy organization. | All pool types             |
| `Size`               | Initial pool size.                         | All pool types             |
| `Increment`          | Expansion size for flexible pools.         | PoolFlexible only          |
| `ParentContainer`    | Parent transform for organizing objects.    | All pool types             |
| `InitializeAction`   | Action fired when an object is first created (receives component and index). | All pool types |
| `EnableAction`       | Action fired whenever an object is activated/reused. | All pool types |
| `ObjectDistribution` | Distribution array for multi-prefab and enum pools. | PoolFixed_MultipleObjects, PoolFixed_ByEnum |

---

### **PoolCreationDataBuilder<T>**

Located in `PoolCreationDataBuilder.cs`

Fluent builder for constructing a `SPoolCreationData<T>` instance using method chaining.

**Available Methods:**

```csharp
var data = new PoolCreationDataBuilder<Bullet>("Bullets")
    .SetSize(100)                                    // Initial pool size
    .SetIncrement(10)                                // For PoolFlexible: resize amount
    .SetParent(parentTransform)                      // Optional parent container
    .WireOnInitialize((bullet, index) => {...})     // Called when object is created
    .WireOnEnable(bullet => {...})                  // Called when object is activated
    .SetDistribution_Manual(30, 20, 10)             // For PoolFixed_MultipleObjects
    .Build();
```

**Distribution Methods (for PoolFixed_MultipleObjects):**

- **SetDistribution_Manual(params int[])** — Manually specify counts for each object type (e.g., `[50, 50, 20]` creates 50 of first, 50 of second, 20 of third).
- **SetDistribution_Fixed(int objectsCount, int value)** — Each object type gets the same count (e.g., `SetDistribution_Fixed(3, 50)` creates `[50, 50, 50]`).
- **SetDistribution_AutoCalculate(int totalSize, int objectsCount)** — Automatically calculates weighted distribution (first gets 50%, others get decreasing shares). Requires at least 3 object types and minimum size of `objectsCount * 5`.

---

### **PoolHelper<T>**

Located in `PoolHelper.cs`

Internal utility class that centralizes common pooling operations:

- **CreateLocalContainer** — Creates a GameObject container to organize pooled objects in the hierarchy.
- **TryDequeObjectSafeguard** — Safely dequeues objects with null checks and automatic queue rebuilding.
- **EnsureQueueNotEmpty** — Rebuilds the queue from inactive objects if it becomes empty.
- **ActivateObject** — Handles object activation (position, rotation, active state, enable action).
- **DestroyAll** — Performs safe teardown in both play mode and edit mode.

This keeps individual pool classes focused on instantiation and sizing logic only.

---

## ⚙️ Queues

### **IQueue**

Located in `IQueue.cs`

Interface defining a strategy for managing object activation order.

```csharp
public interface IQueue
{
    void AddToQueue(int i);
    void BuildQueue();
    int Dequeue();
    void Clear();
    int Count { get; }
}
```

**Methods:**
- **AddToQueue** — Adds an index to the collection (processed when `BuildQueue` is called).
- **BuildQueue** — Builds the internal queue structure from all added indices. Must be called after adding indices and before dequeuing.
- **Dequeue** — Removes and returns the next index (behavior depends on implementation).
- **Clear** — Removes all indices, resetting to empty state.
- **Count** — Gets the number of indices currently in the queue.

---

### **QueueOrdered**

Located in `QueueOrdered.cs`

Standard FIFO (First-In-First-Out) queue implementation. Objects are activated in the order they were added.

**Use when:** You want predictable, sequential object activation.

```csharp
var queue = new QueueOrdered();
```

---

### **QueueRandomized**

Located in `QueueRandomized.cs`

Randomizes activation order using Fisher-Yates shuffle algorithm. Helps distribute wear evenly across pooled objects.

**Use when:** You want unpredictable, randomized object activation.

```csharp
var queue = new QueueRandomized();              // Uses random seed
var queue = new QueueRandomized(12345);        // Uses specified seed for deterministic randomization
```

---

## 🧱 Pool Implementations

### **PoolFixed<T>**

Located in `PoolFixed.cs`

- Uses a **single prefab reference**.
- Fixed size (no growth once created).
- Ideal for predictable object counts (e.g., 100 bullets max).
- Best when you know the maximum number of objects needed and want predictable memory usage.

**Constructor:**
```csharp
var pool = new PoolFixed<Bullet>(
    in data,
    new QueueOrdered(),      // or QueueRandomized()
    bulletPrefab
);
```

**Usage - Single-step activation:**
```csharp
if (pool.TryActivateObject(spawnPos, out var bullet))
    bullet.Fire();
```

**Usage - Two-step activation:**
```csharp
if (pool.TwoStep_TrySelectAndDequeueObject(out var bullet))
{
    // Perform additional setup
    bullet.SetDamage(50);
    bullet.SetOwner(player);
    
    // Complete activation
    pool.TwoStep_EnableObject(spawnPos, bullet);
    bullet.Fire();
}
```

---

### **PoolFixed_MultipleObjects<T>**

Located in `PoolFixed_MultipleObjects.cs`

- Supports **multiple prefab types** in one pool.
- Fixed size determined by `ObjectDistribution` array sum.
- Objects are selected from the entire pool regardless of type.
- Best when you need multiple object types but don't need to select by specific type.

**Setup:**
```csharp
var data = new PoolCreationDataBuilder<Enemy>("EnemyPool")
    .SetDistribution_Manual(30, 20, 10)  // 60 total: 30 of first, 20 of second, 10 of third
    .SetParent(enemyContainer)
    .WireOnInitialize((enemy, index) => enemy.Setup(index))
    .WireOnEnable(enemy => enemy.Reset())
    .Build();

var pool = new PoolFixed_MultipleObjects<Enemy>(
    in data,
    new QueueRandomized(),
    enemyPrefabs  // Array matching ObjectDistribution length
);
```

**Important:** The `references` array length must match the `ObjectDistribution` array length.

---

### **PoolFlexible<T>**

Located in `PoolFlexible.cs`

- Uses a **single prefab reference**.
- **Automatically expands** by `Increment` amount when all objects are active.
- Suitable for highly dynamic or unpredictable use cases where object count may vary.
- Uses `T[]` array which is faster than `List<T>` for infrequent resizes.

**Constructor:**
```csharp
var data = new PoolCreationDataBuilder<Bullet>("Bullets")
    .SetSize(50)              // Initial size
    .SetIncrement(10)         // Expand by 10 when exhausted
    .SetParent(bulletContainer)
    .WireOnEnable(bullet => bullet.Reset())
    .Build();

var pool = new PoolFlexible<Bullet>(
    in data,
    new QueueOrdered(),
    bulletPrefab
);
```

**Note:** The pool will automatically resize when `TryActivateObject` is called and no objects are available, so it should rarely (if ever) return `false`.

---

### **PoolFixed_ByEnum<T, TEnum>**

Located in `PoolFixed_ByEnum.cs`

- Allows **selecting objects by enum value**.
- Each enum value maps to its own separate pool of objects.
- Fixed size for each enum type (determined by `ObjectDistribution`).
- Implements `IPoolByEnum<T, TEnum>` interface.
- Best when you need to select objects by specific enum type.

**Setup:**
```csharp
public enum EnemyType
{
    Basic,
    Fast,
    Tank
}

var data = new PoolCreationDataBuilder<Enemy>("EnemyPool")
    .SetDistribution_Manual(30, 20, 10)  // 30 Basic, 20 Fast, 10 Tank
    .SetParent(enemyContainer)
    .WireOnInitialize((enemy, index) => enemy.Initialize(index))
    .WireOnEnable(enemy => enemy.Reset())
    .Build();

var references = new Dictionary<EnemyType, GameObject>
{
    { EnemyType.Basic, basicEnemyPrefab },
    { EnemyType.Fast, fastEnemyPrefab },
    { EnemyType.Tank, tankEnemyPrefab }
};

var pool = new PoolFixed_ByEnum<Enemy, EnemyType>(
    in data,
    references
);
```

**Usage - Single-step activation:**
```csharp
if (pool.TryActivateObject(spawnPos, EnemyType.Fast, out var fastEnemy))
    fastEnemy.Move();
```

**Usage - Two-step activation:**
```csharp
if (pool.TwoStep_TrySelectObject(EnemyType.Tank, out var tank))
{
    tank.SetHealth(200);
    pool.TwoStep_EnableObject(spawnPos, tank);
}
```

**Important:** The `references` dictionary keys order should match the `ObjectDistribution` array order (enum iteration order may vary - ensure consistency).

---

## 🚀 Usage Flow

### Standard Pools (IPool<T>)

1. **Build** the pool data using `PoolCreationDataBuilder<T>`.
2. **Instantiate** a pool type (Fixed, MultipleObjects, or Flexible) with the data and queue.
3. **Activate** objects using either:
   - **Single-step:** `TryActivateObject(position, out obj)`
   - **Two-step:** `TwoStep_TrySelectAndDequeueObject(out obj)` → setup → `TwoStep_EnableObject(position, obj)`
4. **Recycle** objects by deactivating their `GameObject` when done (they'll be automatically re-added to the queue).
5. **Cleanup** using `DestroyAll(true)` during `OnDestroy()` or shutdown.

### Enum-Based Pools (IPoolByEnum<T, TEnum>)

1. **Build** the pool data using `PoolCreationDataBuilder<T>` with `SetDistribution_Manual`.
2. **Create** a dictionary mapping enum values to GameObject references.
3. **Instantiate** `PoolFixed_ByEnum<T, TEnum>` with the data and references dictionary.
4. **Activate** objects by enum type using either:
   - **Single-step:** `TryActivateObject(position, enumKey, out obj)`
   - **Two-step:** `TwoStep_TrySelectObject(enumKey, out obj)` → setup → `TwoStep_EnableObject(position, obj)`
5. **Recycle** and **Cleanup** same as standard pools.

---

## 📋 Activation Patterns

### Single-Step Activation

Simplest pattern: get and activate in one call.

```csharp
if (pool.TryActivateObject(spawnPosition, out var obj))
{
    // Object is already activated and positioned
    obj.DoSomething();
}
```

### Two-Step Activation

Use when you need to configure the object before activating it.

```csharp
if (pool.TwoStep_TrySelectAndDequeueObject(out var obj))
{
    // Object is selected but NOT yet activated
    obj.Configure(parameters);
    obj.Validate();
    
    // Now activate it
    pool.TwoStep_EnableObject(spawnPosition, obj);
    obj.StartBehavior();
}
```

**Benefits of Two-Step:**
- Validate object state before activation
- Configure properties that affect activation behavior
- Set up references or dependencies before the object becomes active

---

## 🧩 Key Features

| Feature                 | Description                                         |
| ----------------------- | --------------------------------------------------- |
| **Type Safety**         | Strongly typed generic pools (`PoolFixed<Bullet>`) |
| **Queue Strategies**    | Ordered (FIFO) or Randomized activation            |
| **Multiple Patterns**   | Single-step or two-step activation                  |
| **Multi-Prefab Support**| Multiple object types in one pool                   |
| **Enum Selection**      | Select objects by enum value                        |
| **Flexible Expansion**  | Automatic pool growth for dynamic scenarios         |
| **Lifecycle Hooks**     | Initialize (creation) and Enable (activation) actions |
| **Safe Cleanup**        | Handles both play mode and edit mode destruction    |

---

## ✅ Important Notes

- **Destroyed prefab safety:** `PoolHelper.TryDequeObjectSafeguard` skips null references caused by editor play mode reloads.
- **Container cleanup:** `DestroyAll(true)` removes the parent container GameObject. Use `false` if you want to keep the container.
- **Multiple object pools:** Ensure `references.Length == ObjectDistribution.Length` for `PoolFixed_MultipleObjects`.
- **Enum pool references:** Dictionary key order should match `ObjectDistribution` array order for `PoolFixed_ByEnum`.
- **Queue rebuilding:** Queues are automatically rebuilt from inactive objects when they become empty (standard pools only, not enum pools).
- **Thread safety:** Unity object operations occur on the main thread; no async/thread-safe operations are provided.
- **Object recycling:** Objects are recycled by simply deactivating their GameObject. The pool will detect inactive objects and make them available again.
- **Null safety:** All activation methods return `false` when pools are exhausted or objects are unavailable. Always check return values.

---

## 🎯 Choosing the Right Pool Type

| Scenario                          | Recommended Pool Type         | Queue Type            |
| --------------------------------- | ----------------------------- | --------------------- |
| Predictable count, single type    | `PoolFixed<T>`                | `QueueOrdered`        |
| Predictable count, multiple types | `PoolFixed_MultipleObjects<T>`| `QueueRandomized`     |
| Unpredictable count               | `PoolFlexible<T>`             | `QueueOrdered`        |
| Select by enum type               | `PoolFixed_ByEnum<T, TEnum>`  | N/A (no queue)        |
| Even wear distribution            | Any                           | `QueueRandomized`     |
| Deterministic behavior            | Any                           | `QueueOrdered`        |

---

## 📚 Related Documentation

- See `Delegates.md` for information on action/func wrapper classes used in pool initialization.
- See `GameState.md` for state management that can coordinate pool lifecycle.
- See `Instantiation.md` for alternative object creation patterns.
