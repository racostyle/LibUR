using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling
{
    public class PoolFlexible<T> : IPool<T>
    {
        private readonly GameObject _references;
        private readonly GameObject _container;
        private PoolCreationData<T> _data;
        private int _maxPoolSize;

        private IPooledObject<T>[] _pool;
        private IQueue _queue;

        public PoolFlexible (in PoolCreationData<T> data, IQueue queue, GameObject references)
        {
            _data = data;
            _queue = queue;
            _references = references;
            _maxPoolSize = data.Size;
            _pool = new IPooledObject<T>[_data.Size];

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
            for (int i = start; i < size; i++)
            {
                var obj = UnityEngine.GameObject.Instantiate(_references, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<IPooledObject<T>>(out var component))
                {
                    Debug.Log($"{component} could not be found!");
                    continue;
                }

                _pool[i] = component;
                _data.InitializeAction?.Invoke(_pool[i].Script);
                obj.SetActive(false);
                _queue.AddToQueue(i);
            }
            _queue.RebuildQueue();
        }

        private void PopulateQueue()
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                if (!_pool[i].GameObject.activeInHierarchy)
                    _queue.AddToQueue(i);
            }
            _queue.RebuildQueue();
        }

        public IPooledObject<T> ActivateObject(Vector3 position)
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

            _pool[index].GameObject.SetActive(true);
            _pool[index].Transform.position = position;
            _data.EnableAction?.Invoke(_pool[index].Script);
            return _pool[index];
        }

        public IPooledObject<T>[] GetPool()
        {
            return _pool;
        }

        public void Dispose()
        {

        }
    }
}
