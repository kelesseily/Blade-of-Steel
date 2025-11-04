using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // === Public Variables (Set in Inspector) ===
    [Header("Core Settings")]
    public Transform target; // The player
    public float smoothSpeed = 0.125f;
    public float mouseSensitivity = 200f;

    [Header("Camera Offsets")]
    public Vector3 thirdPersonOffset = new Vector3(0f, 2.5f, -4f);
    public Vector3 firstPersonOffset = new Vector3(0f, 1.7f, 0.4f);

    [Header("View Limits")]
    [Tooltip("How far up/down the camera can look.")]
    public float verticalPitchLimit = 85f;
    [Tooltip("How far up the camera can look in 3rd person free-look (to avoid clipping).")]
    public float thirdPersonPitchMax = 45f;

    [Header("Key Input")]
    public KeyCode toggleKey = KeyCode.V;

    // === Private State Variables ===
    private bool isFirstPerson = false;
    private bool isThirdPersonFreelook = false;

    private float cameraPitch = 20f; // Vertical rotation
    private float cameraYaw = 0.0f;  // Horizontal rotation (for free-look)

    private PlayerMovement playerMovement; // Reference to player's script

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            enabled = false;
            return;
        }

        // Get the player's movement script
        playerMovement = target.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("Target is missing PlayerMovement script!");
            enabled = false;
            return;
        }

        // **NEW LINE:** Give the player script a reference to this camera's transform
        playerMovement.cameraTransform = this.transform;

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set initial camera rotation
        cameraYaw = target.eulerAngles.y;
    }

    void Update()
    {
        // --- 1. Handle View Toggles ---

        // Check for view switch (First Person / Third Person)
        if (Input.GetKeyDown(toggleKey))
        {
            isFirstPerson = !isFirstPerson;
            playerMovement.SetFirstPersonView(isFirstPerson); // Tell the player script

            // Reset pitch when switching to TP
            if (!isFirstPerson)
            {
                cameraPitch = 20f; // Reset to a nice default angle
            }
        }

        // Check for free-look (in Third Person only)
        if (!isFirstPerson)
        {
            // **UPDATED LOGIC:**
            bool isFreeLooking = Input.GetMouseButton(1); // Is Right Mouse Button held down?

            // Check if the state *changed*
            if (isFreeLooking != isThirdPersonFreelook)
            {
                isThirdPersonFreelook = isFreeLooking;
                playerMovement.SetFreeLook(isThirdPersonFreelook); // Tell the player script
            }
        }

        // --- 2. Get Mouse Input ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // --- 3. Update Camera Logic ---

        if (isFirstPerson)
        {
            // --- First Person Logic ---
            // Player script handles horizontal rotation. Camera handles vertical.
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -verticalPitchLimit, verticalPitchLimit);

            // Apply vertical rotation (pitch) from this script and horizontal rotation (yaw) from the target.
            transform.rotation = Quaternion.Euler(cameraPitch, target.eulerAngles.y, 0f);
        }
        else if (isThirdPersonFreelook)
        {
            // --- Third Person Free-Look Logic (Right-Click Held) ---
            cameraYaw += mouseX;
            cameraPitch -= mouseY;
            // Clamp pitch to avoid weird angles
            cameraPitch = Mathf.Clamp(cameraPitch, -verticalPitchLimit, thirdPersonPitchMax);
        }
        else
        {
            // --- Third Person Locked Logic (Default) ---
            // Camera snaps back to follow the player
            float targetYaw = target.eulerAngles.y;
            float targetPitch = 20f; // Default downward angle

            // Smoothly move yaw and pitch back to the target's forward view
            cameraYaw = Mathf.LerpAngle(cameraYaw, targetYaw, Time.deltaTime * 10f);
            cameraPitch = Mathf.LerpAngle(cameraPitch, targetPitch, Time.deltaTime * 10f);
        }
    }

    void LateUpdate()
    {
        // This runs after all Update() calls, which is best for cameras

        if (isFirstPerson)
        {
            // --- First Person Position ---
            // (Using the logic we fixed before)
            Vector3 desiredPosition = target.position;
            desiredPosition += new Vector3(0, firstPersonOffset.y, 0); // Height
            desiredPosition += target.forward * firstPersonOffset.z;    // Forward
            desiredPosition += target.right * firstPersonOffset.x;      // Side

            // No smoothing for FP, it should be instant
            transform.position = desiredPosition;
            // Rotation was already set in Update()
        }
        else
        {
            // --- Third Person Position ---
            // Convert Yaw/Pitch to a rotation
            Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);

            // Calculate position behind the player
            Vector3 desiredPosition = target.position + rotation * thirdPersonOffset;

            // Smoothly move the camera
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Always look at the player's center
            transform.LookAt(target.position + Vector3.up * 1f); // Look slightly above feet
        }
    }
}

