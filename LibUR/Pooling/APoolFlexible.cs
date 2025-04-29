using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling
{
    public abstract class APoolFlexible<T> : MonoBehaviour, IPool<T>
    {
        [SerializeField] protected GameObject ObjectRef;

        protected GameObject _container;
        PoolCreationData<T> _data;
        private int _maxPoolSize;

        private PooledObject<T>[] _pool;
        private IQueue _queue;

        protected void Initialize(in PoolCreationData<T> data, IQueue queue)
        {
            _data = data;
            _queue = queue;
            _maxPoolSize = data.Size;
            _pool = new PooledObject<T>[_data.Size];

            CreateLocalContainer(data.PoolName, data.ParentContainer);
            PopulatePool(0, data.Size);
        }

        private void CreateLocalContainer(string poolName, Transform parentContainer)
        {
            _container = new GameObject();
            _container.transform.SetParent(parentContainer);
            _container.name = $"Pool_{poolName}".ToLower();
        }

        private void PopulatePool(int start, int size)
        {
            for (int i = start; i < size; i++)
            {
                var obj = Instantiate(ObjectRef, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent(out T component))
                {
                    Debug.Log($"{component} could not be found!");
                    continue;
                }

                _pool[i] = new PooledObject<T>(component, obj);
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

        public PooledObject<T> ActivatePooledObject(Vector3 position)
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

        public PooledObject<T>[] GetPool()
        {
            return _pool;
        }

        public int QueueLength()
        {
            return _queue.Count;
        }

        protected void Dispose()
        {
            if (_pool != null)
            {
                for (int i = 0; i < _pool.Length; i++)
                {
                    if (_pool[i].GameObject != null)
                        Destroy(_pool[i].GameObject);
                    _pool[i] = null;
                }
            }

            if (_container != null)
                Destroy(_container);
        }
    }
}
