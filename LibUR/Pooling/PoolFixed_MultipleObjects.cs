using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    public class PoolFixed_MultipleObjects<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly PoolHelper<T> _helper;
        private readonly GameObject[] _references;
        private GameObject _container;
        private T[] _pool;
        private SPoolCreationData<T> _data;
        private IQueue _queue;

        public PoolFixed_MultipleObjects(in SPoolCreationData<T> data, IQueue queue, GameObject[] references)
        {
            _data = data;
            _queue = queue;
            _references = references;
            _pool = new T[_data.Size];
            _helper = new PoolHelper<T>(_pool, _queue);

            if (_references.Length != _data.ObjectDistribution.Length)
                throw new System.Exception("ObjectRef must match ObjectDistribution length");

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
            for (int index = 0; index < _data.ObjectDistribution.Length; index++)
            {
                for (int x = 0; x < _data.ObjectDistribution[index]; x++)
                {
                    var obj = Object.Instantiate(_references[index], Vector3.zero, Quaternion.identity, _container.transform);
                    if (!obj.TryGetComponent<T>(out var component))
                    {
                        Debug.Log($"{component} could not be found!");
                        continue;
                    }

                    object additionalInfo = null;
                    if (_data.AdditionalInfo.Count > index)
                        additionalInfo = _data.AdditionalInfo[index];

                    _data.InitializeAction?.Invoke(component, index, additionalInfo);
                    _pool[index] = component;
                    obj.SetActive(false);
                    _queue.AddToQueue(index);
                }
            }
            _queue.RebuildQueue();
        }

        public bool TryActivateObject(Vector3 position, out T obj)
        {
            if (!_helper.TryDequeObjectSafeguard(out var item))
            {
                obj = null;
                return false;
            }

            obj = _helper.ActivateObject(item, position, _data.EnableAction);
            return true;
        }

        public T[] GetPool()
        {
            return _pool;
        }

        public void DestroyAll(bool alsoDestroyContainer = true)
        {
            _helper.DestroyAll(alsoDestroyContainer ? _container : null);
        }
    }
}
