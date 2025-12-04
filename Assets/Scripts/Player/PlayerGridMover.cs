using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using HellTiles.Tiles;

#nullable enable

namespace HellTiles.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerGridMover : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TileGridController gridController = default!; // main grid logic
        [SerializeField] private InputActionReference moveAction = default!;
        [SerializeField] private Rigidbody2D? body2D;

        [Header("Movement")]
        [SerializeField, Min(0.01f)] private float hopDuration = 0.18f;
        [SerializeField] private AnimationCurve hopCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField, Range(0f, 1f)] private float inputDeadzone = 0.2f;
        [SerializeField] private bool snapToNearestWalkable;
        [Header("Landing Bounce")]
        [SerializeField, Min(0f)] private float playerBounceDepth = 0.1f;
        [SerializeField, Min(0f)] private float playerBounceDuration = 0.3f;
        [SerializeField, Min(0f)] private float tileBounceDepth = 0.05f;
        [SerializeField, Min(0f)] private float tileBounceDuration = 0.3f;

        private Vector3Int currentCell;
        private bool isMoving;
        private Coroutine? moveRoutine;

        public bool IsMoving => isMoving;
        public bool IsExternallyLocked => externalLockCount > 0;

        private int externalLockCount;
        private void Awake()
        {
            if (gridController == null)
            {
                gridController = FindFirstObjectByType<TileGridController>();
            }

            if (body2D == null)
            {
                body2D = GetComponent<Rigidbody2D>();
            }
        }

        private void OnEnable()
        {
            moveAction?.action?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.action?.Disable();
        }

        private void Start()
        {
            if (gridController == null || moveAction == null || moveAction.action == null)
            {
                Debug.LogError($"{nameof(PlayerGridMover)} is missing required references.", this);
                enabled = false;
                return;
            }

            if (snapToNearestWalkable && gridController.TryGetNearestWalkableCell(transform.position, out var snappedCell))
            {
                currentCell = snappedCell;
                SnapToCell(snappedCell);
            }
            else
            {
                currentCell = gridController.WorldToCell(transform.position);
            }
        }

        private void Update()
        {
            if (isMoving || IsExternallyLocked || moveAction == null || moveAction.action == null)
            {
                return;
            }

            var input = moveAction.action.ReadValue<Vector2>(); // read WASD / stick
            var direction = ResolveCardinalDirection(input);
            if (direction == Vector3Int.zero)
            {
                return;
            }

            var targetCell = currentCell + direction;
            if (!gridController.IsWalkable(targetCell))
            {
                return;
            }

            var worldTarget = gridController.CellToWorldCenter(targetCell);
            StartCoroutine(HopTo(worldTarget, targetCell));
        }

        /// <summary>
        /// External caller (e.g., hazards) can force a one-tile move if possible.
        /// </summary>
        public bool TryForceMove(Vector3Int direction)
        {
            if (isMoving || gridController == null)
            {
                return false;
            }

            var targetCell = currentCell + direction;
            if (!gridController.IsWalkable(targetCell))
            {
                return false;
            }

            var worldTarget = gridController.CellToWorldCenter(targetCell);
            StartCoroutine(HopTo(worldTarget, targetCell));
            return true;
        }

        /// <summary>
        /// Interrupt any movement and shove the player immediately by one tile if walkable.
        /// </summary>
        public bool ForceImmediatePush(Vector3Int direction)
        {
            if (gridController == null)
            {
                return false;
            }

            StopCurrentMove();

            // Re-evaluate current cell from world to avoid stale state mid-hop.
            currentCell = gridController.WorldToCell(transform.position);
            var targetCell = currentCell + direction;
            if (!gridController.IsWalkable(targetCell))
            {
                return false;
            }

            var worldTarget = gridController.CellToWorldCenter(targetCell);
            moveRoutine = StartCoroutine(HopTo(worldTarget, targetCell));
            return true;
        }

        /// <summary>
        /// Force a move and temporarily lock input until the shove finishes.
        /// </summary>
        public void ForceMoveWithLock(Vector3Int direction)
        {
            StartCoroutine(ForceMoveRoutine(direction));
        }

        private IEnumerator ForceMoveRoutine(Vector3Int direction)
        {
            // wait for any current hop to finish
            while (isMoving)
            {
                yield return null;
            }

            externalLockCount++;
            var success = TryForceMove(direction);

            if (success)
            {
                while (isMoving)
                {
                    yield return null;
                }
            }

            externalLockCount = Mathf.Max(0, externalLockCount - 1);
        }

        private IEnumerator HopTo(Vector3 worldTarget, Vector3Int targetCell)
        {
            isMoving = true;
            var startPosition = transform.position;

            var elapsed = 0f;
            while (elapsed < hopDuration)
            {
                elapsed += Time.deltaTime;
                var t = hopCurve.Evaluate(Mathf.Clamp01(elapsed / hopDuration));
                var newPos = Vector3.Lerp(startPosition, worldTarget, t);
                ApplyPosition(newPos);
                yield return null;
            }

            ApplyPosition(worldTarget);
            currentCell = targetCell;

            var maxLandingDuration = 0f;
            if (tileBounceDepth > 0f && tileBounceDuration > 0f)
            {
                gridController.PlayTileBounce(targetCell, tileBounceDepth, tileBounceDuration);
                maxLandingDuration = Mathf.Max(maxLandingDuration, tileBounceDuration);
            }

            if (playerBounceDepth > 0f && playerBounceDuration > 0f)
            {
                yield return PlayerBounceRoutine(playerBounceDepth, playerBounceDuration, worldTarget);
            }
            else if (maxLandingDuration > 0f)
            {
                yield return new WaitForSeconds(maxLandingDuration);
            }

            isMoving = false;
            moveRoutine = null;
        }

        private void ApplyPosition(Vector3 position)
        {
            if (body2D != null)
            {
                body2D.MovePosition(position);
            }
            else
            {
                transform.position = position;
            }
        }

        private void SnapToCell(Vector3Int cell)
        {
            var world = gridController.CellToWorldCenter(cell);
            ApplyPosition(world);
        }

        private Vector3Int ResolveCardinalDirection(Vector2 input)
        {
            if (input.sqrMagnitude < inputDeadzone * inputDeadzone)
            {
                return Vector3Int.zero;
            }

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return input.x > 0 ? Vector3Int.right : Vector3Int.left;
            }

            return input.y > 0 ? Vector3Int.up : Vector3Int.down;
        }

        private IEnumerator PlayerBounceRoutine(float depth, float duration, Vector3 basePosition)
        {
            // Dip the player sprite slightly then return to rest.
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var normalized = Mathf.Clamp01(elapsed / duration);
                var offset = Mathf.Sin(normalized * Mathf.PI) * -depth;
                ApplyPosition(new Vector3(basePosition.x, basePosition.y + offset, basePosition.z));
                yield return null;
            }

            ApplyPosition(basePosition);
        }

        private void StopCurrentMove()
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
                moveRoutine = null;
            }

            isMoving = false;
        }
    }
}
