using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cubito : MonoBehaviour
{
    public float fuerzaSalto = 8f;

    [Header("Tiempo de espera al inicio")]
    public float retrasoMinimoInicio = 0.35f;
    public float amplitudFlotacion = 0.25f;
    public float velocidadFlotacion = 3f;
    public GameObject mensajeInicio;

    public static bool JuegoIniciado { get; private set; }

    private Rigidbody2D rb;
    private bool yaPerdio = false;
    private float gravedadOriginal;
    private float tiempoEnEspera = 0f;
    private Vector3 posicionInicial;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarEstado()
    {
        JuegoIniciado = false;
    }

    void Start()
    {
        JuegoIniciado = false;
        rb = GetComponent<Rigidbody2D>();
        gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        posicionInicial = transform.position;

        if (mensajeInicio != null)
            mensajeInicio.SetActive(true);
    }

    void Update()
    {
        if (yaPerdio || Time.timeScale == 0f) return;

        bool toco =
            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

        if (!JuegoIniciado)
        {
            tiempoEnEspera += Time.deltaTime;
            float offsetY = Mathf.Sin(tiempoEnEspera * velocidadFlotacion) * amplitudFlotacion;
            transform.position = new Vector3(posicionInicial.x, posicionInicial.y + offsetY, posicionInicial.z);

            if (tiempoEnEspera >= retrasoMinimoInicio && toco)
            {
                JuegoIniciado = true;
                rb.gravityScale = gravedadOriginal;
                rb.linearVelocity = new Vector2(0f, fuerzaSalto);

                if (mensajeInicio != null)
                    mensajeInicio.SetActive(false);
            }
            return;
        }

        if (toco)
        {
            rb.linearVelocity = new Vector2(0f, fuerzaSalto);
        }

        if (transform.position.y < -6.5f || transform.position.y > 6.5f)
        {
            Perder();
        }
    }

    void OnCollisionEnter2D(Collision2D otro)
    {
        Perder();
    }

    void Perder()
    {
        if (yaPerdio) return;
        yaPerdio = true;
        Puntaje.instancia.MostrarResultado();
    }
}