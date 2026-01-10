using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibUR.Pooling
{
    /// <summary>
    /// Pool that allows selecting objects by enum value. Implements IPool&lt;T&gt; for compatibility.
    /// Standard IPool methods operate on all pooled objects, while enum-specific methods select from a specific enum's pool.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    /// <typeparam name="TEnum">The enum type used for selection (must be System.Enum)</typeparam>
    public class PoolFixed_ByEnum<T, TEnum> : IPoolByEnum<T, TEnum> where T : MonoBehaviour where TEnum : Enum
    {
        private PoolHelper<T> _helper;
        private GameObject _container;
        private Dictionary<TEnum, T[]> _pooledObjects;
        private IPoolCreationData<T> _data;

        /// <summary>
        /// Creates a pool that allows selection by enum value.
        /// Each enum value maps to its own separate pool of objects.
        /// The pool size for each enum type is determined by the ObjectDistribution array in the pool creation data.
        /// </summary>
        /// <param name="data">Pool creation data containing ObjectDistribution array and other settings</param>
        /// <param name="references">Dictionary mapping enum values to GameObject references. Keys must match the order in ObjectDistribution.</param>
        /// <exception cref="ArgumentNullException">Thrown when references is null</exception>
        public PoolFixed_ByEnum(
            in IPoolCreationData<T> data,
            Dictionary<TEnum, GameObject> references)
        {
            _data = data;

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            _pooledObjects = new Dictionary<TEnum, T[]>();

            _helper = new PoolHelper<T>();
            _container = _helper.CreateLocalContainer(_data.PoolName, _data.ParentContainer);

            PopulatePool(references);
        }

        /// <summary>
        /// Populates the pool by instantiating objects for each enum value according to ObjectDistribution.
        /// For each enum value in the references dictionary, creates the number of objects specified in ObjectDistribution at the corresponding index.
        /// </summary>
        /// <param name="references">Dictionary mapping enum values to GameObject references</param>
        private void PopulatePool(Dictionary<TEnum, GameObject> references)
        {
            int globalIndex = 0;
            foreach (var kvp in references)
            {
                TEnum enumValue = kvp.Key;
                GameObject reference = kvp.Value;

                var localSize = _data.ObjectDistribution[globalIndex];
                var array = new T[localSize];

                for (int i = 0; i < localSize; i++)
                {
                    var obj = UnityEngine.Object.Instantiate(reference, Vector3.zero, Quaternion.identity, _container.transform);
                    if (!obj.TryGetComponent<T>(out var component))
                    {
                        Debug.LogWarning($"{component} could not be found on {reference.name}!");
                        UnityEngine.Object.Destroy(obj);
                        continue;
                    }

                    array[i] = component;
                    _data.InitializeAction?.Invoke(component, globalIndex);

                    obj.SetActive(false);
                }
                _pooledObjects[enumValue] = array;
                globalIndex++;
            }
        }

        /// <summary>
        /// Attempts to activate an object of a specific enum type from the pool at the specified position.
        /// Searches for the first inactive object in the specified enum's pool and activates it.
        /// Returns false if no objects of that enum type are available.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="key">The enum value specifying which pool to select from</param>
        /// <param name="obj">The activated object, or default if no object of that type was available</param>
        /// <returns>True if an object was successfully activated, false if the pool for that enum type is exhausted</returns>
        public bool TryActivateObject(Vector3 position, TEnum key, out T obj)
        {
            if (!_pooledObjects.TryGetValue(key, out var array))
            {
                obj = default;
                return false;
            }

            foreach (var item in array)
            {
                if (item != null && !item.gameObject.activeInHierarchy)
                {
                    obj = _helper.ActivateObject(item, position, _data.EnableAction);
                    return true;
                }
            }

            obj = default;
            return false;
        }

        /// <summary>
        /// Two-step activation: selects an object of a specific enum type from the pool without activating it.
        /// Searches for the first inactive object in the specified enum's pool.
        /// Use this when you need to perform additional setup before activating the object.
        /// Follow with TwoStep_EnableObject to activate the selected object.
        /// </summary>
        /// <param name="key">The enum value specifying which pool to select from</param>
        /// <param name="obj">The selected object, or default if no inactive object of that type was available</param>
        /// <returns>True if an object was successfully selected, false if the pool for that enum type is exhausted</returns>
        public bool TwoStep_TrySelectObject(TEnum key, out T obj)
        {
            if (!_pooledObjects.TryGetValue(key, out var array))
            {
                obj = default;
                return false;
            }

            foreach (var item in array)
            {
                if (item != null && !item.gameObject.activeInHierarchy)
                {
                    obj = item;
                    return true;
                }
            }

            obj = default;
            return false;
        }

        /// <summary>
        /// Two-step activation: activates a previously selected object at the specified position.
        /// Call this after TwoStep_TrySelectObject to complete the activation.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="obj">The object to activate (previously selected via TwoStep_TrySelectObject)</param>
        public void TwoStep_EnableObject(Vector3 position, T obj)
        {
            _helper.ActivateObject(obj, position, _data.EnableAction);
        }

        /// <summary>
        /// Gets all pooled objects organized by enum type.
        /// Returns a dictionary where each key is an enum value and the value is an array of pooled objects for that enum type.
        /// </summary>
        /// <returns>Dictionary mapping enum values to arrays of pooled objects</returns>
        public Dictionary<TEnum, T[]> GetPool()
        {
            return _pooledObjects;
        }

        /// <summary>
        /// Destroys all pooled objects across all enum types and optionally the container GameObject.
        /// Iterates through all enum pools and destroys their objects, then optionally destroys the container.
        /// Use this for cleanup when the pool is no longer needed (e.g., on scene unload or game shutdown).
        /// </summary>
        /// <param name="alsoDestroyContainer">If true, also destroys the container GameObject that holds all pooled objects</param>
        public void DestroyAll(bool alsoDestroyContainer = true)
        {
            foreach (var item in _pooledObjects)
            {
                _helper.DestroyAll(item.Value, alsoDestroyContainer ? _container : null);
            }
        }
    }
}

