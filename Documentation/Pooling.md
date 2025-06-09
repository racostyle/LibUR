# **LibUR.Pooling** — Object Pooling System for Unity

LibUR.Pooling is a lightweight, flexible object pooling framework for Unity designed to improve performance by reusing objects instead of frequently instantiating and destroying them. It supports both **fixed-size**, **multiple object types**, and **flexible (expandable)** pools.

---

## **Core Components**

### **1. PoolCreationData<T>**

> _Located in `PoolCreationData.cs`_

Defines the creation settings for a pool:

- **PoolName** — Unique name of the pool.
- **Size** — Initial size of the pool.
- **Increment** — Amount by which the pool grows if exhausted (only for flexible pools).
- **ParentContainer** — Optional parent `Transform` to organize pooled objects in the hierarchy.
- **InitializeAction** — Action invoked when a pooled object is created.
- **EnableAction** — Action invoked when a pooled object is enabled (reused).
- **ObjectDistribution** — Distribution of different objects (used only for multiple object pools).

---

### **2. PoolCreationDataBuilder<T>**

> _Located in `PoolCreationDataBuilder.cs`_

Builder class for configuring a `PoolCreationData<T>`.  
**Usage Example**:

```csharp
var builder = new PoolCreationDataBuilder<MyComponent>("enemyPool")
    .SetSize(50)
    .WireInitialize(obj => obj.Initialize())
    .WireEnable(obj => obj.ResetState());
var poolData = builder.Build();
```

**Main methods:**

- `.SetSize(int size)`
- `.SetDistribution(int[] objectDistribution)`
- `.SetIncrement(int increment)`
- `.WireInitialize(Action<T> onCreate)`
- `.WireEnable(Action<T> onEnable)`
- `.Build()`

---

### **3. IPool<T>**

> _Located in `IPool.cs`_

Interface for any pool.

```csharp
PooledObject<T> ActivatePooledObject(Vector3 position);
PooledObject<T>[] GetPool();
```

---

## **Queue Systems**

Queues manage the order in which pooled objects are activated.

### **IQueue**

> _Located in `IQueue.cs`_

Queue interface for custom queuing strategies.

- `void AddToQueue(int i)`
- `void RebuildQueue()`
- `int Dequeue()`
- `int Count { get; }`

---

### **QueueOrdered**

> _Located in `QueueOrdered.cs`_

Processes objects **in order of addition** (FIFO).

---

### **QueueRandomized**

> _Located in `QueueRandomized.cs`_

Processes objects **in randomized order** by shuffling before enqueueing.

---

## **Pool Types**

### **PoolFixed<T>**

> _Located in `PoolFixed.cs`_

Fixed-size single object pool.

- Does not grow.
- Uses a single `ObjectRef`.
- Suitable when pool size can be predetermined.

---

### **APoolFixed_MultipleObjects<T>**

> _Located in `PoolFixed_MO.cs`_

Fixed-size pool supporting **multiple types** of objects.

- Accepts an array of `ObjectRef`.
- Requires setting `ObjectDistribution` to control how many of each prefab type are instantiated.

---

### **PoolFlexible<T>**

> _Located in `PoolFlexible.cs`_

Flexible-size pool that grows when empty.

- Grows by a specified `Increment`.
- Useful when pool size is dynamic and unpredictable (e.g., bullet hell games).

---

## **How It Works**

1. **Setup**
   - Create a **PoolCreationData** using the builder.
   - Define object initialization (`WireInitialize`) and reactivation (`WireEnable`) behaviors.
2. **Initialization**
   - Instantiate and initialize the pool using one of the `Pool*` base classes.
3. **Usage**
   - Call `ActivatePooledObject(Vector3 position)` to retrieve an object from the pool.
   - Object will be positioned, activated, and ready for use.
4. **Automatic Management**
   - **Fixed Pools**: Pull from queue, refill if needed.
   - **Flexible Pools**: Expand when exhausted.

---

## **Example Usage**

```csharp
public class BulletPool : MonoBehaviour
{
    [SerializeField] GameObject ObjectRef;
    private PoolingInfo _poolingInfo;
    private IPool<Bullet> _pool;

    // Start is called before the first frame update
    void Start()
    {
        _poolingInfo = GetComponent<Bullet>();
        var creationData = new PoolCreationDataBuilder<Bullet>("Bullets")
            .SetSize(_poolingInfo.Size)
            .SetIncrement(_poolingInfo.Increment)
            .WireInitialize((Bullet bullet) => sphere.Init("this is fired only on creation"))
            .WireEnable((Bullet bullet) => sphere.Reset("this is fired whenever object is enabled"))
            .Build();

        _pool = new PoolFlexible<Bullet>(in creationData, new QueueOrdered(), ObjectRef);
    }
}
```

To spawn a bullet:

```csharp
var bullet = myBulletPool.ActivatePooledObject(spawnPosition);
```

---

# **Notes**

- `PopulateQueue()` is used internally to refill the pool if inactive objects are available.
- Be mindful of setting the correct `ObjectRef` or `ObjectRefs[]` array in the Unity Inspector.
- For `PoolFixed_MO`, ensure the **length of ObjectRef[] matches ObjectDistribution[]**.
