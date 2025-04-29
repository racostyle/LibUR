using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public struct PoolCreationData<T>
    {
        public readonly string PoolName;
        public readonly int Size;
        public readonly int Increment;
        public readonly Transform ParentContainer;
        public readonly Action<T> InitializeAction;
        public readonly Action<T> EnableAction;
        public readonly int[] ObjectDistribution;

        internal PoolCreationData(string name, int size, Transform parent, Action<T> init, Action<T> enable, int increment = 0, int[] objectDistribution = null)
        {
            PoolName = name;
            Size = size;
            Increment = increment;
            ParentContainer = parent;
            InitializeAction = init;
            EnableAction = enable;
            ObjectDistribution = objectDistribution ?? Array.Empty<int>();
        }
    }
}
