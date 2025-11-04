using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    // === Public Variables (Set in Inspector) ===
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;
    [Tooltip("How fast the character turns to face the move direction (while running).")]
    public float turnSmoothTime = 0.1f; // This is now for MOVING turns
    [Tooltip("How fast the character turns when standing still.")]
    public float standingTurnSmoothTime = 0.2f; // NEW: For standing turns

    [Header("First Person Settings")]
    public float mouseSensitivity = 200f;

    // === Public State Variables (Controlled by Camera) ===
    [HideInInspector]
    public bool IsFirstPerson = false;
    [HideInInspector]
    public bool IsThirdPersonFreeLook = false;
    [HideInInspector]
    public Transform cameraTransform; // Set by the camera script

    // === Private Components & Variables ===
    private CharacterController characterController;
    private Animator animator;
    private Vector3 velocity;
    private float turnSmoothVelocity; // For SmoothDampAngle

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (characterController == null || animator == null)
        {
            Debug.LogError("PlayerMovement script requires both a CharacterController and an Animator component!");
            enabled = false;
        }
    }

    public void SetFirstPersonView(bool isFirstPerson)
    {
        IsFirstPerson = isFirstPerson;
    }

    public void SetFreeLook(bool isFreeLooking)
    {
        IsThirdPersonFreeLook = isFreeLooking;
    }

    void Update()
    {
        // === 0. Get Input ===
        // We get input at the top so we can use it for both movement and animation
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        float inputMagnitude = inputDirection.magnitude;

        // === 1. Handle Gravity ===
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // === 2. Handle Movement ===
        if (IsFirstPerson)
        {
            // --- First Person Movement ---
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);

            Vector3 move = (transform.right * horizontal) + (transform.forward * vertical);
            characterController.Move(move.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            // --- Third Person Movement ---
            if (inputMagnitude >= 0.1f)
            {
                if (IsThirdPersonFreeLook)
                {
                    // Free Look: Move relative to player, NO rotation (strafe)
                    Vector3 move = (transform.forward * vertical) + (transform.right * horizontal);
                    characterController.Move(move.normalized * moveSpeed * Time.deltaTime);
                }
                else
                {
                    // Locked Look: Rotate player to face camera-relative move direction

                    // Find the angle we need to turn
                    float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

                    // Check if we're only turning (standing) or also moving forward/back
                    bool isStandingTurn = Mathf.Abs(vertical) < 0.1f;
                    float currentTurnTime = isStandingTurn ? standingTurnSmoothTime : turnSmoothTime;

                    // Smoothly turn the player
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, currentTurnTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    // **FIXED:** We only move based on the *vertical* input.
                    // The horizontal input is only for turning.
                    // This means `moveDir` is `Vector3.zero` when standing, preventing displacement.
                    Vector3 moveDir = transform.forward * vertical;
                    characterController.Move(moveDir * moveSpeed * Time.deltaTime);
                }
            }
        }

        // === 3. Handle Animations ===
        // **FIXED:** Base animation on input, but differently for each mode

        float animationInputMagnitude;
        float forwardSpeed;

        if (IsThirdPersonFreeLook)
        {
            // In free look, strafing counts as moving
            animationInputMagnitude = inputMagnitude;
            forwardSpeed = inputMagnitude; // You might want a 2D Blend Tree here later
        }
        else
        {
            // In locked look, horizontal-only is just turning (not moving)
            animationInputMagnitude = Mathf.Abs(vertical);
            forwardSpeed = Mathf.Abs(vertical);
        }

        animator.SetBool("IsMoving", animationInputMagnitude >= 0.1f);
        animator.SetFloat("ForwardSpeed", forwardSpeed);
    }
}

