using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using System;
using UnityEngine;

namespace LibUR.Pooling
{
    public class PoolFlexible<T> : IPool<T> where T : MonoBehaviour
    {
        private readonly PoolHelper<T> _helper;
        private readonly GameObject _references;
        private readonly GameObject _container;
        private SPoolCreationData<T> _data;

        private T[] _pool;
        private IQueue _queue;

        //This class is ment as optional where there MIGH be resizes. It is not ment for many realocations,
        //but more as a safeguard for bullets, effects... T[] is faster than List<T> and with less overhead
        //IF the reasize is not called often. 

        public PoolFlexible(in SPoolCreationData<T> data, IQueue queue, GameObject references)
        {
            _data = data;
            _queue = queue;
            _references = references;
            _pool = new T[_data.Size];
            _helper = new PoolHelper<T>(_pool, _queue);

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
            for (int index = start; index < size; index++)
            {
                var obj = UnityEngine.Object.Instantiate(_references, Vector3.zero, Quaternion.identity, _container.transform);
                if (!obj.TryGetComponent<T>(out var component))
                {
                    Debug.Log($"{component} could not be found!");
                    continue;
                }

                object additionalInfo = null;
                if (_data.AdditionalInfo.Count > index)
                    additionalInfo = _data.AdditionalInfo[index];

                _data.InitializeAction?.Invoke(component, 0, additionalInfo);
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
