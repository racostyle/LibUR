using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    public class PoolHelper<T> where T : MonoBehaviour
    {
        private readonly IQueue _queue;

        public PoolHelper()
        {
            
        }

        public PoolHelper(IQueue queue)
        {
            _queue = queue;
        }

        internal GameObject CreateLocalContainer(string poolName, Transform parentContainer)
        {
            var container = new GameObject();
            container.transform.SetParent(parentContainer);
            container.name = $"Pool_{poolName}".ToLower();
            return container;
        }

        internal bool TryDequeObjectSafeguard(T[] objectsPool, out T item)
        {
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

        private bool EnsureQueueNotEmpty(T[] objectsPool)
        {
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
        /// Dispose is a NO-OP for Unity objects. Use Dispose() method for unity lifecycle 
        /// Deactivate and optionally Destroy all spawned instances and the container.
        /// Call this from an owning MonoBehaviour's OnDestroy (or when you intentionally tear down the objectsPool).
        /// </summary>
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

            _queue.Clear();

            if (container != null)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(container);
                else
                    UnityEngine.Object.DestroyImmediate(container);
            }
        }

        internal T ActivateObject(T item, Vector3 position, Action<T> enableAction)
        {
            item.transform.SetPositionAndRotation(position, Quaternion.identity);
            item.gameObject.SetActive(true);
            enableAction?.Invoke(item);

            return item;
        }
    }
}