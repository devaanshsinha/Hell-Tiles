using UnityEngine;
using HellTiles.Player;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Hazard that spans a row; damages the player on contact and self-destructs after a short lifetime.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class RowSweepHazard : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 2f;
        [SerializeField] private bool flipXForRightToLeft = true;
        [SerializeField] private SpriteRenderer? spriteRenderer;

        private float elapsed;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void OnEnable()
        {
            elapsed = 0f;
            // Redundant safety so it cleans up even if Update is disabled.
            Invoke(nameof(Cleanup), lifeTime);
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= lifeTime)
            {
                Cleanup();
            }
        }

        public void Initialise(bool rightToLeft)
        {
            if (spriteRenderer != null && flipXForRightToLeft)
            {
                spriteRenderer.flipX = rightToLeft;
            }
        }

        public void SetLifetime(float seconds)
        {
            lifeTime = Mathf.Max(0.01f, seconds);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeHit();
            }
        }

        private void Cleanup()
        {
            CancelInvoke();
            Destroy(gameObject);
        }
    }
}
