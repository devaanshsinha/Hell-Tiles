using TMPro;
using UnityEngine;

namespace HellTiles.UI
{
    /// <summary>
    /// Displays the player's total coin count.
    /// </summary>
    /// <summary>
    /// Updates a TMP label whenever the wallet total changes.
    /// </summary>
    public class CoinCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text counterText = default!;
        [SerializeField] private string suffix = " coins";

        private void Start()
        {
            Refresh();
            CoinWallet.CoinsChanged += HandleCoinsChanged;
        }

        private void OnDestroy()
        {
            CoinWallet.CoinsChanged -= HandleCoinsChanged;
        }

        private void HandleCoinsChanged(int totalCoins)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (counterText == null)
            {
                return;
            }

            counterText.text = CoinWallet.TotalCoins.ToString() + suffix; // e.g., "12 coins"
        }
    }
}
