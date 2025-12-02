using UnityEngine;

namespace LibUR.Pooling.Auxiliary 
{ 
    public interface IPool<T>
    {
        bool TryActivateObject(Vector3 position, out T obj);
        bool TwoStep_SelectAndDequeueObject(out T obj);
        void TwoStep_EnableObject(Vector3 position, T obj);
        T[] GetPool();
        void DestroyAll(bool alsoDestroyContainer = true);
    }
}