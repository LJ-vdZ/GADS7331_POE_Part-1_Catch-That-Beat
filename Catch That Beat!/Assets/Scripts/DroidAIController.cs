using UnityEngine; // Access Unity APIs.
using UnityEngine.AI; // Access NavMesh navigation.

[RequireComponent(typeof(NavMeshAgent))] // Ensure agent component exists.
public class DroidAIController : MonoBehaviour // Controls droid roaming and dashing.
{
    private enum DroidState { Roaming, Dashing }

    [Header("Targets")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] roamPoints;

    [Header("Detection & Escape")]
    [SerializeField] private float detectionRadius = 7.5f;     // How close player triggers dash
    [SerializeField] private float dashRunTime = 2.5f;         // How long the droid runs away
    [SerializeField] private float minDashDistance = 14f;
    [SerializeField] private float maxDashDistance = 22f;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3.2f;
    [SerializeField] private float speedGainPerCatch = 0.55f;
    [SerializeField] private float dashSpeedMultiplier = 2.2f;

    private NavMeshAgent agent;
    private DroidState currentState = DroidState.Roaming;
    private float dashTimer;
    private int catchesAgainstDroid;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseSpeed;
        agent.stoppingDistance = 0.5f;
        agent.autoBraking = false;           // ? Very important! Prevents stopping
        agent.acceleration = 12f;
        agent.angularSpeed = 300f;
    }

    private void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (roamPoints.Length > 0)
            PickNewRoamPoint();
    }

    private void Update()
    {
        if (player == null || roamPoints.Length == 0) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Trigger dash when player gets close
        if (distToPlayer < detectionRadius && currentState != DroidState.Dashing)
        {
            StartDash();
        }

        switch (currentState)
        {
            case DroidState.Roaming:
                HandleRoaming();
                break;

            case DroidState.Dashing:
                HandleDashing(distToPlayer);
                break;
        }
    }

    public void OnCaughtAndEscalate()
    {
        catchesAgainstDroid++;
        agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
        StartDash();
    }

    private void HandleRoaming()
    {
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            PickNewRoamPoint();
        }
    }

    private void StartDash()
    {
        currentState = DroidState.Dashing;
        dashTimer = dashRunTime;

        // Boost speed
        float normalSpeed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
        agent.speed = normalSpeed * dashSpeedMultiplier;

        ChooseRandomEscapePoint();
    }

    private void ChooseRandomEscapePoint()
    {
        if (player == null)
        {
            PickNewRoamPoint();
            return;
        }

        Vector3 awayDir = (transform.position - player.position).normalized;
        Vector3 bestTarget = transform.position;
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < 15; i++)   // More attempts = better chance of good path
        {
            float randomAngle = Random.Range(-80f, 80f);
            Quaternion rot = Quaternion.Euler(0, randomAngle, 0);
            Vector3 testDir = rot * awayDir;

            float distance = Random.Range(minDashDistance, maxDashDistance);
            Vector3 testPoint = transform.position + testDir * distance;

            if (NavMesh.SamplePosition(testPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                float score = Vector3.Dot(testDir, awayDir); // prefer running directly away
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = hit.position;
                }
            }
        }

        agent.SetDestination(bestTarget);
        Debug.Log("Droid dashing away to new point!");
    }

    private void HandleDashing(float distToPlayer)
    {
        dashTimer -= Time.deltaTime;

        // End dash if time is up or player is now far away
        if (dashTimer <= 0f || distToPlayer > detectionRadius + 8f)
        {
            currentState = DroidState.Roaming;
            agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
            PickNewRoamPoint();
            Debug.Log("Dash ended - back to roaming");
        }
        // Optional: Refresh escape direction every ~0.8s so it doesn't stop if player moves
        else if (dashTimer % 0.8f < Time.deltaTime)
        {
            ChooseRandomEscapePoint();
        }
    }

    private void PickNewRoamPoint()
    {
        if (roamPoints.Length == 0) return;
        int index = Random.Range(0, roamPoints.Length);
        agent.SetDestination(roamPoints[index].position);
    }
} // Class scope ends.
