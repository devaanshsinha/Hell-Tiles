using System.Collections.Generic;
using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Powerups
{
    public class AngelSpawner : MonoBehaviour
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private AngelPickup angelPrefab = default!;
        [SerializeField, Tooltip("Minimum seconds between spawns.")] private float spawnIntervalMin = 15f;
        [SerializeField, Tooltip("Maximum seconds between spawns.")] private float spawnIntervalMax = 25f;
        [SerializeField, Tooltip("Seconds before an uncollected angel despawns.")] private float despawnAfter = 5f;

        private readonly List<Vector3Int> walkableCells = new();
        private AngelPickup? activeAngel;
        private float timer;

        private void Awake()
        {
            if (gridController == null)
            {
                gridController = FindFirstObjectByType<TileGridController>();
            }

            timer = Random.Range(spawnIntervalMin, spawnIntervalMax);
        }

        private void Start()
        {
            CacheWalkableCells();
        }

        private void Update()
        {
            if (angelPrefab == null || gridController == null)
            {
                return;
            }

            if (activeAngel != null)
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f)
            {
                return;
            }

            SpawnAngel();
        }

        private void CacheWalkableCells()
        {
            walkableCells.Clear();
            foreach (var cell in gridController.EnumerateWalkableCells())
            {
                walkableCells.Add(cell);
            }
        }

        private void SpawnAngel()
        {
            if (walkableCells.Count == 0)
            {
                CacheWalkableCells();
                if (walkableCells.Count == 0)
                {
                    return;
                }
            }

            var attempts = walkableCells.Count;
            while (attempts-- > 0)
            {
                var cell = walkableCells[Random.Range(0, walkableCells.Count)];
                if (!gridController.WalkableTilemap.HasTile(cell))
                {
                    continue;
                }

                var pos = gridController.CellToWorldCenter(cell);
                activeAngel = Instantiate(angelPrefab, pos, Quaternion.identity, transform);
                activeAngel.Initialise(this, gridController);
                StartCoroutine(DespawnAfterDelay(activeAngel, despawnAfter));
                return;
            }
        }

        private System.Collections.IEnumerator DespawnAfterDelay(AngelPickup angel, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (activeAngel != null && activeAngel == angel)
            {
                Destroy(activeAngel.gameObject);
                activeAngel = null;
                timer = Random.Range(spawnIntervalMin, spawnIntervalMax);
            }
        }

        public void HandleCollected(AngelPickup pickup)
        {
            if (activeAngel == pickup)
            {
                activeAngel = null;
                timer = Random.Range(spawnIntervalMin, spawnIntervalMax);
            }
        }
    }
}
