using UnityEngine;

#nullable enable

namespace HellTiles.Audio
{
    /// <summary>
    /// Utility for playing 2D one-shot sounds that are not affected by distance.
    /// </summary>
    public static class OneShotAudio
    {
        public static void Play2D(AudioClip? clip, float volume = 1f)
        {
            if (clip == null || volume <= 0f)
            {
                return;
            }

            var go = new GameObject("OneShotAudio2D");
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 0f; // 2D sound
            source.playOnAwake = false;
            source.loop = false;
            source.Play();
            Object.Destroy(go, clip.length / Mathf.Max(0.01f, source.pitch));
        }
    }
}
