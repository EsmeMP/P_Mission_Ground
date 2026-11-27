using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Puntaje : MonoBehaviour
{
    public static Puntaje instancia;
    public TextMeshProUGUI texto;

    [Header("Panel Resultado (Estilo Diana)")]
    public GameObject panelResultado;
    public TMP_Text textoTitulo;        //  TextoPerdiste
    public TMP_Text textoFinal;         //  TextoPuntosFinal
    public GameObject medallaImagen;    //  MedallaImagen

    public int puntosParaMedalla = 10;
    public string escenaRegreso = "Biblioteca";

    private int puntos = 0;
    public int Puntos => puntos;

    public static bool MedallaGanada;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarMedalla()
    {
        MedallaGanada = false;
    }

    void Awake()
    {
        instancia = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (texto != null) texto.text = "Puntos: " + puntos;
        if (panelResultado != null) panelResultado.SetActive(false);
        if (medallaImagen != null) medallaImagen.SetActive(false);
    }

    public void Sumar()
    {
        puntos++;
        if (puntos >= puntosParaMedalla)
            MedallaGanada = true;

        if (texto != null)
            texto.text = "Puntos: " + puntos;
    }

    public void MostrarResultado()
    {
        bool lograda = puntos >= puntosParaMedalla;
        if (lograda) MedallaGanada = true;

        if (textoTitulo != null)
            textoTitulo.text = lograda ? "¡OBJETIVO CUMPLIDO!" : "¡PERDISTE!";

        if (medallaImagen != null)
            medallaImagen.SetActive(lograda);

        if (textoFinal != null)
            textoFinal.text = "PUNTOS: " + puntos + "/" + puntosParaMedalla;

        if (panelResultado != null)
        {
            FuenteDialogos.AplicarEnPanel(panelResultado);
            panelResultado.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Salir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaRegreso);
    }
}