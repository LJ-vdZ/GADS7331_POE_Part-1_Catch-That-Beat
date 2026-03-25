using UnityEngine; // Access Unity APIs.
using UnityEngine.AI; // Access NavMesh navigation.

[RequireComponent(typeof(NavMeshAgent))] // Ensure agent component exists.
public class DroidAIController : MonoBehaviour // Controls droid roaming and dashing.
{
    private enum DroidState
    {
        Idle,      // Stationary at a checkpoint
        Dashing    // Running away from player
    }

    [Header("Targets")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] checkpoints;   // Rename your roamPoints to checkpoints for clarity

    [Header("Detection & Escape")]
    [SerializeField] private float detectionRadius = 8f;      // Player distance to trigger dash
    [SerializeField] private float dashRunTime = 2.8f;        // How long it keeps dashing
    [SerializeField] private float losePlayerDistance = 15f;  // How far player must be to stop dashing
    [SerializeField] private float minDashDistance = 15f;
    [SerializeField] private float maxDashDistance = 25f;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float dashSpeedMultiplier = 2.3f;
    [SerializeField] private float speedGainPerCatch = 0.55f;

    private NavMeshAgent agent;
    private DroidState currentState = DroidState.Idle;
    private float dashTimer;
    private int catchesAgainstDroid;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseSpeed;
        agent.stoppingDistance = 0.6f;
        agent.autoBraking = false;
        agent.acceleration = 15f;
        agent.angularSpeed = 400f;
    }

    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Spawn at a random checkpoint and stay idle
        if (checkpoints.Length > 0)
        {
            SpawnAtRandomCheckpoint();
        }
        else
        {
            Debug.LogWarning("No checkpoints assigned to Droid!");
        }
    }

    private void Update()
    {
        if (player == null || checkpoints.Length == 0) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Trigger dash when player gets close while idle
        if (distToPlayer < detectionRadius && currentState == DroidState.Idle)
        {
            StartDashing();
        }

        switch (currentState)
        {
            case DroidState.Idle:
                // Do nothing - stay completely still
                break;

            case DroidState.Dashing:
                HandleDashing(distToPlayer);
                break;
        }
    }

    // Called from DroidInteractable when player grabs it with E
    public void OnCaughtAndEscalate()
    {
        catchesAgainstDroid++;
        agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
        StartDashing();   // Force dash even if player is not super close
    }

    private void SpawnAtRandomCheckpoint()
    {
        int randomIndex = Random.Range(0, checkpoints.Length);
        Transform chosenPoint = checkpoints[randomIndex];

        // Teleport droid to the checkpoint
        transform.position = chosenPoint.position;
        agent.Warp(chosenPoint.position);   // Important for NavMeshAgent

        currentState = DroidState.Idle;
        agent.isStopped = true;             // Make sure it's fully stopped
        agent.ResetPath();
    }

    private void StartDashing()
    {
        currentState = DroidState.Dashing;
        dashTimer = dashRunTime;

        // Increase speed for dash
        float normalSpeed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
        agent.speed = normalSpeed * dashSpeedMultiplier;
        agent.isStopped = false;

        ChooseRandomEscapePoint();
        Debug.Log("Droid detected player - DASHING AWAY!");
    }

    private void ChooseRandomEscapePoint()
    {
        if (player == null) return;

        Vector3 awayDir = (transform.position - player.position).normalized;
        Vector3 bestTarget = transform.position;
        float bestScore = -Mathf.Infinity;

        for (int i = 0; i < 15; i++)
        {
            float randomAngle = Random.Range(-85f, 85f);
            Quaternion rot = Quaternion.Euler(0, randomAngle, 0);
            Vector3 testDir = rot * awayDir;

            float distance = Random.Range(minDashDistance, maxDashDistance);
            Vector3 testPoint = transform.position + testDir * distance;

            if (NavMesh.SamplePosition(testPoint, out NavMeshHit hit, 12f, NavMesh.AllAreas))
            {
                float score = Vector3.Dot(testDir, awayDir);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = hit.position;
                }
            }
        }

        agent.SetDestination(bestTarget);
    }

    private void HandleDashing(float distToPlayer)
    {
        dashTimer -= Time.deltaTime;

        // End dash when time runs out OR player is far away
        if (dashTimer <= 0f || distToPlayer > losePlayerDistance)
        {
            EndDashAndGoIdle();
        }
        // Refresh escape direction occasionally so it doesn't get stuck
        else if (Time.frameCount % 30 == 0)   // roughly every 0.5s
        {
            ChooseRandomEscapePoint();
        }
    }

    private void EndDashAndGoIdle()
    {
        currentState = DroidState.Idle;
        agent.isStopped = true;
        agent.ResetPath();
        agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;

        // Go to a new random checkpoint and stay there
        SpawnAtRandomCheckpoint();

        Debug.Log("Droid lost the player - now idle at new checkpoint");
    }
} 
