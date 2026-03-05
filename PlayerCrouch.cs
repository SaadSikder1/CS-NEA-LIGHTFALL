using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    // Private reference to the CharacterController component, which handles collision
    CharacterController cc;

    // Public fields allowing parameter tuning within the Unity Inspector
    public float standHeight = 2f;
    public float crouchHeight = 1.1f;

    // A boolean flag tracking the current physical state of the player entity
    public bool isCrouching = false;

    void Start()
    {
        // Initialise by caching the component reference to avoid expensive lookups every frame
        cc = GetComponent<CharacterController>();
    }

    // Public subroutine called by the central PlayerController script
    public void HandleCrouch()
    {
        // Event-driven input handling: detects the exact frame the control key is pressed down
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // Update the state flag and modify the physical height of the collision volume
            isCrouching = true;
            cc.height = crouchHeight;
        }
        // Detects the exact frame the control key is released
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // Revert the state flag and restore the original standing height
            isCrouching = false;
            cc.height = standHeight;
        }
    }
}