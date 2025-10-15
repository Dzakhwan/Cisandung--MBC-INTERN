using UnityEngine;
using UnityEngine.Events; // <-- Tambahkan ini!

public class CutsceneManager : MonoBehaviour
{
    public GameObject[] panels;
    private int currentPanelIndex = 0;

    // Event yang akan dipicu saat cutscene selesai
    public UnityEvent OnCutsceneFinished; // <-- Tambahkan ini!

    // Fungsi untuk memulai cutscene dari awal
    public void StartCutscene()
    {
        currentPanelIndex = 0;
        gameObject.SetActive(true);
        ShowPanel(0);
    }

    public void NextPanel()
    {
        currentPanelIndex++;

        if (currentPanelIndex < panels.Length)
        {
            ShowPanel(currentPanelIndex);
        }
        else
        {
            // Cutscene selesai! Panggil event-nya.
            Debug.Log(gameObject.name + " selesai!");
            OnCutsceneFinished.Invoke(); // <-- Tambahkan ini!
            gameObject.SetActive(false); // Nonaktifkan container cutscene
        }
    }

    void ShowPanel(int panelIndexToShow)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == panelIndexToShow);
        }
    }
}