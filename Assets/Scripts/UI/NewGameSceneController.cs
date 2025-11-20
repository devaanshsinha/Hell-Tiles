using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellTiles.UI
{
    public class NewGameSceneController : MonoBehaviour
    {
        [SerializeField] private string levelSelectSceneName = "LevelSelect";
        [SerializeField] private string tutorialSceneName = "Tutorial";
        [SerializeField] private string shopSceneName = "Shop";
        [SerializeField] private KeyCode tutorialKey = KeyCode.Space;
        [SerializeField] private KeyCode levelSelectKey = KeyCode.Return;
        [SerializeField] private KeyCode shopKey = KeyCode.S;

        private void Update()
        {
            if (Input.GetKeyDown(levelSelectKey))
            {
                if (!Application.CanStreamedLevelBeLoaded(levelSelectSceneName))
                {
                    Debug.LogWarning($"Scene '{levelSelectSceneName}' is not added to Build Settings.");
                    return;
                }

                SceneManager.LoadScene(levelSelectSceneName);
            }
            else if (Input.GetKeyDown(tutorialKey))
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
