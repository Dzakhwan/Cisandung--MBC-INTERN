using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    [TextArea(3, 5)]
    public string objectiveMessage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Panggil fungsi SetObjective dari ObjectiveManager
            ObjectiveManager.instance.SetObjective(objectiveMessage);
            Destroy(gameObject); // Hancurkan trigger agar tidak aktif lagi
        }
    }
}