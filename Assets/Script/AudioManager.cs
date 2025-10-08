using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource SFXMusic;

    [Header("Background Music")]
    public AudioClip background;

    [Header("UI SFX")]
    public AudioClip buttonClick;

    [Header("Player SFX")]
    public AudioClip[] footstepSounds; // Array untuk variasi random
    public AudioClip jumpSound;
    public AudioClip landSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;
    [Range(0f, 0.2f)]
    public float pitchVariation = 0.1f;

    private void Start()
    {
        backgroundMusic.clip = background;
        backgroundMusic.Play();
        backgroundMusic.loop = true;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXMusic.PlayOneShot(clip);
    }

    public void PlayFootstep()
    {
        if (footstepSounds != null && footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            float randomPitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
            SFXMusic.pitch = randomPitch;
            SFXMusic.PlayOneShot(footstepSounds[randomIndex], footstepVolume);
            SFXMusic.pitch = 1f;
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            SFXMusic.PlayOneShot(jumpSound, footstepVolume);
        }
    }

    public void PlayLandSound()
    {
        if (landSound != null)
        {
            SFXMusic.PlayOneShot(landSound, footstepVolume);
        }
    }
}