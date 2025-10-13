using UnityEngine;

public class LevelController : MonoBehaviour,IInteractable
{
    public Animator anim;
    public bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract()
    {
         if (isActive == true)
         {
             anim.SetTrigger("IsUp");
             isActive = false;
         }
         else
        {
            anim.SetTrigger("IsDown");
            isActive = true;
             
         }
    }

    public void OnInteractExit()
    {
        throw new System.NotImplementedException();
    }
}
