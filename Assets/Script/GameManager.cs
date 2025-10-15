using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // Hubungkan objek-objek ini dari Hirarki ke Inspector
    public CutsceneManager introCutscene;
    public CutsceneManager outroCutscene;
    public GameObject gameplayElements; // Objek "Gameplay"

    void Start()
    {
        // Saat game dimulai:
        // 1. Sembunyikan gameplay
        gameplayElements.SetActive(false);
        // 2. Sembunyikan cutscene akhir
        outroCutscene.gameObject.SetActive(false);

        // 3. Mulai cutscene awal
        introCutscene.StartCutscene();

        // 4. Dengarkan event 'OnCutsceneFinished' dari introCutscene
        // Kita akan menghubungkan ini melalui Inspector, cara yang lebih mudah!
    }

    // Fungsi ini akan dipanggil saat intro selesai
    public void StartGameplay()
    {
        Debug.Log("Intro selesai, permainan dimulai!");
        gameplayElements.SetActive(true);
    }

    // Fungsi ini akan dipanggil untuk memicu cutscene akhir
    // Panggil fungsi ini dari skrip lain, misal saat boss terakhir kalah
    public void TriggerOutroCutscene()
    {
        Debug.Log("Permainan selesai, memutar cutscene akhir!");
        gameplayElements.SetActive(false);
        outroCutscene.StartCutscene();
    }
    public void EndGameplay()
    {
        Debug.Log("Permainan berakhir, kembali ke menu utama!");
        SceneManager.LoadScene("Main Menu");
    }
}