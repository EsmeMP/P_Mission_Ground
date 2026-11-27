
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
        if (botonCassandra != null)
        {
            botonCassandra.onClick.RemoveAllListeners();
            botonCassandra.onClick.AddListener(SeleccionarCassandra);
        }

        // Conectar el boton de Leo
        if (botonLeo != null)
        {
            botonLeo.onClick.RemoveAllListeners();
            botonLeo.onClick.AddListener(SeleccionarLeo);
        }

        // Conectar la credencial central
        if (botonCredencial != null)
        {
            botonCredencial.onClick.RemoveAllListeners();
            botonCredencial.onClick.AddListener(IniciarJuego);
        }

        // Mostrar Cassandra al iniciar
        SeleccionarCassandra();
    }

    public void SeleccionarCassandra()
    {
        personajeSeleccionado = "Cassandra";

        if (imagenCredencial != null && imagenCassandra != null)
            imagenCredencial.sprite = imagenCassandra;

        Debug.Log("Seleccionaste a Cassandra");
    }

    public void SeleccionarLeo()
    {
        personajeSeleccionado = "Leo";

        if (imagenCredencial != null && imagenLeo != null)
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

        // Guardar personaje seleccionado (PlayerPrefs + referencia estática del proyecto)
        SeleccionPersonaje.EstablecerPersonaje(personajeSeleccionado);

        // Reiniciar progreso de medallas y estados para nueva sesión
        ContadorMedallas.total = 0;
        GitCatchManager.MedallaGanada = false;
        MemoryMatchManager.MedallaGanada = false;
        FastTyperManager.MedallaGanada = false;
        Puntaje.MedallaGanada = false;
        Medallas.Reiniciar();
        EstadoJuego.Reiniciar();

        Debug.Log("Iniciando juego con: " +
                  personajeSeleccionado);

        // Cargar escena del videojuego
        SceneManager.LoadScene(string.IsNullOrEmpty(nombreEscenaJuego) ? "Salones" : nombreEscenaJuego);
    }
}
