using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MedallaSalon : MonoBehaviour
{
    const string RutaSonidoMedalla = "Medallas/Medalla_Sonido";

    public Transform jugador;
    public float espera = 1f;
    public float duracionVuelo = 0.65f;
    public float escalaFinal = 0.35f;

    [Header("Posición y efecto sobre el jugador")]
    public Vector2 offsetSobreJugador = new Vector2(0f, 2.4f);
    public float duracionAparicion = 0.25f;
    public float amplitudFlote = 0.15f;

    [Header("Sprites de cada medalla (Opcional)")]
    public Sprite medallaDiana; // GitCatch
    public Sprite medallaRuth;  // Memory Match
    public Sprite medallaPaco;  // Fast Typer
    public Sprite medallaSonia; // Flappy VR

    static bool entregadaGitCatch;
    static bool entregadaMemory;
    static bool entregadaFastTyper;
    static bool entregadaFlappy;

    AudioSource audioSource;
    AudioClip sonidoMedalla;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        entregadaGitCatch = false;
        entregadaMemory = false;
        entregadaFastTyper = false;
        entregadaFlappy = false;
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        sonidoMedalla = Resources.Load<AudioClip>(RutaSonidoMedalla);

        Image img = GetComponent<Image>();

        if (GitCatchManager.MedallaGanada && !entregadaGitCatch)
        {
            entregadaGitCatch = true;
            if (img != null && medallaDiana != null) img.sprite = medallaDiana;
            StartCoroutine(Animar());
        }
        else if (MemoryMatchManager.MedallaGanada && !entregadaMemory)
        {
            entregadaMemory = true;
            if (img != null && medallaRuth != null) img.sprite = medallaRuth;
            StartCoroutine(Animar());
        }
        else if (FastTyperManager.MedallaGanada && !entregadaFastTyper)
        {
            entregadaFastTyper = true;
            if (img != null && medallaPaco != null) img.sprite = medallaPaco;
            StartCoroutine(Animar());
        }
        else if (Puntaje.MedallaGanada && !entregadaFlappy)
        {
            entregadaFlappy = true;
            if (img != null && medallaSonia != null) img.sprite = medallaSonia;
            StartCoroutine(Animar());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    Vector3 MundoAPantalla(Vector3 posicionMundo)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(posicionMundo);
        pos.z = 0f;
        return pos;
    }

    IEnumerator Animar()
    {
        Vector3 escalaInicial = transform.localScale;
        transform.localScale = Vector3.zero;

        // Esperamos un frame (y a que no esté pausado) para que CameraFollow ya haya centrado la cámara en el jugador
        yield return null;
        while (Time.timeScale == 0f)
        {
            yield return null;
        }

        bool seguirJugador = jugador != null && Camera.main != null;

        // 1. Aparición suave justo encima del jugador
        float tAparicion = 0f;
        while (tAparicion < 1f && duracionAparicion > 0f)
        {
            tAparicion += Time.deltaTime / duracionAparicion;
            float u = Mathf.Clamp01(tAparicion);
            float rebote = 1f + 2.2f * Mathf.Pow(u - 1f, 3f) + 1.2f * Mathf.Pow(u - 1f, 2f);

            if (seguirJugador)
            {
                transform.position = MundoAPantalla(jugador.position + (Vector3)offsetSobreJugador);
            }

            transform.localScale = escalaInicial * rebote;
            yield return null;
        }

        transform.localScale = escalaInicial;

        // 2. Pequeña pausa flotando sobre el jugador para que se aprecie la medalla
        float tiempoEspera = 0f;
        while (tiempoEspera < espera)
        {
            tiempoEspera += Time.deltaTime;
            if (seguirJugador)
            {
                float flote = Mathf.Sin(tiempoEspera * Mathf.PI * 2f) * amplitudFlote;
                Vector3 posMundo = jugador.position + (Vector3)offsetSobreJugador + Vector3.up * flote;
                transform.position = MundoAPantalla(posMundo);
            }
            yield return null;
        }

        // 3. Vuelo directo hacia el jugador (aunque no se mueva)
        Vector3 posInicial = transform.position;
        if (sonidoMedalla != null)
            SonidoBotonesGlobal.ReproducirClip(sonidoMedalla, 0.8f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionVuelo;
            float u = Mathf.Clamp01(t);
            float suave = u * u;

            if (seguirJugador)
            {
                Vector3 destino = MundoAPantalla(jugador.position);
                transform.position = Vector3.Lerp(posInicial, destino, suave);
            }

            transform.localScale = Vector3.Lerp(escalaInicial, escalaInicial * escalaFinal, suave);
            yield return null;
        }

        ContadorMedallas.Sumar();
        gameObject.SetActive(false);
    }
}
