using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    /// <summary>
    /// Immutable struct containing all data needed to create a pool.
    /// Provides a simple way to pass pool configuration without using the builder pattern.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    public struct SPoolCreationData<T>
    {
        /// <summary>
        /// Name of the pool. Used for organizing pooled objects in the hierarchy.
        /// </summary>
        public readonly string PoolName;
        
        /// <summary>
        /// Initial size of the pool. In PoolFixed_MultipleObjects, this is overwritten by ObjectDistribution sum.
        /// </summary>
        public readonly int Size;
        
        /// <summary>
        /// Amount by which the pool will be resized when it runs out of available objects.
        /// Only used by PoolFlexible. Not needed for PoolFixed or PoolFixed_MultipleObjects.
        /// </summary>
        public readonly int Increment;
        
        /// <summary>
        /// Parent transform container for pooled objects. Used to organize objects in the Unity hierarchy.
        /// </summary>
        public readonly Transform ParentContainer;
        
        /// <summary>
        /// Action called when an object is first created in the pool. Receives the component and object type index.
        /// Can be null if no initialization is needed.
        /// </summary>
        public readonly Action<T, int> InitializeAction;
        
        /// <summary>
        /// Action called when an object is enabled/activated from the pool.
        /// Can be null if no enable action is needed.
        /// </summary>
        public readonly Action<T> EnableAction;
        
        /// <summary>
        /// Distribution array specifying how many of each object type to instantiate.
        /// Only used by PoolFixed_MultipleObjects. Empty array for other pool types.
        /// </summary>
        public readonly int[] ObjectDistribution;

        /// <summary>
        /// Creates a new pool creation data structure with the specified parameters.
        /// </summary>
        /// <param name="name">Name of the pool</param>
        /// <param name="size">Initial size of the pool</param>
        /// <param name="parent">Parent transform container for pooled objects</param>
        /// <param name="onInit">Action called when an object is first created (optional)</param>
        /// <param name="onEnable">Action called when an object is enabled/activated (optional)</param>
        /// <param name="increment">Amount by which the pool resizes when exhausted (only for PoolFlexible)</param>
        /// <param name="objectDistribution">Distribution array for multiple object types (only for PoolFixed_MultipleObjects)</param>
        internal SPoolCreationData(
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
