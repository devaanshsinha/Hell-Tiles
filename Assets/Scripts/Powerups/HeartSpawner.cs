using System.Collections.Generic;
using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Powerups
{
    public class HeartSpawner : MonoBehaviour
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private HeartPickup heartPrefab = default!;
        [SerializeField] private float spawnInterval = 15f;
        [SerializeField] private int maxSimultaneousHearts = 1;
        [SerializeField] private float initialDelay = 5f;

        private readonly Dictionary<Vector3Int, HeartPickup> activeHearts = new();
        private readonly List<Vector3Int> walkableCells = new();
        private float spawnTimer;

        private void Awake()
        {
            if (gridController == null)
            {
                gridController = FindFirstObjectByType<TileGridController>();
            }

            spawnTimer = Mathf.Max(0f, initialDelay);
        }

        private void Start()
        {
            CacheWalkableCells();
        }

        private void Update()
        {
            if (heartPrefab == null || gridController == null)
            {
                return;
            }

            if (activeHearts.Count >= maxSimultaneousHearts)
            {
                return;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f)
            {
                return;
            }

            SpawnHeart();
            spawnTimer = Mathf.Max(0.01f, spawnInterval);
        }

        private void CacheWalkableCells()
        {
            walkableCells.Clear();
            foreach (var cell in gridController.EnumerateWalkableCells())
            {
                walkableCells.Add(cell);
            }
        }

        private void SpawnHeart()
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
                if (activeHearts.ContainsKey(cell))
                {
                    continue;
                }

                var worldPosition = gridController.CellToWorldCenter(cell);
                var heart = Instantiate(heartPrefab, worldPosition, Quaternion.identity, transform);
                heart.Initialise(this, cell);
                activeHearts[cell] = heart;
                return;
            }
        }

        public void HandleHeartCollected(Vector3Int cell, HeartPickup pickup)
        {
            if (activeHearts.TryGetValue(cell, out var existing) && existing == pickup)
            {
                activeHearts.Remove(cell);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (gridController == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            foreach (var kvp in activeHearts)
            {
                var pos = gridController.CellToWorldCenter(kvp.Key);
                Gizmos.DrawWireSphere(pos, 0.2f);
            }
        }
    }
}
