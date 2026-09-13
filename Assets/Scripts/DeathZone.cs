using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    public AudioClip fallSound; // Sonido que se reproduce al morir
    private AudioSource audioSource;

    private void Start()
    {
        // Obtener el componente AudioSource del GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Verificar si el objeto que entra en la zona de muerte es el jugador
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(fallSound, 1.2f); // Reproducir sonido de caída
            Debug.Log("Objeto entró en la zona de muerte"); //Mensaje para verificar que se ha activado la zona de muerte

            if (LifeController.instancia != null)
            {
                LifeController.instancia.LoseLife(); //Llamar al método para perder una vida
                
            }
            else
            {
                Debug.LogError("LifeController no está instanciado");
            }
        }
    }
}