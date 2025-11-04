using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#nullable enable

namespace HellTiles.UI
{
    public class GameOverSceneController : MonoBehaviour
    {
        [SerializeField] private string newGameSceneName = "New Game";
        [SerializeField] private KeyCode restartKey = KeyCode.Space;
        [SerializeField] private TMP_Text? lastRunText;
        [SerializeField] private TMP_Text? bestRunText;

        private void Start()
        {
            UpdateLabels();
        }

        private void Update()
        {
            if (Input.GetKeyDown(restartKey))
            {
                if (!Application.CanStreamedLevelBeLoaded(newGameSceneName))
                {
                    Debug.LogWarning($"Scene '{newGameSceneName}' is not added to Build Settings.");
                    return;
                }

                SceneManager.LoadScene(newGameSceneName);
            }
        }

        private void UpdateLabels()
        {
            if (lastRunText != null)
            {
                lastRunText.text = $"Time Survived: {FormatTime(GameSessionData.LastRunDuration)}";
            }

            if (bestRunText != null)
            {
                bestRunText.text = $"Best Time: {FormatTime(GameSessionData.BestRunDuration)}";
            }
        }

        private static string FormatTime(float seconds)
        {
            var span = System.TimeSpan.FromSeconds(Mathf.Max(0f, seconds));
            return span.ToString(@"mm\:ss");
        }
    }
}
