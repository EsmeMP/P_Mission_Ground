using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ContadorMedallas : MonoBehaviour
{
    public static ContadorMedallas Instancia;
    public static int total;
    static bool transicionIniciada;
    public int medallasTotales = 4;
    [Header("Al completar Planta Baja")]
    public string escenaAlCompletar = "Intro";

    TMP_Text texto;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        total = 0;
        transicionIniciada = false;
    }

    void Awake()
    {
        Instancia = this;
        texto = GetComponent<TMP_Text>();
    }

    void Start()
    {
        Actualizar();
    }

    public static void Sumar()
    {
        total++;
        if (Instancia == null) return;

        Instancia.Actualizar();
        if (!transicionIniciada && total >= Instancia.medallasTotales)
        {
            transicionIniciada = true;
            SceneManager.LoadScene(Instancia.escenaAlCompletar);
        }
    }

    void Actualizar()
    {
        if (texto != null)
            texto.text = total + "/" + medallasTotales;
    }
}
