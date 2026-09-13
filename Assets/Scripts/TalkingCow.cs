using UnityEngine;

public class TalkingCowIntro : MonoBehaviour
{
    public GameObject cowObject;        // Asigna aquí el GameObject de la vaca parlante
    public GameObject speechBubble;     // Asigna aquí el GameObject de la burbuja de texto
    public float displayDuration = 10f; // Tiempo en segundos que se muestra

    private Animator cowAnimator;

    void Start()
    {
        // Activar ambos objetos
        cowObject.SetActive(true);
        speechBubble.SetActive(true);

        // Obtener el Animator de la vaca y reproducir la animación
        cowAnimator = cowObject.GetComponent<Animator>();
        if (cowAnimator != null)
        {
            cowAnimator.Play("Cow_Text"); // Asegúrate que el nombre coincide con tu animación
        }

        // Ocultar después de cierto tiempo
        Invoke("HideCowText", displayDuration);
    }

    void HideCowText()
    {
        cowObject.SetActive(false);
        speechBubble.SetActive(false);
    }
}
