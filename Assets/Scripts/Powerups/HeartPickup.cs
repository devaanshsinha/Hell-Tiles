using UnityEngine;
using HellTiles.Player;

#nullable enable

namespace HellTiles.Powerups
{
    [RequireComponent(typeof(Collider2D))]
    public class HeartPickup : MonoBehaviour
    {
        [SerializeField] private float lifetime = 4f;
        [SerializeField] private float flickerDuration = 1.5f;
        [SerializeField] private SpriteRenderer? spriteRenderer;

        private HeartSpawner? spawner;
        private Vector3Int cell;
        private float timeAlive;
        private float flickerStart;
        private bool isDespawning;

        public void Initialise(HeartSpawner owner, Vector3Int spawnCell)
        {
            spawner = owner;
            cell = spawnCell;
            timeAlive = 0f;
            isDespawning = false;
            flickerStart = Mathf.Max(0f, lifetime - flickerDuration);

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            SetRendererVisible(true);
        }

        private void Update()
        {
            if (isDespawning)
            {
                return;
            }

            timeAlive += Time.deltaTime;
            if (timeAlive >= lifetime)
            {
                Despawn();
                return;
            }

            if (timeAlive >= flickerStart && spriteRenderer != null)
            {
                var remaining = lifetime - timeAlive;
                var t = Mathf.PingPong((flickerDuration - remaining) * 10f, 1f);
                var alpha = Mathf.Lerp(1f, 0.2f, t);
                var color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health == null)
            {
                return;
            }

            health.TryAddHearts(1);
            Despawn();
        }

        private void Despawn()
        {
            if (isDespawning)
            {
                return;
            }

            isDespawning = true;
            if (spawner != null)
            {
                spawner.HandleHeartCollected(cell, this);
            }

            Destroy(gameObject);
        }

        private void SetRendererVisible(bool visible)
        {
            if (spriteRenderer != null)
            {
                var color = spriteRenderer.color;
                color.a = visible ? 1f : 0f;
                spriteRenderer.color = color;
            }
        }
    }
}
