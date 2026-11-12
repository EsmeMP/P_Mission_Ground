
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("Sonidos del Juego")]
    public AudioSource audioSource;
    public AudioClip sonidoError;
    public AudioClip sonidoVictoria;

    [Header("Vidas (Arreglo de Corazones)")]
    public Image[] imagenesCorazones;

    public GameObject panelVictoria;
    public GameObject panelDerrota;
    public Image imgMedallaPremio;

    [Header("Escena al salir")]
    public string escenaSalon = "Salones";

    [Header("Listas de Palabras por Nivel")]
    private string[] palabrasNivel1 = {
        "git", "int", "bug", "html", "css", "api", "json",
        "sql", "http", "ping", "ssh", "dns", "dart", "for", "if"
    };

    private string[] palabrasNivel2 = {
        "flutter", "python", "router", "switch", "docker",
        "query", "server", "token", "async", "class",
        "print", "debug", "merge", "commit", "subnet",
        "packet", "port", "script", "cloud", "state"
    };

    private string[] palabrasNivel3 = {
        "repository", "controller", "virtualization",
        "inheritance", "polymorphism", "middleware",
        "architecture", "implementation", "encrypted",
        "dependencies", "microservices", "infrastructure",
        "authentication", "asynchronous"
    };

    private List<string> palabrasActuales = new List<string>();
    private string palabraCorrecta = "";

    private int nivelActual = 1;
    private int palabrasRestantesNivel = 0;
    private int contadorGlobal = 0;
    private int vidas = 3;
    private int erroresConsecutivosEnPalabra = 0;

    private float tiempoRestante = 0f;
    private float tiempoPorPalabra = 8f;

    private bool juegoActivo = false;

    public static bool MedallaGanada;

    private Color[] coloresVSCode = new Color[] {
        new Color(0.34f, 0.65f, 0.8f),
        new Color(0.85f, 0.65f, 0.38f),
        new Color(0.6f, 0.78f, 0.45f),
        new Color(0.86f, 0.74f, 0.4f),
        new Color(0.78f, 0.45f, 0.75f)
    };

    void Start()
    {
        Time.timeScale = 1f;

        MedallaGanada = false;

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        if (imgMedallaPremio != null)
            imgMedallaPremio.gameObject.SetActive(false);

        txtContador.text = "Aciertos: 0";

        ActualizarVidasUI();

        inputRespuesta.onSubmit.AddListener(VerificarRespuesta);

        IniciarNivel(1);
    }

    // MÉTODO PARA REPRODUCIR LOS SONIDOS
    void ReproducirSonido(AudioClip sonido)
    {
        if (audioSource != null && sonido != null)
        {
            audioSource.PlayOneShot(sonido);
        }
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
            tiempoPorPalabra = 8f;
        }
        else if (nivelActual == 2)
        {
            palabrasActuales.AddRange(palabrasNivel2);
            palabrasRestantesNivel = 30;
            tiempoPorPalabra = 10f;
        }
        else if (nivelActual == 3)
        {
            palabrasActuales.AddRange(palabrasNivel3);
            palabrasRestantesNivel = 50;
            tiempoPorPalabra = 12f;
        }

        juegoActivo = true;

        SiguientePalabra();
    }

    void SiguientePalabra()
    {
        if (contadorGlobal >= 40)
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

        int index = Random.Range(0, palabrasActuales.Count);

        palabraCorrecta = palabrasActuales[index];

        txtPalabraActual.color =
            coloresVSCode[Random.Range(0, coloresVSCode.Length)];

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
            if (!juegoActivo)
                yield break;

            tiempoRestante -= Time.deltaTime;

            txtTemporizador.text =
                "Tiempo: " +
                Mathf.Max(0f, tiempoRestante).ToString("F1") + "s";

            yield return null;
        }

        ManejarErrorPorTiempoOInvalido();
    }

    void VerificarRespuesta(string textoIngresado)
    {
        if (!juegoActivo)
            return;

        textoIngresado = textoIngresado.Trim().ToLower();

        if (string.IsNullOrEmpty(textoIngresado))
        {
            inputRespuesta.ActivateInputField();
            return;
        }

        if (textoIngresado == palabraCorrecta)
        {
            contadorGlobal++;

            palabrasRestantesNivel--;

            txtContador.text = "Aciertos: " + contadorGlobal;

            SiguientePalabra();
        }
        else
        {
            // SONIDO DE ERROR AL ESCRIBIR MAL
            ReproducirSonido(sonidoError);

            erroresConsecutivosEnPalabra++;

            if (erroresConsecutivosEnPalabra >= 3)
            {
                vidas--;

                ActualizarVidasUI();

                if (vidas <= 0)
                {
                    PerderJuego();
                    return;
                }

                SiguientePalabra();
            }
            else
            {
                txtPalabraActual.text =
                    "// " + textoIngresado + " (" + palabraCorrecta + ")";

                txtPalabraActual.color = Color.red;

                inputRespuesta.text = "";

                inputRespuesta.ActivateInputField();
            }
        }
    }

    void ManejarErrorPorTiempoOInvalido()
    {
        // SONIDO DE ERROR CUANDO SE ACABA EL TIEMPO
        ReproducirSonido(sonidoError);

        vidas--;

        ActualizarVidasUI();

        if (vidas <= 0)
        {
            PerderJuego();
        }
        else
        {
            txtPalabraActual.text = "// timeout";

            txtPalabraActual.color = Color.gray;

            SiguientePalabra();
        }
    }

    void ActualizarVidasUI()
    {
        if (imagenesCorazones == null)
            return;

        for (int i = 0; i < imagenesCorazones.Length; i++)
        {
            if (imagenesCorazones[i] != null)
            {
                imagenesCorazones[i].gameObject.SetActive(i < vidas);
            }
        }
    }

    void OcultarUIJuego()
    {
        if (txtPalabraActual != null)
            txtPalabraActual.gameObject.SetActive(false);

        if (inputRespuesta != null)
            inputRespuesta.gameObject.SetActive(false);

        if (txtTemporizador != null)
            txtTemporizador.gameObject.SetActive(false);

        if (txtNivel != null)
            txtNivel.gameObject.SetActive(false);

        if (txtContador != null)
            txtContador.gameObject.SetActive(false);

        foreach (var corazon in imagenesCorazones)
        {
            if (corazon != null)
                corazon.gameObject.SetActive(false);
        }
    }

    void GanarJuego()
    {
        juegoActivo = false;

        StopAllCoroutines();

        // SONIDO DE VICTORIA
        ReproducirSonido(sonidoVictoria);

        MedallaGanada = true;

        OcultarUIJuego();

        if (panelVictoria != null)
        {
            ConfigurarPanelFinal(
                panelVictoria,
                "¡Objetivo cumplido!",
                new Color(0.4f, 1f, 0.5f),
                true
            );

            panelVictoria.SetActive(true);
        }

        if (imgMedallaPremio != null)
        {
            imgMedallaPremio.transform.SetAsLastSibling();

            imgMedallaPremio.rectTransform.anchoredPosition =
                new Vector2(0f, 15f);

            imgMedallaPremio.gameObject.SetActive(true);
        }
    }

    void PerderJuego()
    {
        juegoActivo = false;

        StopAllCoroutines();

        OcultarUIJuego();

        if (panelDerrota != null)
        {
            ConfigurarPanelFinal(
                panelDerrota,
                "¡Perdiste!",
                new Color(1f, 0.4f, 0.4f),
                false
            );

            panelDerrota.SetActive(true);
        }
    }

    void ConfigurarPanelFinal(
        GameObject panel,
        string titulo,
        Color colorTitulo,
        bool esVictoria
    )
    {
        Image imgFondo = panel.GetComponent<Image>();

        if (imgFondo != null)
            imgFondo.color = new Color(0f, 0f, 0f, 0.88f);

        if (panel.transform.childCount == 0)
        {
            TMP_FontAsset fuente =
                txtPalabraActual != null ? txtPalabraActual.font : null;

            CrearTexto(
                panel.transform,
                "TxtTituloFinal",
                titulo,
                42,
                colorTitulo,
                new Vector2(0f, 120f),
                fuente
            );

            string sub = esVictoria
                ? "¡Ganaste la medalla!  Aciertos: " + contadorGlobal
                : "Aciertos logrados: " + contadorGlobal;

            CrearTexto(
                panel.transform,
                "TxtPuntajeFinal",
                sub,
                26,
                Color.white,
                new Vector2(0f, 70f),
                fuente
            );

            CrearBoton(
                panel.transform,
                "BtnReintentar",
                "Reintentar",
                new Vector2(-110f, -110f),
                fuente,
                Reintentar
            );

            CrearBoton(
                panel.transform,
                "BtnSalir",
                "Salir",
                new Vector2(110f, -110f),
                fuente,
                Salir
            );
        }
    }

    void CrearTexto(
        Transform padre,
        string nombre,
        string contenido,
        float tamano,
        Color color,
        Vector2 pos,
        TMP_FontAsset fuente
    )
    {
        GameObject obj = new GameObject(
            nombre,
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        obj.transform.SetParent(padre, false);

        RectTransform rt = obj.GetComponent<RectTransform>();

        rt.anchoredPosition = pos;

        rt.sizeDelta = new Vector2(600f, 60f);

        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();

        if (fuente != null)
            tmp.font = fuente;

        tmp.text = contenido;

        tmp.fontSize = tamano;

        tmp.color = color;

        tmp.alignment = TextAlignmentOptions.Center;
    }

    void CrearBoton(
        Transform padre,
        string nombre,
        string textoBoton,
        Vector2 pos,
        TMP_FontAsset fuente,
        UnityEngine.Events.UnityAction accion
    )
    {
        GameObject btnObj = new GameObject(
            nombre,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        btnObj.transform.SetParent(padre, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();

        rt.anchoredPosition = pos;

        rt.sizeDelta = new Vector2(170f, 45f);

        Image img = btnObj.GetComponent<Image>();

        img.color = new Color(0.2f, 0.25f, 0.35f, 1f);

        Button btn = btnObj.GetComponent<Button>();

        btn.onClick.AddListener(accion);

        CrearTexto(
            btnObj.transform,
            "Texto",
            textoBoton,
            24,
            Color.white,
            Vector2.zero,
            fuente
        );
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void Salir()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(escenaSalon);
    }
}
