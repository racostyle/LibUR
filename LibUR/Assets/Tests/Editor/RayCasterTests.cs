using UnityEngine;
using LibUR.Auxiliary;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class RayCasterTests
    {
        [Test]
        public void SimpleRay_NoHit_ReturnsDidHitFalse()
        {
            var caster = new RayCaster();
            var result = caster.SimpleRay(Vector3.zero, Vector3.forward);
            Assert.That(result.DidHit, Is.False);
        }

        [Test]
        public void SimpleRay_WithMaxDistance_NoHit_ReturnsDidHitFalse()
        {
            var caster = new RayCaster();
            var result = caster.SimpleRay(Vector3.zero, Vector3.forward, maxDistance: 10);
            Assert.That(result.DidHit, Is.False);
        }

        [Test]
        public void Constructor_AcceptsNoTags()
        {
            Assert.DoesNotThrow(() => new RayCaster());
        }

        [Test]
        public void Constructor_AcceptsTags()
        {
            Assert.DoesNotThrow(() => new RayCaster("Player", "Enemy"));
        }
    }
}
