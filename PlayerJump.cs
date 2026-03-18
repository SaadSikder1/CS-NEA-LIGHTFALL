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
        // allowing this script to read states and trigger behaviours in others.
        move = GetComponent<PlayerMovement>();
        cc = GetComponent<CharacterController>();
    }

    // Public subroutine executed every frame via PlayerController
    public void HandleJump()
    {
        // Guard clause that prevents double jump
        if (!cc.isGrounded) return;

        // Asks the Unity Input Manager for the spacebar input
        if (Input.GetButtonDown("Jump"))
        {
            // Delegates the actual modification of the vertical velocity vector
            move.SetVertical(jumpForce);
        }
    }
}