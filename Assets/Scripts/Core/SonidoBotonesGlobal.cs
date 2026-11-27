using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Agrega automáticamente el sonido de clic (aceeldon-click-button-578399) a todos los botones UI
/// de todas las escenas (incluyendo paneles inactivos como Pausa, Game Over e Instrucciones),
/// y usa DontDestroyOnLoad para que el sonido no se corte al cambiar de escena.
/// </summary>
public class SonidoBotonesGlobal : MonoBehaviour
{
    private const string RutaRecursoAudio = "Audio/aceeldon-click-button-578399";

    private static SonidoBotonesGlobal instancia;
    private static readonly HashSet<int> botonesRegistrados = new HashSet<int>();
    private static float ultimoClickTiempo = -1f;

    private AudioSource audioSource;
    private AudioClip clipClick;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        instancia = null;
        botonesRegistrados.Clear();
        ultimoClickTiempo = -1f;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Inicializar()
    {
        if (instancia != null) return;

        GameObject go = new GameObject("SonidoBotonesGlobal");
        DontDestroyOnLoad(go);
        instancia = go.AddComponent<SonidoBotonesGlobal>();
    }

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.ignoreListenerPause = true;

        clipClick = Resources.Load<AudioClip>(RutaRecursoAudio);

        SceneManager.sceneLoaded += OnSceneLoaded;
        RegistrarBotonesEscenaActual();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        botonesRegistrados.Clear();
        RegistrarBotonesEscenaActual();
    }

    void RegistrarBotonesEscenaActual()
    {
        if (clipClick == null) return;

        Button[] botones = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button btn in botones)
        {
            if (btn == null) continue;

            // No poner sonido de botón UI a las cartas de Memory Match
            if (btn.GetComponent<CardController>() != null) continue;

            // Si el botón ya tiene SonidoBoton o llama a BotonSFX en el Inspector, evitar duplicado
            if (btn.GetComponent<SonidoBoton>() != null) continue;
            if (TieneListenerSonidoExistente(btn)) continue;

            int id = btn.GetInstanceID();
            if (botonesRegistrados.Add(id))
            {
                btn.onClick.AddListener(ReproducirClick);
            }
        }
    }

    static bool TieneListenerSonidoExistente(Button btn)
    {
        int count = btn.onClick.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            Object target = btn.onClick.GetPersistentTarget(i);
            if (target is BotonSFX || target is SonidoBoton)
                return true;
        }
        return false;
    }

    public static void ReproducirClick()
    {
        if (instancia == null || instancia.audioSource == null || instancia.clipClick == null)
            return;

        // Evitar doble sonido si dos listeners disparan en el mismo frame
        if (Time.unscaledTime - ultimoClickTiempo < 0.04f)
            return;

        ultimoClickTiempo = Time.unscaledTime;
        instancia.audioSource.PlayOneShot(instancia.clipClick);
    }
}
