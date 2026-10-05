using UnityEngine;
using UnityEngine.SceneManagement;

// Quita una vida si el jugador cae muy por debajo del nivel o, en niveles con "CamLimits", si sale por los lados.
// Se crea solo en cada escena cuyo nombre empieza por "Level"; no hace falta agregarlo a ninguna escena.
// En niveles con camara fija no hay limite lateral: el ancho visible cambia segun la pantalla (PC o celular).
public class LimitesNivel : MonoBehaviour
{
    private const float MargenLateral = 0.5f; // Cuanto puede salir el jugador de los lados antes de morir
    private const float MargenInferior = 3f; // Debajo del limite, para que la DeathZone actue primero

    private Bounds limites;
    private bool limiteLateral; // Solo con CamLimits, que no depende del tamano de la pantalla
    private GameObject player;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Inicializar()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name.StartsWith("Level"))
            {
                new GameObject("LimitesNivel").AddComponent<LimitesNivel>();
            }
        };
    }

    private void Start()
    {
        GameObject camLimits = GameObject.Find("CamLimits");
        Collider2D colliderLimites = camLimits != null ? camLimits.GetComponent<Collider2D>() : null;

        if (colliderLimites != null)
        {
            limites = colliderLimites.bounds;
            limiteLateral = true;
        }
        else if (Camera.main != null)
        {
            //Area visible de la camara ortografica (el alto es igual en cualquier pantalla)
            Camera cam = Camera.main;
            float alto = cam.orthographicSize * 2f;
            limites = new Bounds((Vector2)cam.transform.position, new Vector3(alto * cam.aspect, alto, 0f));
            limiteLateral = false;
        }
        else
        {
            Debug.LogWarning("LimitesNivel: no hay CamLimits ni camara principal, no se aplican limites.");
            enabled = false;
            return;
        }

        Debug.Log("Limites del nivel: y minima " + limites.min.y + (limiteLateral ? ", x " + limites.min.x + " a " + limites.max.x : ", sin limite lateral"));
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
        }

        if (!player.activeInHierarchy || LifeController.instancia == null) return; //Muerto o reapareciendo

        Vector2 pos = player.transform.position;
        bool fuera = pos.y < limites.min.y - MargenInferior;

        if (limiteLateral)
        {
            fuera |= pos.x < limites.min.x - MargenLateral || pos.x > limites.max.x + MargenLateral;
        }

        if (fuera)
        {
            Debug.Log("El jugador salio de los limites del nivel en " + pos);
            LifeController.instancia.LoseLife();
        }
    }
}
