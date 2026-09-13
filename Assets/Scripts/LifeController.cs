using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeController : MonoBehaviour
{
    public static LifeController instancia;
    public int life = 3; //Número de vidas del jugador
    public Transform checkpoint; //Punto de reaparición del jugador
    public GameObject player; //Referencia a la vaquita
    public GameObject[] hearts; //Array de objetos de corazón para mostrar las vidas
    public GameObject gameOverScreen; //Pantalla de Game Over
    public TMPro.TextMeshProUGUI coinSummaryText; //Texto de Game Over
    private bool estaRespawneando = false; //Para evitar reapariciones múltiples
    public AudioSource audioSource; //Fuente de audio para reproducir sonidos
    private PlayerController playerController;

    private void Start()
    {
        Debug.Log("LifeController iniciado con " + life + " vidas.");

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player"); //Buscar al jugador por tag
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else 
        {
            Debug.LogError("No se encontró el jugador. Asegúrate de que el objeto con el tag 'Player' existe en la escena.");
        }

        if (checkpoint == null)
        {
            checkpoint = GameObject.FindGameObjectWithTag("Checkpoint").transform; //Buscar el checkpoint por tag
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); //Asignar AudioSource si no está asignado
        }

        playerController = player.GetComponent<PlayerController>();

    }

    void Awake()
    {
        Debug.Log("LifeController creado en: " + gameObject.scene.name);
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); //No destruir este objeto al cambiar de escena
            Debug.Log("Usando este LifeController (vidas: " + life + ")");
        }
        else
        {
            Debug.Log("Ya existe uno, este se destruye.");
            Destroy(gameObject); //Evita duplicados
        }
    }

    public void LoseLife()
    {
        Debug.Log("LoseLife ejecutado. Vidas actuales: " + life);

        if (life <= 0)
        {
            Debug.LogWarning("Ya no hay vidas al llamar a LoseLife, esto no debería pasar.");
            return; //Si no quedan vidas, no hacer nada
        }

        if (estaRespawneando)
        {
            Debug.Log("Ya se está respawneando, no hacer nada.");
            return; //Evitar reapariciones múltiples
        }

        if (playerController != null)
        {
            playerController.PlayDeathSound(); //Reproducir sonido de muerte
        }
        else
        {
            Debug.LogWarning("No se encontró el CharacterController en el jugador.");
        }

        life--;
        estaRespawneando = true; //Marcar que se está respawneando

        Debug.Log("Vida perdida. Vidas restantes: " + life);   //Reducir la vida del jugador

        if (hearts != null && life < hearts.Length)
        {
            hearts[life].SetActive(false); //Desactivar el corazón correspondiente
        }
        else
        {
            Debug.LogWarning("Hearts no asignado o vida fuera de rango");
        }

        if (life <= 0)
        {
            Debug.Log("No quedan vidas. GameOver!");

            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(true);
                Debug.Log("Pantalla de Game Over activada.");
                
                if (coinSummaryText != null && CoinManager.instance != null)
                {
                    int totalCoins = CoinManager.instance.GetTotalCoins(); //Total de monedas recogidas en el juego
                    int collectedCoins = CoinManager.instance.GetCollectedCoins(); //Monedas recogidas en la escena actual
                    coinSummaryText.text = "Collected Coins: " + collectedCoins + " / " + totalCoins; //Mostrar resumen de monedas
                }
                else
                {
                    Debug.LogWarning("coinsSummaryText no asignado.");
                }
            }
            else
            {
                Debug.Log("GameOverScreen no asignado.");
            }

            if (player != null)
            {
                player.SetActive(false); //Desactivar al jugador
            }
        }
        else
        {
            if (player != null && checkpoint != null) //Reubicar al jugador en el checkpoint
            {
                player.SetActive(false); //Desactivar al jugador antes de reaparecer
                StartCoroutine(Reaparecer());
            }
            else
            {
                Debug.LogWarning("Player o Checkpoint no asignados");
            }
        }
    }

    private IEnumerator Reaparecer()
    {
        yield return new WaitForSeconds(0.5f); //Esperar 0.5 segundos antes de reaparecer

        Vector3 pos = checkpoint.position + new Vector3(0, 0.5f, 0); //Reubicar al jugador en el checkpoint
        player.transform.position = pos; //Actualizar la posición del jugador
        player.SetActive(true); //Activar al jugador
        estaRespawneando = false; //Marcar que ya no se está respawneando

        Debug.Log("Jugador reaparecido en el checkpoint: " + checkpoint.position);
    }

    public void ResetGame()
    {
        life = 3;
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(true); //Activar todos los corazones
        }

        if (CoinManager.instance != null)
        {
            CoinManager.instance.ResetCoins(); //Resetear las monedas
        }

        if (player != null && checkpoint != null)
        {
            player.transform.position = checkpoint.position + new Vector3(0, 0.5f, 0); //Reubicar al jugador en el checkpoint
            player.SetActive(true); //Activar al jugador
        }

        Debug.Log("Juego reiniciado. Vidas: " + life);
    }
}