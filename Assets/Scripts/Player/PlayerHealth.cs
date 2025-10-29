using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace HellTiles.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHearts = 3;
        [SerializeField] private SpriteRenderer playerSprite = default!;
        [SerializeField] private List<Image> heartIcons = new();
        [SerializeField] private float invulnerabilityDuration = 1.5f;
        [SerializeField] private float blinkInterval = 0.2f;
        [SerializeField] private Color hitTint = new(1f, 0.3f, 0.3f, 1f);

        private int currentHearts;
        private bool isInvulnerable;
        private Color defaultColor = Color.white;
        private Coroutine? invulnerabilityRoutine;

        public int CurrentHearts => currentHearts;
        public int MaxHearts => maxHearts;

        private void Awake()
        {
            if (playerSprite == null)
            {
                playerSprite = GetComponentInChildren<SpriteRenderer>();
            }

            defaultColor = playerSprite != null ? playerSprite.color : Color.white;
            currentHearts = Mathf.Clamp(maxHearts, 0, maxHearts);
            RefreshHeartsUI();
        }

        private void OnDisable()
        {
            if (invulnerabilityRoutine != null)
            {
                StopCoroutine(invulnerabilityRoutine);
                invulnerabilityRoutine = null;
            }

            if (playerSprite != null)
            {
                playerSprite.color = defaultColor;
            }

            isInvulnerable = false;
        }

        public void TakeHit()
        {
            if (isInvulnerable || currentHearts <= 0)
            {
                return;
            }

            currentHearts = Mathf.Max(0, currentHearts - 1);
            RefreshHeartsUI();

            if (invulnerabilityRoutine != null)
            {
                StopCoroutine(invulnerabilityRoutine);
            }

            invulnerabilityRoutine = StartCoroutine(InvulnerabilityWindow());
        }

        private IEnumerator InvulnerabilityWindow()
        {
            isInvulnerable = true;

            if (playerSprite == null)
            {
                yield break;
            }

            var elapsed = 0f;
            var visible = true;

            while (elapsed < invulnerabilityDuration)
            {
                playerSprite.color = visible ? hitTint : defaultColor;
                visible = !visible;
                var wait = Mathf.Min(blinkInterval, invulnerabilityDuration - elapsed);
                elapsed += wait;
                yield return new WaitForSeconds(wait);
            }

            playerSprite.color = defaultColor;
            isInvulnerable = false;
            invulnerabilityRoutine = null;
        }

        private void RefreshHeartsUI()
        {
            if (heartIcons == null || heartIcons.Count == 0)
            {
                return;
            }

            for (var i = 0; i < heartIcons.Count; i++)
            {
                if (heartIcons[i] == null)
                {
                    continue;
                }

                heartIcons[i].enabled = i < currentHearts;
            }
        }
    }
}
