using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace Dubinci
{
    public class sceneTransition : MonoBehaviour
    {
        [Header("Transition Settings")]
        [Tooltip("If true, load the next scene in Build Settings")]
        [SerializeField] private bool justAdvance;
        [Tooltip("Index of the specific scene to jump to")]
        [SerializeField] private int sceneNumber;
        [SerializeField] private float delay = 1.0f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip transitionSound;
        [SerializeField] private AudioSource sfxSource;
        [Tooltip("List of AudioSources (music, ambient) to fade out during transition")]
        [SerializeField] private List<AudioSource> sourcesToFade;

        public void Transition()
        {
            StartCoroutine(TransitionRoutine());
        }

        private IEnumerator TransitionRoutine()
        {
            // 1. Play the transition sound effect
            if (sfxSource != null && transitionSound != null)
            {
                sfxSource.PlayOneShot(transitionSound);
            }

            // 2. Handle Fade Out for all listed sources
            float timer = 0;
            // Store initial volumes to fade out correctly
            Dictionary<AudioSource, float> initialVolumes = new Dictionary<AudioSource, float>();

            foreach (var src in sourcesToFade)
            {
                if (src != null) initialVolumes[src] = src.volume;
            }

            while (timer < delay)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / delay;

                foreach (var src in sourcesToFade)
                {
                    if (src != null && initialVolumes.ContainsKey(src))
                    {
                        // Linearly interpolate volume towards zero
                        src.volume = Mathf.Lerp(initialVolumes[src], 0, normalizedTime);
                    }
                }
                yield return null;
            }

            // 3. Determine and load the target scene
            if (justAdvance)
            {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.LogWarning("[SceneTransition] No next scene in Build Settings!");
                }
            }
            else
            {
                SceneManager.LoadScene(sceneNumber);
            }
        }

        public void goToNewScene() => Transition();
    }
}