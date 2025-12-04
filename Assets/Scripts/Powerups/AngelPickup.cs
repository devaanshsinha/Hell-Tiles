using System.Collections;
using UnityEngine;
using HellTiles.Player;
using HellTiles.Projectiles;
using HellTiles.Tiles;
using HellTiles.Hazards;

#nullable enable

namespace HellTiles.Powerups
{
    [RequireComponent(typeof(Collider2D))]
    public class AngelPickup : MonoBehaviour
    {
        [SerializeField] private GameObject? ringVfxPrefab;
        [SerializeField, Tooltip("Seconds before the ring VFX self-destroys.")] private float ringLifetime = 0.42f;

        private AngelSpawner? spawner;
        private TileGridController? gridController;

        public void Initialise(AngelSpawner owner, TileGridController grid)
        {
            spawner = owner;
            gridController = grid;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health == null)
            {
                return;
            }

            ActivateEffects(health);
        }

        private void ActivateEffects(PlayerHealth health)
        {
            // Reset health
            health.RestoreFullHealth();

            // Clear projectiles
            foreach (var proj in FindObjectsByType<BasicProjectile>(FindObjectsSortMode.None))
            {
                Destroy(proj.gameObject);
            }

            // Clear hazards
            foreach (var spike in FindObjectsByType<SpikeHazard>(FindObjectsSortMode.None))
            {
                Destroy(spike.gameObject);
            }

            foreach (var push in FindObjectsByType<PushTileHazard>(FindObjectsSortMode.None))
            {
                Destroy(push.gameObject);
            }

            foreach (var crack in FindObjectsByType<CrackedTile>(FindObjectsSortMode.None))
            {
                Destroy(crack.gameObject);
            }

            foreach (var sweep in FindObjectsByType<RowSweepHazard>(FindObjectsSortMode.None))
            {
                Destroy(sweep.gameObject);
            }

            // Ask spawners to clear internal state if present.
            foreach (var spikeSpawner in FindObjectsByType<SpikeSpawner>(FindObjectsSortMode.None))
            {
                spikeSpawner.ClearAll();
            }

            foreach (var pushSpawner in FindObjectsByType<PushTileSpawner>(FindObjectsSortMode.None))
            {
                pushSpawner.ClearAll();
            }

            foreach (var crackedSpawner in FindObjectsByType<CrackedTileSpawner>(FindObjectsSortMode.None))
            {
                crackedSpawner.ClearAll();
            }

            foreach (var sweepTrack in FindObjectsByType<RowSweepTrack>(FindObjectsSortMode.None))
            {
                sweepTrack.ClearAll();
            }

            // Reset tiles to original layout.
            gridController?.ResetToOriginalLayout();

            // Play VFX at this location
            if (ringVfxPrefab != null)
            {
                var vfx = Instantiate(ringVfxPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, ringLifetime); // ensure VFX cleans up even after this pickup is destroyed
            }

            // Notify spawner and destroy self.
            spawner?.HandleCollected(this);
            Destroy(gameObject);
        }
    }
}
