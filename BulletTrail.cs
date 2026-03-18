using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    // Private fields to track the trajectory state of the projectile
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float progress;
    private float timeToHit;

    [Tooltip("Speed in metres per second")] 
    [SerializeField] private float speed = 100f;

    // Public setter method allowing external scripts to define the destination
    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    private void Start()
    {
        // Initialise the starting coordinate
        startPosition = transform.position;
        
        // Calculate the Euclidean distance between the start and target vectors
        float distance = Vector3.Distance(startPosition, targetPosition);

        // Calculate total travel time using the standard formula: Time = Distance / Speed
        // Includes a control structure to prevent a division by zero runtime error
        if (speed > 0) timeToHit = distance / speed;
        else timeToHit = 0.1f;
    }

    private void Update()
    {
        // Guard clause
        if (timeToHit <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Increment progress as a normalised fraction (from 0.0 to 1.0)
        // Time.deltaTime is used to ensure movement is independent of the frame rate
        progress += Time.deltaTime / timeToHit;

        // Update the position using Linear Interpolation (Lerp)
        // This calculates the exact coordinate between start and target based on the progress percentage
        transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

        // Memory management: remove the instance from the scene once it reaches its destination
        if (progress >= 1) Destroy(gameObject);
    }
}