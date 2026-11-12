# Func Delegate Wrapper Classes

This mediator system uses custom `WrappedFunc` classes where each delegate can **only be registered once** and can then be **invoked by multiple classes**.

The following outlines **why this approach is often better than using traditional events or observer patterns**:

---

### **Benefits of the `WrappedFunc` Approach over Events/Observers**

1. **Strict Single Assignment (Single Responsibility)**

   - With `WrappedFunc`, only **one** method can be registered.
   - This design **avoids unexpected multi-subscriber behavior** that occurs with standard C# events or observer patterns, where **multiple handlers** may subscribe accidentally, leading to duplicated execution, memory leaks, or race conditions.

2. **Controlled Setup Phase**

   - Because registration is allowed **only once**, it becomes clear **where and when** connections between modules are made.
   - Initialization (registration) and execution (invocation) are **explicitly separated**, ensuring **predictable and stable** program flow.

3. **Clear Failures and Easy Debugging**

   - If no method is registered, `Invoke()` throws `InvalidOperationException`, so a missing registration fails immediately instead of returning a default value.
   - It is easier to track **which component is responsible** for specific logic because there is **only one registered method** per delegate.

4. **Loose Coupling Between Classes**

   - Classes that need to invoke functionality **do not need to know** who implements it.
   - They interact only with the `WrappedFunc`, leading to an **architecture that is modular and testable** without tight dependencies.

5. **Avoidance of "Event Storms"**

   - Traditional events allow many listeners to subscribe, which can result in **"event storms"** where too many objects are notified unnecessarily.
   - This system ensures that **exactly one registered method** is called, promoting **performance stability**, particularly in large or complex Unity scenes.

6. **Fine-Grained Control**

   - The `WrappedFunc` classes can be **easily extended** with additional features such as "replace registered function," "clear registered function," or "invoke with fallback" behavior.
   - Traditional events often require a more complex custom event manager to achieve a similar level of control.

7. **Simplified Lifetime Management**
   - Manual unsubscription, often required with C# events to avoid memory leaks, is not necessary.
   - Since the system uses a single, controlled registration, managing the lifetime of Unity objects becomes more straightforward.

---

### **When Events or Observers Might Be a Better Fit**

- If **multiple listeners** are needed for a single event (e.g., UI button clicks, achievement tracking, analytics collection), a traditional event or observer model would be more appropriate.
- If runtime **dynamic subscriptions and unsubscriptions** are required, traditional C# events or Unity's event system may better serve those use cases.

---

### **Summary**

This mediator system is ideal when:

- Only **one responsible handler** per action is needed.
- **Predictability** in connections and behavior is critical.
- **Loose coupling** between modules is desired.
- A **simple, robust** system without the risk of event misuse is required.

Essentially, this design implements a **"Single-Slot Mediator"** pattern—a clean, safe approach for game development where multiple classes need to call into centralized logic without introducing instability or complexity.

---

## Features

- **WrappedFunc**: Manages `Func` delegates, supporting up to five input parameters and one return type.
- **ManagerDelegate**: Provides an example of how to use the wrapper classes to manage and invoke actions within a manager class.

### Installation

1. Clone this repository to your local machine:

   ```bash
   git clone https://github.com/your-username/repository-name.git
   ```

2. Import the project into your Unity project or C# solution.

### Usage

#### Registering a Func

To register a function, use the `Register` method of the `WrappedFunc` class.

```csharp
var wrappedFunc = new WrappedFunc<string, string, bool>();
wrappedFunc.Register(MyFuncMethod);

bool MyFuncMethod(string arg1, string arg2)
{
    return true;
}
```

#### Invoking a Func

Once registered, you can invoke the function using the `Invoke` method.

```csharp
bool result = wrappedFunc.Invoke("arg1", "arg2");
```

### Files Overview

- **WrappedFuncClasses.cs**: Contains classes for wrapping `Func` delegates, allowing for return types and multiple input parameters.
