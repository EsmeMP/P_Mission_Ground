using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SeleccionPersonaje : MonoBehaviour
{
    // Guarda quién se eligió (dura mientras el juego está abierto)
    public static string personajeElegido = "Cassandra";
    static bool definidoEnSesion;

    [Header("Botones")]
    public Button botonCassandra;   // arrastra btn_Cassandra
    public Button botonLeo;         // arrastra btn_Leo

    [Header("Siguiente escena")]
    public string escenaSiguiente = "Salones";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarEstaticos()
    {
        personajeElegido = "Cassandra";
        definidoEnSesion = false;
    }

    public static void EstablecerPersonaje(string nombre)
    {
        personajeElegido = nombre;
        definidoEnSesion = true;
        PlayerPrefs.SetString("NombrePersonaje", nombre);
        PlayerPrefs.Save();
    }

    public static bool EsLeo()
    {
        string actual = definidoEnSesion
            ? personajeElegido
            : PlayerPrefs.GetString("NombrePersonaje", "Cassandra");
        return string.Equals(actual, "Leo", System.StringComparison.OrdinalIgnoreCase);
    }

    void Start()
    {
        if (botonCassandra != null)
            botonCassandra.onClick.AddListener(() => Elegir("Cassandra"));

        if (botonLeo != null)
            botonLeo.onClick.AddListener(() => Elegir("Leo"));
    }

    void Elegir(string nombre)
    {
        EstablecerPersonaje(nombre);
        SceneManager.LoadScene(escenaSiguiente);
    }
}