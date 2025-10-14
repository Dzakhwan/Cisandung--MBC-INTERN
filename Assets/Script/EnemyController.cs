using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
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
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError($"{name} tidak berada di area NavMesh!");
            }
        }
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
