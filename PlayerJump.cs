using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    // Private references to sibling components attached to the same GameObject
    PlayerMovement move;
    CharacterController cc;

    // The initial vertical impulse applied to overcome gravity
    public float jumpForce = 6f;

    void Start()
    {
        // Initialise dependencies by caching the required components, 
        // allowing this script to read states and trigger behaviours in others.
        move = GetComponent<PlayerMovement>();
        cc = GetComponent<CharacterController>();
    }

    // Public subroutine executed every frame via the central PlayerController hub
    public void HandleJump()
    {
        // Guard clause: immediately exit the function if the player is currently mid-air.
        // This is a crucial state check that prevents unintended behaviour like double-jumping.
        if (!cc.isGrounded) return;

        // Polls the Unity Input Manager for the discrete 'Jump' action (typically the Spacebar)
        if (Input.GetButtonDown("Jump"))
        {
            // Delegates the actual modification of the vertical velocity vector 
            // to the dedicated movement module
            move.SetVertical(jumpForce);
        }
    }
}