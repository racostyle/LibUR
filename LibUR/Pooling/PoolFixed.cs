using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;
using UnityEngine.Pool;

namespace LibUR.Pooling
{
    public class PoolFixed<T> : IPool<T>
    {
        private readonly GameObject _objectRef;
        private readonly GameObject _container;
        private IPooledObject<T>[] _pool;
        private PoolCreationData<T> _data;
        private IQueue _queue;

        public PoolFixed(in PoolCreationData<T> data, IQueue queue, GameObject reference)
        {
            _data = data;
            _queue = queue;
            _objectRef = reference;
            _pool = new IPooledObject<T>[_data.Size];

            _container = CreateLocalContainer(_data.PoolName, _data.ParentContainer);
            PopulatePool(_data.Size);
        }

        private GameObject CreateLocalContainer(string poolName, Transform parentContainer)
        {
            var container = new GameObject();
            container.transform.SetParent(parentContainer);
            container.name = $"Pool_{poolName}".ToLower();
            return container;
        }

        private void PopulatePool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                var obj = UnityEngine.GameObject.Instantiate(_objectRef, Vector3.zero, Quaternion.identity, _container.transform);
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
                    return null;
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
