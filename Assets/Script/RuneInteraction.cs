using UnityEngine;

public class RuneInteraction : MonoBehaviour, IInteractable
{
    private bool playerInRange = false;

    public void OnInteract()
    {
        Debug.Log("Interacted with the rune.");
        // Tambahkan logika memasukkan ke inventory di sini
        Destroy(gameObject); // Contoh: rune diambil
    }

    public void OnInteractExit()
    {
        Debug.Log("Stopped interacting with the rune.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public bool IsPlayerInRange()
    {
        return playerInRange;
    }
}