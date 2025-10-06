using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource SFXMusic;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip buttonClick;

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



}
