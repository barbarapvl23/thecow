using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeController : MonoBehaviour
{
    public static LifeController instancia;
    public int life = 3; //N�mero de vidas del jugador
    public Transform checkpoint; //Punto de reaparici�n del jugador
    public GameObject player; //Referencia a la vaquita
    public GameObject[] hearts; //Array de objetos de coraz�n para mostrar las vidas
    public GameObject gameOverScreen; //Pantalla de Game Over
    public TMPro.TextMeshProUGUI coinSummaryText; //Texto de Game Over
    private bool estaRespawneando = false; //Para evitar reapariciones m�ltiples
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
            Debug.LogError("No se encontr� el jugador. Aseg�rate de que el objeto con el tag 'Player' existe en la escena.");
        }

        if (checkpoint == null)
        {
            checkpoint = BuscarCheckpoint();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); //Asignar AudioSource si no est� asignado
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Al cargar un nivel nuevo, el jugador y el checkpoint del nivel anterior ya no existen
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines(); //Cancelar una reaparici�n pendiente del nivel anterior
        estaRespawneando = false;

        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player != null ? player.GetComponent<PlayerController>() : null;
        checkpoint = BuscarCheckpoint();

        Debug.Log("LifeController reasignado en " + scene.name + ". Player: " + (player != null) + ", Checkpoint: " + (checkpoint != null));
    }

    private Transform BuscarCheckpoint()
    {
        GameObject cp = null;
        try
        {
            cp = GameObject.FindGameObjectWithTag("Checkpoint"); //Buscar el checkpoint por tag
        }
        catch (UnityException)
        {
            //El tag "Checkpoint" no existe en el proyecto
        }

        if (cp == null)
        {
            cp = GameObject.Find("Checkpoint"); //Buscar el checkpoint por nombre
        }

        return cp != null ? cp.transform : null;
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
            Debug.LogWarning("Ya existe un LifeController, se elimina el duplicado que hay en: " + gameObject.name, gameObject);
            Destroy(this); //Evita duplicados (solo el componente, no el objeto que lo contiene)
        }
    }

    public void LoseLife()
    {
        Debug.Log("LoseLife ejecutado. Vidas actuales: " + life);

        if (life <= 0)
        {
            Debug.LogWarning("Ya no hay vidas al llamar a LoseLife, esto no deber�a pasar.");
            return; //Si no quedan vidas, no hacer nada
        }

        if (estaRespawneando)
        {
            Debug.Log("Ya se est� respawneando, no hacer nada.");
            return; //Evitar reapariciones m�ltiples
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>(); //El jugador cambia en cada nivel
        }

        if (playerController != null)
        {
            playerController.PlayDeathSound(); //Reproducir sonido de muerte
        }
        else
        {
            Debug.LogWarning("No se encontr� el CharacterController en el jugador.");
        }

        life--;
        estaRespawneando = true; //Marcar que se est� respawneando

        Debug.Log("Vida perdida. Vidas restantes: " + life);   //Reducir la vida del jugador

        if (hearts != null && life < hearts.Length)
        {
            hearts[life].SetActive(false); //Desactivar el coraz�n correspondiente
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
                
                if (CoinManager.instance != null)
                {
                    CoinManager.instance.MostrarResumen(gameOverScreen); //Mostrar resumen de monedas del nivel actual
                }
                else
                {
                    Debug.LogWarning("No hay CoinManager para mostrar el resumen de monedas.");
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
        player.transform.position = pos; //Actualizar la posici�n del jugador
        player.SetActive(true); //Activar al jugador
        estaRespawneando = false; //Marcar que ya no se est� respawneando

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