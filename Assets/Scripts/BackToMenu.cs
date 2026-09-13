using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        // Cargar la escena del menú principal
        SceneManager.LoadScene("Menu");
        LifeController.instancia.ResetGame(); // Reiniciar el juego
    }
}
