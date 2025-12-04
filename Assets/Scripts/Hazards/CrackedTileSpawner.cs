using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Hazards
{
    public class CrackedTileSpawner : MonoBehaviour
    {
        [SerializeField] private TileGridController gridController = default!; // tilemap provider
        [SerializeField] private CrackedTile crackedTilePrefab = default!;
        [SerializeField, Tooltip("TileBase used to visually mark a cracked tile once it arms.")] private TileBase crackedTileAsset = default!;
        [SerializeField, Tooltip("Seconds between spawn attempts.")] private float spawnInterval = 14f;
        [SerializeField, Tooltip("Seconds to wait before first cracked tile appears.")] private float initialDelay = 6f;
        [SerializeField, Tooltip("Max cracked tiles alive at once.")] private int maxSimultaneous = 2;

        private readonly Dictionary<Vector3Int, CrackedTile> activeTiles = new();
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
            if (crackedTilePrefab == null || crackedTileAsset == null || gridController == null)
            {
                return;
            }

            if (activeTiles.Count >= maxSimultaneous)
            {
                return;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f)
            {
                return;
            }

            TrySpawn();
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

        private void TrySpawn()
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
                if (activeTiles.ContainsKey(cell))
                {
                    continue;
                }

                // Skip empty tiles; must have an original tile to restore.
                if (!gridController.WalkableTilemap.HasTile(cell))
                {
                    continue;
                }

                var worldPos = gridController.CellToWorldCenter(cell);
                var cracked = Instantiate(crackedTilePrefab, worldPos, Quaternion.identity, transform);
                cracked.Initialise(this, gridController, cell, crackedTileAsset);
                activeTiles[cell] = cracked;
                return;
            }
        }

        public void HandleCrackedTileDespawn(Vector3Int cell, CrackedTile cracked)
        {
            if (activeTiles.TryGetValue(cell, out var existing) && existing == cracked)
            {
                activeTiles.Remove(cell);
            }
        }
    }
}
