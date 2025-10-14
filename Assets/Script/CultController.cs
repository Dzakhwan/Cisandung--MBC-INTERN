using UnityEngine;
using UnityEngine.AI;

public class CultController : MonoBehaviour
{
     public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public float chaseDistance = 10f;

    private bool isChasing = false;
    Vector3 StartPosition;
    void Start()
    {
        StartPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    
    }

    void Update()
    {
        if (agent != null && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= chaseDistance)
            {
                agent.SetDestination(player.position);
                if (!isChasing)
                {
                    anim.SetBool("IsRunning", true);
                    Debug.Log("Chasing player");
                    isChasing = true;
                }
            }
            else
            {
                agent.ResetPath();
                if (isChasing)
                {
                    anim.SetBool("IsRunning", false);
                    isChasing = false;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }
    
    void Die()
    {
        Debug.Log("Player Died");
        Respawn();
    }
    void Respawn()
    {
       transform.position = StartPosition;
    }
}
