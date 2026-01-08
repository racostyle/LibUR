using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    /// <summary>
    /// Interface for object pooling systems. Provides methods for activating objects from a pool
    /// and managing their lifecycle. Supports both single-step and two-step activation patterns.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Attempts to activate an object from the pool at the specified position in a single step.
        /// This is the simplest way to get and activate an object from the pool.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="obj">The activated object, or null if no object was available</param>
        /// <returns>True if an object was successfully activated, false if the pool is exhausted or activation failed</returns>
        bool TryActivateObject(Vector3 position, out T obj);

        /// <summary>
        /// Two-step activation: selects and dequeues an object from the pool without activating it.
        /// Use this when you need to perform additional setup (e.g., configuration, validation) before activating the object.
        /// After calling this method, you must call TwoStep_EnableObject to complete the activation.
        /// </summary>
        /// <param name="obj">The selected object, or null if no object was available</param>
        /// <returns>True if an object was successfully selected, false if the pool is exhausted</returns>
        bool TwoStep_TrySelectAndDequeueObject(out T obj);

        /// <summary>
        /// Two-step activation: activates a previously selected object at the specified position.
        /// Call this after TwoStep_TrySelectAndDequeueObject to complete the activation.
        /// This completes the two-step activation pattern, allowing you to configure the object between selection and activation.
        /// </summary>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="obj">The object to activate (previously selected via TwoStep_TrySelectAndDequeueObject)</param>
        void TwoStep_EnableObject(Vector3 position, T obj);

        /// <summary>
        /// Gets all pooled objects, including both active and inactive ones.
        /// Useful for iterating over all objects, checking states, or performing batch operations.
        /// </summary>
        /// <returns>Array containing all pooled objects</returns>
        T[] GetPool();

        /// <summary>
        /// Destroys all pooled objects and optionally the container GameObject that holds them.
        /// Use this for cleanup when the pool is no longer needed (e.g., on scene unload or game shutdown).
        /// </summary>
        /// <param name="alsoDestroyContainer">If true, also destroys the container GameObject that holds all pooled objects</param>
        void DestroyAll(bool alsoDestroyContainer = true);
    }
}