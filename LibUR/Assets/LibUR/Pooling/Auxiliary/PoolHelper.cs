using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    /// <summary>
    /// Helper class providing common functionality for object pooling operations.
    /// Handles container creation, object activation, queue management, and cleanup operations.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour component type to pool</typeparam>
    public class PoolHelper<T> where T : MonoBehaviour
    {
        private readonly IQueue _queue;

        /// <summary>
        /// Creates a pool helper without a queue. Used for pools that don't use queue-based selection (e.g., PoolFixed_ByEnum).
        /// </summary>
        public PoolHelper()
        {
            
        }

        /// <summary>
        /// Creates a pool helper with the specified queue implementation for managing object selection order.
        /// </summary>
        /// <param name="queue">Queue implementation for managing object selection order</param>
        public PoolHelper(IQueue queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// Creates a GameObject container to hold all pooled objects in the Unity hierarchy.
        /// The container is parented to the specified parent container and named based on the pool name.
        /// </summary>
        /// <param name="poolName">Name of the pool, used for generating the container name</param>
        /// <param name="parentContainer">Parent transform to attach the container to</param>
        /// <returns>The created container GameObject</returns>
        internal GameObject CreateLocalContainer(string poolName, Transform parentContainer)
        {
            var container = new GameObject();
            container.transform.SetParent(parentContainer);
            container.name = $"Pool_{poolName}".ToLower();
            return container;
        }

        /// <summary>
        /// Attempts to dequeue an object from the pool with safeguards.
        /// Ensures the queue is not empty by rebuilding it from inactive objects if needed.
        /// Guards against destroyed references (e.g., in editor play mode reloads).
        /// </summary>
        /// <param name="objectsPool">Array of all pooled objects</param>
        /// <param name="item">The dequeued object, or null if no objects are available or queue is null</param>
        /// <returns>True if an object was successfully dequeued, false otherwise</returns>
        internal bool TryDequeObjectSafeguard(T[] objectsPool, out T item)
        {
            if (_queue == null)
            {
                item = null;
                return false;
            }

            if (!EnsureQueueNotEmpty(objectsPool)) 
            {
                item = null;
                return false;
            }

            int index = _queue.Dequeue();
            item = objectsPool[index];

            //guard against destroyed refs in editor play mode reloads
            if (item == null)
                return false;

            return true;
        }

        /// <summary>
        /// Ensures the queue is not empty by rebuilding it from inactive objects if needed.
        /// Scans the pool array for inactive objects and adds their indices to the queue.
        /// </summary>
        /// <param name="objectsPool">Array of all pooled objects</param>
        /// <returns>True if the queue has items after ensuring, false if no inactive objects are available</returns>
        private bool EnsureQueueNotEmpty(T[] objectsPool)
        {
            if (_queue == null)
                return false;

            if (_queue.Count == 0)
            {
                for (int i = 0; i < objectsPool.Length; i++)
                {
                    var item = objectsPool[i];
                    if (item != null && !item.gameObject.activeInHierarchy)
                        _queue.AddToQueue(i);
                }
                _queue.BuildQueue();

                if (_queue.Count == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Destroys all pooled objects and optionally the container GameObject.
        /// Handles both play mode (Destroy) and edit mode (DestroyImmediate) appropriately.
        /// Call this from an owning MonoBehaviour's OnDestroy or when intentionally tearing down the pool.
        /// </summary>
        /// <param name="objectsPool">Array of all pooled objects to destroy</param>
        /// <param name="container">Optional container GameObject to destroy. If null, only objects are destroyed.</param>
        internal void DestroyAll(T[] objectsPool, GameObject container = null)
        {
            for (int i = 0; i < objectsPool.Length; i++)
            {
                var item = objectsPool[i];
                if (item == null) continue;

                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(item.gameObject);
                else
                    UnityEngine.Object.DestroyImmediate(item.gameObject);

                objectsPool[i] = null;
            }

            _queue?.Clear();

            if (container != null)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(container);
                else
                    UnityEngine.Object.DestroyImmediate(container);
            }
        }

        /// <summary>
        /// Activates an object at the specified position and invokes the enable action if provided.
        /// Sets the object's position, rotation, and active state, then calls the optional enable action.
        /// </summary>
        /// <param name="item">The object to activate</param>
        /// <param name="position">World position where the object should be activated</param>
        /// <param name="enableAction">Optional action to invoke when the object is enabled</param>
        /// <returns>The activated object</returns>
        internal T ActivateObject(T item, Vector3 position, Action<T> enableAction)
        {
            item.transform.SetPositionAndRotation(position, Quaternion.identity);
            item.gameObject.SetActive(true);
            enableAction?.Invoke(item);

            return item;
        }
    }
}