using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Controlador del personaje principal del juego
    Vector2 checkpointPos;
    public Rigidbody2D rigidBody;
    public CapsuleCollider2D capsuleCollider;
    public SpriteRenderer spriteRenderer;
    [Range(1, 500)] public float potenciaSalto;
    public LayerMask sueloLayer;
    Animator animator;

    // Sonidos del personaje
    public AudioClip clip; // Sonido de salto
    public AudioClip clip2; // Sonido de recoger monedas
    public AudioClip clip3; // Sonido de muerte
    public AudioSource audioSource;

    private TouchController touchController; // Controlador de toques para dispositivos móviles

    public int maxSaltos = 2; // Número máximo de saltos permitidos
    private int saltosRestantes; // Contador de saltos restantes
    private bool puedeSaltar = true; // Indica si el personaje puede saltar

    public List<string> inventario; // Lista para almacenar los nombres de las monedas recogidas

    private void Awake()
    {
        var duplicados = FindObjectsOfType<PlayerController>();
        if (duplicados.Length > 1)
        {
            Debug.LogWarning("Hay más de un CharacterController en la escena. Se eliminará este objeto: " + gameObject.name);
            Destroy(gameObject); // Evitar duplicados
        }
        else
        {
            Debug.Log("CharacterController único en la escena: " + gameObject.name);
        }
    }

    private void Start()
    {
        saltosRestantes = maxSaltos; // Inicializar saltos restantes

        checkpointPos = transform.position; // Guardar la posición inicial del personaje
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            Debug.LogError("Rigidbody2D no asignado");
        }
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        inventario = new List<string>(); // Inicializar la lista de monedas

        touchController = FindObjectOfType<TouchController>();
    }

    private void Update()
    {
        float horizontalInput = 0f;

        if (touchController != null) //Movimiento desde touch o teclado
        {
            if (touchController.moveLeft)
                horizontalInput = -1f;
            else if (touchController.moveRight)
                horizontalInput = 1f;
        }
        else
        {
            horizontalInput = Input.GetAxisRaw("Horizontal"); // Obtener entrada horizontal del teclado si no hay touch
        }

        //Movimiento

        float horizontalSpeed = horizontalInput * 5f; // Velocidad horizontal del personaje
        rigidBody.linearVelocity = new Vector2(horizontalSpeed, rigidBody.linearVelocity.y); // Actualizar la velocidad del Rigidbody2D

        animator.SetFloat("Velocidad", Mathf.Abs(horizontalInput)); //Actualizar la velocidad en el Animator
   
        //Flip del personaje
        if (horizontalInput > 0)
           spriteRenderer.flipX = false; // Mirar a la derecha
        else if (horizontalInput < 0f)
           spriteRenderer.flipX = true; // Mirar a la izquierda
                
        //Salto
        bool jumpInput = false;

        if (touchController != null)
            jumpInput = touchController.jump; // Si el botón de salto está presionado en la pantalla táctil
        else
            jumpInput = Input.GetButtonDown("Jump"); // Si se presiona la tecla de salto en el teclado

        if (estaEnSuelo())
        {
            saltosRestantes = maxSaltos; // Reiniciar saltos restantes al tocar el suelo
        }

        if (jumpInput && saltosRestantes > 0 && puedeSaltar)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f); //Reiniciar velocidad vertical
            rigidBody.AddForce(Vector2.up * potenciaSalto, ForceMode2D.Impulse); //Aplicar fuerza de salto
            audioSource.PlayOneShot(clip, 0.5f); //Reproducir sonido de salto
            saltosRestantes--; // Disminuir el contador de saltos restantes
            puedeSaltar = false; // Desactivar salto temporalmente para evitar saltos múltiples
            StartCoroutine(ReactivarSalto()); // Reiniciar la posibilidad de saltar después de un breve tiempo
        }
        
    }

    private bool estaEnSuelo()
    {
        // Verificar si el personaje está tocando el suelo
        return capsuleCollider.IsTouchingLayers(sueloLayer);
    }


    //Este método se llama cuando el collider del personaje entra en contacto con otro collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detectado con: " + collision.gameObject.name);

        if (collision.CompareTag("DeathZone"))
        {
            audioSource.PlayOneShot(clip3, 1.2f); // Sonido de muerte
            return;
        }

        // Lógica de colisiones, como recoger objetos o activar eventos
        if (collision.CompareTag("Collectable"))
        {
            audioSource.PlayOneShot(clip2, 1.2f); // Reproducir sonido de recoger monedas
            string itemType = collision.GetComponent<CollectableScripts>().itemType; // Obtener el tipo de objeto recogible
            print("You've picked up: " + itemType);

            inventario.Add(itemType); // Añadir el tipo de objeto a la lista de monedas recogidas

            CoinManager.instance.AddCoins(); // Actualizar el contador de monedas

            Destroy(collision.gameObject); //Destruir el objeto recogido
        }

        if (collision.CompareTag("Hazard"))
        {
            if (LifeController.instancia != null)
            {
                LifeController.instancia.LoseLife(); // Llamar al método LoseLife del LifeController
            }
        }
    }

    public void PlayDeathSound()
    {
        if (audioSource != null && clip3 != null)
        {
            audioSource.PlayOneShot(clip3, 1.2f); //Reproducir sonido de muerte
            Debug.Log("Sonido de muerte reproducido.");
        }
        else
        {
            Debug.LogWarning("Clip de muerte o AudioSource no asignados.");
        }
    }

    private IEnumerator ReactivarSalto()
    {
        yield return new WaitForSeconds(0.1f);
        puedeSaltar = true;
    }
}
