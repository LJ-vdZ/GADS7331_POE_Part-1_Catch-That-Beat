using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    [Header("Interaction")]
    public float grabDistance = 3f;
    public LayerMask droidLayer;

    [Header("UI")]
    public GameObject pauseMenuPanel;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isPaused = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera == null)
            playerCamera = Camera.main.transform;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (isPaused) return;

        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandlePause();
        HandleGrabDroid();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        float currentSpeed = walkSpeed;

        // Sprint check using individual if statement
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = sprintSpeed;
        }

        float moveX = 0f;
        float moveZ = 0f;

        // Individual if statements for each movement key
        if (Input.GetKey("w") || Input.GetKey(KeyCode.W))
        {
            moveZ = 1f;
        }
        if (Input.GetKey("s") || Input.GetKey(KeyCode.S))
        {
            moveZ = -1f;
        }
        if (Input.GetKey("a") || Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        if (Input.GetKey("d") || Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Individual if statement for Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandlePause()
    {
        // Individual if statement for Pause (Tab key)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
        }
    }

    private void HandleGrabDroid()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Find any nearby DroidInteractable that the player is inside
            DroidInteractable droid = FindObjectOfType<DroidInteractable>();   // Works fine if you have only 1 droid

            if (droid != null)
            {
                droid.TryGrab();
            }
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // Call this from your Pause Menu Resume button
    public void ResumeGame()
    {
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
