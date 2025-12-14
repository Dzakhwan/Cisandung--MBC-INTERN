using UnityEngine;
using TMPro;

public class SimpleCaption : MonoBehaviour
{
    public TextMeshProUGUI captionText;

    public void Show(string text, float duration)
    {
        captionText.text = text;
        captionText.gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Hide), duration);
    }

    public void Hide()
    {
        captionText.gameObject.SetActive(false);
    }
}
