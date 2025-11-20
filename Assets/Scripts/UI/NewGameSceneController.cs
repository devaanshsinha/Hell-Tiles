using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellTiles.UI
{
    public class NewGameSceneController : MonoBehaviour
    {
        [SerializeField] private string tutorialSceneName = "Tutorial"; // first stop after menu
        [SerializeField] private string shopSceneName = "Shop";
        [SerializeField] private KeyCode advanceKey = KeyCode.Space;
        [SerializeField] private KeyCode shopKey = KeyCode.S;

        private void Update()
        {
            if (Input.GetKeyDown(advanceKey)) // press space to start
            {
                if (!Application.CanStreamedLevelBeLoaded(tutorialSceneName))
                {
                    Debug.LogWarning($"Scene '{tutorialSceneName}' is not added to Build Settings.");
                    return;
                }

                SceneManager.LoadScene(tutorialSceneName);
            }
            else if (Input.GetKeyDown(shopKey))
            {
                if (!Application.CanStreamedLevelBeLoaded(shopSceneName))
                {
                    Debug.LogWarning($"Scene '{shopSceneName}' is not added to Build Settings.");
                    return;
                }

                SceneManager.LoadScene(shopSceneName);
            }
        }
    }
}
