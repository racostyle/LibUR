# **LibUR.Pooling** — Updated Overview (v2)

LibUR.Pooling provides a **modular object pooling system for Unity** built around strong type-safety, optional expansion, and queue-based activation strategies.
It supports **fixed**, **multi-prefab**, and **flexible (expandable)** pools with identical usage patterns.

---

## 🧩 Core Architecture

### **IPool<T>**

Located in `IPool.cs`

Common contract for all pool types.

```csharp
public interface IPool<T>
{
    bool TryActivateObject(Vector3 position, out T obj);
    T[] GetPool();
    void DestroyAll(bool alsoDestroyContainer = true);
}
```

- **TryActivateObject** — Returns an available object positioned at the given location. Returns `false` if the pool is empty.
- **GetPool** — Access the internal array of pooled instances.
- **DestroyAll** — Deactivates and optionally destroys all pooled instances (and their container).

---

### **SPoolCreationData<T>**

Located in `SPoolCreationData.cs`

Immutable struct holding all pool configuration data:

| Field                | Description                                |
| -------------------- | ------------------------------------------ |
| `PoolName`           | Unique pool name.                          |
| `Size`               | Initial pool size.                         |
| `Increment`          | Expansion size for flexible pools.         |
| `ParentContainer`    | Optional parent transform.                 |
| `InitializeAction`   | Fired when an object is first created.     |
| `EnableAction`       | Fired whenever an object is reused.        |
| `ObjectDistribution` | Distribution array for multi-prefab pools. |

---

### **PoolCreationDataBuilder<T>**

Located in `PoolCreationDataBuilder.cs`

Fluent builder for constructing a `SPoolCreationData<T>` instance.

```csharp
var data = new PoolCreationDataBuilder<Bullet>("Bullets")
    .SetSize(100)
    .SetIncrement(10)
    .WireInitialize(b => b.InitOnCreate())
    .WireEnable(b => b.OnReuse())
    .Build();
```

---

### **PoolHelper<T>**

Located in `PoolHelper.cs`

Internal utility that:

- Manages queue refilling (`EnsureQueueNotEmpty`).
- Handles activation logic (`ActivateObject`).
- Performs safe teardown (`DestroyAll`).

This keeps individual pool classes focused on instantiation and sizing only.

---

## ⚙️ Queues

### **IQueue**

Located in `IQueue.cs`

Defines a strategy for activation order.

### **QueueOrdered**

Located in `QueueOrdered.cs`
Standard FIFO queue (useful for deterministic reuse).

### **QueueRandomized**

Located in `QueueRandomized.cs`
Randomizes activation order using an internal shuffle.

---

## 🧱 Pool Implementations

### **PoolFixed<T>**

Located in `PoolFixed.cs`

- Uses a **single prefab reference**.
- Fixed size (no growth).
- Ideal for predictable object counts (e.g., 100 bullets max).

```csharp
var pool = new PoolFixed<Bullet>(
    in data,
    new QueueOrdered(),
    bulletPrefab
);
```

**Activation:**

```csharp
if (pool.TryActivateObject(spawnPos, out var bullet))
    bullet.Fire();
```

---

### **PoolFixed_MultipleObjects<T>**

Located in `PoolFixed_MultipleObjects.cs`

Supports **multiple prefab types** in one pool.

| Field                  | Description                           |
| ---------------------- | ------------------------------------- |
| `references[]`         | Array of prefab objects.              |
| `ObjectDistribution[]` | Number of each prefab to instantiate. |

```csharp
var data = new PoolCreationDataBuilder<Enemy>("EnemyPool")
    .SetDistribution(new[] { 30, 20, 10 }) // 60 total
    .WireInitialize(e => e.Setup())
    .WireEnable(e => e.Reset())
    .Build();

var pool = new PoolFixed_MultipleObjects<Enemy>(
    in data,
    new QueueRandomized(),
    enemyPrefabs
);
```

---

### **PoolFlexible<T>**

_(Implementation mirrors PoolFixed<T>, but expands when empty.)_

- Grows by `Increment` when all objects are active.
- Suitable for highly dynamic or unpredictable use cases.

Expected constructor signature:

```csharp
var pool = new PoolFlexible<Bullet>(
    in data,
    new QueueOrdered(),
    bulletPrefab
);
```

---

## 🚀 Usage Flow

1. **Build** the pool data using the builder.
2. **Instantiate** a pool type (Fixed, Multi, or Flexible).
3. **Activate** objects using `TryActivateObject(position, out obj)`.
4. **Recycle** objects by deactivating their `GameObject` when done.
5. **Cleanup** using `DestroyAll()` during `OnDestroy()` or shutdown.

---

## 🧩 Key Differences from Older Docs

| Aspect             | Old                                          | New                                                |
| ------------------ | -------------------------------------------- | -------------------------------------------------- |
| Activation         | `ActivatePooledObject()` (returned directly) | `TryActivateObject()` (bool + out param)           |
| Queue interface    | `Dequeue()` only                             | Added `TryDequeue()` and `Clear()`                 |
| Pool configuration | `PoolCreationData` class                     | `SPoolCreationData` struct + builder               |
| Helper             | Previously internal logic per pool           | Centralized in `PoolHelper<T>`                     |
| Destruction        | No unified method                            | `DestroyAll()` now standard                        |
| Flexible pools     | Not implemented yet                          | Designed to mirror fixed logic with dynamic growth |

---

## ✅ Notes

- **Destroyed prefab safety:** `PoolHelper.TryDequeObjectSafeguard` skips nulls caused by playmode reloads.
- **Container cleanup:** `DestroyAll(true)` removes the parent container GameObject.
- **Multiple object pools:** Ensure `references.Length == ObjectDistribution.Length`.
- **Thread-safe:** Unity object operations occur on the main thread; no async use.
