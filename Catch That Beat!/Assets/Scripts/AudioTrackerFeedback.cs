using UnityEngine; // Access Unity core systems.

[RequireComponent(typeof(AudioSource))] // Ensure audio source exists.
public class AudioTrackerFeedback : MonoBehaviour // Controls dynamic droid audio feedback.
{ // Class scope starts.
    [SerializeField] private Transform player; // Player transform for distance checks.
    [SerializeField] private float maxHearingDistance = 30f; // Distance where cues fade.
    //[SerializeField] private float minPitch = 0.92f; // Lowest pitch when far away.
    //[SerializeField] private float maxPitch = 1.22f; // Highest pitch when very close.
    [SerializeField] private bool useOcclusionRaycast = true; // Toggle wall occlusion simulation.
    [SerializeField] private LayerMask occlusionLayers; // Layers treated as blocking audio.
    [SerializeField] private float occludedLowPass = 900f; // Muffled cutoff through obstacles.
    [SerializeField] private float clearLowPass = 22000f; // Clear cutoff without obstacles.

    private AudioSource audioSource; // Cached audio source.
    private AudioLowPassFilter lowPassFilter; // Cached low-pass filter.

    private void Awake() // Cache required components.
    { // Method scope starts.
        audioSource = GetComponent<AudioSource>(); // Get attached audio source.
        lowPassFilter = GetComponent<AudioLowPassFilter>(); // Get existing low-pass filter.
        if (lowPassFilter == null) // Add filter if missing.
        { // Condition scope starts.
            lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>(); // Create new filter component.
        } // Condition scope ends.
    } // Method scope ends.

    private void Start() // Resolve player reference at runtime.
    { // Method scope starts.
        if (player == null) // Only search when unassigned.
        { // Condition scope starts.
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player"); // Find object tagged player.
            if (foundPlayer != null) // Confirm player object exists.
            { // Condition scope starts.
                player = foundPlayer.transform; // Store player transform.
            } // Condition scope ends.
        } // Condition scope ends.
    } // Method scope ends.

    private void Update() // Update audio each frame.
    { // Method scope starts.
        if (player == null) // Abort without player reference.
        { // Condition scope starts.
            return; // Skip processing this frame.
        } // Condition scope ends.

        float distance = Vector3.Distance(transform.position, player.position); // Measure player distance.
        float t = 1f - Mathf.Clamp01(distance / maxHearingDistance); // Normalize closeness factor.

        audioSource.volume = Mathf.Lerp(0.15f, 1f, t); // Increase volume when closer.
        //audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t); // Raise pitch when closer.

        if (!useOcclusionRaycast) // Skip raycast when disabled.
        { // Condition scope starts.
            lowPassFilter.cutoffFrequency = clearLowPass; // Keep clear tone.
            return; // Exit early.
        } // Condition scope ends.

        Vector3 dir = (player.position - transform.position).normalized; // Direction toward player.
        bool blocked = Physics.Raycast(transform.position, dir, distance, occlusionLayers); // Check obstacle blocking.
        lowPassFilter.cutoffFrequency = blocked ? occludedLowPass : clearLowPass; // Apply muffled or clear filter.
    } // Method scope ends.
} // Class scope ends.
