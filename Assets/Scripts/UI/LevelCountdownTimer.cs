using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#nullable enable

namespace HellTiles.UI
{
    /// <summary>
    /// Counts down from a start time; on timeout loads the win scene. Intended for level-based modes.
    /// </summary>
    public class LevelCountdownTimer : MonoBehaviour
    {
        [SerializeField, Tooltip("Starting time in seconds.")] private float startSeconds = 80f;
        [SerializeField, Tooltip("Text element to display the remaining time.")] private TMP_Text? timerLabel;
        [SerializeField, Tooltip("Scene to load when the timer reaches zero.")] private string winSceneName = "Won";

        private float remaining;
        private bool finished;

        private void Awake()
        {
            remaining = startSeconds;
            finished = false;
            UpdateLabel();
        }

        private void Update()
        {
            if (finished)
            {
                return;
            }

            remaining -= Time.deltaTime;
            if (remaining <= 0f)
            {
                remaining = 0f;
                finished = true;
                UpdateLabel();
                LoadWinScene();
                return;
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (timerLabel == null)
            {
                return;
            }

            var clamped = Mathf.Max(0f, remaining);
            var minutes = Mathf.FloorToInt(clamped / 60f);
            var seconds = Mathf.FloorToInt(clamped % 60f);
            timerLabel.text = $"{minutes:00}:{seconds:00}";
        }

        private void LoadWinScene()
        {
            if (string.IsNullOrWhiteSpace(winSceneName))
            {
                Debug.LogWarning($"{nameof(LevelCountdownTimer)} has no win scene configured.", this);
                return;
            }

            if (!SceneManager.GetSceneByName(winSceneName).IsValid() && !Application.CanStreamedLevelBeLoaded(winSceneName))
            {
                Debug.LogWarning($"Scene '{winSceneName}' is not in Build Settings.", this);
                return;
            }

            SceneManager.LoadScene(winSceneName);
        }
    }
}
