using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    // Takes the index of the currently active weapon
    // Serialised to allow the default starting weapon to be set in Unity
    [SerializeField] private int selectedWeapon = 0;

    private void Start()
    {
        // Initialise the weapon state as soon as the object loads
        SelectWeapon();
    }

    private void Update()
    {
        //Stores the current state locally to check against later
        int previousSelectedWeapon = selectedWeapon;

        //Asks for direct numeric key inputs to jump to specific array indices
        // (Main Keyboard)
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 0;
        
        // Ensures a second weapon actually exists in the hierarchy before attempting to switch to it to pregvent errors
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2) selectedWeapon = 1;

        // (Numpad)
        if (Input.GetKeyDown(KeyCode.Keypad1)) selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Keypad2) && transform.childCount >= 2) selectedWeapon = 1;

        // Scroll wheel handling for cyclic navigation through the weapon inventory
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // If at the end of the collection, wrap around to the start index (0)
            if (selectedWeapon >= transform.childCount - 1) selectedWeapon = 0;
            else selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // If at the beginning of the collection, wrap around to the final index
            if (selectedWeapon <= 0) selectedWeapon = transform.childCount - 1;
            else selectedWeapon--;
        }

        //only trigger the visual update if the selected index has actually changed
        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private void SelectWeapon()
    {
        int i = 0;
        
        //Iterates through all child objects attached to this transform
        foreach (Transform weapon in transform)
        {
            //Enables the GameObject if its index matches the selected weapon, disables it otherwise
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            
            i++;
        }
    }
}