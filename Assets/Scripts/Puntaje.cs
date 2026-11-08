using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Puntaje : MonoBehaviour
{
    public static Puntaje instancia;
    public TextMeshProUGUI texto;
    public GameObject panelResultado;
    public TextMeshProUGUI textoFinal;
    private int puntos = 0;

    public int Puntos => puntos;

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
        texto.text = "Puntos: " + puntos;
    }

    public void MostrarResultado()
    {
        textoFinal.text = "Puntos: " + puntos;
        panelResultado.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}