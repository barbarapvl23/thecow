using UnityEngine;

public class ExitGame : MonoBehaviour
{

    public void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Closing the game...");
    }
}