using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using LibUR.Pooling;
using LibUR.Pooling.Auxiliary;
using LibUR.Pooling.Queues;

namespace LibUR.Tests
{
    /// <summary>Simple MonoBehaviour used as pooled type in tests.</summary>
    public class TestPoolable : MonoBehaviour { }

    public class PoolFixedPlayModeTests
    {
        [UnityTest]
        public IEnumerator TryActivateObject_ReturnsTrueAndActivatesObject()
        {
            var parent = new GameObject("PoolParent").transform;
            var prefab = new GameObject("Prefab");
            prefab.AddComponent<TestPoolable>();
            prefab.SetActive(false);

            var data = new PoolCreationDataBuilder<TestPoolable>("Test")
                .SetSize(2)
                .SetParent(parent)
                .Build();
            var queue = new QueueOrdered();
            var pool = new PoolFixed<TestPoolable>(data, queue, prefab);

            bool got = pool.TryActivateObject(Vector3.one, out var obj);
            Assert.That(got, Is.True);
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.gameObject.activeInHierarchy, Is.True);
            Assert.That(obj.transform.position, Is.EqualTo(Vector3.one));

            pool.DestroyAll(true);
            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(parent.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TryActivateObject_ExhaustedPool_ReturnsFalse()
        {
            var parent = new GameObject("PoolParent").transform;
            var prefab = new GameObject("Prefab");
            prefab.AddComponent<TestPoolable>();
            prefab.SetActive(false);

            var data = new PoolCreationDataBuilder<TestPoolable>("Test")
                .SetSize(1)
                .SetParent(parent)
                .Build();
            var queue = new QueueOrdered();
            var pool = new PoolFixed<TestPoolable>(data, queue, prefab);

            pool.TryActivateObject(Vector3.zero, out _);
            bool got = pool.TryActivateObject(Vector3.zero, out var obj2);
            Assert.That(got, Is.False);
            Assert.That(obj2, Is.Null);

            pool.DestroyAll(true);
            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(parent.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Constructor_PrefabWithoutComponent_Throws()
        {
            var parent = new GameObject("PoolParent").transform;
            var prefab = new GameObject("NoComponent"); // no TestPoolable

            var data = new PoolCreationDataBuilder<TestPoolable>("Test")
                .SetSize(1)
                .SetParent(parent)
                .Build();
            var queue = new QueueOrdered();

            Assert.Throws<System.ArgumentException>(() =>
                new PoolFixed<TestPoolable>(data, queue, prefab));

            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(parent.gameObject);
            yield return null;
        }
    }
}
