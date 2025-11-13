using System.Collections.Generic;
using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Launches arrows between random tiles.
    /// </summary>
    public class ArrowProjectileTrack : MonoBehaviour, IProjectileTrack
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private BasicProjectile arrowPrefab = default!;
        [SerializeField] private bool cacheTilesOnStart = true;

        private readonly List<Vector3Int> walkableCells = new();
        private bool tilesCached;

        private void Start()
        {
            if (cacheTilesOnStart)
            {
                CacheWalkableCells();
            }
        }

        public void SpawnProjectile()
        {
            if (gridController == null || arrowPrefab == null)
            {
                Debug.LogWarning($"{nameof(ArrowProjectileTrack)} missing references.", this);
                return;
            }

            if (!tilesCached)
            {
                CacheWalkableCells();
            }

            if (walkableCells.Count < 2)
            {
                return;
            }

            var startCell = walkableCells[Random.Range(0, walkableCells.Count)];
            var endCell = walkableCells[Random.Range(0, walkableCells.Count)];
            if (endCell == startCell)
            {
                endCell += Vector3Int.right;
            }

            var startPos = gridController.CellToWorldCenter(startCell);
            var endPos = gridController.CellToWorldCenter(endCell);
            var direction = (endPos - startPos).normalized;

            var projectile = Instantiate(arrowPrefab, startPos, Quaternion.identity);
            if (direction == Vector3.zero)
            {
                direction = Vector3.right;
            }

            projectile.Initialise(startPos + direction);
        }

        private void CacheWalkableCells()
        {
            walkableCells.Clear();
            foreach (var cell in gridController.EnumerateWalkableCells())
            {
                walkableCells.Add(cell);
            }

            tilesCached = true;
        }
    }
}
