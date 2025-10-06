using UnityEngine;

public class GateController : MonoBehaviour
{
    public int totalRunes = 3; // Jumlah rune yang harus dikumpulkan
    private int collectedRunes = 0;

    public void AddRune()
    {
        collectedRunes++;
        Debug.Log("Rune collected: " + collectedRunes + "/" + totalRunes);

        if (collectedRunes >= totalRunes)
        {
            OpenGate();
        }
    }

    private void OpenGate()
    {
        Debug.Log("Gate opened!");
        // Misal: nonaktifkan pintu, mainkan animasi, dsb
        gameObject.SetActive(false);
    }
}