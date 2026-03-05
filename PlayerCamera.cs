using UnityEngine;

// Manages the first-person perspective mathematics and peripheral input
public class PlayerCamera : MonoBehaviour
{
    // Public reference to the camera component
    public Camera cam;
    
    // A scalar multiplier to adjust the responsiveness of the peripheral input
    public float mouseSensitivity = 2f;

    // Accumulator variable to store the current vertical angle of the camera
    float pitch = 0f;

    // This public method is likely called every frame from a central Player Controller script
    public void HandleLook()
    {
        // Guard clause to prevent null reference exceptions if the camera is unassigned
        if (cam == null) return;

        // Polls the input manager for raw mouse delta movement across the X and Y axes
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Applies horizontal rotation (yaw) to the entire player GameObject hierarchy
        // Vector3.up represents the Y-axis (0, 1, 0)
        transform.Rotate(Vector3.up * mx);

        // Accumulates the vertical input. 
        // Subtraction is used here to invert the axis, aligning raw input with standard 3D space expectations
        pitch -= my;
        
        // Restricts the vertical angle to a mathematically safe range.
        // This prevents the camera from flipping over backwards, which avoids disorientation and gimbal lock.
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Applies the constrained pitch strictly to the camera's local rotation using Euler angles.
        // This ensures the camera looks up/down without tilting the physical player capsule.
        cam.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
}