using UnityEngine; // Access Unity APIs.
using UnityEngine.AI; // Access NavMesh navigation.

[RequireComponent(typeof(NavMeshAgent))] // Ensure agent component exists.
public class DroidAIController : MonoBehaviour // Controls droid roaming and dashing.
{ // Class scope starts.
    private enum DroidState // Internal behavior states.
    { // Enum scope starts.
        Roaming, // Default roaming behavior.
        Dashing // Fast escape behavior.
    } // Enum scope ends.

    [Header("Targets")] // Inspector group label.
    [SerializeField] private Transform player; // Player transform reference.
    [SerializeField] private Transform[] roamPoints; // Patrol points around apartment.

    [Header("Movement")] // Inspector group label.
    [SerializeField] private float baseSpeed = 3.2f; // Default movement speed.
    [SerializeField] private float speedGainPerCatch = 0.55f; // Speed increase each catch.
    [SerializeField] private float stopDistanceFromPoint = 1.0f; // Arrival threshold distance.
    [SerializeField] private float dashDuration = 1.5f; // Dash state duration.
    [SerializeField] private float dashSpeedMultiplier = 1.8f; // Dash speed multiplier.

    private NavMeshAgent agent; // Cached navigation agent.
    private DroidState currentState = DroidState.Roaming; // Current active state.
    private float dashTimer; // Remaining dash time.
    private int catchesAgainstDroid; // Number of times player caught droid.

    private void Awake() // Cache components and defaults.
    { // Method scope starts.
        agent = GetComponent<NavMeshAgent>(); // Get attached nav agent.
        agent.speed = baseSpeed; // Set initial speed.
    } // Method scope ends.

    private void Start() // Resolve references and begin movement.
    { // Method scope starts.
        if (player == null) // Search only when unassigned.
        { // Condition scope starts.
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player"); // Locate tagged player object.
            if (foundPlayer != null) // Confirm object exists.
            { // Condition scope starts.
                player = foundPlayer.transform; // Store player transform.
            } // Condition scope ends.
        } // Condition scope ends.

        PickNewRoamPoint(); // Choose initial destination.
    } // Method scope ends.

    private void Update() // Run state machine each frame.
    { // Method scope starts.
        if (roamPoints == null || roamPoints.Length == 0) // Guard against missing points.
        { // Condition scope starts.
            return; // Stop update when invalid.
        } // Condition scope ends.

        switch (currentState) // Branch by active state.
        { // Switch scope starts.
            case DroidState.Roaming: // Roaming state branch.
                HandleRoam(); // Process roam behavior.
                break; // Exit this case.
            case DroidState.Dashing: // Dashing state branch.
                HandleDash(); // Process dash behavior.
                break; // Exit this case.
        } // Switch scope ends.
    } // Method scope ends.

    public void OnCaughtAndEscalate() // Called after successful player catch.
    { // Method scope starts.
        catchesAgainstDroid++; // Increase internal difficulty counter.
        agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch; // Increase agent speed.
        StartDash(); // Trigger immediate escape dash.
    } // Method scope ends.

    public void ResetDifficulty() // Restore baseline difficulty.
    { // Method scope starts.
        catchesAgainstDroid = 0; // Reset catch count.
        agent.speed = baseSpeed; // Reset speed to base.
    } // Method scope ends.

    private void HandleRoam() // Handle patrol movement logic.
    { // Method scope starts.
        if (!agent.pathPending && agent.remainingDistance <= stopDistanceFromPoint) // Check arrival at destination.
        { // Condition scope starts.
            PickNewRoamPoint(); // Select another patrol point.
        } // Condition scope ends.
    } // Method scope ends.

    private void StartDash() // Enter dashing state.
    { // Method scope starts.
        currentState = DroidState.Dashing; // Set current state.
        dashTimer = dashDuration; // Reset dash countdown.
        agent.speed = (baseSpeed + catchesAgainstDroid * speedGainPerCatch) * dashSpeedMultiplier; // Apply dash speed.

        if (player != null) // Prefer dashing away from player.
        { // Condition scope starts.
            Vector3 awayDir = (transform.position - player.position).normalized; // Direction opposite player.
            Vector3 target = transform.position + awayDir * 6f; // Compute dash destination.
            agent.SetDestination(target); // Move to dash destination.
        } // Condition scope ends.
        else // Fallback without player reference.
        { // Else scope starts.
            PickNewRoamPoint(); // Continue roaming behavior.
        } // Else scope ends.
    } // Method scope ends.

    private void HandleDash() // Handle active dash behavior.
    { // Method scope starts.
        dashTimer -= Time.deltaTime; // Decrease dash timer.
        if (dashTimer <= 0f) // Check dash completion.
        { // Condition scope starts.
            currentState = DroidState.Roaming; // Return to roaming state.
            agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch; // Keep escalated roam speed.
            PickNewRoamPoint(); // Choose next roam destination.
        } // Condition scope ends.
    } // Method scope ends.

    private void PickNewRoamPoint() // Select random patrol destination.
    { // Method scope starts.
        int index = Random.Range(0, roamPoints.Length); // Pick random index.
        agent.SetDestination(roamPoints[index].position); // Set agent destination.
    } // Method scope ends.
} // Class scope ends.
