using UnityEngine;

namespace LibUR.Auxiliary
{
    /// <summary>Result of a raycast: whether something was hit and hit details.</summary>
    public struct RayCastInfo
    {
        /// <summary>True if the ray hit an object with one of the expected tags.</summary>
        public bool DidHit;
        /// <summary>World position where the ray hit.</summary>
        public Vector3 Point;
        /// <summary>Transform of the hit object.</summary>
        public Transform Transform;
        /// <summary>Distance from ray origin to the hit point.</summary>
        public float Distance;
    }

    /// <summary>Performs raycasts, optionally filtered by tags and max distance.</summary>
    public class RayCaster
    {
        /// <summary>Default debug draw length when max distance is unlimited (-1).</summary>
        private const float DefaultDebugDrawDistance = 100f;

        private readonly string[] _tags;

        /// <summary>Creates a raycaster that only considers hits on objects with the given tags. Empty or null = any tag.</summary>
        /// <param name="targetTags">Unity tag names to accept; pass none or null for any tag.</param>
        public RayCaster(params string[] targetTags)
        {
            _tags = targetTags;
        }

        /// <summary>Cast a ray and return hit info if something valid was hit within max distance.</summary>
        /// <param name="originPosition">Ray start in world space.</param>
        /// <param name="direction">Ray direction (will be normalized by Unity).</param>
        /// <param name="maxDistance">Max distance to consider; -1 for no limit.</param>
        /// <returns>Hit info; DidHit is false if nothing hit or hit was beyond max distance or wrong tag.</returns>
        public RayCastInfo SimpleRay(Vector3 originPosition, Vector3 direction, int maxDistance = -1)
        {
            var ray = new Ray(originPosition, direction);

            if (!Physics.Raycast(ray, out var hitInfo))
                return new RayCastInfo { DidHit = false };

            var localHitInfo = DidHitAnyOfExpectedTags(hitInfo.transform, _tags);
            
            if (maxDistance != -1 && hitInfo.distance > maxDistance)
                return new RayCastInfo { DidHit = false };

            return new RayCastInfo 
            { 
                DidHit = localHitInfo, 
                Point = hitInfo.point, 
                Transform = hitInfo.transform, 
                Distance = hitInfo.distance 
            };
        }

        /// <summary>Like SimpleRay but draws the ray in the Scene view (red = miss or out of range, green = valid hit).</summary>
        /// <param name="originPosition">Ray start in world space.</param>
        /// <param name="direction">Ray direction.</param>
        /// <param name="maxDistance">Max distance; -1 for no limit.</param>
        /// <returns>Same as SimpleRay.</returns>
        public RayCastInfo SimpleRayDebug(Vector3 originPosition, Vector3 direction, int maxDistance = -1)
        {
            var ray = new Ray(originPosition, direction);
            float debugDrawLength = maxDistance >= 0 ? maxDistance : DefaultDebugDrawDistance;

            if (!Physics.Raycast(ray, out var hitInfo))
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * debugDrawLength, Color.red);
                return new RayCastInfo { DidHit = false };
            }

            var localHitInfo = DidHitAnyOfExpectedTags(hitInfo.transform, _tags);

            if (maxDistance != -1 && hitInfo.distance > maxDistance)
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * debugDrawLength, Color.red);
                return new RayCastInfo { DidHit = false };
            }

            Debug.DrawLine(ray.origin, hitInfo.point, Color.green);

            return new RayCastInfo
            {
                DidHit = localHitInfo,
                Point = hitInfo.point,
                Transform = hitInfo.transform,
                Distance = hitInfo.distance
            };
        }

        /// <summary>Returns true if the hit object has one of the target tags, or if no tags are specified.</summary>
        private bool DidHitAnyOfExpectedTags(Transform hitObject, string[] targetTags)
        {
            if (targetTags == null || targetTags.Length == 0)
                return true;

            foreach (var tag in targetTags)
            {
                if (hitObject.CompareTag(tag))
                    return true;
            }

            return false;
        }
    }
}
