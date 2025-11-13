using UnityEngine;

#nullable enable

namespace HellTiles.UI
{
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

            TotalCoins += amount;
            PlayerPrefs.SetInt(CoinsKey, TotalCoins);
            PlayerPrefs.Save();
            CoinsChanged?.Invoke(TotalCoins);
        }

        public static void ResetCoins()
        {
            TotalCoins = 0;
            PlayerPrefs.SetInt(CoinsKey, TotalCoins);
            PlayerPrefs.Save();
            CoinsChanged?.Invoke(TotalCoins);
        }
    }
}
