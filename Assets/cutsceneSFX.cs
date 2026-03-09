using UnityEngine;

namespace Dubinci
{
    public class cutsceneSFX : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxSource;

        // This function will be visible in the Animation Event dropdown
        public void PlaySoundEffect(AudioClip clip)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip); // Uses logic similar to sceneTransition
            }
        }
    }
}