using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace HellTiles.Projectiles
{
    /// <summary>
    /// Coordinates multiple projectile tracks and throttles spawn rates.
    /// </summary>
    public class ProjectileDirector : MonoBehaviour
    {
        [System.Serializable]
        private class TrackConfig
        {
            public string label = "Track";
            public MonoBehaviour? trackBehaviour;
            public bool enabled = true;
            public float initialDelay = 0f;
            public float spawnInterval = 2f;
        }

        [SerializeField] private int maxActiveProjectiles = 50;
        [SerializeField] private TrackConfig[] tracks = new TrackConfig[0]; // configure in Inspector

        private readonly Dictionary<TrackConfig, Coroutine> activeCoroutines = new();

        private void OnEnable()
        {
            ProjectileRegistry.MaxActiveProjectiles = maxActiveProjectiles; // share cap globally
            foreach (var config in tracks)
            {
                if (config.trackBehaviour is IProjectileTrack track && config.enabled)
                {
                    var routine = StartCoroutine(RunTrack(config, track));
                    activeCoroutines[config] = routine;
                }
                else if (config.trackBehaviour == null)
                {
                    Debug.LogWarning($"ProjectileDirector missing track reference for '{config.label}'.", this);
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
            ProjectileRegistry.MaxActiveProjectiles = int.MaxValue;
        }

        private IEnumerator RunTrack(TrackConfig config, IProjectileTrack track)
        {
            if (config.initialDelay > 0f)
            {
                yield return new WaitForSeconds(config.initialDelay);
            }

            var wait = new WaitForSeconds(Mathf.Max(0.01f, config.spawnInterval));
            while (config.enabled)
            {
                if (ProjectileRegistry.CanSpawn)
                {
                    track.SpawnProjectile();
                }

                yield return wait;
            }
        }
    }
}
