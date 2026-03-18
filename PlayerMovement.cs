using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Private references to internal and sibling components
    CharacterController cc;
    PlayerCrouch crouch;

    // Public scalar values to define movement magnitudes, easily adjustable in the editor
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2f;

    // Constants for physics simulation
    public float gravity = -20f;
    
    // Tracks the current vertical momentum across frames
    float verticalVelocity;

    void Start()
    {
        // Initialise dependencies by caching the necessary components
        cc = GetComponent<CharacterController>();
        crouch = GetComponent<PlayerCrouch>();
    }

    // Public subroutine called continuously by the PlayerController update loop
    public void HandleMovement()
    {
        // Asks for the input axes, returning a float between -1.0 and 1.0
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //boolean expression to determine if the player is allowed to sprint
        bool running = Input.GetKey(KeyCode.LeftShift) && !crouch.isCrouching;

        // It checks the current state and assigns the corresponding speed multiplier
        float speed =
            crouch.isCrouching ? crouchSpeed :
            running ? runSpeed :
                                 walkSpeed;

        // Calculates the movement vector relative to the character's current orientation
        Vector3 move =
            (transform.forward * v + transform.right * h) * speed;

        // Applies the horizontal translation to the collision model
        // Time.deltaTime ensures the movement is frame-rate independent and thus doesnt appear laggy
        cc.Move(move * Time.deltaTime);
        ApplyGravity();
    }

    void ApplyGravity()
    {
        //if touching the floor, maintain a small constant downward force to prevent bouncing
        if (cc.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        // Simulates acceleration due to gravity (Velocity = Initial Velocity + Acceleration * Time)
        verticalVelocity += gravity * Time.deltaTime;
        
        cc.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    public void SetVertical(float v) { verticalVelocity = v; }
}