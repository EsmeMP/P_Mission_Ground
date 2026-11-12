using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Puntaje : MonoBehaviour
{
    public static Puntaje instancia;
    public TextMeshProUGUI texto;
    public GameObject panelResultado;
    public TextMeshProUGUI textoFinal;
    public int puntosParaMedalla = 5;
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
        texto.text = "Puntos: " + puntos;
        panelResultado.SetActive(false);
    }

    public void Sumar()
    {
        puntos++;
        if (puntos >= puntosParaMedalla)
            MedallaGanada = true;

        texto.text = "Puntos: " + puntos;
    }

    public void MostrarResultado()
    {
        if (puntos >= puntosParaMedalla)
            MedallaGanada = true;

        textoFinal.text = "Puntos: " + puntos;
        panelResultado.SetActive(true);
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