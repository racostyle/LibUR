using System;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public struct SPoolCreationData<T>
    {
        public readonly string PoolName;
        public readonly int Size;
        public readonly int Increment;
        public readonly Transform ParentContainer;
        public readonly Action<T> InitializeAction;
        public readonly Action<T> EnableAction;
        public readonly int[] ObjectDistribution;

        internal SPoolCreationData(
            string name, 
            int size, 
            Transform parent, 
            Action<T> onInit = null, 
            Action<T> onEnable = null, 
            int increment = 0, 
            int[] objectDistribution = null)
        {
            PoolName = name;
            Size = size;
            Increment = increment;
            ParentContainer = parent;
            InitializeAction = onInit;
            EnableAction = onEnable;
            ObjectDistribution = objectDistribution ?? Array.Empty<int>();
        }
    }
}
