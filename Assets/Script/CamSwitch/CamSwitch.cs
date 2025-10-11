using UnityEngine;
using Unity.Cinemachine;
using UnityEditor;

public class CamSwitch : MonoBehaviour
{
    public CinemachineCamera PrimaryVcam;
    public CinemachineCamera[] vcams;
    public string tag;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    void Start()
    {
        SwitchCam(PrimaryVcam);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tag))
        {
            CinemachineCamera targetVcam = other.GetComponentInChildren<CinemachineCamera>();
            if (targetVcam != null)
            {
                SwitchCam(targetVcam);
                Debug.Log("Switching to " + targetVcam.name);
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tag))
        {
            SwitchCam(PrimaryVcam);
        }
    }

    // Update is called once per frame
    private void SwitchCam(CinemachineCamera targetVcam)
    {
        foreach (CinemachineCamera vcam in vcams)
        {
            vcam.enabled = vcam == targetVcam;
        }
    }

    [ContextMenu("Get Vcams")]
    private void GetVcams()
    {
        vcams = GameObject.FindObjectsOfType<CinemachineCamera>();
    }

    
}
