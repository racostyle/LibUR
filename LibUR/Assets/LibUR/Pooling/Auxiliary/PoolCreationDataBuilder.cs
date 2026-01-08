using System;
using System.Linq;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public class PoolCreationDataBuilder<T> where T : MonoBehaviour
    {
        private string _name;
        private int _size;
        private int _increment;
        private Transform _parent;
        private Action<T, int> _onCreate;
        private Action<T> _onEnable;
        private int[] _objectDistribution;

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

        public PoolCreationDataBuilder<T> SetDistribution_Fixed(int objectsCount, int value)
        {
            _objectDistribution = new int[objectsCount];
            Array.Fill(_objectDistribution, value);
            _size = _objectDistribution.Sum();
            return this;
        }

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

        public IPoolCreationData<T> Build()
        {
            return new PoolCreationData(_name, _size, _parent, _onCreate, _onEnable, _increment, _objectDistribution);
        }

        /// <summary>
        /// Private nested class implementing IPoolCreationData. Only accessible from within PoolCreationDataBuilder.
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
