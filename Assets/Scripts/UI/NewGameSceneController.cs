using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellTiles.UI
{
    public class NewGameSceneController : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "SampleScene"; // main gameplay scene
        [SerializeField] private KeyCode advanceKey = KeyCode.Space;

        private void Update()
        {
            if (Input.GetKeyDown(advanceKey)) // press space to start
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
