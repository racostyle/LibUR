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
        private T[] _pooledObjects;
        private SPoolCreationData<T> _data;
        private IQueue _queue;

        public PoolFixed_MultipleObjects(in SPoolCreationData<T> data, IQueue queue, GameObject[] references)
        {
            _data = data;
            _queue = queue;
            _references = references;
            _pooledObjects = new T[_data.Size];
            _helper = new PoolHelper<T>(_pooledObjects, _queue);

            if (_references.Length != _data.ObjectDistribution.Length)
                throw new System.Exception("ObjectRef must match ObjectDistribution length");

            _container = _helper.CreateLocalContainer(_data.PoolName, _data.ParentContainer);
            PopulatePool(_data.Size);
        }

        private void PopulatePool(int size)
        {
            int spawnIndex = 0;
            for (int objectTypeIndex = 0; objectTypeIndex < _data.ObjectDistribution.Length; objectTypeIndex++)
            {
                for (int specificObjectCounter = 0; specificObjectCounter < _data.ObjectDistribution[objectTypeIndex]; specificObjectCounter++)
                {
                    var obj = Object.Instantiate(_references[objectTypeIndex], Vector3.zero, Quaternion.identity, _container.transform);
                    if (!obj.TryGetComponent<T>(out var component))
                    {
                        Debug.Log($"{component} could not be found!");
                        continue;
                    }

                    _data.InitializeAction?.Invoke(component, objectTypeIndex);

                    _pooledObjects[spawnIndex] = component;
                    obj.SetActive(false);
                    _queue.AddToQueue(spawnIndex);
                    spawnIndex++;
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
            return _pooledObjects;
        }

        public void DestroyAll(bool alsoDestroyContainer = true)
        {
            _helper.DestroyAll(alsoDestroyContainer ? _container : null);
        }
    }
}
