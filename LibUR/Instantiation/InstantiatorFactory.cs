using UnityEngine;

namespace LibUR.Instantiation
{
    public class InstantiatorFactory
    {
        public readonly Transform ObjectsContainer;

        public InstantiatorFactory()
        {
            ObjectsContainer = new GameObject().transform;
            ObjectsContainer.name = "Objects_Container";
        }

        public GameObject Build(GameObject reference, Vector3 position, params object[] args)
        {
            var created = UnityEngine.GameObject.Instantiate(reference, ObjectsContainer);
            created.transform.position = position;

            if (created.TryGetComponent<IInitializable>(out var component))
                component.Initialize(args);

            return created;
        }
    }
}



