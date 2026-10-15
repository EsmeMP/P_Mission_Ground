using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SeleccionPersonaje : MonoBehaviour
{
    // Guarda quién se eligió (dura mientras el juego está abierto)
    public static string personajeElegido = "Cassandra";

    [Header("Botones")]
    public Button botonCassandra;   // arrastra btn_Cassandra
    public Button botonLeo;         // arrastra btn_Leo

    [Header("Siguiente escena")]
    public string escenaSiguiente = "Salones";

    void Start()
    {
        if (botonCassandra != null)
            botonCassandra.onClick.AddListener(() => Elegir("Cassandra"));

        if (botonLeo != null)
            botonLeo.onClick.AddListener(() => Elegir("Leo"));
    }

    void Elegir(string nombre)
    {
        personajeElegido = nombre;
        SceneManager.LoadScene(escenaSiguiente);
    }
}