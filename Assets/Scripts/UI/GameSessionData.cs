namespace HellTiles.UI
{
    public static class GameSessionData
    {
        // Simple in-memory scoreboard for the current play session.
        public static float LastRunDuration { get; private set; }
        public static float BestRunDuration { get; private set; }

        public static void RegisterRun(float durationSeconds)
        {
            if (durationSeconds < 0f)
            {
                durationSeconds = 0f;
            }

            LastRunDuration = durationSeconds; // store latest attempt
            if (durationSeconds > BestRunDuration)
            {
                BestRunDuration = durationSeconds;
            }
        }

        public static void ResetCurrentRun()
        {
            LastRunDuration = 0f;
        }
    }
}
