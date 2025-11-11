namespace HellTiles.UI
{
    public static class GameSessionData
    {
        // Simple in-memory scoreboard for the current play session.
        public static int LastRunScore { get; private set; }
        public static int BestRunScore { get; private set; }

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
            }
        }

        public static void ResetCurrentRun()
        {
            LastRunScore = 0;
        }
    }
}
