using UnityEngine;
using HellTiles.UI;

#nullable enable

namespace HellTiles.Powerups
{
    [RequireComponent(typeof(Collider2D))]
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private int coinValue = 1;
        [SerializeField] private float lifetime = 6f;
        [SerializeField] private float flickerDuration = 1f;
        [SerializeField] private SpriteRenderer? spriteRenderer;

        private float elapsed;
        private float flickerStart;

        private void Awake()
        {
            flickerStart = Mathf.Max(0f, lifetime - flickerDuration);
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            if (elapsed >= flickerStart && spriteRenderer != null)
            {
                var t = Mathf.PingPong((elapsed - flickerStart) * 10f, 1f);
                var color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0.2f, t);
                spriteRenderer.color = color;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<Player.PlayerHealth>() ?? other.GetComponentInParent<Player.PlayerHealth>();
            if (player == null)
            {
                return;
            }

            CoinWallet.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}
