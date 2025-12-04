using System.Collections.Generic;
using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Hazards
{
    public class SpikeSpawner : MonoBehaviour
    {
        [SerializeField] private TileGridController gridController = default!; // tilemap provider
        [SerializeField] private SpikeHazard spikePrefab = default!;
        [SerializeField, Tooltip("Seconds between spawn attempts.")] private float spawnInterval = 12f;
        [SerializeField, Tooltip("Seconds to wait before the first spike can appear.")] private float initialDelay = 4f;
        [SerializeField, Tooltip("Max spike instances alive at once.")] private int maxSimultaneousSpikes = 2;

        private readonly Dictionary<Vector3Int, SpikeHazard> activeSpikes = new();
        private readonly List<Vector3Int> walkableCells = new();
        private float spawnTimer;

        public float SpawnInterval
        {
            get => spawnInterval;
            set => spawnInterval = Mathf.Max(0.01f, value);
        }

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
            if (spikePrefab == null || gridController == null)
            {
                return;
            }

            if (activeSpikes.Count >= maxSimultaneousSpikes)
            {
                return;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f)
            {
                return;
            }

            TrySpawnSpike();
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

        private void TrySpawnSpike()
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
                if (activeSpikes.ContainsKey(cell))
                {
                    continue;
                }

                var worldPos = gridController.CellToWorldCenter(cell);
                var spike = Instantiate(spikePrefab, worldPos, Quaternion.identity, transform);
                spike.Initialise(this, cell);
                activeSpikes[cell] = spike;
                return;
            }
        }

        public void HandleSpikeDespawn(Vector3Int cell, SpikeHazard spike)
        {
            if (activeSpikes.TryGetValue(cell, out var existing) && existing == spike)
            {
                activeSpikes.Remove(cell);
            }
        }

        public void ClearAll()
        {
            foreach (var kvp in activeSpikes)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
            }
            activeSpikes.Clear();
        }
    }
}
