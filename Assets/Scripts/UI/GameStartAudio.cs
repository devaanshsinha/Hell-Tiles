using System.Collections;
using UnityEngine;

#nullable enable

namespace HellTiles.UI
{
    /// <summary>
    /// Plays an audio clip once after an initial delay (e.g., after the countdown says GO).
    /// </summary>
    public class GameStartAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip? clip;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Tooltip("Delay before playing, to line up after any countdown.")] private float delay = 0f;
        [SerializeField, Tooltip("If true, creates a looping AudioSource instead of one-shot.")] private bool loop = false;
        private AudioSource? loopSource;

        private void OnEnable()
        {
            if (clip != null)
            {
                StartCoroutine(PlayAfterDelay());
            }
        }

        private IEnumerator PlayAfterDelay()
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            if (loop)
            {
                // Persist across scenes if looping music is desired.
                if (loopSource == null)
                {
                    var go = new GameObject("GameStartAudioLoop");
                    DontDestroyOnLoad(go);
                    loopSource = go.AddComponent<AudioSource>();
                    loopSource.clip = clip;
                    loopSource.volume = volume;
                    loopSource.loop = true;
                    loopSource.playOnAwake = false;
                }
                loopSource.Play();
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume);
            }
        }
    }
}
