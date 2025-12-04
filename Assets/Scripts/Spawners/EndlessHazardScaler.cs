using UnityEngine;
using HellTiles.Hazards;
using HellTiles.Powerups;

#nullable enable

namespace HellTiles.Spawners
{
    /// <summary>
    /// Scales hazard spawn intervals over time for endless mode. Leave out of level scenes.
    /// </summary>
    public class EndlessHazardScaler : MonoBehaviour
    {
        [Header("Targets (optional)")]
        [SerializeField] private SpikeSpawner? spikeSpawner;
        [SerializeField] private PushTileSpawner? pushSpawner;
        [SerializeField] private CrackedTileSpawner? crackedSpawner;
        [SerializeField] private HeartSpawner? heartSpawner;
        [SerializeField] private CoinSpawner? coinSpawner;

        [Header("Ramp Settings")]
        [SerializeField, Tooltip("Multiplier at t=0 (1 = no change).")] private float startMultiplier = 1f;
        [SerializeField, Tooltip("Multiplier at t=rampDuration.")] private float endMultiplier = 0.5f;
        [SerializeField, Tooltip("Seconds to reach end multiplier.")] private float rampDuration = 120f;

        private float startTime;

        // Cache base intervals
        private float baseSpikeInterval;
        private float basePushInterval;
        private float baseCrackedInterval;
        private float baseHeartInterval;
        private float baseCoinInterval;

        private void Awake()
        {
            startTime = Time.time;

            if (spikeSpawner != null) baseSpikeInterval = GetSpawnInterval(spikeSpawner.SpawnInterval);
            if (pushSpawner != null) basePushInterval = GetSpawnInterval(pushSpawner.SpawnInterval);
            if (crackedSpawner != null) baseCrackedInterval = GetSpawnInterval(crackedSpawner.SpawnInterval);
            if (heartSpawner != null) baseHeartInterval = GetSpawnInterval(heartSpawner.SpawnInterval);
            if (coinSpawner != null) baseCoinInterval = GetSpawnInterval(coinSpawner.SpawnInterval);
        }

        private void Update()
        {
            var mult = CurrentMultiplier();

            if (spikeSpawner != null) spikeSpawner.SpawnInterval = baseSpikeInterval * mult;
            if (pushSpawner != null) pushSpawner.SpawnInterval = basePushInterval * mult;
            if (crackedSpawner != null) crackedSpawner.SpawnInterval = baseCrackedInterval * mult;
            if (heartSpawner != null) heartSpawner.SpawnInterval = baseHeartInterval * mult;
            if (coinSpawner != null) coinSpawner.SpawnInterval = baseCoinInterval * mult;
        }

        private float CurrentMultiplier()
        {
            if (rampDuration <= 0f) return endMultiplier;
            var t = Mathf.Clamp01((Time.time - startTime) / rampDuration);
            return Mathf.Lerp(startMultiplier, endMultiplier, t);
        }

        private static float GetSpawnInterval(float value) => Mathf.Max(0.01f, value);
    }
}
