using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling
{
    public class PoolFlexible<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly GameObject _references;
        private readonly GameObject _container;
        private SPoolCreationData<T> _data;
        private int _maxPoolSize;

        private T[] _pool;
        private IQueue _queue;

        public PoolFlexible (in SPoolCreationData<T> data, IQueue queue, GameObject references)
        {
            _data = data;
            _queue = queue;
            _references = references;
            _maxPoolSize = data.Size;
            _pool = new T[_data.Size];

            _container = CreateLocalContainer(data.PoolName, data.ParentContainer);
            PopulatePool(0, data.Size);
        }

        private GameObject CreateLocalContainer(string poolName, Transform parentContainer)
        {
            var container = new GameObject();
            container.transform.SetParent(parentContainer);
            container.name = $"Pool_{poolName}".ToLower();
            return container;
        }

        private void PopulatePool(int start, int size)
        {
            for (int index = start; index < size; index++)
            {
                var obj = UnityEngine.Object.Instantiate(_references, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<T>(out var component))
                {
                    Debug.Log($"{component} could not be found!");
                    continue;
                }

                _data.InitializeAction?.Invoke(component);
                _pool[index] = component;
                obj.SetActive(false);
                _queue.AddToQueue(index);
            }
            _queue.RebuildQueue();
        }

        private void PopulateQueue()
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                if (!_pool[i].gameObject.activeInHierarchy)
                    _queue.AddToQueue(i);
            }
            _queue.RebuildQueue();
        }

        public T ActivateObject(Vector3 position)
        {
            if (_queue.Count == 0)
            {
                PopulateQueue();
                if (_queue.Count == 0)
                {
                    var currentSize = _maxPoolSize;
                    Array.Resize(ref _pool, _maxPoolSize + _data.Increment);
                    PopulatePool(currentSize, _pool.Length);
                    _maxPoolSize += _data.Increment;
                }
            }

            int index = _queue.Dequeue();

            _pool[index].gameObject.SetActive(true);
            _pool[index].transform.position = position;
            _data.EnableAction?.Invoke(_pool[index]);
            return _pool[index];
        }

        public T[] GetPool()
        {
            return _pool;
        }

        public void Dispose()
        {

        }
    }
}
