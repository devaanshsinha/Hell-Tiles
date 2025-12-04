using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

namespace HellTiles.UI
{
    /// <summary>
    /// Simple scene loader on spacebar.
    /// </summary>
    public class SpaceToScene : MonoBehaviour
    {
        [SerializeField] private string sceneName = "LevelSelect";
        [SerializeField] private KeyCode key = KeyCode.Space;

        private void Update()
        {
            if (Input.GetKeyDown(key))
            {
                if (string.IsNullOrWhiteSpace(sceneName))
                {
                    Debug.LogWarning($"{nameof(SpaceToScene)} has no scene assigned.", this);
                    return;
                }

                if (!SceneManager.GetSceneByName(sceneName).IsValid() && !Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    Debug.LogWarning($"Scene '{sceneName}' is not in Build Settings.", this);
                    return;
                }

                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
