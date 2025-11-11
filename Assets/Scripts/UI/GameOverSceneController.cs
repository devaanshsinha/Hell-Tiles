using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#nullable enable

namespace HellTiles.UI
{
    public class GameOverSceneController : MonoBehaviour
    {
        [SerializeField] private string menuSceneName = "New Game"; // menu scene name
        [SerializeField] private KeyCode restartKey = KeyCode.Space;
        [SerializeField] private TMP_Text? lastRunText;
        [SerializeField] private TMP_Text? bestRunText;

        private void Start()
        {
            UpdateLabels();
        }

        private void Update()
        {
            if (Input.GetKeyDown(restartKey)) // tap space to restart
            {
                if (!Application.CanStreamedLevelBeLoaded(menuSceneName))
                {
                    Debug.LogWarning($"Scene '{menuSceneName}' is not added to Build Settings.");
                    return;
                }

                SceneManager.LoadScene(menuSceneName);
            }
        }

        private void UpdateLabels()
        {
            // Show results pulled from the last gameplay run.
            if (lastRunText != null)
            {
                lastRunText.text = $"Score: {GameSessionData.LastRunScore}";
            }

            if (bestRunText != null)
            {
                bestRunText.text = $"Best Score: {GameSessionData.BestRunScore}";
            }
        }
    }
}
