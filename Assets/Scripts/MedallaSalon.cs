using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MedallaSalon : MonoBehaviour
{
    public Transform jugador;
    public float espera = 1.5f;
    public float duracionVuelo = 1f;
    public float escalaFinal = 0.3f;

    [Header("Sprites de cada medalla (Opcional)")]
    public Sprite medallaDiana; // GitCatch
    public Sprite medallaRuth;  // Memory Match
    public Sprite medallaPaco;  // Fast Typer
    public Sprite medallaSonia; // Flappy VR

    static bool entregadaGitCatch;
    static bool entregadaMemory;
    static bool entregadaFastTyper;
    static bool entregadaFlappy;

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
        Image img = GetComponent<Image>();

        if (GitCatchManager.MedallaGanada && !entregadaGitCatch)
        {
            entregadaGitCatch = true;
            if (img != null && medallaDiana != null) img.sprite = medallaDiana;
            StartCoroutine(Animar());
        }
        else if (GameManager.MedallaGanada && !entregadaMemory)
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

    IEnumerator Animar()
    {
        Vector3 posInicial = transform.position;
        Vector3 escalaInicial = transform.localScale;

        yield return new WaitForSeconds(espera);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionVuelo;
            float suave = t * t;

            Vector3 destino = Camera.main.WorldToScreenPoint(jugador.position);
            transform.position = Vector3.Lerp(posInicial, destino, suave);
            transform.localScale = Vector3.Lerp(escalaInicial, escalaInicial * escalaFinal, suave);
            yield return null;
        }

        ContadorMedallas.Sumar();
        gameObject.SetActive(false);
    }
}