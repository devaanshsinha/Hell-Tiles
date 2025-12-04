using UnityEngine;
using HellTiles.Player;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Simple straight-line projectile with configurable sprite forward offset.
    /// </summary>
    public class DirectionalProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 6f;
        [SerializeField] private float maxLifetime = 8f;
        [SerializeField] private float maxTravelDistance = 20f;
        [SerializeField, Tooltip("Degrees added to the facing so sprites with an off-axis forward can be aligned.")] private float spriteForwardOffset = 0f;
        [SerializeField] private Rigidbody2D? body2D;

        private Vector3 moveDirection = Vector3.zero;
        private Vector3 spawnPosition;
        private float elapsedLifetime;
        private bool hasTarget;

        private void Awake()
        {
            if (body2D == null)
            {
                body2D = GetComponent<Rigidbody2D>();
            }
        }

        private void OnEnable()
        {
            spawnPosition = transform.position;
            elapsedLifetime = 0f;
        }

        private void FixedUpdate()
        {
            if (!hasTarget)
            {
                return;
            }

            elapsedLifetime += Time.fixedDeltaTime;
            if (elapsedLifetime >= maxLifetime)
            {
                Destroy(gameObject);
                return;
            }

            var position = transform.position;
            if (moveDirection.sqrMagnitude == 0f)
            {
                return;
            }

            var step = moveDirection * (speed * Time.fixedDeltaTime);
            var nextPosition = position + step;

            if (body2D != null)
            {
                body2D.MovePosition(nextPosition);
            }
            else
            {
                transform.position = nextPosition;
            }

            var angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg + spriteForwardOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            var travelled = (nextPosition - spawnPosition).sqrMagnitude;
            if (travelled >= maxTravelDistance * maxTravelDistance)
            {
                Destroy(gameObject);
            }
        }

        public void Initialise(Vector3 targetPosition)
        {
            moveDirection = (targetPosition - transform.position).normalized;
            if (moveDirection.sqrMagnitude == 0f)
            {
                moveDirection = Vector3.right;
            }
            hasTarget = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health == null)
            {
                return;
            }

            health.TakeHit();
            Destroy(gameObject);
        }
    }
}
