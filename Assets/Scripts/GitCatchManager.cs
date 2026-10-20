using UnityEngine;
using TMPro;

public class GitCatchManager : MonoBehaviour
{
    public static GitCatchManager Instancia;

    public int vidas = 3;
    public int puntos = 0;
    public int puntosParaMedalla = 50;

    public SpriteRenderer[] corazones;
    public Sprite corazonLleno;
    public Sprite corazonVacio;

    public TMP_Text textoPuntos;

    [Header("Game Over")]
    public GameObject panelGameOver;
    public TMP_Text textoPuntosFinal;
    public TMP_Text textoTitulo;
    public GameObject textoObjetivo;

    public static bool MedallaGanada;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarMedalla()
    {
        MedallaGanada = false;
    }
    bool medallaLograda;

    void Awake()
    {
        Instancia = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (textoObjetivo != null) textoObjetivo.SetActive(false);
        ActualizarTexto();
    }

    void ActualizarTexto()
    {
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntos;
    }

    public void SumarPunto()
    {
        if (vidas <= 0) return;
        puntos++;
        if (puntos >= puntosParaMedalla) medallaLograda = true;
        ActualizarTexto();
    }

    public void PerderVida()
    {
        if (vidas <= 0) return;

        vidas--;

        if (corazones != null && vidas < corazones.Length && corazones[vidas] != null)
            corazones[vidas].sprite = corazonVacio;

        if (vidas <= 0)
            TerminarJuego();
    }

    void TerminarJuego()
    {
        if (medallaLograda) MedallaGanada = true;

        if (textoTitulo != null)
            textoTitulo.text = medallaLograda ? "Objetivo cumplido!" : "Perdiste!";

        if (textoObjetivo != null)
            textoObjetivo.SetActive(medallaLograda);

        if (textoPuntosFinal != null)
            textoPuntosFinal.text = "Puntos: " + puntos;

        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        Time.timeScale = 0f;
    }
}