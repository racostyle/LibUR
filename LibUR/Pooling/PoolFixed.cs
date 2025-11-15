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
        private T[] _pool;
        private SPoolCreationData<T> _data;
        private IQueue _queue;

        public PoolFixed(in SPoolCreationData<T> data, IQueue queue, GameObject reference)
        {
            _data = data;
            _queue = queue;
            _objectRef = reference;
            _pool = new T[_data.Size];
            _helper = new PoolHelper<T>(_pool, _queue);

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
            for (int index = 0; index < size; index++)
            {
                var obj = Object.Instantiate(_objectRef, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<T>(out var component))
                {
                    Debug.Log($"{component} could not be found!");
                    continue;
                }

                _data.InitializeAction?.Invoke(component, 0);
                _pool[index] = component;
                obj.SetActive(false);
                _queue.AddToQueue(index);
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
