using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

namespace LibUR.Pooling
{
    public class PoolFixed<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly PoolHelper<T> _helper;
        private readonly GameObject _objectRef;
        private readonly GameObject _container;
        private T[] _pooledObjects;
        private SPoolCreationData<T> _data;
        private IQueue _queue;

        public PoolFixed(in SPoolCreationData<T> data, IQueue queue, GameObject reference)
        {
            _data = data;
            _queue = queue;
            _objectRef = reference;
            _pooledObjects = new T[_data.Size];
            _helper = new PoolHelper<T>(_queue);

            _container = _helper.CreateLocalContainer(_data.PoolName, _data.ParentContainer);
            PopulatePool(_data.Size);
        }

        private void PopulatePool(int size)
        {
            for (int index = 0; index < size; index++)
            {
                var obj = Object.Instantiate(_objectRef, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<T>(out var component))
                {
                    Debug.Log($"{component} could not be found!");
                    continue;
                }

                _data.InitializeAction?.Invoke(component, 0);
                _pooledObjects[index] = component;
                obj.SetActive(false);
                _queue.AddToQueue(index);
            }
            _queue.BuildQueue();
        }

        public bool TryActivateObject(Vector3 position, out T obj)
        {
            if (!_helper.TryDequeObjectSafeguard(_pooledObjects, out var item))
            {
                obj = null;
                return false;
            }

            obj = _helper.ActivateObject(item, position, _data.EnableAction);
            return true;
        }

        public T[] GetPool()
        {
            return _pooledObjects;
        }

        public void DestroyAll(bool alsoDestroyContainer = true)
        {
            _helper.DestroyAll(_pooledObjects, alsoDestroyContainer ? _container : null);
        }
    }
}
