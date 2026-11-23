using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PuertaInteraccion : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string nombreEscenaDestino = "Biblioteca";
    [TextArea(2, 3)]
    public string mensajePregunta = "¿Deseas ir a la Biblioteca?";

    [Header("Referencias del Jugador y Rango")]
    public Transform jugador;
    public float radioInteraccion = 3f;
    public GameObject avisoTeclaE;

    [Header("Efecto de Iluminación / Pulso")]
    public SpriteRenderer spriteIluminado;
    public bool pulsarColor = true;
    public Color colorNormal = Color.white;
    public Color colorBrillo = new Color(0.12f, 0.78f, 0.15f, 1f); // Verde brillante del letrero
    public float velocidadPulso = 3f;

    [Header("Efecto Flotante Opcional (ej. Flecha sobre la puerta)")]
    public Transform objetoFlotante;
    public float amplitudFlote = 0.15f;
    public float velocidadFlote = 4f;

    [Header("Panel de Confirmación (UI)")]
    public GameObject panelConfirmacion;
    public TMP_Text textoPregunta;
    public Button botonSi;
    public Button botonNo;

    Vector3 posInicialFlotante;
    bool panelAbierto = false;

    void Reset()
    {
        // Se ejecuta automáticamente al añadir el componente o darle a "Reset" en el Inspector
        nombreEscenaDestino = "Biblioteca";
        mensajePregunta = "¿Deseas ir a la Biblioteca?";
        radioInteraccion = 3f;
        pulsarColor = true;
        colorNormal = Color.white;
        colorBrillo = new Color(0.12f, 0.78f, 0.15f, 1f);
        velocidadPulso = 3f;
        amplitudFlote = 0.15f;
        velocidadFlote = 4f;

        // Auto-encontrar jugador si existe
        if (jugador == null)
        {
            GameObject p = GameObject.Find("cassandra") ?? GameObject.FindWithTag("Player");
            if (p != null) jugador = p.transform;
        }

        // Auto-asignar sprite del mismo objeto si tiene uno
        if (spriteIluminado == null)
            spriteIluminado = GetComponent<SpriteRenderer>();

        // Auto-asignar avisoTeclaE si existe en escena
        if (avisoTeclaE == null)
            avisoTeclaE = GameObject.Find("AvisoInteraccion");

        // Auto-asignar panel de confirmación si existe
        if (panelConfirmacion == null)
        {
            panelConfirmacion = GameObject.Find("PanelConfirmacionPuerta");
            if (panelConfirmacion != null)
            {
                textoPregunta = panelConfirmacion.GetComponentInChildren<TMP_Text>();
                Button[] botones = panelConfirmacion.GetComponentsInChildren<Button>(true);
                foreach (var b in botones)
                {
                    if (b.gameObject.name.ToLower().Contains("si")) botonSi = b;
                    else if (b.gameObject.name.ToLower().Contains("no")) botonNo = b;
                }
            }
        }
    }

    void Start()
    {
        // Por seguridad, si no se arrastró el jugador o aviso en el Inspector, los busca al inicio
        if (jugador == null)
        {
            GameObject p = GameObject.Find("cassandra") ?? GameObject.FindWithTag("Player");
            if (p != null) jugador = p.transform;
        }

        if (avisoTeclaE == null)
            avisoTeclaE = GameObject.Find("AvisoInteraccion");

        if (avisoTeclaE != null) avisoTeclaE.SetActive(false);
        if (panelConfirmacion != null) panelConfirmacion.SetActive(false);

        if (objetoFlotante != null)
            posInicialFlotante = objetoFlotante.localPosition;

        if (botonSi != null)
            botonSi.onClick.AddListener(ConfirmarCambio);
        if (botonNo != null)
            botonNo.onClick.AddListener(CerrarPanel);
    }

    void Update()
    {
        // 1. Efecto de luz / respiración
        if (pulsarColor && spriteIluminado != null)
        {
            float t = (Mathf.Sin(Time.time * velocidadPulso) + 1f) * 0.5f;
            spriteIluminado.color = Color.Lerp(colorNormal, colorBrillo, t);
        }

        // Efecto flotante
        if (objetoFlotante != null)
        {
            float nuevoY = posInicialFlotante.y + Mathf.Sin(Time.time * velocidadFlote) * amplitudFlote;
            objetoFlotante.localPosition = new Vector3(posInicialFlotante.x, nuevoY, posInicialFlotante.z);
        }

        if (panelAbierto) return;
        if (jugador == null) return;

        // 2. Detección de distancia
        float distancia = Vector2.Distance(jugador.position, transform.position);
        bool cerca = distancia <= radioInteraccion;

        if (cerca)
        {
            GestorAvisoInteraccion.MostrarSobre(transform);
            if (avisoTeclaE != null) avisoTeclaE.SetActive(true);
        }
        else
        {
            GestorAvisoInteraccion.OcultarSiEs(transform);
            if (avisoTeclaE != null && avisoTeclaE != GestorAvisoInteraccion.Instancia?.gameObject)
                avisoTeclaE.SetActive(false);
        }

        // 3. Abrir con tecla E o Ctrl
        var k = Keyboard.current;
        if (k != null && cerca && (k.eKey.wasPressedThisFrame || k.leftCtrlKey.wasPressedThisFrame))
        {
            AbrirPanel();
        }
    }

    void AbrirPanel()
    {
        panelAbierto = true;
        DialogoNPC.Activo = true; // Detiene el movimiento del jugador

        GestorAvisoInteraccion.OcultarSiEs(transform);
        if (avisoTeclaE != null) avisoTeclaE.SetActive(false);

        if (textoPregunta != null)
            textoPregunta.text = mensajePregunta;

        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(true);
    }

    public void ConfirmarCambio()
    {
        DialogoNPC.Activo = false;
        SceneManager.LoadScene(nombreEscenaDestino);
    }

    public void CerrarPanel()
    {
        panelAbierto = false;
        DialogoNPC.Activo = false; // Le devuelve el control al jugador

        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}
