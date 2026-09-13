using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HazardDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detectado con: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("La vaquita tocó al robot!");
            LifeController life = LifeController.instancia;
            if (life != null)
            {
                life.LoseLife(); // Llama al método LoseLife del LifeController
            }
            else
            {
                Debug.LogError("No se encontró el componente LifeController en la vaquita.");
            }
        }
    }
}
