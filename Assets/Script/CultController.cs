using UnityEngine;
using UnityEngine.AI;

public class CultController : MonoBehaviour
{
     public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    private Vector3 startPosition;
    
    public float chaseDistance = 10f;

    private bool isChasing = false;

    void Start()
    {
        // Simpan posisi awal
        startPosition = transform.position;

        // Ambil komponen
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        // Cari player berdasarkan tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player tidak ditemukan! Pastikan GameObject Player memiliki tag 'Player'");
        }

        // Pastikan enemy ada di NavMesh
        if (agent != null && !agent.isOnNavMesh)
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

        // Pastikan parameter agent tidak 0 agar tidak error di WebGL
        if (agent != null)
        {
            if (agent.speed <= 0) agent.speed = 3.5f;
            if (agent.acceleration <= 0) agent.acceleration = 8f;
            if (agent.angularSpeed <= 0) agent.angularSpeed = 120f;
        }
    }

    void Update()
    {
        if (agent == null || player == null || !agent.isOnNavMesh)
            return;

        // Cegah perhitungan jika posisi tidak valid
        if (float.IsNaN(transform.position.x) || float.IsNaN(player.position.x))
        {
            Debug.LogError("NaN terdeteksi pada posisi! Update dihentikan.");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Hanya kejar player jika posisi player valid di NavMesh
        if (distance <= chaseDistance && 
            NavMesh.SamplePosition(player.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            if (!isChasing)
            {
                anim.SetBool("IsRunning", true);
                isChasing = true;
                Debug.Log("Mulai mengejar player");
            }
        }
        else
        {
            agent.ResetPath();
            if (isChasing)
            {
                anim.SetBool("IsRunning", false);
                isChasing = false;
                Debug.Log("Berhenti mengejar");
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
        // Pastikan kembali ke posisi awal yang valid
        if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            transform.position = startPosition;
        }

        anim.SetBool("IsRunning", false);
        isChasing = false;
        agent.ResetPath();
    }
}
