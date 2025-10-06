using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent != null && agent.isOnNavMesh && player!= null)
        {
            agent.SetDestination(player.position);
        }
        
    }
}
