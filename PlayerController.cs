using UnityEngine;

// Enforces structural dependency which guarantees the presence of a CharacterController.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera cam;

    // Private fields
    PlayerCamera cameraScript;
    PlayerMovement moveScript;
    PlayerJump jumpScript;
    PlayerCrouch crouchScript;

    void Start()
    {
        // Fallback assignment to locate the primary camera in the scene
        if (cam == null)
            cam = Camera.main;

        // Locks the mouse cursor to the centre of the screen and hides it,
        Cursor.lockState = CursorLockMode.Locked;

        // This allows the scripts to communicate without needing manual configuration in the editor
        cameraScript = GetComponent<PlayerCamera>();
        moveScript = GetComponent<PlayerMovement>();
        jumpScript = GetComponent<PlayerJump>();
        crouchScript = GetComponent<PlayerCrouch>();

        // Passes the initialised camera reference down to the script responsible for handling vision
        cameraScript.cam = cam;
    }

    void Update()
    {
        cameraScript.HandleLook();

        crouchScript.HandleCrouch();
        jumpScript.HandleJump();
        moveScript.HandleMovement();
    }
}