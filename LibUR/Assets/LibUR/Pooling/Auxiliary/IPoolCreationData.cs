using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    /// <summary>
    /// Configuration passed into a pool. Built by <see cref="PoolCreationDataBuilder{T}"/>.
    /// </summary>
    public struct PoolCreationData<T>
    {
        public string PoolName;
        public int Size;
        public int Increment;
        public Transform ParentContainer;
        public Action<T, int> InitializeAction;
        public Action<T> EnableAction;
        public int[] ObjectDistribution;
    }
}
