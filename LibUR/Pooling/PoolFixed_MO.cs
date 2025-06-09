using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    public class PoolFixed_MO<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly GameObject[] _references;
        private GameObject _container;
        private T[] _pool;
        private PoolCreationData<T> _data;
        private IQueue _queue;

        public PoolFixed_MO(in PoolCreationData<T> data, IQueue queue, GameObject[] references)
        {
            _data = data;
            _queue = queue;
            _references = references;
            _pool = new T[_data.Size];

            if (_references.Length != _data.ObjectDistribution.Length)
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
                    var obj = UnityEngine.GameObject.Instantiate(_references[i], Vector3.zero, Quaternion.identity, _container.transform);
                    if (!obj.TryGetComponent<T>(out var component))
                    {
                        Debug.Log($"{component} could not be found!");
                        continue;
                    }

                    _pool[index] = component;
                    _data.InitializeAction?.Invoke(_pool[index]);
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
                    return null;
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
