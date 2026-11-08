using UnityEngine;

public class MoverTorres : MonoBehaviour
{
    [Header("Velocidad")]
    public float velocidad = 3f;
    public float aumentoPorPunto = 0.1f;
    public float velocidadMaxima = 6f;

    [Header("Posiciones")]
    public float limiteIzquierdo = -11f;
    public float inicioDerecha = 13f;
    public float xMinimoAparicion = 11f;
    public float separacionAparicion = 8f;
    public float alturaMin = -2.5f;
    public float alturaMax = 2.5f;

    [Header("Evitar caso imposible")]
    public float distanciaCercana = 4f;
    public float maxDiferenciaAltura = 1.5f;

    [Header("Movimiento arriba y abajo (nivel 1)")]
    public int puntosParaMovimiento = 25;
    public float amplitud = 1f;
    public float velocidadVertical = 1.5f;
    public float limiteVertical = 3f;

    [Header("Movimiento arriba y abajo (nivel 2)")]
    public int puntosNivel2 = 60;
    public float amplitud2 = 1.8f;
    public float velocidadVertical2 = 2.2f;

    private ElegirTorre elegir;
    private float baseY;
    private float fase;
    private float amplitudActual = 0f;

    void Start()
    {
        elegir = GetComponent<ElegirTorre>();
        baseY = transform.position.y;
        fase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        int puntos = 0;
        if (Puntaje.instancia != null)
        {
            puntos = Puntaje.instancia.Puntos;
        }

        // Velocidad horizontal
        float velocidadActual = velocidad + puntos * aumentoPorPunto;
        if (velocidadActual > velocidadMaxima)
        {
            velocidadActual = velocidadMaxima;
        }

        transform.Translate(Vector3.left * velocidadActual * Time.deltaTime);

        // Movimiento vertical
        bool nivel2 = puntos >= puntosNivel2;
        float meta = 0f;
        if (puntos >= puntosParaMovimiento)
        {
            meta = nivel2 ? amplitud2 : amplitud;
        }
        float velV = nivel2 ? velocidadVertical2 : velocidadVertical;

        amplitudActual = Mathf.MoveTowards(amplitudActual, meta, Time.deltaTime * 0.5f);
        fase += velV * Time.deltaTime;

        float y = baseY + Mathf.Sin(fase) * amplitudActual;
        y = Mathf.Clamp(y, -limiteVertical, limiteVertical);

        transform.position = new Vector3(transform.position.x, y, 0f);

        // Reaparecer
        if (transform.position.x < limiteIzquierdo)
        {
            Reaparecer();
        }
    }

    void Reaparecer()
    {
        MoverTorres masADerecha = null;
        MoverTorres[] todos = FindObjectsByType<MoverTorres>(FindObjectsSortMode.None);

        foreach (MoverTorres otro in todos)
        {
            if (otro == this) continue;
            if (masADerecha == null || otro.transform.position.x > masADerecha.transform.position.x)
            {
                masADerecha = otro;
            }
        }

        float x = inicioDerecha;
        if (masADerecha != null)
        {
            x = Mathf.Clamp(masADerecha.transform.position.x + separacionAparicion, xMinimoAparicion, inicioDerecha);
        }

        float nuevaY = Random.Range(alturaMin, alturaMax);

        MoverTorres cercano = null;
        float menorDistancia = 999f;
        foreach (MoverTorres otro in todos)
        {
            if (otro == this) continue;
            float d = Mathf.Abs(otro.transform.position.x - x);
            if (d < menorDistancia)
            {
                menorDistancia = d;
                cercano = otro;
            }
        }

        if (cercano != null && menorDistancia < distanciaCercana)
        {
            float yCercano = cercano.transform.position.y;
            nuevaY = Mathf.Clamp(nuevaY, yCercano - maxDiferenciaAltura, yCercano + maxDiferenciaAltura);
            nuevaY = Mathf.Clamp(nuevaY, alturaMin, alturaMax);
        }

        baseY = nuevaY;
        transform.position = new Vector3(x, baseY, 0f);

        if (elegir != null)
        {
            elegir.Elegir();
        }
    }
    public void MoverAltura(float cambio)
    {
        baseY = Mathf.Clamp(baseY + cambio, alturaMin, alturaMax);
    }
}