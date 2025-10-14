using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource SFXMusic;
    [SerializeField] AudioSource ambientSFX;
    // [SerializeField] AudioSource randomAmbientPlayer;

    [Header("Background Music")]
    public AudioClip background;
    public AudioClip intenseBackground;

    [Header("UI SFX")]
    public AudioClip buttonClick;

    [Header("Player SFX")]
    public AudioClip[] footstepSounds; 
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip deathSound;
    [Header("Ambient SFX")]
    public AudioClip dogBarkingLoop;
    public AudioClip[] randomAmbientClips; 
    public float minTimeBetweenAmbient = 15f; 
    public float maxTimeBetweenAmbient = 30f;

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
        if (ambientSFX != null && randomAmbientClips.Length > 0)
        {
            StartCoroutine(PlayRandomAmbientSounds());
        }
    }

    private IEnumerator PlayRandomAmbientSounds()
    {

        while (true)
        {

            float waitTime = Random.Range(minTimeBetweenAmbient, maxTimeBetweenAmbient);
        
            yield return new WaitForSeconds(waitTime);

            if (randomAmbientClips.Length > 0)
            {
                int randomIndex = Random.Range(0, randomAmbientClips.Length);
                AudioClip clipToPlay = randomAmbientClips[randomIndex];
                if (clipToPlay != null)
                {
                    ambientSFX.PlayOneShot(clipToPlay);
                }
            }
        }
    }

    public void ChangeBGM()
    {
        StopCoroutine("PlayRandomAmbientSounds");
        backgroundMusic.Stop();
        backgroundMusic.clip = intenseBackground;
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

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            SFXMusic.PlayOneShot(deathSound, footstepVolume);
        }
    }
    public void PlayDogBarking()
    {
        if (dogBarkingLoop != null && !ambientSFX.isPlaying)
        {
            ambientSFX.clip = dogBarkingLoop;
            ambientSFX.loop = true; 
            ambientSFX.Play();
            Debug.Log("Dog barking started.");
        }
    }

    public void StopDogBarking()
    {
        if (ambientSFX.isPlaying && ambientSFX.clip == dogBarkingLoop)
        {
            ambientSFX.Stop();
            ambientSFX.clip = null;
            Debug.Log("Dog barking stopped.");
        }
    }

}