using UnityEngine;
using TMPro; // Wajib ditambahkan untuk menggunakan TextMeshPro
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    // Singleton Pattern: Membuat skrip ini mudah diakses dari mana saja
    public static TutorialManager instance;

    public TextMeshProUGUI objectiveText; // Referensi ke komponen teks di UI
    private Coroutine currentCoroutine = null;

    private void Awake()
    {
        // Setup Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Sembunyikan teks saat game dimulai
        if (objectiveText != null)
        {
            objectiveText.gameObject.SetActive(false);
        }
    }

    // Fungsi utama yang akan kita panggil dari skrip lain
    public void ShowObjective(string message, float duration)
    {
        // Jika sudah ada tutorial yang berjalan, hentikan dulu
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        
        // Mulai coroutine baru untuk menampilkan pesan
        currentCoroutine = StartCoroutine(ShowObjectiveCoroutine(message, duration));
    }

    private IEnumerator ShowObjectiveCoroutine(string message, float duration)
    {
        // 1. Tampilkan teks
        objectiveText.text = message;
        objectiveText.gameObject.SetActive(true);

        // 2. Tunggu selama durasi yang ditentukan
        yield return new WaitForSeconds(duration);

        // 3. Sembunyikan kembali teksnya
        objectiveText.gameObject.SetActive(false);
        currentCoroutine = null;
    }
}