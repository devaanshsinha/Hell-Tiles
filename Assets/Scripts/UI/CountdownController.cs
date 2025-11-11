using System.Collections;
using TMPro;
using UnityEngine;

namespace HellTiles.UI
{
    /// <summary>
    /// Shows a simple 3-2-1 countdown overlay, then starts gameplay.
    /// </summary>
    public class CountdownController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text countdownText = default!;
        [SerializeField] private CanvasGroup? overlayGroup;

        [Header("Timing")]
        [SerializeField, Min(1)] private int startNumber = 3;
        [SerializeField] private float stepDuration = 1f;
        [SerializeField] private string finalMessage = "GO!";
        [SerializeField] private float finalMessageDuration = 0.75f;

        [Header("Gameplay Hooks")]
        [SerializeField] private SurvivalTimer? survivalTimer;
        [SerializeField] private MonoBehaviour[] enableAfterCountdown = new MonoBehaviour[0];

        private void Awake()
        {
            foreach (var behaviour in enableAfterCountdown)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = false;
                }
            }

            if (survivalTimer != null)
            {
                survivalTimer.ResetTimer();
            }

            if (overlayGroup != null)
            {
                overlayGroup.alpha = 1f;
            }
        }

        private void Start()
        {
            StartCoroutine(RunCountdown());
        }

        private IEnumerator RunCountdown()
        {
            var number = Mathf.Max(1, startNumber);
            for (var i = number; i > 0; i--)
            {
                if (countdownText != null)
                {
                    countdownText.text = i.ToString();
                }

                yield return new WaitForSeconds(stepDuration);
            }

            if (countdownText != null)
            {
                countdownText.text = finalMessage;
            }

            yield return new WaitForSeconds(finalMessageDuration);

            if (overlayGroup != null)
            {
                overlayGroup.alpha = 0f;
                overlayGroup.gameObject.SetActive(false);
            }
            else if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
            }

            if (survivalTimer != null)
            {
                survivalTimer.StartTimer();
            }

            foreach (var behaviour in enableAfterCountdown)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = true;
                }
            }
        }
    }
}
