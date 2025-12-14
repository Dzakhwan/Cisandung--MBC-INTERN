using UnityEngine;
using TMPro;

public class CaptionSignalReceiver : MonoBehaviour
{
    public TextMeshProUGUI captionText;

    [TextArea]
    public string[] dialogs;

    public float duration = 6f;

    int index = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            ShowNextCaption();
    }


    public void ShowNextCaption()
    {
        if (index >= dialogs.Length) return;

        captionText.text = dialogs[index];
        captionText.gameObject.SetActive(true);

        CancelInvoke();
        Invoke(nameof(HideCaption), duration);

        index++;
    }

    public void HideCaption()
    {
        captionText.gameObject.SetActive(false);
    }
}
