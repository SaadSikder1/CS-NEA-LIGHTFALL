using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigationHandler : MonoBehaviour
{
    public GameObject firstSelectedButton;

    void OnEnable()
    {
        // Selects NewCharacterButton when the menu is enabled
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    void Update()
    {
        //If the player clicks the background and nothing is selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // put the selection back on NewCharacterButton
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}