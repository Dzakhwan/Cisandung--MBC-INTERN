using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    [TextArea(3, 5)]
    public string objectiveMessage;

    [Tooltip("Set to 1 for a single objective. Set to >1 for a multi-item objective.")]
    public int requiredAmount = 1; // Defaultnya 1 untuk objektif tunggal

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        // Buat objektif baru menggunakan class Objective.
        Objective newObjective = new Objective(objectiveMessage, requiredAmount);
        ObjectiveManager.instance.SetObjective(newObjective);
        Destroy(gameObject);
    }
}
}