using System.Collections;
using UnityEngine;
using TMPro; // Don't forget to import TextMeshPro!

public class InteractableTorch : MonoBehaviour
{
    [Header("Torch Components")]
    [Tooltip("The Light component that will flicker (should be a child of the flame).")]
    public Light torchLight;
    [Tooltip("The Particle System for the flame.")]
    public ParticleSystem flameParticles;

    [Header("Interaction")]
    [Tooltip("The UI Text object to show the interaction message.")]
    public TextMeshProUGUI interactionText;
    [Tooltip("The key to press to interact with the torch.")]
    public KeyCode interactKey = KeyCode.X;

    [Header("Flicker Settings")]
    [Tooltip("The minimum light intensity.")]
    public float minIntensity = 0.5f;
    [Tooltip("The maximum light intensity.")]
    public float maxIntensity = 2.0f;
    [Tooltip("How fast the light flickers.")]
    public float flickerSpeed = 0.1f;

    // --- Private State ---
    private bool isLit = true; // Is the torch on?
    private bool playerInRange = false; // Is the player inside the trigger?

    void Start()
    {
        // --- Error Checking ---
        if (torchLight == null)
            Debug.LogError("Torch Light is not assigned on " + gameObject.name);
        if (flameParticles == null)
            Debug.LogError("Flame Particles are not assigned on " + gameObject.name);
        if (interactionText == null)
            Debug.LogError("Interaction Text is not assigned on " + gameObject.name);

        // Hide the text at the start
        interactionText.gameObject.SetActive(false);

        // Start the flicker coroutine *ONCE*
        StartCoroutine(FlickerCoroutine());
    }

    void Update()
    {
        // Check if the player is in range and presses the interact key
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            ToggleTorch();
        }
    }

    /// <summary>
    /// This Coroutine runs forever, flickering the light if it's on.
    /// </summary>
    private IEnumerator FlickerCoroutine()
    {
        while (true) // This loop runs forever in the background
        {
            if (isLit && torchLight != null)
            {
                // Set a new random intensity
                float newIntensity = Random.Range(minIntensity, maxIntensity);
                torchLight.intensity = newIntensity;
            }

            // Wait for a short time before flickering again
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    /// <summary>
    /// Toggles the torch on or off.
    /// </summary>
    private void ToggleTorch()
    {
        isLit = !isLit; // Flip the state (true to false, false to true)

        if (isLit)
        {
            // --- Turn ON ---
            torchLight.enabled = true;
            flameParticles.Play();
            UpdateInteractionText();
        }
        else
        {
            // --- Turn OFF ---
            torchLight.enabled = false;
            flameParticles.Stop();
            UpdateInteractionText();
        }
    }

    /// <summary>
    /// Updates the UI text based on the torch's current state.
    /// </summary>
    private void UpdateInteractionText()
    {
        if (isLit)
        {
            interactionText.text = $"Press {interactKey} to take out the light";
        }
        else
        {
            interactionText.text = $"Press {interactKey} to light";
        }
    }

    // --- Trigger Detection ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionText.gameObject.SetActive(true);
            UpdateInteractionText(); // Show the correct message
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionText.gameObject.SetActive(false); // Hide the text
        }
    }
}
