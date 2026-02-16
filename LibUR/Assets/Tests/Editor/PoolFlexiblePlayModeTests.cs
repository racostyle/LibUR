using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using LibUR.Pooling;
using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;

namespace LibUR.Tests
{
    public class PoolFlexiblePlayModeTests
    {
        [UnityTest]
        public IEnumerator TryActivateObject_WhenExhausted_ResizesAndReturnsTrue()
        {
            var parent = new GameObject("PoolParent").transform;
            var prefab = new GameObject("Prefab");
            prefab.AddComponent<TestPoolable>();
            prefab.SetActive(false);

            var data = new PoolCreationDataBuilder<TestPoolable>("Test")
                .SetSize(1)
                .SetIncrement(2)
                .SetParent(parent)
                .Build();
            var queue = new QueueOrdered();
            var pool = new PoolFlexible<TestPoolable>(data, queue, prefab);

            Assert.That(pool.TryActivateObject(Vector3.zero, out var first), Is.True);
            Assert.That(first, Is.Not.Null);
            Assert.That(pool.TryActivateObject(Vector3.zero, out var second), Is.True);
            Assert.That(second, Is.Not.Null);
            Assert.That(pool.GetPool().Length, Is.GreaterThanOrEqualTo(2));

            pool.DestroyAll(true);
            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(parent.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Constructor_PrefabWithoutComponent_Throws()
        {
            var parent = new GameObject("PoolParent").transform;
            var prefab = new GameObject("NoComponent");

            var data = new PoolCreationDataBuilder<TestPoolable>("Test")
                .SetSize(1)
                .SetIncrement(1)
                .SetParent(parent)
                .Build();
            var queue = new QueueOrdered();

            Assert.Throws<System.ArgumentException>(() =>
                new PoolFlexible<TestPoolable>(data, queue, prefab));

            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(parent.gameObject);
            yield return null;
        }
    }
}
