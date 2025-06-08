using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public interface IPooledObject<T>
    {
        T Script { get; }
        GameObject GameObject { get; }
        Transform Transform { get; }
    }
}
