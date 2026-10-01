using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EVAMiniBoss : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private Transform player;

    private PlayerHealthEVA playerHealth;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 10f;
    [SerializeField] private float stoppingDistance = 0.8f;

    [Header("Kill")]
    [SerializeField] private float killDistance = 1.2f;

    [Header("NavMesh")]
    [SerializeField] private float navMeshCheckRadius = 3f;

    private NavMeshAgent agent;
    private bool chaseActive = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError(
                name + ": NavMeshAgent tidak ditemukan."
            );

            enabled = false;
            return;
        }

        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistance;

        FindPlayer();
    }

    private void OnEnable()
    {
        chaseActive = true;

        FindPlayer();

        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.stoppingDistance = stoppingDistance;

            EnsureOnNavMesh();
        }

        Debug.Log(
            name + " AKTIF! MINI-BOSS MULAI MENGEJAR PLAYER."
        );
    }

    private void Update()
    {
        if (!chaseActive)
            return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            StopMiniBoss();
            return;
        }

        if (agent == null || !agent.enabled)
            return;

        if (!agent.isOnNavMesh)
        {
            EnsureOnNavMesh();
            return;
        }

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Jika Mini-Boss sudah sangat dekat,
        // Player langsung mati.
        if (distance <= killDistance)
        {
            KillPlayer();
            return;
        }

        agent.isStopped = false;

        NavMeshHit targetHit;

        if (NavMesh.SamplePosition(
            player.position,
            out targetHit,
            navMeshCheckRadius,
            NavMesh.AllAreas))
        {
            agent.SetDestination(targetHit.position);
        }
    }

    private void KillPlayer()
    {
        if (playerHealth == null)
            return;

        if (playerHealth.IsDead)
            return;

        if (agent != null &&
            agent.enabled &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        playerHealth.InstantDeath();

        Debug.Log(
            "MINI-BOSS MENANGKAP PLAYER - INSTANT GAME OVER."
        );
    }

    private void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            playerHealth =
                player.GetComponent<PlayerHealthEVA>();

            if (playerHealth == null)
            {
                playerHealth =
                    player.GetComponentInParent<PlayerHealthEVA>();
            }

            if (playerHealth == null)
            {
                playerHealth =
                    player.GetComponentInChildren<PlayerHealthEVA>();
            }
        }
    }

    private void EnsureOnNavMesh()
    {
        if (agent == null || !agent.enabled)
            return;

        if (agent.isOnNavMesh)
            return;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            transform.position,
            out hit,
            navMeshCheckRadius,
            NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning(
                name +
                ": Mini-Boss tidak menemukan NavMesh di sekitar spawn."
            );
        }
    }

    public void ActivateMiniBoss()
    {
        chaseActive = true;

        FindPlayer();

        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.stoppingDistance = stoppingDistance;

            EnsureOnNavMesh();

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
    }

    public void StopMiniBoss()
    {
        chaseActive = false;

        if (agent != null &&
            agent.enabled &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
}