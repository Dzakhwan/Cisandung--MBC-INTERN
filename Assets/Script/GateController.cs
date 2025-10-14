using UnityEngine;

public class GateController : MonoBehaviour, IInteractable
{
    public int totalRunes = 3; // Jumlah rune yang harus dikumpulkan
    public int collectedRunes = 0;
    private Animator anim;
    private bool objectiveStarted = false;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void AddRune()
    {
        collectedRunes++;
        Debug.Log("Rune collected: " + collectedRunes + "/" + totalRunes);
        if (objectiveStarted)
        {
            // Cukup kirim jumlah saat ini
            ObjectiveManager.instance.UpdateObjectiveProgress(collectedRunes);
        }

        // Pengecekan penyelesaian objektif sekarang ditangani oleh ObjectiveManager,
        // jadi baris CompleteObjective() di sini bisa dihapus untuk menghindari panggilan ganda.
    }
    private void OpenGate()
    {
        Debug.Log("Gate Terbuka!");
        // Misal: nonaktifkan pintu, mainkan animasi, dsb
        anim.SetTrigger("Open");
        // Atau nonaktifkan collider pintu agar pemain bisa melewatinya
        GetComponents<Collider>()[1].enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !objectiveStarted)
        {
            objectiveStarted = true;
            // Membuat objektif baru menggunakan class Objective
            string message = "Kumpulkan Rune untuk membuka gerbang";
            Objective newObjective = new Objective(message, collectedRunes, totalRunes);
            ObjectiveManager.instance.SetObjective(newObjective);
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
            TutorialManager.instance.ShowObjective("You need more runes to open the gate.",1);
        }
    }
    public void OnInteractExit()
    {
        Debug.Log("Stopped interacting with the gate.");
    }
}