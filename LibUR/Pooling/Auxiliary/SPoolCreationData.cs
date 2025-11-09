using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public struct SPoolCreationData<T>
    {
        public readonly string PoolName;
        public readonly int Size;
        public readonly int Increment;
        public readonly Transform ParentContainer;
        public readonly Action<T, int, object> InitializeAction;
        public readonly Action<T> EnableAction;
        public readonly int[] ObjectDistribution;
        public readonly List<object> AdditionalInfo;

        internal SPoolCreationData(
            string name, 
            int size, 
            Transform parent, 
            Action<T, int, object> onInit = null, 
            Action<T> onEnable = null, 
            int increment = 0, 
            int[] objectDistribution = null,
            List<object> additionalInfo = null)
        {
            PoolName = name;
            Size = size;
            Increment = increment;
            ParentContainer = parent;
            InitializeAction = onInit;
            EnableAction = onEnable;
            ObjectDistribution = objectDistribution ?? Array.Empty<int>();
            AdditionalInfo = additionalInfo ?? new List<object>();
        }
    }
}
