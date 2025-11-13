using System.Collections.Generic;
using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Powerups
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private CoinPickup coinPrefab = default!;
        [SerializeField] private float spawnInterval = 12f;
        [SerializeField] private int maxSimultaneousCoins = 2;
        [SerializeField] private float initialDelay = 3f;

        private readonly Dictionary<Vector3Int, CoinPickup> activeCoins = new();
        private readonly List<Vector3Int> walkableCells = new();
        private float timer;

        private void Start()
        {
            timer = initialDelay;
            CacheTiles();
        }

        private void Update()
        {
            if (coinPrefab == null || gridController == null)
            {
                return;
            }

            if (activeCoins.Count >= maxSimultaneousCoins)
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f)
            {
                return;
            }

            SpawnCoin();
            timer = spawnInterval;
        }

        private void CacheTiles()
        {
            walkableCells.Clear();
            foreach (var cell in gridController.EnumerateWalkableCells())
            {
                walkableCells.Add(cell);
            }
        }

        private void SpawnCoin()
        {
            if (walkableCells.Count == 0)
            {
                CacheTiles();
                if (walkableCells.Count == 0)
                {
                    return;
                }
            }

            var attempts = walkableCells.Count;
            while (attempts-- > 0)
            {
                var cell = walkableCells[Random.Range(0, walkableCells.Count)];
                if (activeCoins.ContainsKey(cell))
                {
                    continue;
                }

                var world = gridController.CellToWorldCenter(cell);
                var coin = Instantiate(coinPrefab, world, Quaternion.identity, transform);
                activeCoins[cell] = coin;
                StartCoroutine(RemoveOnDestroy(cell, coin));
                return;
            }
        }

        private System.Collections.IEnumerator RemoveOnDestroy(Vector3Int cell, CoinPickup pickup)
        {
            yield return new WaitUntil(() => pickup == null);
            activeCoins.Remove(cell);
        }
    }
}
