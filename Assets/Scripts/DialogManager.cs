using System.Collections;
using UnityEngine;
using TMPro;

public class DialogManager: MonoBehaviour
{
    public TextMeshProUGUI dialogText;   // Texto donde se escribe
    [TextArea(3, 10)]
    public string fullText;              // Texto editable en Inspector
    public float typingSpeed = 0.05f;    // Tiempo entre letras
    public float displayDuration = 10f; // Tiempo total de visualización

    public GameObject cowObject;         // Vaquita para activar animación
    public GameObject speechBubble;

    private Animator cowAnimator;

    void Start()
    {
        // Seguridad: si falta algún campo, lo notificamos
        if (dialogText == null)
        {
            Debug.LogError("No se asignó el campo dialogText (TextMeshProUGUI).");
            return;
        }

        if (cowObject == null)
        {
            Debug.LogError("No se asignó el objeto cowObject.");
            return;
        }

        if (speechBubble == null)
        {
            Debug.LogError("No se asignó el objeto speechBubble.");
            return;
        }

        dialogText.text = "";
        cowAnimator = cowObject.GetComponent<Animator>();

        cowObject.SetActive(true);
        speechBubble.SetActive(true);

        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        cowAnimator.Play("Cow_Text");

        foreach (char c in fullText)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        cowAnimator.Play("Idle");
        
        yield return new WaitForSeconds(displayDuration);

        cowObject.SetActive(false);
        speechBubble.SetActive(false);
    }
}