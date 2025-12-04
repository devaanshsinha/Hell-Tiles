using UnityEngine;
using HellTiles.Player;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Row sweep hazard: telegraph with collider off, then enable collider for the active window, then destroy.
    /// Assumes the collider shape comes from the attached Collider2D (e.g., polygon with per-frame physics shapes).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class RowSweepHazard : MonoBehaviour
    {
        [SerializeField, Tooltip("Seconds spent telegraphing before the hitbox turns on.")] private float telegraphTime = 0.5f;
        [SerializeField, Tooltip("Seconds the hitbox remains active before self-destruction.")] private float activeLifetime = 1.5f;
        [SerializeField, Tooltip("Safety timeout in case something goes wrong.")] private float maxLifetime = 3f;
        [SerializeField, Tooltip("Flip the sprite when sweeping right-to-left.")] private bool flipForRightToLeft = true;
        [SerializeField] private SpriteRenderer? spriteRenderer;
        [SerializeField] private Collider2D? hitCollider;

        private float telegraphElapsed;
        private float activeElapsed;
        private float totalElapsed;
        private bool armed;

        public void Initialise(bool fireLeft, Vector3 spawnPos)
        {
            transform.position = spawnPos;
            telegraphElapsed = 0f;
            activeElapsed = 0f;
            totalElapsed = 0f;
            armed = false;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (hitCollider == null)
            {
                hitCollider = GetComponent<Collider2D>();
            }

            if (spriteRenderer != null && flipForRightToLeft)
            {
                // If firing left, keep default; if firing right, flip.
                spriteRenderer.flipX = !fireLeft;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = false; // off during telegraph
            }

        }

        private void Update()
        {
            totalElapsed += Time.deltaTime;
            if (maxLifetime > 0f && totalElapsed >= maxLifetime)
            {
                Cleanup();
                return;
            }

            if (!armed)
            {
                telegraphElapsed += Time.deltaTime;
                if (telegraphElapsed >= telegraphTime)
                {
                    Arm();
                }
                return;
            }

            activeElapsed += Time.deltaTime;
            if (activeElapsed >= activeLifetime)
            {
                Cleanup();
            }
        }

        private void Arm()
        {
            armed = true;
            activeElapsed = 0f;
            if (hitCollider != null)
            {
                hitCollider.enabled = true;

                // If the player is already inside when the collider turns on, apply damage immediately.
                var filter = ContactFilter2D.noFilter;
                filter.useTriggers = true;
                var overlaps = new Collider2D[8];
                var hitCount = hitCollider.Overlap(filter, overlaps);
                for (int i = 0; i < hitCount; i++)
                {
                    var health = overlaps[i].GetComponent<PlayerHealth>() ?? overlaps[i].GetComponentInParent<PlayerHealth>();
                    if (health != null)
                    {
                        health.TakeHit();
                        break; // only hit once on activation
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!armed)
            {
                return;
            }

            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeHit();
            }
        }

        private void Cleanup()
        {
            Destroy(gameObject);
        }
    }
}
