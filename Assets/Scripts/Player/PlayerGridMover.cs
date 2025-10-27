using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using HellTiles.Tiles;

namespace HellTiles.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerGridMover : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TileGridController gridController = default!;
        [SerializeField] private InputActionReference moveAction = default!;
        [SerializeField] private Rigidbody2D? body2D;

        [Header("Movement")]
        [SerializeField, Min(0.01f)] private float hopDuration = 0.18f;
        [SerializeField] private AnimationCurve hopCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField, Range(0f, 1f)] private float inputDeadzone = 0.2f;
        [SerializeField] private bool snapToNearestWalkable;

        private Vector3Int currentCell;
        private bool isMoving;

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
            if (isMoving || moveAction == null || moveAction.action == null)
            {
                return;
            }

            var input = moveAction.action.ReadValue<Vector2>();
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
            isMoving = false;
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
    }
}
