using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TypingSoundManager : MonoBehaviour, ITypingSoundProvider
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] charClips;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failClip;

    [Header("Randomization")]
    [Range(0f, 0.2f)][SerializeField] private float pitchVariation = 0.05f;

    private AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayCharSound(char c)
    {
        if (charClips == null || charClips.Length == 0) return;

        // Pick a random clip from the array for variety
        AudioClip clip = charClips[Random.Range(0, charClips.Length)];

        // Randomize pitch slightly to avoid robotic sound
        _source.pitch = 1.0f + Random.Range(-pitchVariation, pitchVariation);
        _source.PlayOneShot(clip);
    }

    public void PlaySuccess()
    {
        _source.pitch = 1.0f;
        if (successClip) _source.PlayOneShot(successClip);
    }

    public void PlayFail()
    {
        _source.pitch = 1.0f;
        if (failClip) _source.PlayOneShot(failClip);
    }
}