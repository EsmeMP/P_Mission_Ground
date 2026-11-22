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

    [Header("Meta y Escena al salir")]
    public int aciertosParaMedalla = 20;
    public string escenaSalon = "Salones";

    // NIVEL 1: Palabras cortas (Primeras 10 palabras)
    private string[] palabrasNivel1 = {
        // Software
        "git", "int", "bug", "html", "css", "api", "sql",
        "for", "if", "var", "json", "php", "dart", "web", "app", "ide",
        // Redes y Ciberseguridad
        "lan", "wan", "vpn", "dns", "ssh", "ping", "ip", "tux", "linux",
        "tcp", "udp", "nat", "mac", "wifi", "vlan", "ssl", "port", "hub",
        // Entornos Virtuales
        "vr", "ar", "3d", "mesh", "uv", "rig", "fps",
        "hud", "npc", "sfx", "vfx", "lod", "rgb", "maya", "game", "luz"
    };

    // NIVEL 2: Palabras medianas (Siguientes 10 palabras)
    private string[] palabrasNivel2 = {
        // Software
        "flutter", "python", "docker", "commit", "branch", "server", "function",
        "return", "string", "object", "method", "sprint", "scrum", "widget", "lambda", "closure", "refactor", "debug", "compile",
        "kanban", "backend", "deploy", "codigo", "github", "script",
        // Redes y Ciberseguridad
        "router", "switch", "subnet", "packet", "firewall", "proxy",
        "gateway", "malware", "cifrado", "telnet", "servidor", "trafico",
        // Entornos Virtuales
        "blender", "shader", "textura", "render", "hitbox", "sprite",
        "prefab", "canvas", "camara", "avatar", "modelo", "escena",
        "collider", "fisicas", "pivote", "unity"
    };

    // NIVEL 3: Comandos y líneas cortas (Infinito hasta perder las 3 vidas)
    private string[] palabrasNivel3 = {
        // Software
        "git init", "git push", "git status", "int x = 0;",
        "return true;", "npm start", "print(x)", "if (x > 0)",
        "while (true)", "code .",
        // Redes y Ciberseguridad
        "ping 8.8.8.8", "ip route", "ssh root", "chmod 777",
        "sudo su", "ipconfig", "vlan 10", "port 443",
        "ufw enable", "netstat",
        // Entornos Virtuales (Unity / 3D)
        "vector2.zero", "destroy(obj)", "setactive(true)", "loadscene(1)",
        "debug.log", "new color()", "anim.play()", "rb.velocity",
        "scale = 1;", "fov = 60;"
    };

    private List<string> palabrasActuales = new List<string>();
    private string palabraCorrecta = "";

    private int nivelActual = 1;
    private int palabrasRestantesNivel = 0;
    private int contadorGlobal = 0;
    private int vidas = 3;

    private float tiempoRestante = 0f;
    private float tiempoPorPalabra = 10f;

    private bool juegoActivo = false;
    private bool medallaLograda = false;

    public static bool MedallaGanada;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarMedalla()
    {
        MedallaGanada = false;
    }

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

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        if (imgMedallaPremio != null)
            imgMedallaPremio.gameObject.SetActive(false);

        txtContador.text = "Aciertos: 0";

        ActualizarVidasUI();

        inputRespuesta.onSubmit.AddListener(VerificarRespuesta);
        inputRespuesta.onValueChanged.AddListener(RevisarMientrasEscribe);

        IniciarNivel(1);
    }

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
        txtNivel.text = "nivel_" + nivelActual + ".ts";
        palabrasActuales.Clear();

        if (nivelActual == 1)
        {
            palabrasActuales.AddRange(palabrasNivel1);
            palabrasRestantesNivel = 10; // Primeras 10 palabras cortas
            tiempoPorPalabra = 10f;
        }
        else if (nivelActual == 2)
        {
            palabrasActuales.AddRange(palabrasNivel2);
            palabrasRestantesNivel = 10; // Siguientes 10 palabras medianas
            tiempoPorPalabra = 11f;
        }
        else
        {
            palabrasActuales.AddRange(palabrasNivel3);
            palabrasRestantesNivel = 999999; // Nivel 3 infinito hasta perder las vidas
            tiempoPorPalabra = 13f;
        }

        juegoActivo = true;
        SiguientePalabra();
    }

    void RellenarBolsaNivelActual()
    {
        if (nivelActual == 1) palabrasActuales.AddRange(palabrasNivel1);
        else if (nivelActual == 2) palabrasActuales.AddRange(palabrasNivel2);
        else
        {
            // En el nivel 3 infinito, si se acaban las líneas, vuelve a rellenar con nivel 3 y 2
            palabrasActuales.AddRange(palabrasNivel3);
            palabrasActuales.AddRange(palabrasNivel2);
        }
    }

    private Color colorActualPalabra;

    void SiguientePalabra()
    {
        if (palabrasRestantesNivel <= 0 && nivelActual < 3)
        {
            IniciarNivel(nivelActual + 1);
            return;
        }

        if (palabrasActuales.Count == 0)
        {
            RellenarBolsaNivelActual();
        }

        int index = Random.Range(0, palabrasActuales.Count);
        palabraCorrecta = palabrasActuales[index];
        palabrasActuales.RemoveAt(index); // No repetir palabras

        colorActualPalabra = coloresVSCode[Random.Range(0, coloresVSCode.Length)];
        txtPalabraActual.color = colorActualPalabra;
        txtPalabraActual.text = palabraCorrecta;

        inputRespuesta.text = "";
        inputRespuesta.ActivateInputField();

        // Cada palabra va descontando poquito tiempo para meter presión (mínimo 3.5s)
        tiempoRestante = tiempoPorPalabra;
        tiempoPorPalabra = Mathf.Max(3.5f, tiempoPorPalabra - 0.35f);

        StopAllCoroutines();
        StartCoroutine(RutinaTiempoPalabra());
    }

    // Se pone en ROJO en tiempo real si vas escribiendo una letra mal
    void RevisarMientrasEscribe(string textoParcial)
    {
        if (!juegoActivo || string.IsNullOrEmpty(palabraCorrecta)) return;

        string limpio = textoParcial.TrimStart().ToLower();

        if (palabraCorrecta.StartsWith(limpio))
        {
            // Va escribiendo bien: vuelve a su color original
            txtPalabraActual.color = colorActualPalabra;
            txtPalabraActual.text = palabraCorrecta;
        }
        else
        {
            // Se equivocó en una letra: se pinta en ROJO de inmediato
            txtPalabraActual.color = new Color(1f, 0.2f, 0.2f);
        }
    }

    IEnumerator RutinaTiempoPalabra()
    {
        while (tiempoRestante > 0)
        {
            if (!juegoActivo)
                yield break;

            tiempoRestante -= Time.deltaTime;
            txtTemporizador.text = "Tiempo: " + Mathf.Max(0f, tiempoRestante).ToString("F1") + "s";

            yield return null;
        }

        StartCoroutine(MostrarErrorYRestarVida(" TIEMPO AGOTADO"));
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

            if (contadorGlobal >= aciertosParaMedalla && !medallaLograda)
            {
                medallaLograda = true;
                MedallaGanada = true;
                ReproducirSonido(sonidoVictoria);
            }

            txtContador.text = "Aciertos: " + contadorGlobal;
            SiguientePalabra();
        }
        else
        {
            // Muestra en ROJO el error medio segundo y quita 1 vida
            StartCoroutine(MostrarErrorYRestarVida("✖ " + textoIngresado + " (" + palabraCorrecta + ")"));
        }
    }

    IEnumerator MostrarErrorYRestarVida(string mensajeError)
    {
        StopCoroutine(nameof(RutinaTiempoPalabra));
        ReproducirSonido(sonidoError);

        vidas--;
        ActualizarVidasUI();

        // Pintar en ROJO brillante el error en el centro de la pantalla
        txtPalabraActual.color = new Color(1f, 0.15f, 0.15f);
        txtPalabraActual.text = mensajeError;
        inputRespuesta.text = "";

        if (vidas <= 0)
        {
            yield return new WaitForSeconds(0.6f);
            TerminarJuego();
        }
        else
        {
            yield return new WaitForSeconds(0.6f);
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

    void TerminarJuego()
    {
        juegoActivo = false;
        StopAllCoroutines();
        OcultarUIJuego();

        if (medallaLograda)
        {
            MedallaGanada = true;

            if (panelVictoria != null)
            {
                ConfigurarPanelFinal(
                    panelVictoria,
                    "¡OBJETIVO CUMPLIDO!",
                    Color.white,
                    true
                );
                panelVictoria.SetActive(true);
            }

            if (imgMedallaPremio != null && panelVictoria != null)
            {
                imgMedallaPremio.transform.SetParent(panelVictoria.transform, false);
                imgMedallaPremio.transform.SetAsLastSibling();
                imgMedallaPremio.rectTransform.anchoredPosition = new Vector2(0f, 125f);
                imgMedallaPremio.gameObject.SetActive(true);
            }
        }
        else
        {
            if (panelDerrota != null)
            {
                ConfigurarPanelFinal(
                    panelDerrota,
                    "¡PERDISTE!",
                    Color.white,
                    false
                );
                panelDerrota.SetActive(true);
            }
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
            imgFondo.color = new Color(0f, 0f, 0f, 0.65f);

        if (panel.transform.childCount == 0)
        {
            TMP_FontAsset fuente = txtPalabraActual != null ? txtPalabraActual.font : null;

            CrearTexto(
                panel.transform,
                "TxtTituloFinal",
                titulo,
                42,
                colorTitulo,
                new Vector2(0f, 15f),
                fuente
            );

            string sub = "ACIERTOS: " + contadorGlobal + "/" + aciertosParaMedalla;

            CrearTexto(
                panel.transform,
                "TxtPuntajeFinal",
                sub,
                26,
                Color.white,
                new Vector2(0f, -40f),
                fuente
            );

            CrearBoton(
                panel.transform,
                "BtnReintentar",
                "REINTENTAR",
                new Vector2(0f, -95f),
                fuente,
                Reintentar
            );

            CrearBoton(
                panel.transform,
                "BtnSalir",
                "SALIR",
                new Vector2(0f, -145f),
                fuente,
                Salir
            );
        }
        else
        {
            // Actualizar puntaje si el panel ya tenía hijos
            Transform txtPuntaje = panel.transform.Find("TxtPuntajeFinal");
            if (txtPuntaje != null)
            {
                TextMeshProUGUI tmp = txtPuntaje.GetComponent<TextMeshProUGUI>();
                if (tmp != null) tmp.text = "ACIERTOS: " + contadorGlobal + "/" + aciertosParaMedalla;
            }
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
        rt.sizeDelta = new Vector2(650f, 60f);

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
            typeof(TextMeshProUGUI),
            typeof(Button)
        );

        btnObj.transform.SetParent(padre, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(250f, 45f);

        TextMeshProUGUI tmp = btnObj.GetComponent<TextMeshProUGUI>();
        if (fuente != null)
            tmp.font = fuente;

        tmp.text = textoBoton;
        tmp.fontSize = 26;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        Button btn = btnObj.GetComponent<Button>();
        btn.targetGraphic = tmp;
        btn.onClick.AddListener(accion);
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Salir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaSalon);
    }
}