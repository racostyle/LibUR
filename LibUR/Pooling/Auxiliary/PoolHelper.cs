using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    public class PoolHelper<T> where T : MonoBehaviour
    {
        private readonly T[] _pool;
        private readonly IQueue _queue;

        public PoolHelper(T[] pool, IQueue queue)
        {
            _pool = pool;
            _queue = queue;
        }

        internal bool TryDequeObjectSafeguard(out T item)
        {
            if (!EnsureQueueNotEmpty()) 
            {
                item = null;
                return false;
            }

            int index = _queue.Dequeue();
            item = _pool[index];

            //guard against destroyed refs in editor play mode reloads
            if (item == null)
                return false;

            return true;
        }

        private bool EnsureQueueNotEmpty()
        {
            if (_queue.Count == 0)
            {
                for (int i = 0; i < _pool.Length; i++)
                {
                    var item = _pool[i];
                    if (item != null && !item.gameObject.activeInHierarchy)
                        _queue.AddToQueue(i);
                }
                _queue.RebuildQueue();

                if (_queue.Count == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Dispose is a NO-OP for Unity objects. Use Dispose() method for unity lifecycle 
        /// Deactivate and optionally Destroy all spawned instances and the container.
        /// Call this from an owning MonoBehaviour's OnDestroy (or when you intentionally tear down the pool).
        /// </summary>
        internal void DestroyAll(GameObject container = null)
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                var item = _pool[i];
                if (item == null) continue;

                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(item.gameObject);
                else
                    UnityEngine.Object.DestroyImmediate(item.gameObject);

                _pool[i] = null;
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
            enableAction?.Invoke(item);
            item.gameObject.SetActive(true);

            return item;
        }
    }
}