using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel de instrucciones por paginas (titulo + imagen + texto + flechas + continuar).
/// Poner este script en un objeto que SIEMPRE este activo (por ejemplo el Canvas
/// o un objeto vacio "GestorInstrucciones"), NO en el panel que se desactiva.
/// </summary>
public class PanelInstrucciones : MonoBehaviour
{
    [Serializable]
    public class Pagina
    {
        public string titulo;
        [TextArea(2, 6)] public string texto;
        public Sprite imagen;               // opcional: si esta vacio se oculta la imagen
    }

    [Header("Contenido (una entrada = una pagina)")]
    public Pagina[] paginas;

    [Header("Referencias de UI")]
    public GameObject panel;                // el objeto PanelInstrucciones
    public TMP_Text textoTitulo;
    public TMP_Text textoCuerpo;
    public Image imagenPagina;
    public Button botonAnterior;
    public Button botonSiguiente;
    public Button botonContinuar;           // cierra el panel

    [Header("Opciones")]
    public bool mostrarAlIniciar = true;    // aparece al entrar a la escena
    public bool pausarJuego = true;         // Time.timeScale = 0 mientras esta abierto
    public bool soloContinuarEnUltimaPagina = false;
    public bool mostrarSoloUnaVez = false;  // recuerda que ya se vio (PlayerPrefs)
    public string claveGuardado = "Instrucciones_Salones";

    [Header("Ocultar mientras el panel esta abierto (opcional)")]
    public GameObject[] ocultarMientrasAbierto;   // p. ej. AvisoInteraccion, Minimapa, TextoMedallas

    int indice;
    bool abierto;
    float escalaPrevia = 1f;
    bool[] estadoPrevio;

    void Awake()
    {
        if (botonAnterior)   botonAnterior.onClick.AddListener(Anterior);
        if (botonSiguiente)  botonSiguiente.onClick.AddListener(Siguiente);
        if (botonContinuar)  botonContinuar.onClick.AddListener(Cerrar);

        if (panel) panel.SetActive(false);
    }

    void Start()
    {
        if (!mostrarAlIniciar) return;
        if (mostrarSoloUnaVez && PlayerPrefs.GetInt(claveGuardado, 0) == 1) return;
        Abrir();
    }

    /// <summary>Se puede llamar desde otros scripts o desde un boton de ayuda.</summary>
    public void Abrir()
    {
        if (paginas == null || paginas.Length == 0 || panel == null) return;

        indice = 0;
        abierto = true;

        if (pausarJuego)
        {
            escalaPrevia = Time.timeScale;
            Time.timeScale = 0f;
        }

        // Ocultar otros elementos de la UI y recordar como estaban
        estadoPrevio = new bool[ocultarMientrasAbierto.Length];
        for (int i = 0; i < ocultarMientrasAbierto.Length; i++)
        {
            if (!ocultarMientrasAbierto[i]) continue;
            estadoPrevio[i] = ocultarMientrasAbierto[i].activeSelf;
            ocultarMientrasAbierto[i].SetActive(false);
        }

        panel.SetActive(true);
        Refrescar();
    }

    public void Cerrar()
    {
        if (!abierto) return;
        abierto = false;

        panel.SetActive(false);

        for (int i = 0; i < ocultarMientrasAbierto.Length; i++)
            if (ocultarMientrasAbierto[i]) ocultarMientrasAbierto[i].SetActive(estadoPrevio[i]);

        if (pausarJuego) Time.timeScale = escalaPrevia;

        if (mostrarSoloUnaVez)
        {
            PlayerPrefs.SetInt(claveGuardado, 1);
            PlayerPrefs.Save();
        }
    }

    public void Siguiente()
    {
        if (indice < paginas.Length - 1) { indice++; Refrescar(); }
    }

    public void Anterior()
    {
        if (indice > 0) { indice--; Refrescar(); }
    }

    void Refrescar()
    {
        Pagina p = paginas[indice];

        if (textoTitulo) textoTitulo.text = p.titulo;
        if (textoCuerpo) textoCuerpo.text = p.texto;

        if (imagenPagina)
        {
            imagenPagina.sprite = p.imagen;
            imagenPagina.gameObject.SetActive(p.imagen != null);
        }

        bool ultima = indice == paginas.Length - 1;

        if (botonAnterior)  botonAnterior.gameObject.SetActive(indice > 0);
        if (botonSiguiente) botonSiguiente.gameObject.SetActive(!ultima);
        if (botonContinuar) botonContinuar.gameObject.SetActive(!soloContinuarEnUltimaPagina || ultima);
    }

    // Seguridad: si cambias de escena con el panel abierto, no dejes el juego pausado.
    void OnDestroy()
    {
        if (abierto && pausarJuego) Time.timeScale = escalaPrevia;
    }
}