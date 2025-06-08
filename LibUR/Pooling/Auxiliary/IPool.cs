using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    public interface IPool<T>
    {
        IPooledObject<T> ActivateObject(Vector3 position);
        IPooledObject<T>[] GetPool();
        void Dispose();
    }
}