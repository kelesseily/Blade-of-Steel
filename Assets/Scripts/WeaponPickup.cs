using UnityEngine;
using TMPro; // Import TextMeshPro for the UI text

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class WeaponPickup : MonoBehaviour
{
    // The name of the empty "hand" object on the player
    public string weaponHolderName = "WeaponHolder";

    // The key the player needs to press
    public KeyCode pickupKey = KeyCode.X;

    // The UI Text element to show the "Press X..." message
    public TextMeshProUGUI pickupText;

    // --- Private State Variables ---
    private bool canPickup = false; // Is the player inside the trigger?
    private bool isEquipped = false; // Is this weapon currently being held?
    private Transform playerTransform; // Store the player's transform
    private Transform weaponHolder; // Store the player's hand transform

    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        // Get and store component references
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Ensure the weapon starts in a "pickable" state (on the ground)
        if (!isEquipped)
        {
            rb.isKinematic = false; // Can be a trigger
            rb.useGravity = false;  // We use false for floating items
            col.enabled = true;
            col.isTrigger = true;
        }

        // Hide the pickup text at the start
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Don't run update logic if we're equipped or not in range
        if (isEquipped || !canPickup)
            return;

        // Check if the player pressed the key
        if (Input.GetKeyDown(pickupKey) && playerTransform != null)
        {
            PerformPickup();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Don't trigger if we're already equipped
        if (isEquipped)
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the pickup zone.");

            canPickup = true;
            playerTransform = other.transform; // Get the player's transform
            weaponHolder = playerTransform.Find(weaponHolderName); // Find the hand

            if (weaponHolder == null)
            {
                Debug.LogError("Player is missing WeaponHolder child object!");
                canPickup = false;
                return;
            }

            // Show the pickup message and change text if hand is full
            if (pickupText != null)
            {
                if (weaponHolder.childCount > 0)
                {
                    pickupText.text = $"Press {pickupKey} to swap weapon";
                }
                else
                {
                    pickupText.text = $"Press {pickupKey} to pick up weapon";
                }
                pickupText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Don't trigger if we're already equipped
        if (isEquipped)
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has left the pickup zone.");

            canPickup = false;
            playerTransform = null;
            weaponHolder = null;

            // Hide the pickup message
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// This is the main pickup/swap function.
    /// </summary>
    private void PerformPickup()
    {
        Debug.Log("Pickup key pressed.");

        // Check if the player is already holding a weapon
        if (weaponHolder.childCount > 0)
        {
            // Get the weapon they are holding
            Transform oldWeapon = weaponHolder.GetChild(0);

            // Tell that weapon's script to "Drop" itself
            Debug.Log($"Dropping {oldWeapon.name}");
            oldWeapon.GetComponent<WeaponPickup>().Drop();
        }

        // Now, equip this new weapon
        Equip();
    }

    /// <summary>
    /// Puts the weapon into the player's hand.
    /// </summary>
    public void Equip()
    {
        Debug.Log($"Equipping {this.name}");
        isEquipped = true;
        canPickup = false; // No longer can be "picked up"

        // Parent to hand and snap to position/rotation
        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Disable physics and trigger
        col.enabled = false;
        rb.isKinematic = true;

        // Hide the UI text
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Drops the weapon from the player's hand onto the ground.
    /// </summary>
    public void Drop()
    {
        isEquipped = false;

        // Un-parent from hand
        transform.SetParent(null);

        // --- MODIFIED LOGIC TO PLACE ON FLOOR ---
        // Raycast down from the player's position to find the ground
        RaycastHit hit;
        Vector3 dropPosition;

        // Get the Player's layer to ignore it in the raycast
        int playerLayer = playerTransform.gameObject.layer;
        int layerMask = ~(1 << playerLayer); // Bitmask to ignore the player's layer

        // Raycast from just above the player's position, downwards, 10 units, ignoring the player's own layer
        if (playerTransform != null && Physics.Raycast(playerTransform.position + (Vector3.up * 0.5f), Vector3.down, out hit, 10f, layerMask))
        {
            // We found the ground! Place the weapon on the ground at the hit point.
            // Add a tiny offset so it doesn't clip *into* the ground.
            dropPosition = hit.point + (hit.normal * 0.1f);
        }
        else if (playerTransform != null)
        {
            // Raycast failed, just drop it near the player's feet as a fallback
            dropPosition = playerTransform.position + (playerTransform.up * 0.5f);
        }
        else
        {
            // Failsafe if playerTransform is null (shouldn't happen)
            dropPosition = transform.position;
        }

        transform.position = dropPosition;
        // --- END MODIFIED LOGIC ---

        // Re-enable physics and trigger
        rb.isKinematic = false;
        rb.useGravity = false; // Continue to float (so it stays a trigger and doesn't fall through)
        col.enabled = true;
        col.isTrigger = true;
    }
}

