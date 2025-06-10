using System;
using System.Linq;
using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public class PoolCreationDataBuilder<T> where T : MonoBehaviour
    {
        private string _name;
        private int _size;
        private int _increment;
        private Transform _parent;
        private Action<T> _onCreate;
        private Action<T> _onEnable;
        private int[] _objectDistribution;

        public PoolCreationDataBuilder(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Initial size of the pool. In FixedPool this is maximum size and in MultipleObjectsPool this will be overwritten by SetDistribution method
        /// </summary>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public PoolCreationDataBuilder<T> SetSize(int size)
        {
            _size = size;
            return this;
        }

        /// <summary>
        /// Distribution is number of each objects designated for the  pool. For example 50, 50, 20 will instantiate
        /// 50 first objects, 50 of second object and 20 of the third object. Sum of this values will be the total pool size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public PoolCreationDataBuilder<T> SetDistribution(int[] objectDistribution)
        {
            _size = objectDistribution.Sum();
            _objectDistribution = objectDistribution;
            return this;
        }

        /// <summary>
        /// Only used in Flexible pool. This is by how much pool will be resized if there is no more place in the pool
        /// </summary>
        /// <param name="increment"></param>
        /// <returns></returns>
        public PoolCreationDataBuilder<T> SetIncrement(int increment)
        {
            _increment = increment;
            return this;
        }

        /// <summary>
        /// Used when object is created. This enables you to inject required parameters into a class
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public PoolCreationDataBuilder<T> WireInitialize(Action<T> onCreate)
        {
            _onCreate = onCreate;
            return this;
        }

        /// <summary>
        /// Used when object enabled. This enables you to inject required parameters into a class when object is created and whenever it is reenabled
        /// </summary>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public PoolCreationDataBuilder<T> WireEnable(Action<T> onEnable)
        {
            _onEnable = onEnable;
            return this;
        }

        public SPoolCreationData<T> Build()
        {
            return new SPoolCreationData<T>(_name, _size, _parent, _onCreate, _onEnable, _increment, _objectDistribution);
        }
    }
}
