using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace HellTiles.Tiles
{
    /// <summary>
    /// Central point for querying grid data; keeps tile lookups out of gameplay code.
    /// </summary>
    public class TileGridController : MonoBehaviour
    {
        [SerializeField] private Grid grid = default!;
        [SerializeField] private Tilemap walkableTilemap = default!;
        [SerializeField] private Tilemap? blockedTilemap;
        [SerializeField] private TileBase[]? explicitWalkableTiles;
        [SerializeField] private TileBase[]? explicitBlockedTiles;

        private readonly HashSet<TileBase> walkableTileSet = new();
        private readonly HashSet<TileBase> blockedTileSet = new();
        private readonly Dictionary<Vector3Int, Coroutine> activeTileTweens = new();
        private readonly Dictionary<Vector3Int, TileBase?> originalTiles = new();

        public Tilemap WalkableTilemap => walkableTilemap;

        private void Awake()
        {
            CacheTileSets();
            if (grid == null)
            {
                grid = GetComponentInParent<Grid>();
            }

            CacheOriginalTiles();
        }

        private void OnValidate()
        {
            CacheTileSets();
        }

        private void CacheTileSets()
        {
            walkableTileSet.Clear();
            blockedTileSet.Clear();

            if (explicitWalkableTiles != null)
            {
                foreach (var tile in explicitWalkableTiles)
                {
                    if (tile != null)
                    {
                        walkableTileSet.Add(tile);
                    }
                }
            }

            if (explicitBlockedTiles != null)
            {
                foreach (var tile in explicitBlockedTiles)
                {
                    if (tile != null)
                    {
                        blockedTileSet.Add(tile);
                    }
                }
            }
        }

        private void CacheOriginalTiles()
        {
            originalTiles.Clear();
            foreach (var position in walkableTilemap.cellBounds.allPositionsWithin)
            {
                originalTiles[position] = walkableTilemap.GetTile(position);
            }
        }

        /// <summary>
        /// Reset walkable tilemap back to its original layout captured on Awake.
        /// </summary>
        public void ResetToOriginalLayout()
        {
            if (originalTiles.Count == 0)
            {
                CacheOriginalTiles();
            }

            foreach (var position in walkableTilemap.cellBounds.allPositionsWithin)
            {
                walkableTilemap.SetTile(position, originalTiles.TryGetValue(position, out var t) ? t : null);
                walkableTilemap.SetTransformMatrix(position, Matrix4x4.identity);
            }
        }

        public Vector3 CellToWorldCenter(Vector3Int cellPosition)
        {
            return grid != null
                ? grid.CellToWorld(cellPosition) + grid.cellSize * 0.5f
                : walkableTilemap.CellToWorld(cellPosition) + walkableTilemap.cellSize * 0.5f;
        }

        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return grid != null
                ? grid.WorldToCell(worldPosition)
                : walkableTilemap.WorldToCell(worldPosition);
        }

        public bool TryGetNearestWalkableCell(Vector3 worldPosition, out Vector3Int cellPosition)
        {
            var bounds = walkableTilemap.cellBounds;
            if (bounds.size.x == 0 || bounds.size.y == 0 || bounds.size.z == 0)
            {
                cellPosition = Vector3Int.zero;
                return false;
            }

            var initialCell = walkableTilemap.WorldToCell(worldPosition);
            initialCell.x = Mathf.Clamp(initialCell.x, bounds.xMin, bounds.xMax - 1);
            initialCell.y = Mathf.Clamp(initialCell.y, bounds.yMin, bounds.yMax - 1);
            initialCell.z = Mathf.Clamp(initialCell.z, bounds.zMin, bounds.zMax - 1);

            if (IsWalkable(initialCell))
            {
                cellPosition = initialCell;
                return true;
            }

            // Fallback search: breadth-first within tilemap bounds until we find a walkable tile.
            var visited = new HashSet<Vector3Int> { initialCell };
            var searchQueue = new Queue<Vector3Int>();
            searchQueue.Enqueue(initialCell);

            while (searchQueue.Count > 0)
            {
                var current = searchQueue.Dequeue();
                foreach (var dir in FourDirections)
                {
                    var next = current + dir;
                    if (!bounds.Contains(next) || !visited.Add(next))
                    {
                        continue;
                    }

                    if (IsWalkable(next))
                    {
                        cellPosition = next;
                        return true;
                    }

                    searchQueue.Enqueue(next);
                }
            }

            cellPosition = initialCell;
            return false;
        }

        public bool IsWalkable(Vector3Int cellPosition)
        {
            var tile = walkableTilemap.GetTile(cellPosition);
            if (tile == null)
            {
                return false;
            }

            if (blockedTileSet.Count > 0 && blockedTileSet.Contains(tile))
            {
                return false;
            }

            if (walkableTileSet.Count == 0)
            {
                // When no explicit whitelist exists, any tile present is considered walkable.
                return true;
            }

            return walkableTileSet.Contains(tile);
        }

        public bool IsBlocked(Vector3Int cellPosition)
        {
            if (blockedTilemap != null && blockedTilemap.GetTile(cellPosition) != null)
            {
                return true;
            }

            var tile = walkableTilemap.GetTile(cellPosition);
            if (tile == null)
            {
                return true;
            }

            if (blockedTileSet.Count > 0 && blockedTileSet.Contains(tile))
            {
                return true;
            }

            if (walkableTileSet.Count > 0 && !walkableTileSet.Contains(tile))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<Vector3Int> EnumerateWalkableCells()
        {
            foreach (var position in walkableTilemap.cellBounds.allPositionsWithin)
            {
                if (IsWalkable(position))
                {
                    yield return position;
                }
            }
        }

        public void PlayTileBounce(Vector3Int cell, float depth, float duration)
        {
            // Ignore bounce requests on invalid tiles.
            if (depth <= 0f || duration <= 0f)
            {
                return;
            }

            if (!walkableTilemap.HasTile(cell))
            {
                return;
            }

            if (activeTileTweens.TryGetValue(cell, out var routine))
            {
                StopCoroutine(routine);
            }

            activeTileTweens[cell] = StartCoroutine(TileBounceRoutine(cell, depth, duration));
        }

        private IEnumerator TileBounceRoutine(Vector3Int cell, float depth, float duration)
        {
            // Simple sine wave to fake squash and stretch.
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var normalized = Mathf.Clamp01(elapsed / duration);
                var offset = Mathf.Sin(normalized * Mathf.PI) * -depth;
                var matrix = Matrix4x4.TRS(new Vector3(0f, offset, 0f), Quaternion.identity, Vector3.one);
                walkableTilemap.SetTransformMatrix(cell, matrix);
                yield return null;
            }

            walkableTilemap.SetTransformMatrix(cell, Matrix4x4.identity);
            activeTileTweens.Remove(cell);
        }

        private static readonly Vector3Int[] FourDirections =
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };
    }
}
