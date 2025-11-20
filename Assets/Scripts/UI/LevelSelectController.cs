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
            public bool unlocked = true;
        }

        [SerializeField] private Transform cursor;
        [SerializeField] private LevelNode[] nodes = System.Array.Empty<LevelNode>();
        [SerializeField] private KeyCode leftKey = KeyCode.A;
        [SerializeField] private KeyCode rightKey = KeyCode.D;
        [SerializeField] private KeyCode upKey = KeyCode.W;
        [SerializeField] private KeyCode downKey = KeyCode.S;
        [SerializeField] private KeyCode confirmKey = KeyCode.Return;
        [SerializeField] private KeyCode exitKey = KeyCode.Escape;
        [SerializeField] private string menuSceneName = "New Game";

        private int currentIndex;
        private bool onLevelNode;

        private void Start()
        {
            UpdateCursorPosition();
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
        }

        private void MoveHorizontal(int direction)
        {
            if (onLevelNode)
            {
                return; // must drop down before moving sideways
            }

            currentIndex = Mathf.Clamp(currentIndex + direction, 0, nodes.Length - 1);
            UpdateCursorPosition();
        }

        private void ToggleLevelNode(bool goingUp)
        {
            var node = nodes[currentIndex];
            if (!node.unlocked)
            {
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
            }
        }

        private void LoadCurrentLevel()
        {
            var node = nodes[currentIndex];
            if (string.IsNullOrWhiteSpace(node.levelSceneName) || !node.unlocked)
            {
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
    }
}
