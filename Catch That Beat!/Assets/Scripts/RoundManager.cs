using System; // Provides Action delegates.
using UnityEngine; // Access Unity runtime APIs.

public class RoundManager : MonoBehaviour // Controls rounds, timer, and outcomes.
{ // Class scope starts.
    public static RoundManager Instance { get; private set; } // Global manager singleton.

    [Header("Rules")] // Inspector grouping label.
    [SerializeField] private int catchesToWin = 3; // Catches required for victory.
    [SerializeField] private float roundTimeSeconds = 35f; // Time allowed each round.

    [Header("References")] // Inspector grouping label.
    [SerializeField] private DroidAIController droidAI; // Reference to droid controller.
    [SerializeField] private Transform playerSpawnPoint; // Player respawn transform.
    [SerializeField] private Transform droidSpawnPoint; // Droid respawn transform.
    [SerializeField] private GameObject player; // Player object reference.
    [SerializeField] private GameObject droid; // Droid object reference.

    public event Action<float> OnTimerUpdated; // Event for timer UI updates.
    public event Action<int, int> OnCatchesUpdated; // Event for catches UI updates.
    public event Action<string> OnGameStateChanged; // Event for state transitions.

    private int catches; // Current catches this session.
    private float timer; // Current countdown timer.
    private bool isGameActive; // True while match is active.

    private void Awake() // Initialize singleton instance.
    { // Method scope starts.
        if (Instance != null && Instance != this) // Prevent duplicate managers.
        { // Condition scope starts.
            Destroy(gameObject); // Destroy duplicate object.
            return; // Stop duplicate initialization.
        } // Condition scope ends.
        Instance = this; // Assign singleton instance.
    } // Method scope ends.

    private void Start() // Begin first game session.
    { // Method scope starts.
        StartNewGame(); // Initialize gameplay state.
    } // Method scope ends.

    private void Update() // Process timer each frame.
    { // Method scope starts.
        if (!isGameActive) // Skip when game is inactive.
        { // Condition scope starts.
            return; // Exit frame update.
        } // Condition scope ends.

        timer -= Time.deltaTime; // Decrease timer by frame time.
        OnTimerUpdated?.Invoke(Mathf.Max(0f, timer)); // Broadcast clamped timer value.

        if (timer <= 0f) // Check timeout condition.
        { // Condition scope starts.
            LoseGame(); // Trigger loss state.
        } // Condition scope ends.
    } // Method scope ends.

    public void StartNewGame() // Reset game to initial state.
    { // Method scope starts.
        catches = 0; // Reset catches count.
        timer = roundTimeSeconds; // Reset round timer.
        isGameActive = true; // Enable gameplay updates.

        if (droidAI != null) // Ensure droid reference exists.
        { // Condition scope starts.
            //droidAI.ResetDifficulty(); // Restore base droid speed.
        } // Condition scope ends.

        RespawnActors(); // Move actors to spawn points.

        OnCatchesUpdated?.Invoke(catches, catchesToWin); // Update catches UI.
        OnTimerUpdated?.Invoke(timer); // Update timer UI.
        OnGameStateChanged?.Invoke("Playing"); // Notify play state.
    } // Method scope ends.

    public void RegisterCatch() // Called when player catches droid.
    { // Method scope starts.
        if (!isGameActive) // Ignore during inactive states.
        { // Condition scope starts.
            return; // Exit without changes.
        } // Condition scope ends.

        catches++; // Increment catches progress.
        OnCatchesUpdated?.Invoke(catches, catchesToWin); // Broadcast catches update.

        if (catches >= catchesToWin) // Check victory condition.
        { // Condition scope starts.
            WinGame(); // Trigger win state.
            return; // Stop further processing.
        } // Condition scope ends.

        timer = roundTimeSeconds; // Reset timer after successful catch.
        OnTimerUpdated?.Invoke(timer); // Broadcast timer reset.

        if (droidAI != null) // Ensure droid reference exists.
        { // Condition scope starts.
            droidAI.OnCaughtAndEscalate(); // Increase challenge and dash.
        } // Condition scope ends.
    } // Method scope ends.

    private void WinGame() // Set game to win state.
    { // Method scope starts.
        isGameActive = false; // Stop gameplay updates.
        OnGameStateChanged?.Invoke("Win"); // Broadcast win event.
    } // Method scope ends.

    private void LoseGame() // Set game to lose state.
    { // Method scope starts.
        isGameActive = false; // Stop gameplay updates.
        OnGameStateChanged?.Invoke("Lose"); // Broadcast lose event.
    } // Method scope ends.

    private void RespawnActors() // Reposition player and droid.
    { // Method scope starts.
        if (player != null && playerSpawnPoint != null) // Validate player references.
        { // Condition scope starts.
            player.transform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation); // Move player to spawn.
        } // Condition scope ends.

        if (droid != null && droidSpawnPoint != null) // Validate droid references.
        { // Condition scope starts.
            droid.transform.SetPositionAndRotation(droidSpawnPoint.position, droidSpawnPoint.rotation); // Move droid to spawn.
        } // Condition scope ends.
    } // Method scope ends.
} // Class scope ends.
