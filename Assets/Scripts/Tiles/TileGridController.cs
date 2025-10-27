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

        private void Awake()
        {
            CacheTileSets();
            if (grid == null)
            {
                grid = GetComponentInParent<Grid>();
            }
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

        public Vector3 CellToWorldCenter(Vector3Int cellPosition)
        {
            var world = walkableTilemap.GetCellCenterWorld(cellPosition);
            return world;
        }

        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return walkableTilemap.WorldToCell(worldPosition);
        }

        public bool TryGetNearestWalkableCell(Vector3 worldPosition, out Vector3Int cellPosition)
        {
            cellPosition = walkableTilemap.WorldToCell(worldPosition);
            if (IsWalkable(cellPosition))
            {
                return true;
            }

            // Fallback search: breadth-first until we find a walkable tile.
            var visited = new HashSet<Vector3Int> { cellPosition };
            var searchQueue = new Queue<Vector3Int>();
            searchQueue.Enqueue(cellPosition);

            while (searchQueue.Count > 0)
            {
                var current = searchQueue.Dequeue();
                foreach (var dir in FourDirections)
                {
                    var next = current + dir;
                    if (!visited.Add(next))
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

        private static readonly Vector3Int[] FourDirections =
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };
    }
}
