using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellTiles.UI
{
    public class NewGameSceneController : MonoBehaviour
    {
        [SerializeField] private string tutorialSceneName = "Tutorial"; // first stop after menu
        [SerializeField] private KeyCode advanceKey = KeyCode.Space;

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
        }
    }
}
