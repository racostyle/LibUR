using LibUR.Pooling;
using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

public class Generator_Fixed_MO : MonoBehaviour
{
    [SerializeField] GameObject[] ObjectsRef;
    int counter = 0;
    SPoolCreationData<SphereGM> creationData;
    PoolingInfo _poolingInfo;
    IPool<SphereGM> _pool;

    // Start is called before the first frame update
    void Start()
    {
        _poolingInfo = GetComponent<PoolingInfo>();

        creationData = new PoolCreationDataBuilder<SphereGM>("Spheres")
            .SetDistribution(_poolingInfo.ObjectDistribution)
            .WireInitialize((SphereGM sphere, int index) => sphere.InitOnCreate("this is injected on creation"))
            .WireEnable((SphereGM sphere) => sphere.ReInit("this is injected whenever object is enabled"))
            .Build();

        _pool = new PoolFixed_MultipleObjects<SphereGM>(in creationData, new QueueRandomized(), ObjectsRef);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (counter > _poolingInfo.SpawnRate)
        {
            counter = 0;
            for (int i = 0; i < _poolingInfo.SpawnAmount; i++)
                _pool.TryActivateObject(new Vector3(Random.value * 10f - 5, 15, Random.value * 10f - 5), out var obj);

            Debug.Log($"Pool Size: {_pool.GetPool().Length}");
        }
        counter++;
    }

    private void OnDestroy()
    {
        _pool?.DestroyAll();
    }

}
