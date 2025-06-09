using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    public interface IPool<T>
    {
        T ActivateObject(Vector3 position);
        T[] GetPool();
        void Dispose();
    }
}