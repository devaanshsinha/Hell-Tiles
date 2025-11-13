using UnityEngine;

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Tracks active projectile count so the director can throttle spawns.
    /// </summary>
    public static class ProjectileRegistry
    {
        public static int ActiveCount { get; private set; }
        public static int MaxActiveProjectiles { get; set; } = 100;

        public static bool CanSpawn => ActiveCount < MaxActiveProjectiles;

        public static void Register(BasicProjectile projectile)
        {
            ActiveCount++;
        }

        public static void Unregister(BasicProjectile projectile)
        {
            ActiveCount = Mathf.Max(0, ActiveCount - 1);
        }
    }
}
