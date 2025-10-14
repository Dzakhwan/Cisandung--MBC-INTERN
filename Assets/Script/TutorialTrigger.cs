using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [TextArea(3, 5)] // Membuat kolom teks lebih besar di Inspector
    public string objectiveMessage; // Pesan yang ingin ditampilkan
    public float displayDuration = 3f; // Berapa lama pesan ditampilkan

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah yang masuk adalah pemain (pastikan objek player punya tag "Player")
        if (other.CompareTag("Player"))
        {
            // Panggil fungsi ShowObjective dari TutorialManager
            TutorialManager.instance.ShowObjective(objectiveMessage, displayDuration);

            // Hancurkan trigger ini agar tidak muncul lagi
            Destroy(gameObject);
        }
    }
}