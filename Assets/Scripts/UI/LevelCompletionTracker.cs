using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

namespace HellTiles.UI
{
    /// <summary>
    /// Marks the current level as completed in PlayerPrefs when told, then loads the LevelSelect scene.
    /// </summary>
    public class LevelCompletionTracker : MonoBehaviour
    {
        [SerializeField, Tooltip("Index of this level in the LevelSelect nodes array.")] private int levelIndex = 0;
        [SerializeField, Tooltip("Scene name of the LevelSelect screen.")] private string levelSelectSceneName = "LevelSelect";

        /// <summary>
        /// Marks completion, but does not change scenes.
        /// </summary>
        public void MarkCompleted()
        {
            PlayerPrefs.SetInt(GetCompletedKey(levelIndex), 1);
            PlayerPrefs.Save();
        }

        public void MarkCompletedAndReturn()
        {
            MarkCompleted();
            SceneManager.LoadScene(levelSelectSceneName);
        }

        public static string GetCompletedKey(int index) => $"hellTiles_level_{index}_completed";
    }
}
