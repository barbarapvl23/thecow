using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public Text coinText;
    private int coins = 0;
    private int totalCoins = 0; // Total de monedas recogidas en el juego

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

        GameObject[] coinObjects = GameObject.FindGameObjectsWithTag("Coin");
        SetTotalCoins(coinObjects.Length); // Actualiza el total de monedas en la escena
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

    public void SetTotalCoins(int total)
    {
        totalCoins = total;
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }

    public int GetCollectedCoins()
    {
        return coins;
    }
}