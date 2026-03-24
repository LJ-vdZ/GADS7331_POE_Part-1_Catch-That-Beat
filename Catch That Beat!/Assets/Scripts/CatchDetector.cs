using UnityEngine;

public class CatchDetector : MonoBehaviour
{
    [SerializeField] private KeyCode catchKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    private bool playerInRange;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(catchKey))
        {
            RoundManager.Instance?.RegisterCatch();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
}
