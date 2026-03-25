using UnityEngine;

public class DroidInteractable : MonoBehaviour
{
    [Header("Win Menu")]
    public GameObject winMenuPanel;

    [Header("Feedback")]
    public string grabMessage = "Droid Collected!";

    private bool hasBeenGrabbed = false;
    private bool playerIsInsideTrigger = false;   // New flag

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInsideTrigger = true;
            Debug.Log("Player entered droid trigger zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInsideTrigger = false;
            Debug.Log("Player left droid trigger zone");
        }
    }

    // This runs every frame in the Player's Update()
    public void TryGrab()
    {
        if (hasBeenGrabbed || !playerIsInsideTrigger) return;

        hasBeenGrabbed = true;

        Debug.Log(grabMessage);

        if (winMenuPanel != null)
        {
            winMenuPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Win Menu Panel is not assigned!");
        }

        // Optional: Hide or disable the droid
        // gameObject.SetActive(false);
    }


}
