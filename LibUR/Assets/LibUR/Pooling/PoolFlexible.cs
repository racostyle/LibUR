using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling
{
    /// <summary>
    /// Flexible-size pool that can dynamically resize when exhausted. 
    /// When the pool runs out of available objects, it automatically expands by the Increment amount.
    /// This class is intended for scenarios where resizes might occur occasionally (e.g., bullets, effects),
    /// but not frequently. T[] is faster than List&lt;T&gt; with less overhead when resizes are infrequent.
    /// For frequent resizing, consider using a different data structure.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    public class PoolFlexible<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly PoolHelper<T> _helper;
        private readonly GameObject _references;
        private readonly GameObject _container;
        private IPoolCreationData<T> _data;

        private T[] _pooledObjects;
        private IQueue _queue;

        /// <summary>
        /// Creates a flexible-size pool with a single GameObject reference.
        /// The pool will automatically expand by the Increment amount when exhausted.
        /// </summary>
        /// <param name="data">Pool creation data containing initial size, increment amount, parent container, and initialization actions</param>
        /// <param name="queue">Queue implementation for managing object selection order</param>
        /// <param name="references">The GameObject prefab to instantiate for all pooled objects</param>
        public PoolFlexible(in IPoolCreationData<T> data, IQueue queue, GameObject references)
        {
            if (references == null)
                throw new ArgumentNullException(nameof(references));
            if (!references.TryGetComponent<T>(out _))
                throw new ArgumentException($"Prefab '{references.name}' must have a {typeof(T).Name} component.", nameof(references));

            _data = data;
            _queue = queue;
            _references = references;
            _pooledObjects = new T[_data.Size];
            _helper = new PoolHelper<T>(_queue);

            _container = _helper.CreateLocalContainer(data.PoolName, data.ParentContainer);
            PopulatePool(0, data.Size);
        }

        /// <summary>
        /// Populates a range of the pool by instantiating objects from start to end index.
        /// Used both for initial population and when expanding the pool.
        /// </summary>
        /// <param name="start">Starting index (inclusive) to begin populating</param>
        /// <param name="end">Ending index (exclusive) to stop populating</param>
        private void PopulatePool(int start, int end)
        {
            for (int index = start; index < end; index++)
            {
                var obj = UnityEngine.Object.Instantiate(_references, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<T>(out var component))
                {
                    Debug.LogWarning($"{typeof(T).Name} could not be found on {_references.name}. Skipping.");
                    UnityEngine.Object.Destroy(obj);
                    continue;
                }

                _data.InitializeAction?.Invoke(component, 0);
                _pooledObjects[index] = component;
                obj.SetActive(false);
                _queue.AddToQueue(index);
            }
            _queue.BuildQueue();
        }

        /// <summary>
        /// Attempts to activate an object from the pool at the specified position.
        /// If the pool is exhausted, it will automatically expand by the Increment amount before attempting activation.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="obj">The activated object, or null if activation failed (should not happen with automatic resizing)</param>
        /// <returns>True if an object was successfully activated, false if activation failed</returns>
        public bool TryActivateObject(Vector3 position, out T obj)
        {
            if (!TryGetObject_ResizeIfEmptyQueue(out var item))
            {
                obj = item;
                return false;
            }

            obj = _helper.ActivateObject(item, position, _data.EnableAction);
            return true;
        }

        /// <summary>
        /// Two-step activation: selects and dequeues an object from the pool without activating it.
        /// If the pool is exhausted, it will automatically expand by the Increment amount.
        /// Use this when you need to perform additional setup before activating the object.
        /// Follow with TwoStep_EnableObject to activate the selected object.
        /// </summary>
        /// <param name="obj">The selected object, or null if selection failed</param>
        /// <returns>True if an object was successfully selected, false if selection failed</returns>
        public bool TwoStep_TrySelectAndDequeueObject(out T obj)
        {
            if (!TryGetObject_ResizeIfEmptyQueue(out var item))
            {
                obj = item;
                return false;
            }

            obj = item;
            return true;
        }

        /// <summary>
        /// Two-step activation: activates a previously selected object at the specified position.
        /// Call this after TwoStep_TrySelectAndDequeueObject to complete the activation.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="obj">The object to activate (previously selected via TwoStep_TrySelectAndDequeueObject)</param>
        public void TwoStep_EnableObject(Vector3 position, T obj)
        {
            _helper.ActivateObject(obj, position, _data.EnableAction);
        }

        /// <summary>
        /// Gets all pooled objects, including both active and inactive ones.
        /// Note: The array size may be larger than the initial size if the pool has been expanded.
        /// </summary>
        /// <returns>Array containing all pooled objects</returns>
        public T[] GetPool()
        {
            return _pooledObjects;
        }

        /// <summary>
        /// Attempts to get an object from the pool. If the queue is empty, automatically resizes the pool
        /// by the Increment amount and then attempts to get an object again.
        /// </summary>
        /// <param name="selected">The selected object, or null if selection failed</param>
        /// <returns>True if an object was successfully selected, false otherwise</returns>
        private bool TryGetObject_ResizeIfEmptyQueue(out T selected)
        {
            if (!_helper.TryDequeObjectSafeguard(_pooledObjects, out var item1))
            {
                var oldSize = _pooledObjects.Length;
                Array.Resize(ref _pooledObjects, oldSize + _data.Increment);
                PopulatePool(oldSize, _pooledObjects.Length);

                if (!_helper.TryDequeObjectSafeguard(_pooledObjects, out var item2))
                {
                    selected = null;
                    return false;
                }

                selected = item2;
                return true;
            }

            selected = item1;
            return true;
        }

        /// <summary>
        /// Destroys all pooled objects and optionally the container GameObject.
        /// </summary>
        /// <param name="alsoDestroyContainer">If true, also destroys the container GameObject that holds all pooled objects</param>
        public void DestroyAll(bool alsoDestroyContainer = true)
        {
            _helper.DestroyAll(_pooledObjects, alsoDestroyContainer ? _container : null);
        }
    }
}
