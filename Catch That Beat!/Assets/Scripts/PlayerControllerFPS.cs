using UnityEngine; // Access Unity gameplay APIs.
using UnityEngine.InputSystem; // New Input System (Player Settings).

[RequireComponent(typeof(CharacterController))] // Ensure CharacterController is present.
public class PlayerControllerFPS : MonoBehaviour // Handles first-person movement and look.
{ // Class scope starts.
    [Header("Movement")] // Inspector grouping label.
    [SerializeField] private float walkSpeed = 4.5f; // Base move speed when not sprinting.
    [SerializeField] private float sprintSpeed = 7.5f; // Speed while holding Shift (not crouching).
    [SerializeField] private float crouchSpeed = 2.25f; // Speed while crouch key held.
    [SerializeField] private float jumpHeight = 1.15f; // Jump peak height in world units.
    [SerializeField] private float gravity = -20f; // Gravity acceleration when airborne.

    [Header("Camera")] // Eye position relative to CharacterController root (tune in Inspector).
    [Tooltip("Local position of the camera rig (raise Y for taller eye height).")]
    [SerializeField] private Vector3 cameraLocalOffset = new Vector3(0f, 0.65f, 0f); // Inspector camera height / offset.

    [Header("Look (mouse)")] // Inspector grouping label.
    [SerializeField] private Transform cameraPivot; // Pitch pivot; set after auto-promote or assign by hand.
    [SerializeField] private float mouseSensitivity = 180f; // Mouse look strength; tune in Inspector.
    [SerializeField] private float verticalLookLimit = 80f; // Max look up/down in degrees.

    [Header("Crouch / Jump")] // Inspector grouping label.
    [SerializeField] private Key crouchKey = Key.LeftCtrl; // Hold to crouch (slows movement).
    [SerializeField] private Key jumpKey = Key.Space; // Jump when grounded; default Space.
    [SerializeField] private float standingHeight = 1.8f; // Standing CharacterController height.
    [SerializeField] private float crouchHeight = 1.1f; // Crouching CharacterController height.
    [SerializeField] private float crouchLerpSpeed = 10f; // Capsule height lerp speed.

    private CharacterController characterController; // Cached CharacterController.
    private float verticalVelocity; // Vertical velocity for jump and gravity.
    private float cameraPitch; // Local pitch angle on camera pivot in degrees.
    private bool isCrouching; // True while crouch key held.

    private void Awake() // Promote root Camera to child rig so local offset always works.
    { // Method scope starts.
        characterController = GetComponent<CharacterController>(); // Required movement component.

        Camera camOnRoot = GetComponent<Camera>(); // Camera on same GameObject as CharacterController.
        if (camOnRoot != null) // Root camera needs a child rig for eye-height offset.
        { // Condition scope starts.
            //PromoteCameraToChildRig(camOnRoot); // Creates FPSCameraRig and reparents camera logic.
        } // Condition scope ends.

        if (cameraPivot == null && Camera.main != null) // Designer left pivot empty.
        { // Condition scope starts.
            cameraPivot = Camera.main.transform; // Use scene Main Camera as pitch pivot.
        } // Condition scope ends.
    } // Method scope ends.

    private void PromoteCameraToChildRig(Camera camOnRoot) // Build child object; destroy root Camera same frame.
    { // Method scope starts.
        GameObject rig = new GameObject("FPSCameraRig"); // Holder for Inspector offset plus pitch.
        rig.transform.SetParent(transform, false); // Child of CharacterController root.
        rig.transform.localPosition = cameraLocalOffset; // Initial eye offset from Inspector.
        rig.transform.localRotation = Quaternion.identity; // Yaw still follows parent rotation.
        Camera newCam = rig.AddComponent<Camera>(); // New Camera on rig object.
        CopyCameraSettings(camOnRoot, newCam); // Match FOV, clips, layers, clear flags.
        if (camOnRoot.TryGetComponent(out AudioListener listener)) // Listener usually stays with eyes.
        { // Condition scope starts.
            Destroy(listener); // Drop listener from root to avoid duplicates.
            rig.AddComponent<AudioListener>(); // Single listener on camera rig.
        } // Condition scope ends.
        DestroyImmediate(camOnRoot); // Remove root Camera now so Awake sees one Camera path only.
        cameraPivot = rig.transform; // All vertical look rotates this pivot.
    } // Method scope ends.

    private static void CopyCameraSettings(Camera from, Camera to) // Minimal copy for gameplay continuity.
    { // Method scope starts.
        to.fieldOfView = from.fieldOfView; // Vertical field of view.
        to.nearClipPlane = from.nearClipPlane; // Near clip distance.
        to.farClipPlane = from.farClipPlane; // Far clip distance.
        to.renderingPath = from.renderingPath; // Forward vs deferred, etc.
        to.allowHDR = from.allowHDR; // HDR toggle.
        to.allowMSAA = from.allowMSAA; // MSAA toggle.
        to.stereoTargetEye = from.stereoTargetEye; // VR eye target.
        to.clearFlags = from.clearFlags; // Skybox, solid color, depth only.
        to.backgroundColor = from.backgroundColor; // Clear color for solid flags.
        to.cullingMask = from.cullingMask; // Layer mask for rendering.
        to.depth = from.depth; // Camera stack ordering.
        to.orthographic = from.orthographic; // Ortho vs perspective.
        to.orthographicSize = from.orthographicSize; // Ortho half-height.
        to.depthTextureMode = from.depthTextureMode; // Depth normals for effects.
    } // Method scope ends.

