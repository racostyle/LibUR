using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    public interface IPool<T>
    {
        PooledObject<T> ActivatePooledObject(Vector3 position);
        PooledObject<T>[] GetPool();
    }
}