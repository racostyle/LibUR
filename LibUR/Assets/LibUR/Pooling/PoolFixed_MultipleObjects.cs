using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    /// <summary>
    /// Fixed-size pool that supports multiple different GameObject references. 
    /// The pool is populated according to ObjectDistribution array, which specifies how many of each object type to instantiate.
    /// Objects are selected randomly from the entire pool regardless of type.
    /// Best for scenarios where you need multiple object types but don't need to select by type.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    public class PoolFixed_MultipleObjects<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly PoolHelper<T> _helper;
        private GameObject _container;
        private T[] _pooledObjects;
        private IPoolCreationData<T> _data;
        private IQueue _queue;

        /// <summary>
        /// Creates a fixed-size pool with multiple GameObject references.
        /// The number of references must match the length of ObjectDistribution in the pool creation data.
        /// </summary>
        /// <param name="data">Pool creation data containing ObjectDistribution array and other settings</param>
        /// <param name="queue">Queue implementation for managing object selection order</param>
        /// <param name="references">Array of GameObject prefabs to instantiate. Length must match ObjectDistribution length.</param>
        /// <exception cref="System.Exception">Thrown when references array length doesn't match ObjectDistribution length</exception>
        public PoolFixed_MultipleObjects(in IPoolCreationData<T> data, IQueue queue, GameObject[] references)
        {
            _data = data;
            _queue = queue;
            _pooledObjects = new T[_data.Size];
            _helper = new PoolHelper<T>(_queue);

            if (references.Length != _data.ObjectDistribution.Length)
                throw new System.Exception("ObjectRef must match ObjectDistribution length");

            _container = _helper.CreateLocalContainer(_data.PoolName, _data.ParentContainer);
            PopulatePool(references, _data.Size);
        }

        /// <summary>
        /// Populates the pool by instantiating objects according to ObjectDistribution.
        /// For each reference, instantiates the number specified in ObjectDistribution at the corresponding index.
        /// </summary>
        /// <param name="size">Total number of objects to instantiate (sum of ObjectDistribution values)</param>
        private void PopulatePool(GameObject[] references, int size)
        {
            int spawnIndex = 0;
            for (int objectTypeIndex = 0; objectTypeIndex < _data.ObjectDistribution.Length; objectTypeIndex++)
            {
                for (int specificObjectCounter = 0; specificObjectCounter < _data.ObjectDistribution[objectTypeIndex]; specificObjectCounter++)
                {
                    var obj = Object.Instantiate(references[objectTypeIndex], Vector3.zero, Quaternion.identity, _container.transform);
                    if (!obj.TryGetComponent<T>(out var component))
                    {
                        Debug.Log($"{component} could not be found!");
                        continue;
                    }

                    _data.InitializeAction?.Invoke(component, objectTypeIndex);

                    _pooledObjects[spawnIndex] = component;
                    obj.SetActive(false);
                    _queue.AddToQueue(spawnIndex);
                    spawnIndex++;
                }
            }
            _queue.BuildQueue();
        }

        /// <summary>
        /// Attempts to activate an object from the pool at the specified position.
        /// Objects are selected from the entire pool regardless of type.
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
