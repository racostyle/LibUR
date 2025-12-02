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

        private T[] _pooledObjects;
        private IQueue _queue;

        //This class is ment as optional where there MIGHT be resizes. It is not ment for many realocations,
        //but more as a safeguard for bullets, effects... T[] is faster than List<T> and with less overhead
        //IF the reasize is not called often. 
        public PoolFlexible(in SPoolCreationData<T> data, IQueue queue, GameObject references)
        {
            _data = data;
            _queue = queue;
            _references = references;
            _pooledObjects = new T[_data.Size];
            _helper = new PoolHelper<T>(_queue);

            _container = _helper.CreateLocalContainer(data.PoolName, data.ParentContainer);
            PopulatePool(0, data.Size);
        }

        private void PopulatePool(int start, int end)
        {
            for (int index = start; index < end; index++)
            {
                var obj = UnityEngine.Object.Instantiate(_references, Vector3.zero, Quaternion.identity, _container.transform);
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
            if (!TryGetObject_ResizeIfEmptyQueue(out var item))
            {
                obj = item;
                return false;
            }

            obj = _helper.ActivateObject(item, position, _data.EnableAction);
            return true;
        }

        public bool TwoStep_SelectAndDequeueObject(out T obj)
        {
            if (!TryGetObject_ResizeIfEmptyQueue(out var item))
            {
                obj = item;
                return false;
            }

            obj = item;
            return true;
        }

        public void TwoStep_EnableObject(Vector3 position, T obj)
        {
            _helper.ActivateObject(obj, position, _data.EnableAction);
        }

        public T[] GetPool()
        {
            return _pooledObjects;
        }


        private bool TryGetObject_ResizeIfEmptyQueue(out T selected)
        {
            if (!_helper.TryDequeObjectSafeguard(_pooledObjects, out var item1))
            {
                var oldSize = _pooledObjects.Length;
                Array.Resize(ref _pooledObjects, oldSize + _data.Increment);
                PopulatePool(oldSize, _pooledObjects.Length);

                if (!_helper.TryDequeObjectSafeguard(_pooledObjects, out var item2))
                {
                    selected = null;
                    return false;
                }

                selected = item2;
                return true;
            }

            selected = item1;
            return true;
        }

        public void DestroyAll(bool alsoDestroyContainer = true)
        {
            _helper.DestroyAll(_pooledObjects, alsoDestroyContainer ? _container : null);
        }
    }
}
