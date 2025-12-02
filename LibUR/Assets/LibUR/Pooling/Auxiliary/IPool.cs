using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    public interface IPool<T>
    {
        bool TryActivateObject(Vector3 position, out T obj);
        T[] GetPool();
        void DestroyAll(bool alsoDestroyContainer = true);
    }
}