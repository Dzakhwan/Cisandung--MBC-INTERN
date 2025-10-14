using UnityEngine;
using UnityEngine.UI;

public class Koran : MonoBehaviour, IInteractable
{
    public GameObject koranUI;

    public void OnInteract()
    {
        koranUI.SetActive(true);
    }

    public void OnInteractExit()
    {
        koranUI.SetActive(false);
    }
}
