using System.Collections;
using UnityEngine;
using HellTiles.Player;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Hazards
{
    [RequireComponent(typeof(Collider2D))]
    public class PushTileHazard : MonoBehaviour, IPushPadConfigurable
    {
        public enum PushDirection { Up, Down, Left, Right }

        [SerializeField, Tooltip("Direction this pad pushes the player.")] private PushDirection direction;
        [SerializeField, Tooltip("Seconds each harmless blink stays visible.")] private float telegraphBlinkOn = 0.2f;
        [SerializeField, Tooltip("Seconds each harmless blink stays invisible.")] private float telegraphBlinkOff = 0.2f;
        [SerializeField, Tooltip("Visible blinks before the pad becomes active.")] private int telegraphBlinkCount = 2;
        [SerializeField, Tooltip("Optional sprite renderer for telegraph.")] private SpriteRenderer? spriteRenderer;
        [SerializeField] private Collider2D? hitCollider;

        private TileGridController? gridController;
        private Vector3Int cell;
        private bool isArmed;
        private bool isVisible;
        private float stateTimer;
        private int blinksCompleted;
        private PushDirection currentDirection;

        public void Initialise(TileGridController grid, Vector3Int spawnCell)
        {
            gridController = grid;
            cell = spawnCell;
            currentDirection = direction;

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
                hitCollider.enabled = false;
            }

            isArmed = false;
            isVisible = false;
            stateTimer = 0f;
            blinksCompleted = 0;
            SetVisible(false);
        }

        private void Update()
        {
            if (!isArmed)
            {
                RunTelegraph();
            }
        }

        private void RunTelegraph()
        {
            var segmentDuration = isVisible ? telegraphBlinkOn : telegraphBlinkOff;
            stateTimer += Time.deltaTime;
            if (stateTimer < segmentDuration)
            {
                return;
            }

            stateTimer = 0f;
            isVisible = !isVisible;
            SetVisible(isVisible);

            if (isVisible)
            {
                blinksCompleted++;
                if (blinksCompleted >= telegraphBlinkCount)
                {
                    Arm();
                }
            }
        }

        private void Arm()
        {
            isArmed = true;
            SetVisible(true);
            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isArmed)
            {
                return;
            }

            var mover = other.GetComponent<PlayerGridMover>() ?? other.GetComponentInParent<PlayerGridMover>();
            if (mover == null)
            {
                return;
            }

            var offset = DirectionToOffset(currentDirection);
            StartCoroutine(PushWhenReady(mover, offset));
        }

        private Vector3Int DirectionToOffset(PushDirection dir)
        {
            return dir switch
            {
                PushDirection.Up => Vector3Int.up,
                PushDirection.Down => Vector3Int.down,
                PushDirection.Left => Vector3Int.left,
                _ => Vector3Int.right,
            };
        }

        public void SetDirection(PushDirection newDirection)
        {
            currentDirection = newDirection;
        }

        private IEnumerator PushWhenReady(PlayerGridMover mover, Vector3Int offset)
        {
            // Wait for the player to finish their current hop so the shove always applies.
            while (mover != null && mover.IsMoving)
            {
                yield return null;
            }

            if (mover != null)
            {
                mover.TryForceMove(offset);
            }
        }

        private void SetVisible(bool visible)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            var color = spriteRenderer.color;
            color.a = visible ? 1f : 0f;
            spriteRenderer.color = color;
        }
    }
}
