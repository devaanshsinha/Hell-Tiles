using UnityEngine;
using HellTiles.Player;

#nullable enable

namespace HellTiles.Hazards
{
    [RequireComponent(typeof(Collider2D))]
    public class SpikeHazard : MonoBehaviour
    {
        [SerializeField, Tooltip("How long each harmless blink stays visible during telegraphing.")]
        private float telegraphBlinkOn = 0.2f;
        [SerializeField, Tooltip("How long each harmless blink stays invisible during telegraphing.")]
        private float telegraphBlinkOff = 0.2f;
        [SerializeField, Tooltip("How many harmless blinks play before the spike becomes deadly. The final state stays visible and active.")]
        private int harmlessBlinkCount = 2;
        [SerializeField, Tooltip("Seconds the spike stays active after the telegraph blinks finish.")]
        private float activeDuration = 5f;
        [SerializeField] private SpriteRenderer? spriteRenderer;
        [SerializeField] private Collider2D? hitCollider;

        private SpikeSpawner? spawner;
        private Vector3Int cell;
        private float stateTimer;
        private int blinksCompleted;
        private bool isVisible;
        private bool isActive;

        public void Initialise(SpikeSpawner owner, Vector3Int spawnCell)
        {
            spawner = owner;
            cell = spawnCell;
            stateTimer = 0f;
            blinksCompleted = 0;
            isVisible = false;
            isActive = false;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (hitCollider == null)
            {
                hitCollider = GetComponent<Collider2D>();
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = false; // off until telegraph completes
            }

            SetAlpha(0f); // start invisible
        }

        private void Update()
        {
            stateTimer += Time.deltaTime;

            if (!isActive)
            {
                RunTelegraph();
                return;
            }

            if (stateTimer >= activeDuration)
            {
                Despawn();
            }
        }

        private void RunTelegraph()
        {
            float segmentDuration = isVisible ? telegraphBlinkOn : telegraphBlinkOff;
            if (stateTimer < segmentDuration)
            {
                return;
            }

            // Switch visibility state
            stateTimer = 0f;
            isVisible = !isVisible;
            SetAlpha(isVisible ? 1f : 0f);

            if (isVisible)
            {
                blinksCompleted++;
                if (blinksCompleted >= harmlessBlinkCount + 1)
                {
                    // After final on-phase, become active and stay visible
                    Activate();
                }
            }
        }

        private void Activate()
        {
            isActive = true;
            stateTimer = 0f;
            SetAlpha(1f);
            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }
        }

        private void Despawn()
        {
            if (spawner != null)
            {
                spawner.HandleSpikeDespawn(cell, this);
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive)
            {
                return;
            }

            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health == null)
            {
                return;
            }

            health.TakeHit();
            // Spike stays; no self-despawn on hit.
        }

        private void SetAlpha(float value)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            var color = spriteRenderer.color;
            color.a = Mathf.Clamp01(value);
            spriteRenderer.color = color;
        }
    }
}
