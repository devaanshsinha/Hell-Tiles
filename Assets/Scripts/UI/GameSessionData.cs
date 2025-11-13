using UnityEngine;

namespace HellTiles.UI
{
    public static class GameSessionData
    {
        // Simple in-memory scoreboard for the current play session.
        private const string BestScoreKey = "hellTiles_bestScore";

        public static int LastRunScore { get; private set; }
        public static int BestRunScore { get; private set; } = PlayerPrefs.GetInt(BestScoreKey, 0);

        public static void RegisterScore(int score)
        {
            if (score < 0)
            {
                score = 0;
            }

            LastRunScore = score; // store latest attempt
            if (score > BestRunScore)
            {
                BestRunScore = score;
                PlayerPrefs.SetInt(BestScoreKey, BestRunScore);
                PlayerPrefs.Save();
            }
        }

        public static void ResetCurrentRun()
        {
            LastRunScore = 0;
        }
    }
}
