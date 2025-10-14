using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    // Singleton Pattern
    public static ObjectiveManager instance;

    public TextMeshProUGUI objectiveText; // Referensi UI Text
    public float completeMessageDuration = 2f; // Durasi pesan "Objective Complete!"

    private AudioSource audioSource;

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
        
        // Ambil komponen AudioSource jika ada
        audioSource = GetComponent<AudioSource>();
    }

    // FUNGSI BARU: Untuk menampilkan objektif baru
    public void SetObjective(string message)
    {
        StopAllCoroutines(); // Hentikan semua proses yang sedang berjalan
        objectiveText.color = Color.white; // Kembalikan warna ke normal
        objectiveText.text = message;
        objectiveText.gameObject.SetActive(true);
    }

    // FUNGSI BARU: Untuk menandai objektif selesai
    public void CompleteObjective()
    {
        if (objectiveText.gameObject.activeSelf)
        {
            StartCoroutine(ShowCompleteMessage());
        }
    }

    private IEnumerator ShowCompleteMessage()
    {

        // Tampilkan pesan selesai dengan warna hijau
        objectiveText.text = "Objective Complete!";
        objectiveText.color = Color.green;
        
        // Tunggu beberapa detik
        yield return new WaitForSeconds(completeMessageDuration);

        // Sembunyikan kembali teksnya
        objectiveText.gameObject.SetActive(false);
    }
}