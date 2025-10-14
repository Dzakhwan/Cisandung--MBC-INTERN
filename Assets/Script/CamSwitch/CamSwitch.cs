using UnityEngine;
using Unity.Cinemachine;
using UnityEditor;
using System.Collections;

public class CamSwitch : MonoBehaviour
{
    public CinemachineCamera PrimaryVcam;
    public CinemachineCamera[] vcams;
    public PlayerController player;
    public CultController[] cults;
    public GameObject ColliderVcam2;
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
            if (vcams[1].enabled == true)
            {

                StartCoroutine(DisableMovementForSeconds(3f));
                StartCoroutine(DisableCultMovementForSeconds(3f));

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

    IEnumerator DisableMovementForSeconds(float seconds)
    {
        player.canMove = false;
        yield return new WaitForSeconds(seconds);
        player.canMove = true;
        ColliderVcam2.SetActive(false);
    
        
        
    }
    IEnumerator DisableCultMovementForSeconds(float seconds)
    {
      yield return new WaitForSeconds(seconds);
    foreach (var cult in cults)
    {
        if (cult != null)
            cult.enabled = true;
    }
    }

    
}
