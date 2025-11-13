using UnityEngine;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Spawns homing projectiles from around the arena.
    /// </summary>
    public class HomingProjectileTrack : MonoBehaviour, IProjectileTrack
    {
        [SerializeField] private BasicProjectile projectilePrefab = default!;
        [SerializeField] private Transform playerTransform = default!;
        [SerializeField] private Vector2 spawnAreaSize = new(12f, 8f);
        [SerializeField] private bool useCircleSpawn = true;
        [SerializeField] private float circleRadius = 10f;
        [SerializeField] private bool spawnFromPerimeter = true;

        public void SpawnProjectile()
        {
            if (projectilePrefab == null || playerTransform == null)
            {
                Debug.LogWarning($"{nameof(HomingProjectileTrack)} missing references.", this);
                return;
            }

            var spawnPoint = SelectSpawnPoint();
            var projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity);
            projectile.Initialise(playerTransform);
        }

        private Vector3 SelectSpawnPoint()
        {
            if (useCircleSpawn)
            {
                var angle = Random.Range(0f, Mathf.PI * 2f);
                var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circleRadius;
                return playerTransform.position + (Vector3)offset;
            }

            var half = spawnAreaSize * 0.5f;
            if (spawnFromPerimeter)
            {
                var side = Random.Range(0, 4);
                float x = 0f, y = 0f;
                switch (side)
                {
                    case 0:
                        x = Random.Range(-half.x, half.x);
                        y = half.y;
                        break;
                    case 1:
                        x = Random.Range(-half.x, half.x);
                        y = -half.y;
                        break;
                    case 2:
                        x = -half.x;
                        y = Random.Range(-half.y, half.y);
                        break;
                    default:
                        x = half.x;
                        y = Random.Range(-half.y, half.y);
                        break;
                }

                return transform.TransformPoint(new Vector2(x, y));
            }

            var randomX = Random.Range(-half.x, half.x);
            var randomY = Random.Range(-half.y, half.y);
            return transform.TransformPoint(new Vector2(randomX, randomY));
        }
    }
}
