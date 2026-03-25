using UnityEngine; // Access Unity APIs.
using UnityEngine.UI; // Access legacy UI text components.

using TMPro;

public class GameUIController : MonoBehaviour // Updates HUD and end panels.
{ // Class scope starts.
    [SerializeField] private TMP_Text timerText; // Displays remaining round time.
    [SerializeField] private Text catchesText; // Displays catches progress.
    [SerializeField] private Text stateText; // Displays current game state.
    [SerializeField] private GameObject endPanel; // Holds end-screen UI.

    private void OnEnable() // Subscribe to round events.
    { // Method scope starts.
        if (RoundManager.Instance == null) // Guard if manager unavailable.
        { // Condition scope starts.
            return; // Stop subscription attempt.
        } // Condition scope ends.

        RoundManager.Instance.OnTimerUpdated += HandleTimerUpdated; // Listen for timer changes.
        RoundManager.Instance.OnCatchesUpdated += HandleCatchesUpdated; // Listen for catch updates.
        RoundManager.Instance.OnGameStateChanged += HandleStateChanged; // Listen for state changes.
    } // Method scope ends.

    private void OnDisable() // Unsubscribe from round events.
    { // Method scope starts.
        if (RoundManager.Instance == null) // Guard if manager unavailable.
        { // Condition scope starts.
            return; // Stop unsubscription attempt.
        } // Condition scope ends.

        RoundManager.Instance.OnTimerUpdated -= HandleTimerUpdated; // Remove timer listener.
        RoundManager.Instance.OnCatchesUpdated -= HandleCatchesUpdated; // Remove catches listener.
        RoundManager.Instance.OnGameStateChanged -= HandleStateChanged; // Remove state listener.
    } // Method scope ends.

    private void HandleTimerUpdated(float seconds) // Update timer label text.
    { // Method scope starts.
        if (timerText != null) // Ensure timer text exists.
        { // Condition scope starts.
            timerText.text = $"Time: {seconds:0.0}s"; // Format and show seconds.
        } // Condition scope ends.
    } // Method scope ends.

    private void HandleCatchesUpdated(int catches, int target) // Update catches label text.
    { // Method scope starts.
        if (catchesText != null) // Ensure catches text exists.
        { // Condition scope starts.
            catchesText.text = $"Catches: {catches}/{target}"; // Format catch progress text.
        } // Condition scope ends.
    } // Method scope ends.

    private void HandleStateChanged(string state) // Update state label and panel.
    { // Method scope starts.
        if (stateText != null) // Ensure state text exists.
        { // Condition scope starts.
            stateText.text = state; // Show current state.
        } // Condition scope ends.

        if (endPanel != null) // Ensure end panel exists.
        { // Condition scope starts.
            bool showEnd = state == "Win" || state == "Lose"; // Check terminal states.
            endPanel.SetActive(showEnd); // Toggle end panel visibility.
        } // Condition scope ends.
    } // Method scope ends.

    public void RestartGame() // Called by restart button.
    { // Method scope starts.
        RoundManager.Instance?.StartNewGame(); // Restart round sequence.
    } // Method scope ends.
} // Class scope ends.
