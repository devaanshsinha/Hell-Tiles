using UnityEngine;
using TMPro;

#nullable enable

namespace HellTiles.UI
{
    /// <summary>
    /// Tracks survival time during gameplay and keeps the UI text updated.
    /// </summary>
    public class SurvivalTimer : MonoBehaviour
    {
        public static SurvivalTimer? Instance { get; private set; }

        [SerializeField] private TMP_Text? timerText; // world-space text on the HUD
        [SerializeField] private bool autoStart = true;
        [SerializeField] private float pointsPerSecond = 1f;
        [SerializeField] private string scoreSuffix = " pts";

        private float accumulatedPoints;
        private int currentScore;
        private bool isRunning;

        public int CurrentScore => currentScore;
        public bool IsRunning => isRunning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            accumulatedPoints = 0f;
            currentScore = 0;
            isRunning = autoStart;
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            accumulatedPoints += pointsPerSecond * Time.deltaTime;
            var newScore = Mathf.FloorToInt(accumulatedPoints);
            if (newScore != currentScore)
            {
                currentScore = newScore;
                UpdateDisplay();
            }
        }

        public void ResetTimer()
        {
            accumulatedPoints = 0f;
            currentScore = 0;
            UpdateDisplay();
        }

        public void StartTimer()
        {
            isRunning = true;
        }

        public void StopAndStore()
        {
            if (!isRunning)
            {
                return;
            }

            isRunning = false;
            GameSessionData.RegisterScore(currentScore);
        }

        private void UpdateDisplay()
        {
            if (timerText == null)
            {
                return;
            }

            var display = currentScore.ToString();
            if (!string.IsNullOrEmpty(scoreSuffix))
            {
                display += scoreSuffix;
            }

            timerText.text = display;
        }
    }
}
