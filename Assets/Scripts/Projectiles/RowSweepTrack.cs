using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Spawns a row-wide hazard that sweeps left-to-right or right-to-left across the grid row.
    /// </summary>
    public class RowSweepTrack : MonoBehaviour, IProjectileTrack
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private RowSweepHazard sweepPrefab = default!;
        [SerializeField] private float horizontalPadding = 1f; // spawn just outside the grid
        [Header("Spawn Columns")]
        [SerializeField] private bool useCustomColumns = false;
        [SerializeField] private int leftColumn = -5;  // cell column to use when spawning from the left
        [SerializeField] private int rightColumn = 5;  // cell column to use when spawning from the right

        public void SpawnProjectile()
        {
            if (gridController == null || sweepPrefab == null)
            {
                Debug.LogWarning($"{nameof(RowSweepTrack)} missing references.", this);
                return;
            }

            var bounds = gridController.WalkableTilemap.cellBounds;
            if (bounds.size.y <= 0)
            {
                return;
            }

            // Pick a random row within tilemap bounds.
            var y = Random.Range(bounds.yMin, bounds.yMax);
            var startFromLeft = Random.value > 0.5f;

            // Determine spawn column: either just outside bounds or a custom column you set.
            var spawnX = startFromLeft
                ? (useCustomColumns ? leftColumn : bounds.xMin - 1)
                : (useCustomColumns ? rightColumn : bounds.xMax);

            var spawnCell = new Vector3Int(spawnX, y, 0);
            var spawnPos = gridController.CellToWorldCenter(spawnCell) + (startFromLeft ? Vector3.left : Vector3.right) * horizontalPadding;

            var hazard = Instantiate(sweepPrefab, spawnPos, Quaternion.identity);
            hazard.Initialise(!startFromLeft); // flip when going right->left
        }
    }
}
