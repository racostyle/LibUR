using LibUR.Pooling;
using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;
using UnityEngine;

public class Generator_Flexible : MonoBehaviour
{
    [SerializeField] GameObject ObjectRef;
    private int counter = 0;
    private PoolingInfo _poolingInfo;
    private IPool<SphereGM> _pool;

    // Start is called before the first frame update
    void Start()
    {
        _poolingInfo = GetComponent<PoolingInfo>();
        var creationData = new PoolCreationDataBuilder<SphereGM>("Spheres")
            .SetParent(transform)
            .SetSize(_poolingInfo.Size)
            .SetIncrement(_poolingInfo.Increment)
            .WireOnInitialize((SphereGM sphere, int index) => sphere.InitOnCreate("this is injected on creation"))
            .WireOnEnable((SphereGM sphere) => sphere.ReInit("this is injected whenever object is enabled"))
            .Build();

        _pool = new PoolFlexible<SphereGM>(in creationData, new QueueOrdered(), ObjectRef);
    }

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
