using UnityEngine;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Fires an arrow from outside the board through a random tile (x,y in [-2..2]) and continues straight.
    /// </summary>
    public class ArrowTileTrack : MonoBehaviour, IProjectileTrack
    {
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private DirectionalProjectile arrowPrefab = default!;
        [SerializeField, Tooltip("Spawn distance from the target tile in world units.")] private float spawnRadius = 10f;
        [SerializeField, Tooltip("Inclusive min/max tile coords to target (x and y).")] private int minCoord = -2;
        [SerializeField, Tooltip("Inclusive min/max tile coords to target (x and y).")] private int maxCoord = 2;

        public void SpawnProjectile()
        {
            if (gridController == null || arrowPrefab == null)
            {
                Debug.LogWarning($"{nameof(ArrowTileTrack)} missing references.", this);
                return;
            }

            // Pick a random tile in the specified range.
            var cell = new Vector3Int(Random.Range(minCoord, maxCoord + 1), Random.Range(minCoord, maxCoord + 1), 0);
            var targetPos = gridController.CellToWorldCenter(cell);

            // Pick a random direction to approach from and place spawn at spawnRadius away.
            var dir = Random.insideUnitCircle.normalized;
            if (dir == Vector2.zero)
            {
                dir = Vector2.right;
            }

            var spawnPos = targetPos + (Vector3)(dir * spawnRadius);

            var proj = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
            proj.Initialise(targetPos);
        }
    }
}
