using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public GameObject newPlayer;
    public Transform newCheckpoint;
    public GameObject[] newHearts;
    public GameObject gameOverScreen;
    public GameObject winnerScreen;

    void Start()
    {
        if (LifeController.instancia != null)
        {
            LifeController.instancia.player = newPlayer;
            LifeController.instancia.player = winnerScreen;
            LifeController.instancia.checkpoint = newCheckpoint;
            LifeController.instancia.hearts = newHearts;
            LifeController.instancia.gameOverScreen = gameOverScreen;
           
            //Activar corazones según vidas actuales
            for (int i = 0; i < newHearts.Length; i++)
            {
                newHearts[i].SetActive(i < LifeController.instancia.life);
            }
            Debug.Log("SceneSetup sincronizado. Vidas actuales " + LifeController.instancia.life);
        }
        else
        {
            Debug.LogWarning("LifeController no encontrado en SceneSetup.");
        }
    }
}