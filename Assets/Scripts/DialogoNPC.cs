using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogoNPC : MonoBehaviour
{
    public Transform jugador;
    public GameObject cajaDialogo;
    public TMP_Text textoDialogo;
    public GameObject aviso;
    public float radioInteraccion = 1.5f;

    public string[] lineas =
    {
        "¡Hola! Soy la maestra Diana Lizeth Ahuatzi Reyes.",
        "Me encargo de dar materias como Gestion de Procesos de Desarrollo de Software, Ética y Legislación en Tecnologías, etc.",
        "Dato curioso:"
    };

    public static bool Activo;
    int indice;

    void Start()
    {
        cajaDialogo.SetActive(false);
        if (aviso != null) aviso.SetActive(false);
        Activo = false;
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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}