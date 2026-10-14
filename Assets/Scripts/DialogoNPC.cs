using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogoNPC : MonoBehaviour
{
    public Transform jugador;
    public GameObject cajaDialogo;
    public TMP_Text textoDialogo;
    public GameObject aviso;
    public float radioInteraccion = 1.5f;

    [Header("Minijuego (opcional)")]
    public bool abreMinijuego = false;
    public string nombreEscenaMinijuego = "MinijuegoGitCatch";

    public string[] lineas =
    {
        "¡Hola! Soy la maestra Diana Lizeth Ahuatzi Reyes.",
        "Me encargo de dar materias como Gestion de Procesos de Desarrollo de Software, Ética y Legislación en Tecnologías, etc.",
        "Dato curioso:"
    };

    public static bool Activo;
    static bool hayPosicionGuardada;
    static Vector3 posicionGuardada;
    int indice;

    void Start()
    {
        cajaDialogo.SetActive(false);
        if (aviso != null) aviso.SetActive(false);
        Activo = false;

        if (hayPosicionGuardada && jugador != null)
        {
            jugador.position = posicionGuardada;
            Physics2D.SyncTransforms();
            hayPosicionGuardada = false;
        }
    }

    void Update()
    {
        var k = Keyboard.current;
        if (k == null) return;

        bool interactuar = k.eKey.wasPressedThisFrame || k.leftCtrlKey.wasPressedThisFrame;
        bool avanzar = interactuar || k.spaceKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame;

        if (!Activo)
        {
            float distancia = Vector2.Distance(jugador.position, transform.position);
            bool cerca = distancia <= radioInteraccion;

            if (aviso != null) aviso.SetActive(cerca);

            if (cerca && interactuar)
                Abrir();
        }
        else if (avanzar)
        {
            Siguiente();
        }
    }

    void Abrir()
    {
        Activo = true;
        indice = 0;
        if (aviso != null) aviso.SetActive(false);
        cajaDialogo.SetActive(true);
        textoDialogo.text = lineas[indice];
    }

    void Siguiente()
    {
        indice++;
        if (indice >= lineas.Length)
            Cerrar();
        else
            textoDialogo.text = lineas[indice];
    }

    void Cerrar()
    {
        Activo = false;
        cajaDialogo.SetActive(false);

        if (abreMinijuego)
        {
            posicionGuardada = jugador.position;
            hayPosicionGuardada = true;
            SceneManager.LoadScene(nombreEscenaMinijuego);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}