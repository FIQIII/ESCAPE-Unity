using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EVAPatrolRobot : MonoBehaviour
{
    private enum RobotState
    {
        Patrol,
        Chase,
        Return
    }

    [Header("Referensi")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Animation")]
    public Animator robotAnimator;
    public string walkingParameter = "IsWalking";
    public float animationMoveThreshold = 0.05f;

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float waypointReachDistance = 0.7f;
    public float patrolStoppingDistance = 0.1f;

    [Header("Deteksi")]
    public float viewDistance = 10f;

    [Range(10f, 180f)]
    public float viewAngle = 70f;

    public float eyeHeight = 1.5f;

    [Header("Chase")]
    public float chaseSpeed = 3.5f;
    public float chaseStoppingDistance = 1.4f;
    public float losePlayerDelay = 2f;

    [Header("Damage")]
    public float attackDistance = 1.8f;
    public float attackCooldown = 1f;
    public float damage = 10f;

    [Header("NavMesh")]
    public float navMeshSearchRadius = 2f;

    [Header("Indicator Optional")]
    public Renderer indicatorRenderer;
    public Color patrolColor = Color.green;
    public Color chaseColor = Color.red;

    private NavMeshAgent agent;
    private RobotState state = RobotState.Patrol;

    private int currentWaypoint = 0;

    private float loseTimer = 0f;
    private float attackTimer = 0f;

    private Vector3 currentWaypointPosition;
    private Vector3 lastSafePosition;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }

        /*
         * Cari Animator otomatis dari child
         * kalau belum diassign manual.
         */
        if (robotAnimator == null)
        {
            robotAnimator = GetComponentInChildren<Animator>();
        }

        if (!SnapToNavMesh())
        {
            Debug.LogError(name + " tidak berada dekat NavMesh.");
            enabled = false;
            return;
        }

        lastSafePosition = transform.position;

        StartPatrol();

        UpdateAnimation();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        KeepOnNavMesh();

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        bool seesPlayer = CanSeePlayer();

        switch (state)
        {
            case RobotState.Patrol:

                if (seesPlayer)
                    StartChase();
                else
                    UpdatePatrol();

                break;

            case RobotState.Chase:

                if (seesPlayer)
                {
                    loseTimer = losePlayerDelay;
                    UpdateChase();
                }
                else
                {
                    loseTimer -= Time.deltaTime;

                    if (loseTimer <= 0f)
                        StartReturn();
                    else
                        UpdateChase();
                }

                break;

            case RobotState.Return:

                if (seesPlayer)
                    StartChase();
                else
                    UpdateReturn();

                break;
        }

        /*
         * Update Idle / Walk setelah movement selesai dihitung.
         */
        UpdateAnimation();
    }

    // =====================================================
    // PATROL
    // =====================================================

    private void StartPatrol()
    {
        if (!HasPatrolPoints())
            return;

        state = RobotState.Patrol;

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = patrolStoppingDistance;

        SetIndicator(patrolColor);

        currentWaypoint = Mathf.Clamp(
            currentWaypoint,
            0,
            patrolPoints.Length - 1
        );

        SetWaypointDestination(currentWaypoint);
    }

    private void UpdatePatrol()
    {
        if (!HasPatrolPoints() || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = patrolStoppingDistance;

        float distanceToWaypoint =
            Vector3.Distance(
                new Vector3(
                    transform.position.x,
                    0f,
                    transform.position.z
                ),
                new Vector3(
                    currentWaypointPosition.x,
                    0f,
                    currentWaypointPosition.z
                )
            );

        if (distanceToWaypoint <= waypointReachDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= patrolPoints.Length)
                currentWaypoint = 0;

            SetWaypointDestination(currentWaypoint);

            Debug.Log(
                name +
                " sampai waypoint, lanjut ke " +
                patrolPoints[currentWaypoint].name
            );

            return;
        }

        if (
            !agent.pathPending &&
            (!agent.hasPath ||
             agent.pathStatus == NavMeshPathStatus.PathInvalid)
        )
        {
            SetWaypointDestination(currentWaypoint);
        }
    }

    private void SetWaypointDestination(int index)
    {
        if (!HasPatrolPoints())
            return;

        if (index < 0 || index >= patrolPoints.Length)
            return;

        Transform point = patrolPoints[index];

        if (point == null)
            return;

        NavMeshHit hit;

        if (
            NavMesh.SamplePosition(
                point.position,
                out hit,
                navMeshSearchRadius,
                NavMesh.AllAreas
            )
        )
        {
            currentWaypointPosition = hit.position;

            agent.isStopped = false;
            agent.ResetPath();

            bool success =
                agent.SetDestination(currentWaypointPosition);

            Debug.Log(
                name +
                " target patrol = " +
                point.name +
                " | destination = " +
                currentWaypointPosition +
                " | SetDestination = " +
                success
            );
        }
        else
        {
            Debug.LogWarning(
                name +
                ": waypoint " +
                point.name +
                " tidak dekat NavMesh."
            );
        }
    }

    // =====================================================
    // CHASE
    // =====================================================

    private void StartChase()
    {
        state = RobotState.Chase;

        loseTimer = losePlayerDelay;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = chaseStoppingDistance;

        SetIndicator(chaseColor);
    }

    private void UpdateChase()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        NavMeshHit hit;

        if (
            !NavMesh.SamplePosition(
                player.position,
                out hit,
                navMeshSearchRadius,
                NavMesh.AllAreas
            )
        )
        {
            StartReturn();
            return;
        }

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distanceToPlayer <= attackDistance)
        {
            agent.isStopped = true;

            /*
             * Supaya saat berhenti menyerang,
             * animasi kembali Idle.
             */
            if (agent.hasPath)
                agent.ResetPath();

            FacePlayer();

            if (attackTimer <= 0f)
                AttackPlayer();

            return;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = chaseStoppingDistance;

        agent.SetDestination(hit.position);
    }

    // =====================================================
    // RETURN
    // =====================================================

    private void StartReturn()
    {
        if (!HasPatrolPoints())
            return;

        state = RobotState.Return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = patrolStoppingDistance;

        SetIndicator(patrolColor);

        currentWaypoint = FindNearestWaypoint();

        SetWaypointDestination(currentWaypoint);

        Debug.Log(
            name +
            " kehilangan Player, kembali ke " +
            patrolPoints[currentWaypoint].name
        );
    }

    private void UpdateReturn()
    {
        if (!HasPatrolPoints() || !agent.isOnNavMesh)
            return;

        float distance =
            Vector3.Distance(
                new Vector3(
                    transform.position.x,
                    0f,
                    transform.position.z
                ),
                new Vector3(
                    currentWaypointPosition.x,
                    0f,
                    currentWaypointPosition.z
                )
            );

        if (distance <= waypointReachDistance)
        {
            state = RobotState.Patrol;

            currentWaypoint++;

            if (currentWaypoint >= patrolPoints.Length)
                currentWaypoint = 0;

            agent.speed = patrolSpeed;
            agent.stoppingDistance = patrolStoppingDistance;
            agent.isStopped = false;

            SetWaypointDestination(currentWaypoint);

            Debug.Log(
                name +
                " kembali ke rute patrol."
            );
        }
        else if (
            !agent.pathPending &&
            !agent.hasPath
        )
        {
            SetWaypointDestination(currentWaypoint);
        }
    }

    private int FindNearestWaypoint()
    {
        int nearest = 0;
        float nearestDistance = Mathf.Infinity;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null)
                continue;

            float d =
                Vector3.Distance(
                    transform.position,
                    patrolPoints[i].position
                );

            if (d < nearestDistance)
            {
                nearestDistance = d;
                nearest = i;
            }
        }

        return nearest;
    }

    // =====================================================
    // ANIMATION
    // =====================================================

    private void UpdateAnimation()
    {
        if (robotAnimator == null)
            return;

        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
        {
            robotAnimator.SetBool(
                walkingParameter,
                false
            );

            return;
        }

        /*
         * desiredVelocity lebih stabil untuk NavMeshAgent
         * dibanding hanya velocity saat pergantian waypoint.
         */
        bool moving =
            !agent.isStopped &&
            (
                agent.velocity.sqrMagnitude >
                animationMoveThreshold * animationMoveThreshold
                ||
                agent.desiredVelocity.sqrMagnitude >
                animationMoveThreshold * animationMoveThreshold
            );

        robotAnimator.SetBool(
            walkingParameter,
            moving
        );
    }

    // =====================================================
    // DETECTION
    // =====================================================

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 eye =
            transform.position +
            Vector3.up * eyeHeight;

        Vector3 target =
            player.position +
            Vector3.up;

        Vector3 dir =
            target - eye;

        float distance =
            dir.magnitude;

        if (distance > viewDistance)
            return false;

        float angle =
            Vector3.Angle(
                transform.forward,
                dir.normalized
            );

        if (angle > viewAngle * 0.5f)
            return false;

        RaycastHit hit;

        if (
            Physics.Raycast(
                eye,
                dir.normalized,
                out hit,
                distance,
                ~0,
                QueryTriggerInteraction.Ignore
            )
        )
        {
            if (
                hit.transform == player ||
                hit.transform.IsChildOf(player)
            )
            {
                return true;
            }
        }

        return false;
    }

    // =====================================================
    // ATTACK
    // =====================================================

    private void AttackPlayer()
    {
        attackTimer = attackCooldown;

        player.SendMessage(
            "TakeDamage",
            damage,
            SendMessageOptions.DontRequireReceiver
        );

        Debug.Log(
            name +
            " menyerang Player. Damage: " +
            damage
        );
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion target =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                target,
                8f * Time.deltaTime
            );
    }

    // =====================================================
    // NAVMESH
    // =====================================================

    private bool SnapToNavMesh()
    {
        if (agent.isOnNavMesh)
            return true;

        NavMeshHit hit;

        if (
            NavMesh.SamplePosition(
                transform.position,
                out hit,
                5f,
                NavMesh.AllAreas
            )
        )
        {
            agent.Warp(hit.position);
            return agent.isOnNavMesh;
        }

        return false;
    }

    private void KeepOnNavMesh()
    {
        if (agent.isOnNavMesh)
        {
            lastSafePosition = transform.position;
            return;
        }

        NavMeshHit hit;

        if (
            NavMesh.SamplePosition(
                transform.position,
                out hit,
                navMeshSearchRadius,
                NavMesh.AllAreas
            )
        )
        {
            agent.Warp(hit.position);
            lastSafePosition = hit.position;
            return;
        }

        transform.position = lastSafePosition;
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private bool HasPatrolPoints()
    {
        if (
            patrolPoints == null ||
            patrolPoints.Length == 0
        )
            return false;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] != null)
                return true;
        }

        return false;
    }

    private void SetIndicator(Color color)
    {
        if (indicatorRenderer == null)
            return;

        Material mat =
            indicatorRenderer.material;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);

        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");

            mat.SetColor(
                "_EmissionColor",
                color * 2f
            );
        }
    }

    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        Vector3 eye =
            transform.position +
            Vector3.up * eyeHeight;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            eye,
            viewDistance
        );

        Vector3 left =
            Quaternion.Euler(
                0f,
                -viewAngle * 0.5f,
                0f
            ) *
            transform.forward;

        Vector3 right =
            Quaternion.Euler(
                0f,
                viewAngle * 0.5f,
                0f
            ) *
            transform.forward;

        Gizmos.color = Color.cyan;

        Gizmos.DrawRay(
            eye,
            left * viewDistance
        );

        Gizmos.DrawRay(
            eye,
            right * viewDistance
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackDistance
        );
    }
}