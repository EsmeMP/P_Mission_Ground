using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorStoryboard : MonoBehaviour
{
    [Header("Historia")]
    public Image imagenHistoria;      // arrastra imageStoryBoard
    public Sprite[] paginas;          // las imágenes del storyboard, en orden

    [Header("Audio")]
    public AudioClip[] audiosPaginas; // Los audios correspondientes a cada página (opcional)
    private AudioSource audioSource;

    [Header("Botón (Opcional en esta escena)")]
    public Button botonSkip;          // arrastra btn_Skip solo si está aquí

    [Header("Siguiente escena")]
    public string escenaSiguiente = "Salones";

    private int indice = 0;

    void Start()
    {
        // Configuramos o buscamos el AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Aseguramos que el objeto esté en la raíz para usar DontDestroyOnLoad sin advertencias
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (paginas.Length > 0 && imagenHistoria != null)
        {
            imagenHistoria.sprite = paginas[0];
            ReproducirAudioActual();
        }

        // Clic en la imagen = siguiente página
        if (imagenHistoria != null)
        {
            Button botonImagen = imagenHistoria.GetComponent<Button>();
            if (botonImagen == null)
                botonImagen = imagenHistoria.gameObject.AddComponent<Button>();
            botonImagen.onClick.AddListener(Siguiente);
        }

        // Skip = solo se conecta si el botón realmente fue asignado en el Inspector
        if (botonSkip != null)
        {
            botonSkip.onClick.AddListener(Saltar);
        }
    }

    public void Siguiente()
    {
        indice++;
        if (indice >= paginas.Length)
        {
            Saltar();
            return;
        }

        if (imagenHistoria != null)
        {
            imagenHistoria.sprite = paginas[indice];
        }

        ReproducirAudioActual();
    }

    void ReproducirAudioActual()
    {
        if (audiosPaginas != null && indice < audiosPaginas.Length && audiosPaginas[indice] != null && audioSource != null)
        {
            audioSource.PlayOneShot(audiosPaginas[indice]);
        }
    }

    public void Saltar()
    {
        SceneManager.LoadScene(escenaSiguiente);

        // Destruimos el objeto del storyboard después de cambiar de escena para limpiar memoria
        Destroy(gameObject, 1.5f);
    }
}