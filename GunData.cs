using UnityEngine;

// This creates the menu item to generate data files
[CreateAssetMenu(fileName = "New Gun", menuName = "Weapon System/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Identity")]
    public new string name;

    [Header("Shooting Stats")]
    public float damage;        // Damage per bullet 
    public float maxDistance;   // Range of the gun

    [Header("Ammo Config")]
    public int currentAmmoInMag; // Rounds currently in the gun
    public int magCapacity;      // Max rounds per mag 
    public int storedAmmo;       // Total ammo in backpack 

    [Header("Settings")]
    public float fireRate;       // RPM (e.g. 600)
    public float reloadTime;     // Time in seconds to reload

    [HideInInspector]
    public bool isReloading;
}
