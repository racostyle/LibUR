using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    public abstract class APoolFixed<T> : MonoBehaviour, IPool<T>
    {
        [SerializeField] protected GameObject ObjectRef;

        protected GameObject _container;
        private PooledObject<T>[] _pool;
        private PoolCreationData<T> _data;
        private IQueue _queue;

        protected void Initialize(in PoolCreationData<T> data, IQueue queue)
        {
            _data = data;
            _queue = queue;
            _pool = new PooledObject<T>[_data.Size];

            CreateLocalContainer(_data.PoolName, _data.ParentContainer);
            PopulatePool(_data.Size);
        }

        private void CreateLocalContainer(string poolName, Transform parentContainer)
        {
            _container = new GameObject();
            _container.transform.SetParent(parentContainer);
            _container.name = $"Pool_{poolName}".ToLower();
        }

        private void PopulatePool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                var obj = Instantiate(ObjectRef, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<T>(out var component))
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
                    return null;
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
