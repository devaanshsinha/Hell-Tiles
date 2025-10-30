using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellTiles.UI
{
    public class NewGameSceneController : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "SampleScene";
        [SerializeField] private KeyCode advanceKey = KeyCode.Space;

        private void Update()
        {
            if (Input.GetKeyDown(advanceKey))
            {
                if (!Application.CanStreamedLevelBeLoaded(gameplaySceneName))
                {
                    Debug.LogWarning($"Scene '{gameplaySceneName}' is not added to Build Settings.");
                    return;
                }

                SceneManager.LoadScene(gameplaySceneName);
            }
        }
    }
}
