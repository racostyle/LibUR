using System;
using System.Linq;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    /// <summary>
    /// Builder class for creating pool configuration data with a fluent interface.
    /// Allows step-by-step configuration of pool parameters using method chaining.
    /// Use this for complex pool configurations that benefit from readable, chainable setup.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    public class PoolCreationDataBuilder<T> where T : MonoBehaviour
    {
        private string _name;
        private int _size;
        private int _increment;
        private Transform _parent;
        private Action<T, int> _onCreate;
        private Action<T> _onEnable;
        private int[] _objectDistribution;

        /// <summary>
        /// Creates a new builder instance with the specified pool name.
        /// </summary>
        /// <param name="name">Name of the pool (required)</param>
        public PoolCreationDataBuilder(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Initial size of the pool. Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// In PoolFixed this is the maximum size. In PoolFixed_MultipleObjects this will be overwritten by SetDistribution_Manual method.
        /// </summary>
        /// <param name="size">The initial size of the pool</param>
        /// <returns>This builder instance for method chaining</returns>
        public PoolCreationDataBuilder<T> SetSize(int size)
        {
            _size = size;
            return this;
        }

        /// <summary>
        /// Sets the parent transform container for pooled objects. Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// </summary>
        /// <param name="parent">The parent transform to contain pooled objects</param>
        /// <returns>This builder instance for method chaining</returns>
        public PoolCreationDataBuilder<T> SetParent(Transform parent)
        {
            _parent = parent;
            return this;
        }

        /// <summary>
        /// Distribution array specifying how many of each object type to instantiate. 
        /// For example [50, 50, 20] will instantiate 50 of the first object, 50 of the second object, and 20 of the third object.
        /// The sum of these values will be the total pool size.
        /// Only used by PoolFixed_MultipleObjects. Not needed for PoolFixed or PoolFlexible.
        /// </summary>
        /// <param name="objectDistribution">Array of counts for each object type</param>
        /// <returns>This builder instance for method chaining</returns>
        public PoolCreationDataBuilder<T> SetDistribution_Manual(params int[] objectDistribution)
        {
            _size = objectDistribution.Sum();
            _objectDistribution = objectDistribution;
            return this;
        }

        /// <summary>
        /// Sets a fixed distribution where each object type gets the same count.
        /// For example, SetDistribution_Fixed(3, 50) creates [50, 50, 50] distribution.
        /// The total pool size will be objectsCount * value.
        /// Only used by PoolFixed_MultipleObjects. Not needed for PoolFixed or PoolFlexible.
        /// </summary>
        /// <param name="objectsCount">Number of different object types</param>
        /// <param name="value">Count of objects to create for each type</param>
        /// <returns>This builder instance for method chaining</returns>
        public PoolCreationDataBuilder<T> SetDistribution_Fixed(int objectsCount, int value)
        {
            _objectDistribution = new int[objectsCount];
            Array.Fill(_objectDistribution, value);
            _size = _objectDistribution.Sum();
            return this;
        }

        /// <summary>
        /// Automatically calculates a distribution array based on total size and number of object types.
        /// Uses a weighted distribution algorithm where the first object gets the largest share (50%),
        /// middle objects get progressively smaller shares (60% of remaining), and the last object gets the remainder.
        /// Requires at least 3 object types and a minimum total size of objectsCount * 5.
        /// </summary>
        /// <param name="totalSize">Total number of objects to create (sum of distribution)</param>
        /// <param name="objectsCount">Number of different object types (must be at least 3)</param>
        /// <returns>This builder instance for method chaining</returns>
        /// <exception cref="ArgumentException">Thrown when objectsCount is less than 3 or totalSize is less than objectsCount * 5</exception>
        public PoolCreationDataBuilder<T> SetDistribution_AutoCalculate(int totalSize, int objectsCount)
        {
            if (objectsCount < 3)
                throw new ArgumentException("Distributed array should contain at least 3 different objects");

            var expectedMinimumSize = objectsCount * 5;
            if (totalSize < expectedMinimumSize)
                throw new ArgumentException($"Expected minimum total size of total objects in distributed array should be at least {expectedMinimumSize}");

            _size = totalSize;
            _objectDistribution = new int[objectsCount];

            double distribution_value = totalSize / 2;

            //first
            _objectDistribution[0] = (int)distribution_value;
            //middle
            for (int i = 1; i < objectsCount - 1; i++)
            {
                var local = Math.Ceiling(distribution_value * .6f);
                distribution_value -= local;
                _objectDistribution[i] = (int)local;
            }
            //last
            _objectDistribution[objectsCount - 1] = distribution_value < 0 ? 1 : (int)distribution_value;
            //size safetycheck
            while (totalSize > _objectDistribution.Sum())
            {
                DecreaseDistribution(0);
            }

            void DecreaseDistribution(int index)
            {
                if (index >= objectsCount - 2)
                    return;

                if (_objectDistribution[index] > 1)
                    _objectDistribution[index]--;

                index++;

                if (totalSize == _objectDistribution.Sum())
                    return;

                DecreaseDistribution(index);
            }

            return this;
        }

        /// <summary>
        /// Amount by which the pool will be resized when it runs out of available objects.
        /// Only used by PoolFlexible. Not needed for PoolFixed or PoolFixed_MultipleObjects.
        /// </summary>
        /// <param name="increment">The number of objects to add when the pool needs to resize</param>
        /// <returns>This builder instance for method chaining</returns>
        public PoolCreationDataBuilder<T> SetIncrement(int increment)
        {
            _increment = increment;
            return this;
        }

        /// <summary>
        /// Action called when an object is first created in the pool. This enables you to inject required parameters into a class.
        /// Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// </summary>
        /// <param name="onCreate">Action to call when object is created, receives the component and object type index</param>
        /// <returns>This builder instance for method chaining</returns>
        public PoolCreationDataBuilder<T> WireOnInitialize(Action<T, int> onCreate)
        {
            _onCreate = onCreate;
            return this;
        }

        /// <summary>
        /// Action called when an object is enabled/activated from the pool. This enables you to inject required parameters into a class when object is activated and whenever it is re-enabled.
        /// Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// </summary>
        /// <param name="onEnable">Action to call when object is enabled/activated</param>
        /// <returns>This builder instance for method chaining</returns>
        public PoolCreationDataBuilder<T> WireOnEnable(Action<T> onEnable)
        {
            _onEnable = onEnable;
            return this;
        }

        /// <summary>
        /// Builds and returns an IPoolCreationData instance with all configured parameters.
        /// Call this after setting all desired pool configuration parameters.
        /// </summary>
        /// <returns>An IPoolCreationData instance ready to use for pool creation</returns>
        public IPoolCreationData<T> Build()
        {
            return new PoolCreationData(_name, _size, _parent, _onCreate, _onEnable, _increment, _objectDistribution);
        }

        /// <summary>
        /// Private nested class implementing IPoolCreationData. Only accessible from within PoolCreationDataBuilder.
        /// This is the concrete implementation returned by Build().
        /// </summary>
        private class PoolCreationData : IPoolCreationData<T>
        {
            public string PoolName { get; private set; }
            public int Size { get; private set; }
            public int Increment { get; private set; }
            public Transform ParentContainer { get; private set; }
            public Action<T, int> InitializeAction { get; private set; }
            public Action<T> EnableAction { get; private set; }
            public int[] ObjectDistribution { get; private set; }

            /// <summary>
            /// Creates a new PoolCreationData instance with the specified parameters.
            /// </summary>
            /// <param name="name">Name of the pool</param>
            /// <param name="size">Initial size of the pool</param>
            /// <param name="parent">Parent transform container for pooled objects</param>
            /// <param name="onInit">Action called when an object is first created (optional)</param>
            /// <param name="onEnable">Action called when an object is enabled/activated (optional)</param>
            /// <param name="increment">Amount by which the pool resizes when exhausted (only for PoolFlexible)</param>
            /// <param name="objectDistribution">Distribution array for multiple object types (only for PoolFixed_MultipleObjects)</param>
            public PoolCreationData(
                string name,
                int size,
                Transform parent,
                Action<T, int> onInit = null,
                Action<T> onEnable = null,
                int increment = 0,
                int[] objectDistribution = null)
            {
                PoolName = name;
                Size = size;
                Increment = increment;
                ParentContainer = parent;
                InitializeAction = onInit;
                EnableAction = onEnable;
                ObjectDistribution = objectDistribution ?? Array.Empty<int>();
            }
        }
    }
}
