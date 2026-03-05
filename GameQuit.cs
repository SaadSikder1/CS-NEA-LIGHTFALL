using UnityEngine;

public class GameQuit : MonoBehaviour
{
    public void GameQuit()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
