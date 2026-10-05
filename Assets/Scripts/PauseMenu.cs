using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Va en el boton "Pause" que esta dentro del Canvas del nivel.
// Al pulsarlo pausa el juego y muestra un panel con los botones Continuar, Sonido y Salir.
[RequireComponent(typeof(Button))]
public class PauseMenu : MonoBehaviour
{
    private const string PrefSonido = "SonidoActivado";

    [Header("Imagenes de los botones del panel")]
    public Sprite spriteContinuar;
    public Sprite spriteSilenciar; // Se muestra mientras el sonido esta activado
    public Sprite spriteActivarSonido; // Opcional: se muestra mientras el sonido esta apagado

    public Sprite spriteSalir;

    [Header("Apariencia")]
    public float tamanoBotones = 200f;
    public float separacion = 60f;

    private GameObject panelPausa;
    private Image imagenBotonSonido;
    private bool enPausa = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Inicializar()
    {
        //Aplicar la preferencia de sonido guardada
        AudioListener.volume = PlayerPrefs.GetInt(PrefSonido, 1) == 1 ? 1f : 0f;

        //Nunca empezar un nivel en pausa
        SceneManager.sceneLoaded += (scene, mode) => Time.timeScale = 1f;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Pausar);
        CrearPanel();
    }

    private void Update()
    {
        //Esc en PC, boton "atras" en Android
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (enPausa) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        enPausa = true;
        Time.timeScale = 0f; //Detiene fisicas, animaciones y corrutinas con WaitForSeconds
        panelPausa.SetActive(true);
    }

    public void Reanudar()
    {
        enPausa = false;
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
    }

    public void AlternarSonido()
    {
        bool activado = AudioListener.volume == 0f;
        AudioListener.volume = activado ? 1f : 0f;
        PlayerPrefs.SetInt(PrefSonido, activado ? 1 : 0);
        ActualizarBotonSonido();
    }

    public void SalirDelJuego()
    {
        Debug.Log("Closing the game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //En el editor, detener el modo Play
#endif
    }

    private void ActualizarBotonSonido()
    {
        bool activado = AudioListener.volume > 0f;

        if (!activado && spriteActivarSonido != null)
        {
            imagenBotonSonido.sprite = spriteActivarSonido;
            imagenBotonSonido.color = Color.white;
        }
        else
        {
            imagenBotonSonido.sprite = spriteSilenciar;
            //Sin imagen de "activar sonido", se oscurece el boton para indicar que esta silenciado
            imagenBotonSonido.color = activado ? Color.white : new Color(0.55f, 0.55f, 0.55f);
        }
    }

    // ---------- Construccion del panel ----------

    private void CrearPanel()
    {
        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;

        //Fondo oscuro a pantalla completa, por encima del resto de la interfaz
        panelPausa = new GameObject("PanelPausa", typeof(RectTransform), typeof(Image));
        panelPausa.transform.SetParent(canvas.transform, false);
        panelPausa.transform.SetAsLastSibling();
        RectTransform rtPanel = panelPausa.GetComponent<RectTransform>();
        rtPanel.anchorMin = Vector2.zero;
        rtPanel.anchorMax = Vector2.one;
        rtPanel.offsetMin = rtPanel.offsetMax = Vector2.zero;
        panelPausa.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);

        //Fila central con los botones
        GameObject fila = new GameObject("Opciones", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        fila.transform.SetParent(panelPausa.transform, false);
        fila.GetComponent<RectTransform>().sizeDelta = new Vector2(tamanoBotones * 3 + separacion * 2, tamanoBotones);
        HorizontalLayoutGroup layout = fila.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = separacion;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = layout.childControlHeight = false;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;

        CrearBoton(fila.transform, "Continuar", spriteContinuar).onClick.AddListener(Reanudar);

        Button botonSonido = CrearBoton(fila.transform, "Sonido", spriteSilenciar);
        botonSonido.onClick.AddListener(AlternarSonido);
        imagenBotonSonido = botonSonido.GetComponent<Image>();
        ActualizarBotonSonido();

        CrearBoton(fila.transform, "Salir", spriteSalir).onClick.AddListener(SalirDelJuego);

        panelPausa.SetActive(false);
    }

    private Button CrearBoton(Transform padre, string nombre, Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("PauseMenu: falta asignar la imagen del boton " + nombre, this);
        }

        GameObject go = new GameObject("Boton" + nombre, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(padre, false);
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(tamanoBotones, tamanoBotones);

        Image imagen = go.GetComponent<Image>();
        imagen.sprite = sprite;
        imagen.preserveAspect = true;

        return go.GetComponent<Button>();
    }
}
