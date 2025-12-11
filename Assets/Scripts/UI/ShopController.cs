using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace HellTiles.UI
{
    /// <summary>
    /// Handles simple horizontal skin selection and purchasing with coins.
    /// </summary>
    public class ShopController : MonoBehaviour
    {
        [System.Serializable]
        private class ShopItemView
        {
            public string itemId = "skin";
            public int cost = 10;
            public GameObject? selectorHighlight;
            public GameObject? purchasedBadge;
            public GameObject? equippedBadge;
            public TMP_Text? label;
            public TMP_Text? costLabel;
        }

        [SerializeField] private ShopItemView[] items = System.Array.Empty<ShopItemView>();
        [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
        [SerializeField] private KeyCode moveRightKey = KeyCode.D;
        [SerializeField] private KeyCode purchaseKey = KeyCode.Return;
        [SerializeField] private KeyCode equipKey = KeyCode.E;
        [SerializeField] private TMP_Text? infoLabel;
        [SerializeField] private string menuSceneName = "New Game";
        [SerializeField] private KeyCode exitKey = KeyCode.Escape;
        [SerializeField, Tooltip("Parent layout RectTransform (e.g., the container with Grid/Horizontal/Vertical Layout). Optional but helps force layout before showing highlight.")] private RectTransform? layoutRoot;

        private int currentIndex;
        private float _forceTimer;

        private void OnEnable()
        {
            if (items == null || items.Length == 0)
            {
                return;
            }

            currentIndex = Mathf.Clamp(currentIndex, 0, items.Length - 1);
            EnsureDefaultOwnedAndEquipped();
            StartCoroutine(InitHighlight());
        }

        private void Start()
        {
            EnsureDefaultOwnedAndEquipped();
            for (var i = 0; i < items.Length; i++)
            {
                var owned = IsOwned(items[i].itemId);
                UpdateItemVisual(items[i], i == currentIndex, owned);
            }

            CoinWallet.CoinsChanged += HandleCoinsChanged;
            HandleCoinsChanged(CoinWallet.TotalCoins);
        }

        private void OnDestroy()
        {
            CoinWallet.CoinsChanged -= HandleCoinsChanged;
        }

        private void Update()
        {
            if (items.Length == 0)
            {
                return;
            }

            if (_forceTimer > 0f)
            {
                _forceTimer -= Time.unscaledDeltaTime;
                ForceSelectorVisible();
            }

            if (Input.GetKeyDown(moveLeftKey))
            {
                currentIndex = (currentIndex - 1 + items.Length) % items.Length;
                RefreshSelection();
            }
            else if (Input.GetKeyDown(moveRightKey))
            {
                currentIndex = (currentIndex + 1) % items.Length;
                RefreshSelection();
            }
            else if (Input.GetKeyDown(purchaseKey))
            {
                AttemptPurchase(items[currentIndex]);
            }
            else if (Input.GetKeyDown(equipKey))
            {
                EquipItem(items[currentIndex]);
            }
            else if (Input.GetKeyDown(exitKey))
            {
                if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName(menuSceneName).IsValid())
                {
                    if (!UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()))
                    {
                        // fall through
                    }
                }

                if (Application.CanStreamedLevelBeLoaded(menuSceneName))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
                }
                else
                {
                    DisplayMessage($"Scene '{menuSceneName}' missing");
                }
            }
        }

        private void RefreshSelection()
        {
            for (var i = 0; i < items.Length; i++)
            {
                var owned = IsOwned(items[i].itemId);
                UpdateItemVisual(items[i], i == currentIndex, owned);
            }
        }

        private void AttemptPurchase(ShopItemView item)
        {
            if (IsOwned(item.itemId))
            {
                DisplayMessage($"Already owned: {item.itemId}");
                return;
            }

            if (!CoinWallet.TrySpend(item.cost))
            {
                DisplayMessage("Not enough coins");
                return;
            }

            PlayerPrefs.SetInt(GetOwnedKey(item.itemId), 1);
            PlayerPrefs.Save();
            DisplayMessage($"Purchased {item.itemId}!");
            RefreshSelection();
        }

        private void EquipItem(ShopItemView item)
        {
            if (!IsOwned(item.itemId))
            {
                DisplayMessage("Buy it first");
                return;
            }

            PlayerPrefs.SetString("hellTiles_equippedSkin", item.itemId);
            PlayerPrefs.Save();
            DisplayMessage($"Equipped {item.itemId}");
            RefreshSelection();
        }

        private bool IsOwned(string itemId)
        {
            return PlayerPrefs.GetInt(GetOwnedKey(itemId), 0) == 1;
        }

        private static string GetOwnedKey(string itemId) => $"hellTiles_skin_{itemId}";

        private static bool IsEquipped(string itemId)
        {
            return PlayerPrefs.GetString("hellTiles_equippedSkin", string.Empty) == itemId;
        }

        private void UpdateItemVisual(ShopItemView view, bool isSelected, bool owned)
        {
            if (view.selectorHighlight != null)
            {
                view.selectorHighlight.SetActive(isSelected);
            }

            var isEquipped = IsEquipped(view.itemId);

            if (view.purchasedBadge != null)
            {
                view.purchasedBadge.SetActive(owned);
            }

            if (view.equippedBadge != null)
            {
                view.equippedBadge.SetActive(isEquipped);
            }

            if (view.label != null)
            {
                view.label.text = view.itemId;
            }

            if (view.costLabel != null)
            {
                view.costLabel.text = isEquipped
                    ? "Equipped"
                    : owned
                        ? "Owned"
                        : $"{view.cost} coins";
            }
        }

        private void ForceSelectorVisible()
        {
            if (items == null || items.Length == 0)
            {
                return;
            }

            var view = items[Mathf.Clamp(currentIndex, 0, items.Length - 1)];
            if (view.selectorHighlight != null)
            {
                // Toggle to force UI update in WebGL
                view.selectorHighlight.SetActive(false);
                view.selectorHighlight.SetActive(true);

                // Ensure RectTransform is properly anchored if it got messed up by layout
                var rt = view.selectorHighlight.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.localScale = Vector3.one;
                    // Optional: if you want it to fill the parent, reset anchors/offsets.
                    // Assuming the prefab is set up correctly, we might just want to ensure scale is 1.
                }

                var graphic = view.selectorHighlight.GetComponent<UnityEngine.UI.Graphic>();
                if (graphic != null)
                {
                    var c = graphic.color;
                    c.a = 1f;
                    graphic.color = c;
                    graphic.enabled = true;
                    graphic.SetAllDirty();

                    if (_forceTimer > 0.4f) // Log only on the first few frames of forcing
                    {
                         var rect = view.selectorHighlight.GetComponent<RectTransform>();
                         Debug.Log($"[ShopController] Selector Force: Active={view.selectorHighlight.activeInHierarchy}, " +
                                   $"Alpha={c.a}, Rect={rect.rect}, Scale={rect.localScale}, Pos={rect.anchoredPosition}");
                    }
                }
            }
        }

        private IEnumerator InitHighlight()
        {
            // Wait a small delay so layout groups finish positioning children (important for WebGL).
            // Use Realtime to avoid Time.timeScale issues.
            yield return new WaitForSecondsRealtime(0.1f);

            _forceTimer = 0.5f; // Force visibility for 0.5s
            Canvas.ForceUpdateCanvases();

            if (layoutRoot != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
            }
            else
            {
                Debug.LogWarning("[ShopController] layoutRoot is not assigned; layout rebuild might be skipped.");
            }

            RefreshSelection();
            ForceSelectorVisible();
        }

        private void EnsureDefaultOwnedAndEquipped()
        {
            if (items == null || items.Length == 0)
            {
                return;
            }

            var defaultId = items[0].itemId;
            // Own the first skin by default.
            PlayerPrefs.SetInt(GetOwnedKey(defaultId), 1);

            // If nothing is equipped yet, equip the default.
            var equipped = PlayerPrefs.GetString("hellTiles_equippedSkin", string.Empty);
            if (string.IsNullOrWhiteSpace(equipped))
            {
                PlayerPrefs.SetString("hellTiles_equippedSkin", defaultId);
            }

            PlayerPrefs.Save();
        }

        private void HandleCoinsChanged(int totalCoins)
        {
            if (infoLabel != null)
            {
                infoLabel.text = $"{totalCoins}";
            }
        }

        private void DisplayMessage(string message)
        {
            if (infoLabel != null)
            {
                infoLabel.text = message;
                CancelInvoke(nameof(RefreshInfoDisplay));
                Invoke(nameof(RefreshInfoDisplay), 1.5f);
            }
        }

        private void RefreshInfoDisplay()
        {
            HandleCoinsChanged(CoinWallet.TotalCoins);
        }
    }
}
