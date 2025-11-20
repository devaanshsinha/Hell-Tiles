using UnityEngine;

#nullable enable

namespace HellTiles.UI
{
    /// <summary>
    /// Simple persistent storage for total coins earned across runs.
    /// </summary>
    public static class CoinWallet
    {
        private const string CoinsKey = "hellTiles_totalCoins";
        public static int TotalCoins { get; private set; }

        public static event System.Action<int>? CoinsChanged;

        static CoinWallet()
        {
            TotalCoins = PlayerPrefs.GetInt(CoinsKey, 0);
        }

        public static void AddCoins(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            TotalCoins += amount; // update running total
            PlayerPrefs.SetInt(CoinsKey, TotalCoins);
            PlayerPrefs.Save();
            CoinsChanged?.Invoke(TotalCoins);
        }

        public static void ResetCoins()
        {
            TotalCoins = 0; // only use for debugging or wipes
            PlayerPrefs.SetInt(CoinsKey, TotalCoins);
            PlayerPrefs.Save();
            CoinsChanged?.Invoke(TotalCoins);
        }

        public static bool TrySpend(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (TotalCoins < amount)
            {
                return false;
            }

            TotalCoins -= amount;
            PlayerPrefs.SetInt(CoinsKey, TotalCoins);
            PlayerPrefs.Save();
            CoinsChanged?.Invoke(TotalCoins);
            return true;
        }
    }
}
