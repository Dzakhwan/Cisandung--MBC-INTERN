using UnityEngine;

public class GateController : MonoBehaviour, IInteractable
{
    public int totalRunes = 3; // Jumlah rune yang harus dikumpulkan
    private int collectedRunes = 0;

    public void AddRune()
    {
        collectedRunes++;
        Debug.Log("Rune collected: " + collectedRunes + "/" + totalRunes);


    }

    private void OpenGate()
    {
        Debug.Log("Gate opened!");
        // Misal: nonaktifkan pintu, mainkan animasi, dsb
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is near the gate. Press 'E' to interact.");
        }
    }


    public void OnInteract()
    {
        Debug.Log("Interacted with the gate.");
        if (collectedRunes >= totalRunes)
        {
            OpenGate();
        }
        else
        {
            Debug.Log("You need more runes to open the gate.");
        }
    }
    public void OnInteractExit()
    {
        Debug.Log("Stopped interacting with the gate.");
    }
}