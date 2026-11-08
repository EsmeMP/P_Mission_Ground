using UnityEngine;
using System.Collections;

public class GlitchTorres : MonoBehaviour
{
    public int puntosParaGlitch = 40;
    public float distanciaActivacion = 8f;
    public float duracionAviso = 0.7f;
    public float velocidadParpadeo = 0.08f;
    public float saltoMaximo = 1.5f;
    [Range(0f, 1f)] public float probabilidad = 0.5f;
    public Color colorApagado = new Color(0.2f, 0.2f, 0.3f, 0.5f);

    private Transform cubito;
    private MoverTorres mover;
    private SpriteRenderer arriba;
    private SpriteRenderer abajo;
    private bool yaProcesado = false;
    private bool enAviso = false;

    void Start()
    {
        mover = GetComponent<MoverTorres>();

        Transform tArriba = transform.Find("TorreArriba");
        Transform tAbajo = transform.Find("TorreAbajo");
        if (tArriba != null) arriba = tArriba.GetComponent<SpriteRenderer>();
        if (tAbajo != null) abajo = tAbajo.GetComponent<SpriteRenderer>();

        Cubito c = FindFirstObjectByType<Cubito>();
        if (c != null) cubito = c.transform;
    }

    void Update()
    {
        if (cubito == null || Puntaje.instancia == null || mover == null) return;

        float x = transform.position.x;

        // El par reapareció a la derecha: puede volver a hacer glitch
        if (x > 9f && !enAviso)
        {
            yaProcesado = false;
        }

        if (!yaProcesado
            && Puntaje.instancia.Puntos >= puntosParaGlitch
            && x > cubito.position.x
            && x - cubito.position.x < distanciaActivacion)
        {
            yaProcesado = true;

            if (Random.value < probabilidad)
            {
                StartCoroutine(Glitch());
            }
        }
    }

    void PonerColor(Color c)
    {
        if (arriba != null) arriba.color = c;
        if (abajo != null) abajo.color = c;
    }

    IEnumerator Glitch()
    {
        enAviso = true;
        float t = 0f;
        bool apagado = false;

        // Aviso: las torres parpadean entre normal y oscuro
        while (t < duracionAviso)
        {
            apagado = !apagado;
            PonerColor(apagado ? colorApagado : Color.white);

            t += velocidadParpadeo;
            yield return new WaitForSeconds(velocidadParpadeo);
        }

        PonerColor(Color.white);

        // Salto
        float cambio = Random.Range(0.8f, saltoMaximo);
        if (Random.value < 0.5f) cambio = -cambio;
        mover.MoverAltura(cambio);

        enAviso = false;
    }
}