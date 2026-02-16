using System;
using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    /// <summary>
    /// Fixed-size pool that maintains a constant number of objects. Once the pool is exhausted, 
    /// no more objects can be activated until objects are returned to the pool (deactivated).
    /// Best for scenarios where you know the maximum number of objects needed and want predictable memory usage.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    public class PoolFixed<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly PoolHelper<T> _helper;
        private readonly GameObject _container;
        private T[] _pooledObjects;
        private IPoolCreationData<T> _data;
        private IQueue _queue;

        /// <summary>
        /// Creates a fixed-size pool with a single GameObject reference.
        /// </summary>
        /// <param name="data">Pool creation data containing size, parent container, and initialization actions</param>
        /// <param name="queue">Queue implementation for managing object selection order</param>
        /// <param name="reference">The GameObject prefab to instantiate for all pooled objects</param>
        public PoolFixed(in IPoolCreationData<T> data, IQueue queue, GameObject reference)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));
            if (!reference.TryGetComponent<T>(out _))
                throw new ArgumentException($"Prefab '{reference.name}' must have a {typeof(T).Name} component.", nameof(reference));

            _data = data;
            _queue = queue;
            _pooledObjects = new T[_data.Size];
            _helper = new PoolHelper<T>(_queue);

            _container = _helper.CreateLocalContainer(_data.PoolName, _data.ParentContainer);
            PopulatePool(reference, _data.Size);
        }

        /// <summary>
        /// Populates the pool by instantiating the specified number of objects.
        /// </summary>
        /// <param name="size">Number of objects to instantiate</param>
        private void PopulatePool(GameObject reference, int size)
        {
            for (int index = 0; index < size; index++)
            {
                var obj = UnityEngine.Object.Instantiate(reference, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<T>(out var component))
                {
                    Debug.LogWarning($"{typeof(T).Name} could not be found on {reference.name}. Skipping.");
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
        /// Returns false if no objects are available in the pool.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="obj">The activated object, or null if no object was available</param>
        /// <returns>True if an object was successfully activated, false if the pool is exhausted</returns>
        public bool TryActivateObject(Vector3 position, out T obj)
        {
            if (!_helper.TryDequeObjectSafeguard(_pooledObjects, out var item))
            {
                obj = null;
                return false;
            }

            obj = _helper.ActivateObject(item, position, _data.EnableAction);
            return true;
        }

        /// <summary>
        /// Two-step activation: selects and dequeues an object from the pool without activating it.
        /// Use this when you need to perform additional setup before activating the object.
        /// Follow with TwoStep_EnableObject to activate the selected object.
        /// </summary>
        /// <param name="obj">The selected object, or null if no object was available</param>
        /// <returns>True if an object was successfully selected, false if the pool is exhausted</returns>
        public bool TwoStep_TrySelectAndDequeueObject(out T obj)
        {
            if (!_helper.TryDequeObjectSafeguard(_pooledObjects, out var item))
            {
                obj = null;
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
        /// </summary>
        /// <returns>Array containing all pooled objects</returns>
        public T[] GetPool()
        {
            return _pooledObjects;
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
