using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibUR.Pooling
{
    /// <summary>
    /// Interface for object pooling systems that support selection by enum value.
    /// Extends basic pooling functionality to allow activating objects of a specific enum type.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    /// <typeparam name="TEnum">The enum type used for selecting objects from specific pools</typeparam>
    public interface IPoolByEnum<T, TEnum>
        where T : MonoBehaviour
        where TEnum : Enum
    {
        /// <summary>
        /// Attempts to activate an object of a specific enum type from the pool at the specified position.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="key">The enum value specifying which pool to select from</param>
        /// <param name="obj">The activated object, or null if no object of that type was available</param>
        /// <returns>True if an object was successfully activated, false if the pool for that enum type is exhausted</returns>
        bool TryActivateObject(Vector3 position, TEnum key, out T obj);

        /// <summary>
        /// Two-step activation: selects an object of a specific enum type from the pool without activating it.
        /// Use this when you need to perform additional setup before activating the object.
        /// Follow with TwoStep_EnableObject to activate the selected object.
        /// </summary>
        /// <param name="key">The enum value specifying which pool to select from</param>
        /// <param name="obj">The selected object, or null if no object of that type was available</param>
        /// <returns>True if an object was successfully selected, false if the pool for that enum type is exhausted</returns>
        bool TwoStep_TrySelectObject(TEnum key, out T obj);

        /// <summary>
        /// Two-step activation: activates a previously selected object at the specified position.
        /// Call this after TwoStep_TrySelectObject to complete the activation.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="obj">The object to activate (previously selected via TwoStep_TrySelectObject)</param>
        void TwoStep_EnableObject(Vector3 position, T obj);

        /// <summary>
        /// Gets all pooled objects organized by enum type.
        /// Returns a dictionary where each key is an enum value and the value is an array of pooled objects for that enum type.
        /// </summary>
        /// <returns>Dictionary mapping enum values to arrays of pooled objects</returns>
        Dictionary<TEnum, T[]> GetPool();

        /// <summary>
        /// Destroys all pooled objects and optionally the container GameObject that holds them.
        /// Use this for cleanup when the pool is no longer needed (e.g., on scene unload or game shutdown).
        /// </summary>
        /// <param name="alsoDestroyContainer">If true, also destroys the container GameObject that holds all pooled objects</param>
        void DestroyAll(bool alsoDestroyContainer = true);
    }
}