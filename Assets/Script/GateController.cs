using UnityEngine;

public class GateController : MonoBehaviour, IInteractable
{
    public int totalRunes = 3; // Jumlah rune yang harus dikumpulkan
    public int collectedRunes = 0;
    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void AddRune()
    {
        collectedRunes++;
        Debug.Log("Rune collected: " + collectedRunes + "/" + totalRunes);
        if (collectedRunes >= totalRunes)
        {
             ObjectiveManager.instance.CompleteObjective();
        }
    }

    private void OpenGate()
    {
        Debug.Log("Gate opened!");
        // Misal: nonaktifkan pintu, mainkan animasi, dsb
        anim.SetTrigger("Open");
        // Atau nonaktifkan collider pintu agar pemain bisa melewatinya
        GetComponents<Collider>()[1].enabled = false;
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
        Debug.Log(collectedRunes + " out of " + totalRunes + " runes collected.");
        if (collectedRunes == totalRunes)
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