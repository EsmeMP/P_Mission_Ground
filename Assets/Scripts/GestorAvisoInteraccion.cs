using UnityEngine;

public class GestorAvisoInteraccion : MonoBehaviour
{
    public static GestorAvisoInteraccion Instancia;

    [Header("Efecto de Rebote")]
    public float velocidadRebote = 6f;
    public float alturaRebote = 0.15f;
    public Vector3 offsetSobreObjeto = new Vector3(0, 1.2f, 0);

    Transform objetoActual;
    CanvasGroup canvasGroup;
    Vector3 escalaInicial;

    void Awake()
    {
        Instancia = this;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        escalaInicial = transform.localScale;
        Ocultar();
    }

    void LateUpdate()
    {
        if (objetoActual == null)
        {
            Ocultar();
            return;
        }

        // Posicionar sobre el objeto en coordenadas de mundo si es World Space, o convertir si es Screen Space
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 posMundo = objetoActual.position + offsetSobreObjeto;
            posMundo.y += Mathf.Sin(Time.time * velocidadRebote) * alturaRebote;

            // Si el Canvas es Overlay / Screen Space Camera
            transform.position = cam.WorldToScreenPoint(posMundo);
        }

        // Efecto pulso de escala suave
        float pulso = 1f + Mathf.Sin(Time.time * velocidadRebote) * 0.08f;
        transform.localScale = escalaInicial * pulso;
    }

    /// <summary>
    /// Pide mostrar el aviso sobre un objeto interactuable (NPC o Puerta)
    /// </summary>
    public static void MostrarSobre(Transform objetivo)
    {
        if (Instancia == null) return;
        Instancia.objetoActual = objetivo;
        Instancia.gameObject.SetActive(true);
        if (Instancia.canvasGroup != null) Instancia.canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Pide ocultar si el objeto que se aleja es el que lo estaba usando
    /// </summary>
    public static void OcultarSiEs(Transform objetivo)
    {
        if (Instancia == null) return;
        if (Instancia.objetoActual == objetivo)
        {
            Instancia.Ocultar();
        }
    }

    public void Ocultar()
    {
        objetoActual = null;
        if (gameObject != null) gameObject.SetActive(false);
    }
}
