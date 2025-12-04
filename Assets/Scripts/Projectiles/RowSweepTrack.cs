using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Spawns a row sweep hazard: telegraph then active hitbox, anchored at the chosen row.
    /// </summary>
    public class RowSweepTrack : MonoBehaviour, IProjectileTrack
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private RowSweepHazard sweepPrefab = default!;
        [SerializeField, Tooltip("Explicit row indices to spawn on. If empty, uses the tilemap center row.")] private int[] allowedRows = System.Array.Empty<int>();
        [SerializeField, Tooltip("Explicit column indices to spawn on. If empty, uses the tilemap center column.")] private int[] allowedColumns = System.Array.Empty<int>();

        // For Angel cleanup
        private readonly System.Collections.Generic.List<RowSweepHazard> activeSweeps = new();

        public void SpawnProjectile()
        {
            if (gridController == null || sweepPrefab == null)
            {
                Debug.LogWarning($"{nameof(RowSweepTrack)} missing references.", this);
                return;
            }

            var tilemap = gridController.WalkableTilemap;
            var bounds = tilemap.cellBounds;
            if (bounds.size.y <= 0)
            {
                return;
            }

            int row;
            if (allowedRows == null || allowedRows.Length == 0)
            {
                row = Mathf.RoundToInt(bounds.center.y);
            }
            else
            {
                row = allowedRows[Random.Range(0, allowedRows.Length)];
            }

            row = Mathf.Clamp(row, bounds.yMin, bounds.yMax);

            int col;
            if (allowedColumns == null || allowedColumns.Length == 0)
            {
                col = Mathf.RoundToInt(bounds.center.x);
            }
            else
            {
                col = allowedColumns[Random.Range(0, allowedColumns.Length)];
            }

            col = Mathf.Clamp(col, bounds.xMin, bounds.xMax);

            var cell = new Vector3Int(col, row, 0);
            var spawnPos = gridController.CellToWorldCenter(cell);

            // Flip when spawning on negative X (fire to the right); keep default when X >= 0.
            var fireLeft = col >= 0;

            var hazard = Instantiate(sweepPrefab, spawnPos, Quaternion.identity);
            hazard.Initialise(fireLeft, spawnPos);
            activeSweeps.Add(hazard);
        }

        public void ClearAll()
        {
            for (int i = activeSweeps.Count - 1; i >= 0; i--)
            {
                if (activeSweeps[i] != null)
                {
                    Destroy(activeSweeps[i].gameObject);
                }
            }
            activeSweeps.Clear();
        }
    }
}
