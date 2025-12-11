using UnityEngine;
using HellTiles.UI;
using HellTiles.Audio;

#nullable enable

namespace HellTiles.Powerups
{
    /// <summary>
    /// Simple rotating coin collectible with lifetime + flicker warning.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private int coinValue = 1;
        [SerializeField] private float lifetime = 6f;
        [SerializeField] private float flickerDuration = 1f;
        [SerializeField] private SpriteRenderer? spriteRenderer;
        [SerializeField, Tooltip("Sound to play when the coin is collected.")] private AudioClip? coinSfx;
        [SerializeField, Range(0f, 1f)] private float coinSfxVolume = 1f;

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
                // Gently flash before the coin disappears.
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

            CoinWallet.AddCoins(coinValue); // persistently add to wallet
            OneShotAudio.Play2D(coinSfx, coinSfxVolume);
            Destroy(gameObject);
        }
    }
}
