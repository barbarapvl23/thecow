using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public Text coinText;
    private int coins = 0;
    public int totalMonedasJuego = 40; // Monedas que hay en todo el juego (Level3: 7 + Level4: 33). Actualizar si se agregan monedas

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Se mantiene entre escenas
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }
    public void AddCoins()
    {
        coins++;
        
        if (coinText == null)
        {
            Debug.LogWarning("CoinText no asignado, buscando en escena...");
            GameObject newText = GameObject.Find("CoinText");
            if (newText != null)
            {
                coinText = newText.GetComponent<Text>();
                Debug.Log("CoinText encontrado y asignado.");
            }
        }
        
        if (coinText != null)
        {
            coinText.text = "x " + coins; // Actualiza el texto de monedas
        }
        else
        {
            Debug.LogError("No fue posible asignar el texto de monedas");
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject newText = GameObject.Find("CoinText");
        if (newText != null)
        {
            coinText = newText.GetComponent<Text>();
            coinText.text = "x " + coins;
            Debug.Log("CoinText asignado al cargar la escena: " + scene.name);
        }
        else
        {
            Debug.LogWarning("CoinText no encontrado al cargar la escena: " + scene.name);
        }
    }

    public void ResetCoins()
    {
        coins = 0;
        if (coinText != null)
        {
            coinText.text = "x " + coins; // Resetea el texto de monedas
        }
        else
        {
            Debug.LogWarning("CoinText no asignado al resetear las monedas");
        }
    }

    // Total de monedas que hay en todo el juego
    public int GetTotalCoins()
    {
        return totalMonedasJuego;
    }

    public int GetCollectedCoins()
    {
        return coins;
    }

    // Escribe el resumen de monedas en el texto "CoinSummaryText" que haya dentro del panel
    public void MostrarResumen(GameObject panel)
    {
        if (panel == null) return;

        foreach (TMP_Text texto in panel.GetComponentsInChildren<TMP_Text>(true))
        {
            if (texto.name.StartsWith("CoinSummaryText"))
            {
                texto.text = "Coins Collected " + coins + "/" + GetTotalCoins();
                return;
            }
        }

        Debug.LogWarning("No se encontro CoinSummaryText dentro de " + panel.name);
    }
}
