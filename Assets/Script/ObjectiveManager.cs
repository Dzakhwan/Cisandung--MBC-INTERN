using UnityEngine;
using TMPro;
using System.Collections;

// Class ini sekarang menjadi struktur data murni untuk menyimpan informasi objektif.
// Ini membuatnya lebih modular dan mudah digunakan di seluruh proyek.
[System.Serializable]
public class Objective
{
    public string description; // Deskripsi dari objektif, contoh: "Kumpulkan Rune"
    public int requiredAmount; // Jumlah yang dibutuhkan untuk selesai
    public int currentAmount; // Jumlah saat ini yang sudah dikumpulkan
    public bool isCompleted;

    // Constructor untuk objektif dengan progres (misal: 0/3)
    public Objective(string desc, int current, int required)
    {
        description = desc;
        currentAmount = current;
        requiredAmount = required;
        isCompleted = false;
    }

    // Constructor untuk objektif tunggal (misal: "Buka Gerbang")
    public Objective(string desc, int required = 1)
    {
        description = desc;
        currentAmount = 0;
        requiredAmount = required;
        isCompleted = false;
    }
}

public class ObjectiveManager : MonoBehaviour
{
    // Singleton Pattern untuk akses mudah dari skrip lain
    public static ObjectiveManager instance;

    public TextMeshProUGUI objectiveText; // Referensi ke UI TextMeshPro
    public float completeMessageDuration = 2f; // Durasi pesan "Objective Complete!"
    public AudioClip completeSound; // (Opsional) Suara saat objektif selesai

    private Objective currentObjective;
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
            return;
        }

        // Sembunyikan teks UI saat game dimulai untuk menghindari tampilan yang tidak perlu
        if (objectiveText != null)
        {
            objectiveText.gameObject.SetActive(false);
        }

        // Ambil komponen AudioSource jika ada pada GameObject ini
        audioSource = GetComponent<AudioSource>();
    }

    // Fungsi utama untuk menetapkan objektif baru
    public void SetObjective(Objective newObjective)
    {
        currentObjective = newObjective;
        StopAllCoroutines(); // Hentikan coroutine sebelumnya agar tidak tumpang tindih
        UpdateObjectiveText(); // Perbarui tampilan UI
        objectiveText.gameObject.SetActive(true); // Tampilkan UI
    }

    // Fungsi untuk memperbarui progres objektif saat ini
    public void UpdateObjectiveProgress(int amount)
    {
        if (currentObjective == null || currentObjective.isCompleted) return;

        currentObjective.currentAmount = amount;
        UpdateObjectiveText(); // Perbarui teks UI sesuai progres baru

        // Cek apakah objektif sudah selesai
        if (currentObjective.currentAmount >= currentObjective.requiredAmount)
        {
            CompleteObjective();
        }
    }

    // Memperbarui teks pada UI berdasarkan status objektif saat ini
    private void UpdateObjectiveText()
    {
        if (currentObjective == null) return;

        string displayText;
        // Jika requiredAmount > 1, tampilkan format progres (contoh: "Kumpulkan Rune: 1/3")
        if (currentObjective.requiredAmount > 1)
        {
            displayText = $"{currentObjective.description}: {currentObjective.currentAmount}/{currentObjective.requiredAmount}";
        }
        else // Jika tidak, tampilkan deskripsi saja (contoh: "Temukan jalan keluar")
        {
            displayText = currentObjective.description;
        }

        objectiveText.color = Color.white; // Pastikan warna teks kembali normal
        objectiveText.text = displayText;
    }

    // Menandai objektif saat ini sebagai selesai
    public void CompleteObjective()
    {
        if (currentObjective == null || currentObjective.isCompleted) return;
        
        currentObjective.isCompleted = true;

        if (objectiveText.gameObject.activeSelf)
        {
            StartCoroutine(ShowCompleteMessage());
        }

        // Mainkan suara jika ada
        if (audioSource != null && completeSound != null)
        {
            audioSource.PlayOneShot(completeSound);
        }
    }

    // Coroutine untuk menampilkan pesan "Objective Complete!" sementara
    private IEnumerator ShowCompleteMessage()
    {
        objectiveText.text = "Objective Complete!";
        objectiveText.color = Color.green; // Ubah warna untuk menandakan keberhasilan

        yield return new WaitForSeconds(completeMessageDuration);

        // Sembunyikan kembali teks setelah durasi selesai
        objectiveText.gameObject.SetActive(false);
    }
}