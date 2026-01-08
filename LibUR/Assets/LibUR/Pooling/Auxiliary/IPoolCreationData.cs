using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public interface IPoolCreationData<T>
    {
        /// <summary>
        /// Name of the pool. Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// </summary>
        string PoolName { get; }
        
        /// <summary>
        /// Initial size of the pool. Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// In PoolFixed_MultipleObjects, this value is overwritten by ObjectDistribution sum.
        /// </summary>
        int Size { get; }
        
        /// <summary>
        /// Amount by which the pool will be resized when it runs out of available objects.
        /// Only used by PoolFlexible. Not needed for PoolFixed or PoolFixed_MultipleObjects.
        /// </summary>
        int Increment { get; }
        
        /// <summary>
        /// Parent transform container for pooled objects. Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// </summary>
        Transform ParentContainer { get; }
        
        /// <summary>
        /// Action called when an object is first created in the pool. Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// </summary>
        Action<T, int> InitializeAction { get; }
        
        /// <summary>
        /// Action called when an object is enabled/activated from the pool. Used by all pool types: PoolFixed, PoolFixed_MultipleObjects, and PoolFlexible.
        /// </summary>
        Action<T> EnableAction { get; }
        
        /// <summary>
        /// Distribution array specifying how many of each object type to instantiate.
        /// Only used by PoolFixed_MultipleObjects. Not needed for PoolFixed or PoolFlexible.
        /// </summary>
        int[] ObjectDistribution { get; }
    }
}
