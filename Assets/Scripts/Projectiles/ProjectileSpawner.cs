using System.Collections;
using UnityEngine;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Spawns projectiles around a defined rectangular area and sends them towards the player.
    /// </summary>
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private BasicProjectile projectilePrefab = default!;
        [SerializeField] private Transform playerTransform = default!;
        [SerializeField] private Vector2 spawnAreaSize = new(12f, 8f);
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private bool useCircleSpawn;
        [SerializeField] private float spawnRadius = 8f;
        [SerializeField] private bool spawnFromPerimeter = true;
        [SerializeField] private bool trackPlayerContinuously;
        [SerializeField] private float initialDelay = 0.5f;

        private Coroutine? spawnRoutine;

        private void OnEnable()
        {
            if (projectilePrefab == null || playerTransform == null)
            {
                Debug.LogWarning($"{nameof(ProjectileSpawner)} missing references; spawner disabled.", this);
                enabled = false;
                return;
            }

            spawnRoutine = StartCoroutine(SpawnLoop());
        }

        private void OnDisable()
        {
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            if (initialDelay > 0f)
            {
                yield return new WaitForSeconds(initialDelay);
            }

            var wait = new WaitForSeconds(spawnInterval);
            while (true)
            {
                SpawnProjectile();
                yield return wait;
            }
        }

        private void SpawnProjectile()
        {
            var spawnPoint = SelectSpawnPoint();
            var projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity);
            if (trackPlayerContinuously)
            {
                projectile.Initialise(playerTransform);
            }
            else
            {
                projectile.Initialise(playerTransform.position);
            }
        }

        private Vector3 SelectSpawnPoint()
        {
            if (useCircleSpawn)
            {
                var angle = Random.Range(0f, Mathf.PI * 2f);
                var radius = spawnRadius;
                var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                return playerTransform.position + (Vector3)offset;
            }

            var half = spawnAreaSize * 0.5f;

            if (spawnFromPerimeter)
            {
                var side = Random.Range(0, 4);
                float x = 0f, y = 0f;
                switch (side)
                {
                    case 0: // top
                        x = Random.Range(-half.x, half.x);
                        y = half.y;
                        break;
                    case 1: // bottom
                        x = Random.Range(-half.x, half.x);
                        y = -half.y;
                        break;
                    case 2: // left
                        x = -half.x;
                        y = Random.Range(-half.y, half.y);
                        break;
                    default: // right
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
