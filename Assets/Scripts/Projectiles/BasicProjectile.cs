using UnityEngine;
using HellTiles.Player;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Moves towards a target position at a constant speed and despawns after travelling a max distance.
    /// </summary>
    public class BasicProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 6f;
        [SerializeField] private float maxLifetime = 8f;
        [SerializeField] private float maxTravelDistance = 20f;
        [SerializeField] private Rigidbody2D? body2D;
        [SerializeField] private bool continuousTracking;

        private Vector3 targetPosition;
        private Transform? dynamicTarget;
        private Vector3 spawnPosition;
        private Vector3 moveDirection = Vector3.zero;
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
            if (continuousTracking && dynamicTarget != null)
            {
                targetPosition = dynamicTarget.position;
                var newDirection = (targetPosition - position).normalized;
                if (newDirection.sqrMagnitude > 0.0001f)
                {
                    moveDirection = newDirection;
                }
            }

            if (moveDirection.sqrMagnitude == 0f)
            {
                moveDirection = (targetPosition - position).normalized;
                if (moveDirection.sqrMagnitude == 0f)
                {
                    moveDirection = Vector3.up;
                }
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

            if (step.sqrMagnitude > 0.0001f)
            {
                var angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }

            var travelled = (nextPosition - spawnPosition).sqrMagnitude;
            if (travelled >= maxTravelDistance * maxTravelDistance)
            {
                Destroy(gameObject);
            }
        }

        public void Initialise(Vector3 target)
        {
            targetPosition = target;
            dynamicTarget = null;
            moveDirection = (targetPosition - transform.position).normalized;
            if (moveDirection.sqrMagnitude == 0f)
            {
                moveDirection = Vector3.up;
            }
            hasTarget = true;
        }

        public void Initialise(Transform target)
        {
            dynamicTarget = continuousTracking ? target : null;
            targetPosition = target.position;
            moveDirection = (targetPosition - transform.position).normalized;
            if (moveDirection.sqrMagnitude == 0f)
            {
                moveDirection = Vector3.up;
            }
            hasTarget = true;
        }

        public void Retarget(Vector3 newTarget)
        {
            targetPosition = newTarget;
            dynamicTarget = null;
            moveDirection = (targetPosition - transform.position).normalized;
            if (moveDirection.sqrMagnitude == 0f)
            {
                moveDirection = Vector3.up;
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
