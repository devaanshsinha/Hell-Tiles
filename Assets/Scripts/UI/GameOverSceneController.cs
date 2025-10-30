using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellTiles.UI
{
    public class GameOverSceneController : MonoBehaviour
    {
        [SerializeField] private string newGameSceneName = "New Game";
        [SerializeField] private KeyCode restartKey = KeyCode.Space;

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
    }
}
