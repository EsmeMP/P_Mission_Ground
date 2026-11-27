using UnityEngine;
using UnityEngine.SceneManagement;

// Música de fondo de Planta Alta: suena en loop y NO se reinicia al cambiar entre
// PlantaAlta y sus 4 minijuegos. En escenas de Planta Baja / Menú permanece en silencio.
// Canción: Assets/Resources/Musica/Musica_Fondo (mp3/ogg/wav)
public class MusicaFondo : MonoBehaviour
{
    // ---- Config rápida ----
    const string Ruta = "Musica/Musica_Fondo";
    const float Volumen = 0.5f;          // volumen normal (0 a 1)
    const float FactorEnPausa = 0.25f;   // en pausa suena al 25% de su volumen
    const float DuracionFadeIn = 1.5f;   // segundos para subir al inicio

    static MusicaFondo instancia;
    AudioSource fuente;
    float volumenActual;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarEstaticos()
    {
        instancia = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Arrancar()
    {
        if (instancia != null) return;
        GameObject go = new GameObject("MusicaFondo");
        DontDestroyOnLoad(go); // sobrevive a los cambios de escena
        instancia = go.AddComponent<MusicaFondo>();
    }

    static bool EsEscenaPlantaAlta(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena)) return false;
        string n = nombreEscena.Trim().ToLowerInvariant();
        return n == "plantaalta"
            || n == "minijuegocables"
            || n == "minijuego_laberinto"
            || n == "minijuego_oracle"
            || n == "minijuego_rompecabezas";
    }

    void Start()
    {
        AudioClip clip = Resources.Load<AudioClip>(Ruta);
        if (clip == null)
        {
            Debug.LogWarning("[MusicaFondo] No encuentro Assets/Resources/" + Ruta + " (mp3/ogg/wav).");
            return;
        }

        fuente = gameObject.AddComponent<AudioSource>();
        fuente.clip = clip;
        fuente.loop = true;
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f; // 2D
        fuente.volume = 0f;
    }

    void Update()
    {
        if (fuente == null) return;

        bool activa = EsEscenaPlantaAlta(SceneManager.GetActiveScene().name);
        if (!activa)
        {
            if (fuente.isPlaying)
            {
                volumenActual = 0f;
                fuente.volume = 0f;
                fuente.Pause();
            }
            return;
        }

        if (!fuente.isPlaying)
            fuente.Play();

        // Sube con fade al inicio y baja suave cuando se pausa el juego
        float objetivo = Volumen * (PausaMenu.Pausado ? FactorEnPausa : 1f);
        float velocidad = Volumen / DuracionFadeIn;
        volumenActual = Mathf.MoveTowards(volumenActual, objetivo, velocidad * Time.unscaledDeltaTime);
        fuente.volume = volumenActual;
    }
}