using LibUR.Instantiation;
using UnityEngine;

namespace TestScenesAssets.InstantiatorTesting
{
    public class Instantiator : MonoBehaviour
    {
        [SerializeField] GameObject Reference1;
        [SerializeField] GameObject Reference2;

        private static Instantiator _instance;

        private InstantiatorFactory _factory;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            _factory = new InstantiatorFactory();

            _factory.Build(Reference1, GetRandomPos(), new InfoToPass("I am 1"));
            _factory.Build(Reference2, GetRandomPos(), new InfoToPass("I am 2"));
        }

        private Vector3 GetRandomPos()
        {
            return new Vector3(Random.value * 5 - 5, .5f, Random.value * 5 - 5);
        }
    }
}
