using TMPro;
using UnityEngine;

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

        private int currentIndex;

        private void Start()
        {
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

        private void HandleCoinsChanged(int totalCoins)
        {
            if (infoLabel != null)
            {
                infoLabel.text = $"Coins: {totalCoins}";
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
