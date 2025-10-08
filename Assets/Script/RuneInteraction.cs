using UnityEngine;

public class RuneInteraction : MonoBehaviour, IInteractable
{
    private bool playerInRange = false;
    public string interactionPrompt = "Press 'E' to collect the rune";
    public GateController gateController; 

    public void OnInteract()
    {
        Debug.Log("Interacted with the rune.");
        if (gateController != null && gameObject.activeInHierarchy)
        {
            gateController.AddRune();
        }
        else
        {
            Debug.LogWarning("GateController reference is missing!");
        }

        

        Destroy(gameObject); 
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
            Debug.Log(interactionPrompt);
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