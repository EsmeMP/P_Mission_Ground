using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class FastTyperManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtPalabraActual;
    public TMP_InputField inputRespuesta;
    public TextMeshProUGUI txtContador;
    public TextMeshProUGUI txtNivel;
    public TextMeshProUGUI txtTemporizador;
    public TextMeshProUGUI txtVidas;
    public GameObject panelVictoria;
    public GameObject panelDerrota;
    public Image imgMedallaPremio;

    [Header("Listas de Palabras por Nivel")]
    // Nivel 1: Cortas (Software, Redes, Entornos)
    private string[] palabrasNivel1 = { "git", "int", "bug", "html", "css", "api", "json", "sql", "http", "ping", "ssh", "dns", "dart", "for", "if" };

    // Nivel 2: Medias
    private string[] palabrasNivel2 = { "flutter", "python", "router", "switch", "docker", "query", "server", "token", "async", "class", "print", "debug", "merge", "commit", "subnet", "packet", "port", "script", "cloud", "state" };

    // Nivel 3: Largas y complejas
    private string[] palabrasNivel3 = { "repository", "controller", "virtualization", "inheritance", "polymorphism", "middleware", "architecture", "implementation", "encrypted", "dependencies", "microservices", "infrastructure", "authentication", "asynchronous" };

    private List<string> palabrasActuales = new List<string>();
    private string palabraCorrecta = "";

    private int nivelActual = 1;
    private int palabrasRestantesNivel = 0;
    private int contadorGlobal = 0;
    private int vidas = 3;
    private int erroresConsecutivosEnPalabra = 0;

    private float tiempoRestante = 0f;
    private float tiempoPorPalabra = 2f;
    private bool juegoActivo = false;

    // Colores tipo VS Code (sintaxis de código)
    private Color[] coloresVSCode = new Color[] {
        new Color(0.34f, 0.65f, 0.8f),  // Azul tipo variables/métodos
        new Color(0.85f, 0.65f, 0.38f), // Naranja tipo clases/tipos
        new Color(0.6f, 0.78f, 0.45f),  // Verde tipo strings
        new Color(0.86f, 0.74f, 0.4f),  // Amarillo tipo funciones
        new Color(0.78f, 0.45f, 0.75f)  // Morado tipo palabras reservadas (keywords)
    };

    void Start()
    {
        inputRespuesta.onEndEdit.AddListener(VerificarRespuesta);
        IniciarNivel(1);
    }

    void IniciarNivel(int nivel)
    {
        nivelActual = nivel;
        txtNivel.text = "Nivel: " + nivelActual;
        palabrasActuales.Clear();

        if (nivelActual == 1)
        {
            palabrasActuales.AddRange(palabrasNivel1);
            palabrasRestantesNivel = 15;
            tiempoPorPalabra = 2f;
        }
        else if (nivelActual == 2)
        {
            palabrasActuales.AddRange(palabrasNivel2);
            palabrasRestantesNivel = 30;
            tiempoPorPalabra = 3f;
        }
        else if (nivelActual == 3)
        {
            palabrasActuales.AddRange(palabrasNivel3);
            palabrasRestantesNivel = 50;
            tiempoPorPalabra = 4f;
        }

        juegoActivo = true;
        SiguientePalabra();
    }

    void SiguientePalabra()
    {
        if (contadorGlobal >= 40) // Meta general para ganar el juego globalmente
        {
            GanarJuego();
            return;
        }

        if (palabrasRestantesNivel <= 0)
        {
            if (nivelActual < 3)
            {
                IniciarNivel(nivelActual + 1);
                return;
            }
            else
            {
                GanarJuego();
                return;
            }
        }

        // Seleccionar palabra aleatoria
        int index = Random.Range(0, palabrasActuales.Count);
        palabraCorrecta = palabrasActuales[index];

        // Cambiar color aleatorio simulando la librería de color de sintaxis de VS Code
        txtPalabraActual.color = coloresVSCode[Random.Range(0, coloresVSCode.Length)];
        txtPalabraActual.text = palabraCorrecta;

        inputRespuesta.text = "";
        inputRespuesta.ActivateInputField();

        erroresConsecutivosEnPalabra = 0;
        tiempoRestante = tiempoPorPalabra;
        StopAllCoroutines();
        StartCoroutine(RutinaTiempoPalabra());
    }

    IEnumerator RutinaTiempoPalabra()
    {
        while (tiempoRestante > 0)
        {
            if (!juegoActivo) yield break;
            tiempoRestante -= Time.deltaTime;
            txtTemporizador.text = "Tiempo: " + tiempoRestante.ToString("F1") + "s";
            yield return null;
        }

        // Si se acaba el tiempo, cuenta como error
        ManejarErrorPorTiempoOInvalido();
    }

    void VerificarRespuesta(string textoIngresado)
    {
        if (!juegoActivo) return;

        // Limpiar espacios en blanco
        textoIngresado = textoIngresado.Trim().ToLower();

        if (textoIngresado == palabraCorrecta)
        {
            // ¡Palabra correcta!
            contadorGlobal++;
            palabrasRestantesNivel--;
            txtContador.text = "Aciertos: " + contadorGlobal;
            SiguientePalabra();
        }
        else
        {
            // Palabra mal escrita
            erroresConsecutivosEnPalabra++;

            if (erroresConsecutivosEnPalabra >= 3)
            {
                // A los 3 intentos fallidos en la misma palabra, pierde un corazón
                vidas--;
                ActualizarVidasUI();

                if (vidas <= 0)
                {
                    PerderJuego();
                    return;
                }

                // Pasa a la siguiente palabra perdiendo esta
                SiguientePalabra();
            }
            else
            {
                // Efecto de comentario de código en la UI para la palabra mal escrita: // palabra_mal
                txtPalabraActual.text = "// " + textoIngresado;
                txtPalabraActual.color = Color.red; // Color de comentario de error

                inputRespuesta.text = "";
                inputRespuesta.ActivateInputField();
            }
        }
    }

    void ManejarErrorPorTiempoOInvalido()
    {
        vidas--;
        ActualizarVidasUI();

        if (vidas <= 0)
        {
            PerderJuego();
        }
        else
        {
            // Mostrar como comentario de código por tiempo agotado
            txtPalabraActual.text = "// timeout";
            txtPalabraActual.color = Color.gray;
            SiguientePalabra();
        }
    }

    void ActualizarVidasUI()
    {
        string cor = "";
        for (int i = 0; i < vidas; i++) cor += "❤️ ";
        txtVidas.text = cor;
    }

    void GanarJuego()
    {
        juegoActivo = false;
        StopAllCoroutines();
        panelVictoria.SetActive(true);
        // Aquí se le otorga el premio / medalla simbólica
        Debug.Log("¡Has ganado una medalla de Ingeniería de Software!");
    }

    void PerderJuego()
    {
        juegoActivo = false;
        StopAllCoroutines();
        panelDerrota.SetActive(true);
        // Se le queda una medalla o registro simbólico de participación
    }
}