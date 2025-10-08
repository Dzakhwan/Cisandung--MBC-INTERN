using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public float chaseDistance = 10f;

    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (agent != null && agent.isOnNavMesh && player != null)
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
}