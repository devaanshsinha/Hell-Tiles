using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using HellTiles.Player;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Hazards
{
    [RequireComponent(typeof(Collider2D))]
    public class CrackedTile : MonoBehaviour
    {
        [SerializeField, Tooltip("How long each harmless blink stays visible during telegraphing.")]
        private float telegraphBlinkOn = 0.2f;
        [SerializeField, Tooltip("How long each harmless blink stays invisible during telegraphing.")]
        private float telegraphBlinkOff = 0.2f;
        [SerializeField, Tooltip("How many visible blinks before the tile arms. Final state stays visible and armed.")]
        private int telegraphBlinkCount = 3;
        [SerializeField, Tooltip("Seconds after breaking before the original tile is restored.")]
        private float restoreDelay = 10f;
        [SerializeField, Tooltip("Optional visual for telegraphing.")] private SpriteRenderer? telegraphRenderer;
        [SerializeField] private Collider2D? hitCollider;

        private CrackedTileSpawner? spawner;
        private TileGridController? gridController;
        private TileBase? crackedTileAsset;
        private TileBase? originalTile;
        private Vector3Int cell;
        private float stateTimer;
        private int blinksCompleted;
        private bool isVisible;
        private bool isArmed;
        private bool isBroken;
        private bool playerOnTile;

        public void Initialise(CrackedTileSpawner owner, TileGridController grid, Vector3Int spawnCell, TileBase crackedTile)
        {
            spawner = owner;
            gridController = grid;
            cell = spawnCell;
            crackedTileAsset = crackedTile;

            if (telegraphRenderer == null)
            {
                telegraphRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (hitCollider == null)
            {
                hitCollider = GetComponent<Collider2D>();
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = false; // off until armed
            }

            stateTimer = 0f;
            blinksCompleted = 0;
            isVisible = false;
            isArmed = false;
            isBroken = false;
            playerOnTile = false;
            SetTelegraphVisible(false);

            if (gridController != null)
            {
                originalTile = gridController.WalkableTilemap.GetTile(cell);
            }
        }

        private void Update()
        {
            if (isBroken)
            {
                return;
            }

            if (!isArmed)
            {
                RunTelegraph();
                return;
            }
        }

        private void RunTelegraph()
        {
            float segmentDuration = isVisible ? telegraphBlinkOn : telegraphBlinkOff;
            if (stateTimer < segmentDuration)
            {
                stateTimer += Time.deltaTime;
                return;
            }

            // Switch visibility state.
            stateTimer = 0f;
            isVisible = !isVisible;
            SetTelegraphVisible(isVisible);

            if (isVisible)
            {
                blinksCompleted++;
                if (blinksCompleted >= telegraphBlinkCount)
                {
                    ArmTile();
                }
            }
        }

        private void ArmTile()
        {
            isArmed = true;
            stateTimer = 0f;
            SetTelegraphVisible(true);

            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }

            if (gridController != null && crackedTileAsset != null)
            {
                gridController.WalkableTilemap.SetTile(cell, crackedTileAsset);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isArmed || isBroken)
            {
                return;
            }

            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health == null)
            {
                return;
            }

            playerOnTile = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isArmed || isBroken || !playerOnTile)
            {
                return;
            }

            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health == null)
            {
                return;
            }

            playerOnTile = false;
            BreakTile(); // breaks once the player leaves after first entry
        }

        private void BreakTile()
        {
            isBroken = true;
            if (hitCollider != null)
            {
                hitCollider.enabled = false;
            }

            SetTelegraphVisible(false);

            if (gridController != null)
            {
                gridController.WalkableTilemap.SetTile(cell, null);
            }

            StartCoroutine(RestoreAfterDelay());
        }

        private IEnumerator RestoreAfterDelay()
        {
            yield return new WaitForSeconds(restoreDelay);

            if (gridController != null && originalTile != null)
            {
                gridController.WalkableTilemap.SetTile(cell, originalTile);
            }

            spawner?.HandleCrackedTileDespawn(cell, this);
            Destroy(gameObject);
        }

        private void SetTelegraphVisible(bool visible)
        {
            if (telegraphRenderer == null)
            {
                return;
            }

            var color = telegraphRenderer.color;
            color.a = visible ? 1f : 0f;
            telegraphRenderer.color = color;
        }
    }
}
