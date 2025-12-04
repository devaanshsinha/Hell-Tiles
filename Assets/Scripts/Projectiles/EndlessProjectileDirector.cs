using System.Collections;
using UnityEngine;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Projectile director variant for endless mode with interval ramping.
    /// Does not touch other directors/tracks used by level scenes.
    /// </summary>
    public class EndlessProjectileDirector : MonoBehaviour
    {
        [System.Serializable]
        private class TrackConfig
        {
            public string label = "Track";
            public MonoBehaviour? trackBehaviour;
            public bool enabled = true;
            public float initialDelay = 0f;
            public float baseSpawnInterval = 2f;
        }

        [Header("Tracks")]
        [SerializeField] private TrackConfig[] tracks = System.Array.Empty<TrackConfig>();

        [Header("Difficulty Ramp")]
        [SerializeField, Tooltip("Multiplier at t=0 (1 = no change).")] private float startIntervalMultiplier = 1f;
        [SerializeField, Tooltip("Multiplier at t=rampDuration (e.g., 0.5 halves the interval).")] private float endIntervalMultiplier = 0.5f;
        [SerializeField, Tooltip("Seconds to reach end multiplier. Set to 0 for immediate.")] private float rampDuration = 120f;

        private readonly System.Collections.Generic.Dictionary<TrackConfig, Coroutine> activeCoroutines = new();
        private float startTime;

        private void OnEnable()
        {
            startTime = Time.time;

            foreach (var config in tracks)
            {
                if (config.trackBehaviour is IProjectileTrack track && config.enabled)
                {
                    var routine = StartCoroutine(RunTrack(config, track));
                    activeCoroutines[config] = routine;
                }
                else if (config.trackBehaviour == null)
                {
                    Debug.LogWarning($"EndlessProjectileDirector missing track reference for '{config.label}'.", this);
                }
            }
        }

        private void OnDisable()
        {
            foreach (var pair in activeCoroutines)
            {
                if (pair.Value != null)
                {
                    StopCoroutine(pair.Value);
                }
            }

            activeCoroutines.Clear();
        }

        private IEnumerator RunTrack(TrackConfig config, IProjectileTrack track)
        {
            if (config.initialDelay > 0f)
            {
                yield return new WaitForSeconds(config.initialDelay);
            }

            while (config.enabled)
            {
                track.SpawnProjectile();

                var interval = Mathf.Max(0.01f, config.baseSpawnInterval * CurrentMultiplier());
                yield return new WaitForSeconds(interval);
            }
        }

        private float CurrentMultiplier()
        {
            if (rampDuration <= 0f)
            {
                return endIntervalMultiplier;
            }

            var t = Mathf.Clamp01((Time.time - startTime) / rampDuration);
            return Mathf.Lerp(startIntervalMultiplier, endIntervalMultiplier, t);
        }
    }
}
