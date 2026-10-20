using UnityEngine;
using TMPro;

public class ContadorMedallas : MonoBehaviour
{
    public static ContadorMedallas Instancia;
    public static int total;
    public int medallasTotales = 3;

    TMP_Text texto;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        total = 0;
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
        if (Instancia != null) Instancia.Actualizar();
    }

    void Actualizar()
    {
        if (texto != null)
            texto.text = "Medallas " + total + "/" + medallasTotales;
    }
}