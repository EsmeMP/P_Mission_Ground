
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SeleccionManager : MonoBehaviour
{
    [Header("Credencial central")]
    public Image imagenCredencial;
    public Button botonCredencial;

    [Header("Botones de nombres")]
    public Button botonCassandra;
    public Button botonLeo;

    [Header("Imagenes de credenciales")]
    public Sprite imagenCassandra;
    public Sprite imagenLeo;

    [Header("Escena del videojuego")]
    public string nombreEscenaJuego;

    private string personajeSeleccionado;

    void Start()
    {
        // Conectar el boton de Cassandra
        botonCassandra.onClick.RemoveAllListeners();
        botonCassandra.onClick.AddListener(SeleccionarCassandra);

        // Conectar el boton de Leo
        botonLeo.onClick.RemoveAllListeners();
        botonLeo.onClick.AddListener(SeleccionarLeo);

        // Conectar la credencial central
        botonCredencial.onClick.RemoveAllListeners();
        botonCredencial.onClick.AddListener(IniciarJuego);

        // Mostrar Cassandra al iniciar
        SeleccionarCassandra();
    }

    public void SeleccionarCassandra()
    {
        personajeSeleccionado = "Cassandra";

        imagenCredencial.sprite = imagenCassandra;

        Debug.Log("Seleccionaste a Cassandra");
    }

    public void SeleccionarLeo()
    {
        personajeSeleccionado = "Leo";

        imagenCredencial.sprite = imagenLeo;

        Debug.Log("Seleccionaste a Leo");
    }

    public void IniciarJuego()
    {
        if (string.IsNullOrEmpty(personajeSeleccionado))
        {
            Debug.LogWarning("Selecciona un personaje primero");
            return;
        }

        // Guardar personaje seleccionado
        PlayerPrefs.SetString(
            "NombrePersonaje",
            personajeSeleccionado
        );

        PlayerPrefs.Save();

        Debug.Log("Iniciando juego con: " +
                  personajeSeleccionado);

        // Cargar escena del videojuego
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}
