using UnityEngine;

// Enforces structural dependency which guarantees the presence of a CharacterController.
// This prevents runtime errors if a developer forgets to add the required physics component.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera cam;

    // Private fields to hold references to our modular behaviour scripts
    PlayerCamera cameraScript;
    PlayerMovement moveScript;
    PlayerJump jumpScript;
    PlayerCrouch crouchScript;

    void Start()
    {
        // Fallback assignment to locate the primary camera in the scene if one hasn't been manually linked
        if (cam == null)
            cam = Camera.main;

        // Locks the mouse cursor to the centre of the screen and hides it,
        // which is standard behaviour for a first-person perspective control scheme.
        Cursor.lockState = CursorLockMode.Locked;

        // Runtime dependency resolution: fetching the attached components from the same GameObject.
        // This allows the scripts to communicate without needing manual configuration in the editor.
        cameraScript = GetComponent<PlayerCamera>();
        moveScript = GetComponent<PlayerMovement>();
        jumpScript = GetComponent<PlayerJump>();
        crouchScript = GetComponent<PlayerCrouch>();

        // Passes the initialised camera reference down to the script responsible for handling vision
        cameraScript.cam = cam;
    }

    void Update()
    {
        // The Update loop now acts purely as a delegation manager.
        // It keeps the main loop clean by triggering the specific subroutines in each modular script every frame.
        
        // Standard First-Person look logic
        cameraScript.HandleLook();

        crouchScript.HandleCrouch();
        jumpScript.HandleJump();
        moveScript.HandleMovement();
    }
}