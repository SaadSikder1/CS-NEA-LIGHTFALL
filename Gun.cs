using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GunData gunData; // Reference to a ScriptableObject holding weapon stats
    [SerializeField] private Transform cam; // Reference to the main camera for recoil effects
    [SerializeField] private TextMeshProUGUI ammoText; // UI element to display current ammunition

    [Header("Shooting Point")]
    [Tooltip("Drag the empty GameObject at the tip of the barrel here.")]
    public Transform muzzle; 

    [Header("Visuals")]
    [Tooltip("Drag ANY GameObject prefab here (Sphere, Capsule, etc).")]
    [SerializeField] private GameObject bulletPrefab; 

    [Header("Aiming Adjustments")]  
    [SerializeField] private Vector3 aimPos; // Target position when ADS (Aim Down Sights) is active
    [SerializeField] private Vector3 aimRot; // Target rotation for ADS
    [SerializeField] private float aimSpeed = 10f; // Speed of the interpolation between hip-fire and ADS

    [Header("Recoil (Visuals)")] 
    [SerializeField] private float cameraRecoil = 2f; // How much the camera snaps upward
    [SerializeField] private float gunKickRecoil = 5f; // How much the gun model kicks back

    [Header("Audio")] 
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip shootSound;

    // Private variables to store the default state of the weapon model
    private Vector3 originalPos;
    private Quaternion originalRot;
    private float timeSinceLastShot;
    private float currentGunKick;

    private void Start()
    {
        // Capture initial transform data so we can return to it after recoil or aiming
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
        UpdateUI();
    }

    private void OnEnable()
    {
        // Reset reloading state if the weapon is swapped back in
        if (gunData != null) gunData.isReloading = false;
        UpdateUI();
    }

    private void Update()
    {
        if (gunData == null) return;

        // Increment timer by real-time seconds for fire rate calculation
        timeSinceLastShot += Time.deltaTime;

        HandleAiming();

        // Convert Rounds Per Minute (RPM) into a time interval between shots
        float fireDelay = 60f / gunData.fireRate;

        // Check for player input and if enough time has passed to fire again
        if (Input.GetMouseButton(0) && timeSinceLastShot >= fireDelay)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    private void HandleAiming()
    {
        Vector3 targetPos;
        Quaternion targetRot;

        // Selection logic for target transform based on right-click input
        if (Input.GetMouseButton(1))
        {
            targetPos = aimPos;
            targetRot = Quaternion.Euler(aimRot);
        }
        else
        {
            targetPos = originalPos;
            targetRot = originalRot;
        }

        // Smoothly transition position using Linear Interpolation (Lerp)
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * aimSpeed);

        // Apply visual kickback rotation to the target rotation
        Quaternion kickRotation = Quaternion.Euler(-currentGunKick, 0, 0);
        // Smoothly transition rotation using Spherical Linear Interpolation (Slerp)
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot * kickRotation, Time.deltaTime * aimSpeed);

        // Decay the recoil kick back to zero over time
        currentGunKick = Mathf.Lerp(currentGunKick, 0, Time.deltaTime * 5f);
    }

    private void Shoot()
    {
        // Validation check for ammo and reload state
        if (gunData.currentAmmoInMag > 0 && !gunData.isReloading)
        {
            if (muzzle == null) return;

            RaycastHit hit;
            Vector3 targetPoint;

            // Use Physics Raycasting to detect hits in a straight line from the muzzle
            if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, gunData.maxDistance))
            {
                // Interface check: see if the object hit can take damage
                IDamageable target = hit.transform.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(gunData.damage);
                }
                targetPoint = hit.point;
            }
            else
            {
                // If nothing was hit, set a point in the distance for the visual trail
                targetPoint = muzzle.position + muzzle.forward * gunData.maxDistance;
            }

            SpawnBulletTrail(targetPoint);

            if (source != null && shootSound != null) source.PlayOneShot(shootSound);

            // Update state variables after a successful shot
            gunData.currentAmmoInMag--;
            timeSinceLastShot = 0;
            cam.Rotate(-cameraRecoil, 0, 0); // Apply instant camera "snap"
            currentGunKick += gunKickRecoil; // Add to the visual recoil variable

            UpdateUI();
        }
    }

    private void SpawnBulletTrail(Vector3 hitPoint)
    {
        if (bulletPrefab != null)
        {
            // Instantiate creates a clone of the bullet prefab at the muzzle
            GameObject bulletObj = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);

            // Access the specific script on the new object to give it direction
            BulletTrail script = bulletObj.GetComponent<BulletTrail>();

            if (script != null)
            {
                script.SetTarget(hitPoint);
            }
            else
            {
                // Fallback: Ensure the object is deleted if it has no logic to destroy itself
                Destroy(bulletObj, 3f);
            }
        }
    }

    private void Reload()
    {
        // Guard clauses to prevent reloading if mag is full or reserve is empty
        if (gunData.currentAmmoInMag == gunData.magCapacity || gunData.storedAmmo <= 0) return;

        gunData.isReloading = true;

        // Calculate the difference between current mag and full capacity
        int ammoNeeded = gunData.magCapacity - gunData.currentAmmoInMag;

        // Determine if we have enough reserve ammo to fill the mag completely
        if (gunData.storedAmmo >= ammoNeeded)
        {
            gunData.storedAmmo -= ammoNeeded;
            gunData.currentAmmoInMag += ammoNeeded;
        }
        else
        {
            // If reserve is low, just take whatever is left
            gunData.currentAmmoInMag += gunData.storedAmmo;
            gunData.storedAmmo = 0;
        }

        gunData.isReloading = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // String interpolation to format the ammo display
        if (ammoText != null)
        {
            ammoText.text = $"{gunData.currentAmmoInMag} / {gunData.storedAmmo}";
        }
    }

    public void AddAmmo(int amount)
    {
        // Public method to allow external objects (pickups) to modify gun state
        gunData.storedAmmo += amount;
        UpdateUI();
    }
}