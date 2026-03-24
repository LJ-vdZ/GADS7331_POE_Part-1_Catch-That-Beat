using UnityEngine; // Access Unity engine APIs.

public class CatchDetector : MonoBehaviour // Detects player catch attempts.
{ // Class scope starts.
    [SerializeField] private KeyCode catchKey = KeyCode.E; // Key used to catch.
    [SerializeField] private string playerTag = "Player"; // Tag identifying player.

    private bool playerInRange; // Tracks if player is nearby.

    private void Update() // Called every frame.
    { // Method scope starts.
        if (playerInRange && Input.GetKeyDown(catchKey)) // Validate range and key press.
        { // Condition scope starts.
            RoundManager.Instance?.RegisterCatch(); // Notify round manager of catch.
        } // Condition scope ends.
    } // Method scope ends.

    private void OnTriggerEnter(Collider other) // Runs when collider enters trigger.
    { // Method scope starts.
        if (other.CompareTag(playerTag)) // Check entering object is player.
        { // Condition scope starts.
            playerInRange = true; // Enable catch input.
        } // Condition scope ends.
    } // Method scope ends.

    private void OnTriggerExit(Collider other) // Runs when collider exits trigger.
    { // Method scope starts.
        if (other.CompareTag(playerTag)) // Check exiting object is player.
        { // Condition scope starts.
            playerInRange = false; // Disable catch input.
        } // Condition scope ends.
    } // Method scope ends.
} // Class scope ends.
