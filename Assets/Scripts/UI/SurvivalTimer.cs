using System;
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
        [SerializeField] private bool showMilliseconds = false;

        private float elapsedTime;
        private bool isRunning;

        public float ElapsedTime => elapsedTime;
        public bool IsRunning => isRunning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            elapsedTime = 0f;
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

            elapsedTime += Time.deltaTime; // accumulate seconds
            UpdateDisplay();
        }

        public void ResetTimer()
        {
            elapsedTime = 0f;
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
            GameSessionData.RegisterRun(elapsedTime);
        }

        private void UpdateDisplay()
        {
            if (timerText == null)
            {
                return;
            }

            var span = TimeSpan.FromSeconds(Mathf.Max(0f, elapsedTime)); // guard against negative times
            var format = showMilliseconds ? @"mm\:ss\.ff" : @"mm\:ss";
            timerText.text = span.ToString(format);
        }
    }
}
