using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibUR.Pooling
{
    public interface IPoolByEnum<T, TEnum>
        where T : MonoBehaviour
        where TEnum : Enum
    {
        void DestroyAll(bool alsoDestroyContainer = true);
        Dictionary<TEnum, T[]> GetPool();
        bool TryActivateObject(Vector3 position, TEnum key, out T obj);
        void TwoStep_EnableObject(Vector3 position, T obj);
        bool TwoStep_TrySelectObject(TEnum key, out T obj);
    }
}