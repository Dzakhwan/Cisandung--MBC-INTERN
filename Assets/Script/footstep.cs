using UnityEngine;

public class footstep : MonoBehaviour
{
    private AudioManager audioManager;
    private void Awake()
    {
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<AudioManager>();
        }
    }
    public void PlayFootstep()
    {
        if (audioManager != null )
        {
            audioManager.PlayFootstep();
        }
    }
}
