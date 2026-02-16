using UnityEngine;
using LibUR.Pooling.Auxiliary;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class PoolCreationDataBuilderTests
    {
        [Test]
        public void Build_ReturnsDataWithSetValues()
        {
            var parent = new GameObject("Parent").transform;
            var data = new PoolCreationDataBuilder<MonoBehaviour>("TestPool")
                .SetSize(10)
                .SetParent(parent)
                .SetIncrement(5)
                .Build();
            Assert.That(data.PoolName, Is.EqualTo("TestPool"));
            Assert.That(data.Size, Is.EqualTo(10));
            Assert.That(data.Increment, Is.EqualTo(5));
            Assert.That(data.ParentContainer, Is.SameAs(parent));
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void SetDistribution_Manual_SetsSizeToSum()
        {
            var parent = new GameObject("Parent").transform;
            var data = new PoolCreationDataBuilder<MonoBehaviour>("P")
                .SetParent(parent)
                .SetDistribution_Manual(2, 3, 5)
                .Build();
            Assert.That(data.Size, Is.EqualTo(10));
            Assert.That(data.ObjectDistribution, Is.EqualTo(new[] { 2, 3, 5 }));
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void SetDistribution_Fixed_FillsArray()
        {
            var parent = new GameObject("Parent").transform;
            var data = new PoolCreationDataBuilder<MonoBehaviour>("P")
                .SetParent(parent)
                .SetDistribution_Fixed(objectsCount: 4, value: 3)
                .Build();
            Assert.That(data.ObjectDistribution, Is.EqualTo(new[] { 3, 3, 3, 3 }));
            Assert.That(data.Size, Is.EqualTo(12));
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void SetDistribution_AutoCalculate_ObjectsCountLessThan3_Throws()
        {
            var parent = new GameObject("Parent").transform;
            var b = new PoolCreationDataBuilder<MonoBehaviour>("P").SetParent(parent);
            Assert.Throws<System.ArgumentException>(() => b.SetDistribution_AutoCalculate(100, 2));
            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
