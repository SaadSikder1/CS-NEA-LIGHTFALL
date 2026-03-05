using UnityEngine;

// Enforces a structural dependency, ensuring the game engine automatically 
// attaches a Rigidbody component when this script is added to an object.
[RequireComponent(typeof(Rigidbody))]
public class EnemyDrone : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform muzzle; 
    public GameObject bulletPrefab;

    [Header("Stats")]
    public float health = 100f;
    public float damage = 10f;
    public float fireRate = 2f;
    public float maxShootDistance = 50f;

    [Header("Movement & Hovering")]
    public float moveSpeed = 4f;
    public float hoverHeight = 2.5f;
    public float hoverBobAmplitude = 0.5f;
    public float hoverBobSpeed = 2f;

    [Header("Behaviour Ranges")]
    public float detectionRange = 30f;
    public float maintainDistance = 12f;

    // State variables
    private float timeSinceLastShot;
    private Rigidbody rb;

    private void Start()
    {
        // Cache the physics component reference to avoid expensive lookups during runtime
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Guard clause: halt execution if the player reference is missing
        if (player == null) return;

        timeSinceLastShot += Time.deltaTime;
        
        // Calculate the Euclidean distance between the drone and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Apply a vertical offset so the drone aims at the player's body, not their feet
            Vector3 aimTarget = player.position + new Vector3(0, 1.5f, 0);

            // Vector subtraction yields the direction. Normalising it sets its magnitude to 1.
            Vector3 lookDirection = (aimTarget - transform.position).normalized;
            
            // Restrict rotation on the Y-axis to prevent the drone from pitching up or down
            lookDirection.y = 0;
            
            if (lookDirection != Vector3.zero)
            {
                // Smoothly rotate the drone's body to face the player using Spherical Linear Interpolation
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            // The weapon muzzle tracks the player precisely on all axes
            if (muzzle != null)
            {
                muzzle.LookAt(aimTarget);
            }

            // Check spatial and temporal conditions before executing the attack subroutine
            if (distanceToPlayer <= maxShootDistance && timeSinceLastShot >= (1f / fireRate))
            {
                Shoot();
            }
        }
    }

    // FixedUpdate operates on a consistent timer, making it essential for stable physics calculations
    private void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Determine whether to apply tracking movement or bring the drone to a halt
        if (distanceToPlayer <= detectionRange)
        {
            HandleMovement(distanceToPlayer);
        }
        else
        {
            // Nullify horizontal velocity while preserving any existing vertical momentum (like falling)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void HandleMovement(float distanceToPlayer)
    {
        Vector3 targetVelocity = Vector3.zero;

        // Conditional logic to maintain an optimal engagement distance from the player
        if (distanceToPlayer > maintainDistance)
        {
            targetVelocity = transform.forward * moveSpeed;
        }
        else if (distanceToPlayer < maintainDistance - 2f)
        {
            targetVelocity = -transform.forward * moveSpeed;
        }

        // Raycast downwards to calculate the distance to the terrain
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, hoverHeight * 3f))
        {
            // Calculate proportional vertical velocity to gently push the drone back to its target altitude
            float targetY = hit.point.y + hoverHeight;
            targetVelocity.y = (targetY - transform.position.y) * 5f;
        }
        else
        {
            // Apply standard gravitational acceleration if the ground is out of range
            targetVelocity.y = rb.linearVelocity.y - 9.81f * Time.fixedDeltaTime;
        }

        // Apply the calculated velocity vector directly to the physics engine
        rb.linearVelocity = targetVelocity;
    }

    private void Shoot()
    {
        if (muzzle == null) return;

        RaycastHit hit;
        Vector3 targetPoint;

        // Visual debugging tool to observe the raycast line within the Unity editor
        Debug.DrawRay(muzzle.position, muzzle.forward * maxShootDistance, Color.red, 2f);

        // Perform the hitscan calculation to see if the theoretical bullet trajectory intersects an object
        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, maxShootDistance))
        {
            // Interface checking: evaluate if the struck object implements the damage protocol
            IDamageable target = hit.transform.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = muzzle.position + muzzle.forward * maxShootDistance;
        }

        SpawnBulletTrail(targetPoint);
        timeSinceLastShot = 0f; // Reset the cooldown timer
    }

    private void SpawnBulletTrail(Vector3 hitPoint)
    {
        // Instantiation logic to generate the visual effect of the projectile
        if (bulletPrefab != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
            BulletTrail script = bulletObj.GetComponent<BulletTrail>();

            if (script != null)
            {
                script.SetTarget(hitPoint);
            }
            else
            {
                // Failsafe memory management to destroy the object if the tracking script is missing
                Destroy(bulletObj, 3f);
            }
        }
    }
}