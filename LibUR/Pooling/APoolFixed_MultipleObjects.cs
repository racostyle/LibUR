using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    public abstract class APoolFixed_MultipleObjects<T> : MonoBehaviour, IPool<T>
    {
        [SerializeField] protected GameObject[] ObjectRef;

        protected GameObject _container;
        private PooledObject<T>[] _pool;
        private PoolCreationData<T> _data;
        private IQueue _queue;

        protected void Initialize(in PoolCreationData<T> data, IQueue queue)
        {
            _data = data;
            _queue = queue;
            _pool = new PooledObject<T>[_data.Size];

            if (ObjectRef.Length != _data.ObjectDistribution.Length)
                throw new System.Exception("ObjectRef must much ObjectDistribution length");

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
            var index = 0;
            for (int i = 0; i < _data.ObjectDistribution.Length; i++)
            {
                for (int x = 0; x < _data.ObjectDistribution[i]; x++)
                {
                    var obj = Instantiate(ObjectRef[i], Vector3.zero, Quaternion.identity, _container.transform);
                    if (!obj.TryGetComponent<T>(out var component))
                    {
                        Debug.Log($"{component} could not be found!");
                        continue;
                    }

                    _pool[index] = new PooledObject<T>(component, obj);
                    _data.InitializeAction?.Invoke(_pool[index].Script);
                    obj.SetActive(false);
                    _queue.AddToQueue(index);
                    index++;
                }
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
