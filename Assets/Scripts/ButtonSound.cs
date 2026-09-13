using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioClip sonido;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirSonido()
    {
        if (sonido != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonido);
        }
    }

}