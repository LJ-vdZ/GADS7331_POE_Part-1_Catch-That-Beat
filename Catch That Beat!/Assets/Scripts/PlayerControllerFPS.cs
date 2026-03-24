using UnityEngine; // Access Unity gameplay APIs.

[RequireComponent(typeof(CharacterController))] // Ensure CharacterController is present.
public class PlayerControllerFPS : MonoBehaviour // Handles first-person movement and look.
{ // Class scope starts.
    [Header("Movement")] // Inspector group label.
    [SerializeField] private float walkSpeed = 4.5f; // Normal movement speed.
    [SerializeField] private float crouchSpeed = 2.25f; // Reduced crouch movement speed.
    [SerializeField] private float jumpHeight = 1.15f; // Jump height in meters.
    [SerializeField] private float gravity = -20f; // Gravity acceleration value.

    [Header("Look")] // Inspector group label.
    [SerializeField] private Transform cameraPivot; // Camera transform to pitch.
    [SerializeField] private float mouseSensitivity = 180f; // Mouse look sensitivity.
    [SerializeField] private float verticalLookLimit = 80f; // Max up/down look angle.

    [Header("Crouch")] // Inspector group label.
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl; // Input key for crouch.
    [SerializeField] private float standingHeight = 1.8f; // Controller height when standing.
    [SerializeField] private float crouchHeight = 1.1f; // Controller height when crouching.
    [SerializeField] private float crouchLerpSpeed = 10f; // Crouch transition smoothing speed.

    private CharacterController characterController; // Cached CharacterController component.
    private float verticalVelocity; // Current vertical movement velocity.
    private float cameraPitch; // Current camera pitch rotation.
    private bool isCrouching; // Current crouch state.

    private void Awake() // Cache components before start.
    { // Method scope starts.
        characterController = GetComponent<CharacterController>(); // Get required movement component.
        if (cameraPivot == null && Camera.main != null) // Auto-assign main camera if needed.
        { // Condition scope starts.
            cameraPivot = Camera.main.transform; // Store camera transform reference.
        } // Condition scope ends.
    } // Method scope ends.

    private void Start() // Initialize cursor and controller.
    { // Method scope starts.
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to window.
        Cursor.visible = false; // Hide cursor during play.
        characterController.height = standingHeight; // Start at standing height.
    } // Method scope ends.

    private void Update() // Process frame-based controls.
    { // Method scope starts.
        HandleLook(); // Process mouse look input.
        HandleMove(); // Process movement and jumping.
        HandleCrouch(); // Process crouch height changes.
    } // Method scope ends.

    private void HandleLook() // Apply horizontal and vertical look.
    { // Method scope starts.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // Read horizontal mouse movement.
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // Read vertical mouse movement.

        transform.Rotate(Vector3.up * mouseX); // Rotate player around Y axis.
        cameraPitch -= mouseY; // Accumulate camera pitch rotation.
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit); // Clamp pitch range.

        if (cameraPivot != null) // Ensure camera transform exists.
        { // Condition scope starts.
            cameraPivot.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f); // Apply pitch to camera.
        } // Condition scope ends.
    } // Method scope ends.

    private void HandleMove() // Move player with gravity and jump.
    { // Method scope starts.
        float moveX = Input.GetAxisRaw("Horizontal"); // Read left-right input axis.
        float moveZ = Input.GetAxisRaw("Vertical"); // Read forward-back input axis.
        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized; // Build local movement vector.

        float speed = isCrouching ? crouchSpeed : walkSpeed; // Choose movement speed by state.
        characterController.Move(move * speed * Time.deltaTime); // Move horizontally each frame.

        if (characterController.isGrounded && verticalVelocity < 0f) // Stabilize when grounded.
        { // Condition scope starts.
            verticalVelocity = -2f; // Keep slight grounded downward force.
        } // Condition scope ends.

        if (Input.GetButtonDown("Jump") && characterController.isGrounded && !isCrouching) // Validate jump conditions.
        { // Condition scope starts.
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); // Compute jump launch velocity.
        } // Condition scope ends.

        verticalVelocity += gravity * Time.deltaTime; // Apply gravity over time.
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime); // Apply vertical displacement.
    } // Method scope ends.

    private void HandleCrouch() // Update crouch state and height.
    { // Method scope starts.
        isCrouching = Input.GetKey(crouchKey); // Read crouch input state.
        float targetHeight = isCrouching ? crouchHeight : standingHeight; // Choose target controller height.
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchLerpSpeed * Time.deltaTime); // Smoothly change height.
    } // Method scope ends.
} // Class scope ends.
