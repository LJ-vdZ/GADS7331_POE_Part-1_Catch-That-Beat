using UnityEngine; // Access Unity APIs.
using UnityEngine.AI; // Access NavMesh navigation.

[RequireComponent(typeof(NavMeshAgent))] // Ensure agent component exists.
public class DroidAIController : MonoBehaviour // Controls droid roaming and dashing.
{
    private enum DroidState
    {
        Idle,      // Stationary at checkpoint
        Running    // Moving to a checkpoint at dash speed
    }

    [Header("Targets")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] checkpoints;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 8f;       // Trigger running when player is this close
    [SerializeField] private float safeDistance = 14f;         // When player is farther than this ? become idle

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float dashSpeedMultiplier = 2.3f;
    [SerializeField] private float speedGainPerCatch = 0.55f;

    private NavMeshAgent agent;
    private DroidState currentState = DroidState.Idle;
    private int catchesAgainstDroid;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseSpeed;
        agent.stoppingDistance = 0.8f;
        agent.autoBraking = false;
        agent.acceleration = 20f;
        agent.angularSpeed = 400f;
    }

    private void Start()
    {
        if (player == null)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

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

        switch (currentState)
        {
            case DroidState.Idle:
                if (distToPlayer < detectionRadius)
                {
                    StartRunningToNewCheckpoint();
                }
                break;

            case DroidState.Running:
                // Check if we arrived at the checkpoint
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
                {
                    // Arrived - decide what to do next
                    if (distToPlayer < detectionRadius)
                    {
                        // Player is still close ? keep running to another checkpoint
                        StartRunningToNewCheckpoint();
                    }
                    else
                    {
                        // Player is far ? become idle here
                        BecomeIdle();
                    }
                }
                else if (distToPlayer > safeDistance)
                {
                    // Player got far while running ? stop and idle at current location
                    BecomeIdle();
                }
                break;
        }
    }

    public void OnCaughtAndEscalate()
    {
        catchesAgainstDroid++;
        agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
        StartRunningToNewCheckpoint();   // Force escape
    }

    private void SpawnAtRandomCheckpoint()
    {
        int index = Random.Range(0, checkpoints.Length);
        Vector3 targetPos = checkpoints[index].position;

        agent.Warp(targetPos);           // Safe teleport on NavMesh
        transform.position = targetPos;

        BecomeIdle();
    }

    private void StartRunningToNewCheckpoint()
    {
        currentState = DroidState.Running;
        agent.isStopped = false;

        // Set dash speed
        float normalSpeed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
        agent.speed = normalSpeed * dashSpeedMultiplier;

        PickRandomCheckpoint();
        Debug.Log("Droid running away to new checkpoint!");
    }

    private void PickRandomCheckpoint()
    {
        if (checkpoints.Length == 0) return;

        int index = Random.Range(0, checkpoints.Length);
        agent.SetDestination(checkpoints[index].position);
    }

    private void BecomeIdle()
    {
        currentState = DroidState.Idle;
        agent.isStopped = true;
        agent.ResetPath();
        agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;

        Debug.Log("Droid reached checkpoint and is now idle");
    }
} 
