
using UnityEngine;

public class SonidoBoton : MonoBehaviour
{
    public AudioClip sonidoClick;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void ReproducirSonido()
    {
        if (sonidoClick == null) return;

        if (sonidoClick.name == "aceeldon-click-button-578399")
        {
            SonidoBotonesGlobal.ReproducirClick();
            return;
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(sonidoClick);
        }
    }
}
