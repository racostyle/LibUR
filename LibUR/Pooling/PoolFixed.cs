using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    public class PoolFixed<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly GameObject _objectRef;
        private readonly GameObject _container;
        private T[] _pool;
        private PoolCreationData<T> _data;
        private IQueue _queue;

        public PoolFixed(in PoolCreationData<T> data, IQueue queue, GameObject reference)
        {
            _data = data;
            _queue = queue;
            _objectRef = reference;
            _pool = new T[_data.Size];

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
                if (!obj.TryGetComponent<T>(out var component))
                {
                    Debug.Log($"{component} could not be found!");
                    continue;
                }

                _pool[i] = component;
                _data.InitializeAction?.Invoke(_pool[i]);
                obj.SetActive(false);
                _queue.AddToQueue(i);
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
                    return default;
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
