using UnityEngine;

namespace LibUR.Instantiation
{
    public abstract class AInstantiatorFactory : MonoBehaviour
    {
        protected Transform _objectsContainer;

        protected void Init()
        {
            _objectsContainer = new GameObject().transform;
            _objectsContainer.name = "Objects_Container";
        }

        protected GameObject CreateObject(GameObject reference, Vector3 position, params object[] args)
        {
            var created = Instantiate(reference, _objectsContainer);
            created.transform.position = position;

            IInitializable script = created.GetComponent<IInitializable>();
            script?.Initialize(args);

            return created;
        }
    }
}



