using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HellTiles.UI;

#nullable enable

namespace HellTiles.Player
{
    /// <summary>
    /// Tracks player health, handles invulnerability blink, and updates UI heart icons.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField, Min(0)] private int maxHearts = 3;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer? playerSprite;
        [SerializeField] private Color hitTint = new(1f, 0.35f, 0.35f, 1f);
        [SerializeField, Tooltip("Seconds spent invulnerable after taking damage.")] private float invulnerabilityDuration = 1.5f;
        [SerializeField, Tooltip("How many visible flashes to play while invulnerable.")] private int blinkCount = 3;
        [SerializeField, Tooltip("Scene to load when all hearts are lost.")] private string gameOverSceneName = "Game Over";

        [Header("UI")]
        [SerializeField] private List<Image> heartIcons = new();

        public int MaxHearts => maxHearts;
        public int CurrentHearts { get; private set; }
        public bool IsInvulnerable => isInvulnerable;

        private Color defaultColor = Color.white;
        private bool isInvulnerable;
        private Coroutine? invulnerabilityRoutine;

        private void Awake()
        {
            if (playerSprite == null)
            {
                playerSprite = GetComponentInChildren<SpriteRenderer>();
            }

            if (playerSprite != null)
            {
                defaultColor = playerSprite.color;
            }

            CurrentHearts = Mathf.Clamp(maxHearts, 0, maxHearts);
            GameSessionData.ResetCurrentRun();
            RefreshHeartUI();
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
            if (isInvulnerable || CurrentHearts <= 0)
            {
                return;
            }

            CurrentHearts = Mathf.Max(0, CurrentHearts - 1);
            RefreshHeartUI();

            if (invulnerabilityRoutine != null)
            {
                StopCoroutine(invulnerabilityRoutine);
            }

            invulnerabilityRoutine = StartCoroutine(InvulnerabilityWindow());

            if (CurrentHearts <= 0)
            {
                SurvivalTimer.Instance?.StopAndStore();
                LoadGameOverScene();
            }
        }

        private IEnumerator InvulnerabilityWindow()
        {
            if (playerSprite == null)
            {
                yield break;
            }

            isInvulnerable = true;

            var flashes = Mathf.Max(1, blinkCount);
            var halfStep = invulnerabilityDuration / (flashes * 2f);

            for (int i = 0; i < flashes; i++)
            {
                playerSprite.color = hitTint;
                yield return new WaitForSeconds(halfStep);

                playerSprite.color = defaultColor;
                yield return new WaitForSeconds(halfStep);
            }

            playerSprite.color = defaultColor;
            isInvulnerable = false;
            invulnerabilityRoutine = null;
        }

        private void RefreshHeartUI()
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

                heartIcons[i].enabled = i < CurrentHearts;
            }
        }

        private void LoadGameOverScene()
        {
            if (string.IsNullOrWhiteSpace(gameOverSceneName))
            {
                Debug.LogWarning($"{nameof(PlayerHealth)} has no Game Over scene name configured.", this);
                return;
            }

            if (SceneManager.GetActiveScene().name == gameOverSceneName)
            {
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(gameOverSceneName))
            {
                Debug.LogWarning($"Scene '{gameOverSceneName}' is not added to Build Settings.", this);
                return;
            }

            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}
