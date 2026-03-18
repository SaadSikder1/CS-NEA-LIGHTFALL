using UnityEngine;

// Manages the first-person perspective
public class PlayerCamera : MonoBehaviour
{
    // Public reference to the camera component
    public Camera cam;
    
    // A scalar multiplier
    public float mouseSensitivity = 2f;

    // variable to store the current vertical angle of the camera
    float pitch = 0f;

    // This public method is likely called every frame from a central Player Controller script
    public void HandleLook()
    {
        // Guard clause
        if (cam == null) return;

        //asks the input manager for raw mouse movement across the X and Y axes
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Applies yaw to the entire player GameObject hierarchy
        // Vector3.up represents the Y-axis (0, 1, 0)
        transform.Rotate(Vector3.up * mx);

        // Subtraction is used here to invert the axis
        pitch -= my;
        
        //Restricts the vertical angle to asafe range.
        // This prevents the camera from flipping over backwardsn which avoids disorientation
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Applies the constrained pitch strictly to the camera's local rotation using Euler angles
        //This ensures the camera looks up ordown without tilting the physical player capsule
        cam.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
}