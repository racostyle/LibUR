using UnityEngine;

namespace LibUR.Pooling.Auxiliary
{
    public class PooledObject<T> 
    {
        public readonly T Script;
        public readonly GameObject GameObject;
        public readonly Transform Transform;
        public readonly Behaviour Behaviour;

        public PooledObject(T script, GameObject gameObject)
        {
            Script = script;
            GameObject = gameObject;
            Transform = gameObject.transform;
            Behaviour = gameObject.GetComponent<Behaviour>();
        }
    }
}
