using UnityEngine;

public class PoolingInfo : MonoBehaviour
{
    [Range(1, 50)]
    [SerializeField] internal int SpawnAmount = 1;
    [Range(1, 500)]
    [SerializeField] internal int SpawnRate = 1;
    [Range(10, 1000)]
    [SerializeField] internal int Increment = 10;
    [Range(10, 10000)]
    [SerializeField] internal int Size = 50;
    [SerializeField] internal int[] ObjectDistribution = new int[] { 50, 25, 10 };
}
