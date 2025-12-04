using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

namespace HellTiles.UI
{
    public class LevelSelectController : MonoBehaviour
    {
        [System.Serializable]
        private class LevelNode
        {
            public string levelSceneName = "SampleScene";
            public Transform? basePosition;
            public Transform? elevatedPosition;
            public bool unlocked = true; // initial unlock (e.g., level 1)
            public bool completed;
            public GameObject? completedMark; // check mark to enable when completed
        }

        [SerializeField] private Transform cursor = default!;
        [SerializeField] private Transform? playerVisual;
        [SerializeField] private LevelNode[] nodes = System.Array.Empty<LevelNode>();
        [SerializeField] private KeyCode leftKey = KeyCode.A;
        [SerializeField] private KeyCode rightKey = KeyCode.D;
        [SerializeField] private KeyCode upKey = KeyCode.W;
        [SerializeField] private KeyCode downKey = KeyCode.S;
        [SerializeField] private KeyCode confirmKey = KeyCode.Return;
        [SerializeField] private KeyCode exitKey = KeyCode.Escape;
        [SerializeField] private string menuSceneName = "New Game";
        [SerializeField, Tooltip("TMP text to show temporary messages.")] private TMPro.TMP_Text? messageLabel;
        [SerializeField, Tooltip("Seconds a message stays visible.")] private float messageDuration = 1.5f;

        private int currentIndex;
        private bool onLevelNode;
        private float messageTimer;

        private void Start()
        {
            LoadProgress();
            RefreshCompletionMarks();
            UpdateCursorPosition();
            UpdatePlayerPosition();
            UpdatePlayerPosition();
        }

        private void Update()
        {
            if (nodes.Length == 0 || cursor == null)
            {
                return;
            }

            if (Input.GetKeyDown(leftKey))
            {
                MoveHorizontal(-1);
            }
            else if (Input.GetKeyDown(rightKey))
            {
                MoveHorizontal(1);
            }
            else if (Input.GetKeyDown(upKey))
            {
                ToggleLevelNode(true);
            }
            else if (Input.GetKeyDown(downKey))
            {
                ToggleLevelNode(false);
            }
            else if (Input.GetKeyDown(confirmKey) && onLevelNode)
            {
                LoadCurrentLevel();
            }
            else if (Input.GetKeyDown(exitKey))
            {
                SceneManager.LoadScene(menuSceneName);
            }

            UpdateMessage();
        }

        private void MoveHorizontal(int direction)
        {
            if (onLevelNode)
            {
                return; // must drop down before moving sideways
            }

            currentIndex = Mathf.Clamp(currentIndex + direction, 0, nodes.Length - 1);
            UpdateCursorPosition();
            UpdatePlayerPosition();
        }

        private void ToggleLevelNode(bool goingUp)
        {
            var node = nodes[currentIndex];
            if (!node.unlocked)
            {
                ShowMessage("Complete previous level first");
                return;
            }

            if (goingUp && !onLevelNode)
            {
                if (node.elevatedPosition != null)
                {
                    cursor.position = node.elevatedPosition.position;
                    onLevelNode = true;
                }
            }
            else if (!goingUp && onLevelNode)
            {
                onLevelNode = false;
                if (node.basePosition != null)
                {
                    cursor.position = node.basePosition.position;
                }
                UpdatePlayerPosition();
            }
        }

        private void LoadCurrentLevel()
        {
            var node = nodes[currentIndex];
            if (string.IsNullOrWhiteSpace(node.levelSceneName))
            {
                return;
            }

            // If not unlocked, block entry.
            if (!node.unlocked)
            {
                ShowMessage("Complete previous level first");
                return;
            }

            if (!SceneManager.GetSceneByName(node.levelSceneName).IsValid() && !Application.CanStreamedLevelBeLoaded(node.levelSceneName))
            {
                Debug.LogWarning($"Scene '{node.levelSceneName}' missing from Build Settings.");
                return;
            }

            SceneManager.LoadScene(node.levelSceneName);
        }

        private void UpdateCursorPosition()
        {
            var node = nodes[currentIndex];
            var target = onLevelNode ? node.elevatedPosition : node.basePosition;
            if (target != null && cursor != null)
            {
                cursor.position = target.position;
            }
        }

        private void UpdatePlayerPosition()
        {
            if (playerVisual == null)
            {
                return;
            }

            var node = nodes[currentIndex];
            var target = node.basePosition;
            if (target != null)
            {
                playerVisual.position = target.position;
            }
        }

        private void ShowMessage(string text)
        {
            if (messageLabel == null)
            {
                return;
            }

            messageLabel.gameObject.SetActive(true);
            messageLabel.text = text;
            messageTimer = messageDuration;
        }

        private void UpdateMessage()
        {
            if (messageLabel == null || messageTimer <= 0f)
            {
                return;
            }

            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f)
            {
                messageLabel.gameObject.SetActive(false);
            }
        }

        private void RefreshCompletionMarks()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                if (node.completedMark != null)
                {
                    node.completedMark.SetActive(node.completed);
                }
            }
        }

        private void LoadProgress()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                var completedKey = GetCompletedKey(i);
                nodes[i].completed = PlayerPrefs.GetInt(completedKey, 0) == 1;

                // Unlock rule: first node always unlocked; others unlock if they or the previous node are completed,
                // or if the previous node is a spacer (no level) that is unlocked.
                if (i == 0)
                {
                    nodes[i].unlocked = true;
                }
                else
                {
                    bool prevIsSpacer = string.IsNullOrWhiteSpace(nodes[i - 1].levelSceneName);
                    bool prevAllows = nodes[i - 1].completed || (prevIsSpacer && nodes[i - 1].unlocked);
                    nodes[i].unlocked = nodes[i].unlocked || nodes[i].completed || prevAllows;
                }
            }
        }

        private static string GetCompletedKey(int index) => $"hellTiles_level_{index}_completed";

        /// <summary>
        /// Call this after completing a level to mark it done and unlock the next.
        /// </summary>
        public void MarkLevelCompleted(int index)
        {
            if (index < 0 || index >= nodes.Length)
            {
                return;
            }

            var node = nodes[index];
            if (node == null)
            {
                return;
            }

            node.completed = true;
            PlayerPrefs.SetInt(GetCompletedKey(index), 1);
            PlayerPrefs.Save();

            if (node.completedMark != null)
            {
                node.completedMark.SetActive(true);
            }

            if (index + 1 < nodes.Length)
            {
                nodes[index + 1].unlocked = true;
            }
        }
    }
}