    private void Start() // Lock cursor; seed pitch; sync first camera offset.
    { // Method scope starts.
        Cursor.lockState = CursorLockMode.Locked; // Lock pointer for mouse look.
        Cursor.visible = false; // Hide cursor in play mode.
        characterController.height = standingHeight; // Start at full capsule height.

        if (cameraPivot != null) // Read initial local pitch from rig or Main Camera.
        { // Condition scope starts.
            cameraPivot.localPosition = cameraLocalOffset; // Apply Inspector offset at start.
            cameraPitch = cameraPivot.localEulerAngles.x; // Preserve scene pitch if any.
            if (cameraPitch > 180f) // Unwrap pitch above horizon wrap.
            { // Condition scope starts.
                cameraPitch -= 360f; // Degrees into -180..180 range.
            } // Condition scope ends.
        } // Condition scope ends.
    } // Method scope ends.

    private void LateUpdate() // Keep rig aligned with Inspector offset every frame for live tweaking.
    { // Method scope starts.
        if (cameraPivot != null) // Child rig or assigned pitch pivot.
        { // Condition scope starts.
            cameraPivot.localPosition = cameraLocalOffset; // Developer-adjustable eye height.
        } // Condition scope ends.
    } // Method scope ends.

    private void Update() // Frame update: look, move, crouch.
    { // Method scope starts.
        HandleLook(); // Mouse drives yaw on root and pitch on pivot.
        HandleMove(); // WASD, Shift sprint, jump, gravity.
        HandleCrouch(); // Capsule height from crouch key.
    } // Method scope ends.

    private void HandleLook() // Mouse movement drives horizontal body and vertical pivot.
    { // Method scope starts.
        if (Mouse.current == null) // No mouse device available.
        { // Condition scope starts.
            return; // Skip look logic safely.
        } // Condition scope ends.

        Vector2 delta = Mouse.current.delta.ReadValue(); // Pixel delta since last frame.
        float mouseX = delta.x * mouseSensitivity * Time.deltaTime; // Yaw delta input.
        float mouseY = delta.y * mouseSensitivity * Time.deltaTime; // Pitch delta input.

        transform.Rotate(Vector3.up * mouseX); // Yaw CharacterController root with mouse X.
        cameraPitch -= mouseY; // Invert mouse Y for intuitive look up.
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit); // Stop barrel rolls.

        if (cameraPivot != null) // Apply pitch to eye rig when assigned.
        { // Condition scope starts.
            cameraPivot.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f); // Pitch only on pivot.
        } // Condition scope ends.
    } // Method scope ends.

    private void HandleMove() // WASD; hold Left or Right Shift to sprint when moving.
    { // Method scope starts.
        float moveX = 0f; // Strafe accumulation -1..1.
        float moveZ = 0f; // Forward accumulation -1..1.

        if (Keyboard.current != null) // Keyboard required for WASD.
        { // Condition scope starts.
            if (Keyboard.current.wKey.isPressed) // Forward.
            { // Condition scope starts.
                moveZ += 1f; // Add forward.
            } // Condition scope ends.
            if (Keyboard.current.sKey.isPressed) // Backward.
            { // Condition scope starts.
                moveZ -= 1f; // Subtract forward.
            } // Condition scope ends.
            if (Keyboard.current.dKey.isPressed) // Strafe right.
            { // Condition scope starts.
                moveX += 1f; // Add right.
            } // Condition scope ends.
            if (Keyboard.current.aKey.isPressed) // Strafe left.
            { // Condition scope starts.
                moveX -= 1f; // Add left.
            } // Condition scope ends.
        } // Condition scope ends.

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized; // Wish direction.

        bool shiftHeld = Keyboard.current != null && // Shift held for sprint.
            (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed); // Either shift.
        bool wantsMove = move.sqrMagnitude > 0.0001f; // Player intends horizontal move.
        bool sprinting = shiftHeld && !isCrouching && wantsMove; // Sprint rules.

        float speed = isCrouching ? crouchSpeed : (sprinting ? sprintSpeed : walkSpeed); // Pick speed.
        characterController.Move(move * speed * Time.deltaTime); // Horizontal step.

        if (characterController.isGrounded && verticalVelocity < 0f) // Ground snap.
        { // Condition scope starts.
            verticalVelocity = -2f; // Small downward stick.
        } // Condition scope ends.

        bool jumpPressed = Keyboard.current != null && Keyboard.current[jumpKey].wasPressedThisFrame; // Space jump.

        if (jumpPressed && characterController.isGrounded && !isCrouching) // Valid jump.
        { // Condition scope starts.
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); // Launch velocity.
        } // Condition scope ends.

        verticalVelocity += gravity * Time.deltaTime; // Apply gravity.
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime); // Vertical step.
    } // Method scope ends.

    private void HandleCrouch() // Hold crouch key to lower capsule.
    { // Method scope starts.
        isCrouching = Keyboard.current != null && Keyboard.current[crouchKey].isPressed; // Crouch state.
        float targetHeight = isCrouching ? crouchHeight : standingHeight; // Target capsule height.
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchLerpSpeed * Time.deltaTime); // Smooth height.
    } // Method scope ends.
} 
