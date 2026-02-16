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
        /// </summary>
        /// <param name="data">Pool creation data</param>
        /// <param name="references">Dictionary mapping enum values to GameObject references</param>
        /// <param name="sizes">Dictionary mapping enum values to pool sizes for each enum</param>
        public PoolFixed_ByEnum(
            in IPoolCreationData<T> data,
            Dictionary<TEnum, GameObject> references)
        {
            _data = data;

            if (references == null)
                throw new ArgumentNullException(nameof(references));

            foreach (var kvp in references)
            {
                var refPrefab = kvp.Value;
                if (refPrefab == null)
                    throw new ArgumentException($"Reference for enum {kvp.Key} is null.", nameof(references));
                if (!refPrefab.TryGetComponent<T>(out _))
                    throw new ArgumentException($"Prefab '{refPrefab.name}' for enum {kvp.Key} must have a {typeof(T).Name} component.", nameof(references));
            }

            _pooledObjects = new Dictionary<TEnum, T[]>();

            _helper = new PoolHelper<T>();
            _container = _helper.CreateLocalContainer(_data.PoolName, _data.ParentContainer);

            PopulatePool(references);
        }

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
        /// Attempts to activate an object of a specific enum type from the pool.
        /// </summary>
        /// <param name="enumValue">The enum value to select from</param>
        /// <param name="position">Position to activate the object at</param>
        /// <param name="obj">The activated object, or null if unavailable</param>
        /// <returns>True if an object was successfully activated, false otherwise</returns>
        public bool TryActivateObject(Vector3 position, TEnum key, out T obj)
        {
            if (!_pooledObjects.TryGetValue(key, out var array) || array == null)
            {
                obj = default;
                return false;
            }

            foreach (var item in array)
            {
                if (!item.gameObject.activeInHierarchy)
                {
                    obj = _helper.ActivateObject(item, position, _data.EnableAction);
                    return true;
                }
            }

            obj = default;
            return false;
        }

        /// <summary>
        /// Two-step activation: selects and dequeues an object from the global pool.
        /// </summary>
        public bool TwoStep_TrySelectObject(TEnum key, out T obj)
        {
            if (!_pooledObjects.TryGetValue(key, out var array) || array == null)
            {
                obj = default;
                return false;
            }

            foreach (var item in array)
            {
                if (!item.gameObject.activeInHierarchy)
                {
                    obj = item;
                    return true;
                }
            }

            obj = default;
            return false;
        }

        /// <summary>
        /// Two-step activation: enables a previously selected object.
        /// </summary>
        public void TwoStep_EnableObject(Vector3 position, T obj)
        {
            _helper.ActivateObject(obj, position, _data.EnableAction);
        }

        /// <summary>
        /// Gets all pooled objects.
        /// </summary>
        public Dictionary<TEnum, T[]> GetPool()
        {
            return _pooledObjects;
        }

        /// <summary>
        /// Destroys all pooled objects and optionally the container.
        /// </summary>
        public void DestroyAll(bool alsoDestroyContainer = true)
        {
            foreach (var item in _pooledObjects)
                _helper.DestroyAll(item.Value, null);

            if (alsoDestroyContainer && _container != null)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(_container);
                else
                    UnityEngine.Object.DestroyImmediate(_container);
                _container = null;
            }
        }
    }
}

