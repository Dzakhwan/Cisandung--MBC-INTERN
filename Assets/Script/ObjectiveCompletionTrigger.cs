using UnityEngine;

public class ObjectiveCompletionTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Panggil fungsi CompleteObjective dari ObjectiveManager
            ObjectiveManager.instance.CompleteObjective();
            Destroy(gameObject); // Hancurkan trigger agar tidak aktif lagi
        }
    }
}