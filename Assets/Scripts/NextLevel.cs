using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public string nextLevelName;
    public GameObject panelYouWin; // Panel que se muestra al ganar

    void Start()
    {
        // Asegura que el panel esté oculto al comenzar
        if (panelYouWin != null)
        {
            panelYouWin.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (panelYouWin != null)
        {
            // Es el último nivel, muestra la pantalla de victoria
            panelYouWin.SetActive(true);
        }
        else
        {
            // Cambia a la siguiente escena
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.LogWarning("No se asignó el nombre de la siguiente escena.");
            }
        }
    }
}