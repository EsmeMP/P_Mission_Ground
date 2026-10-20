using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct RondaConfig
    {
        public int numeroRonda;
        public int totalPares;
        public float tiempoLimite;
    }

    [System.Serializable]
    public struct ParDeSprites
    {
        public Sprite sprite1;
        public Sprite sprite2;
    }

    [Header("Configuración de las 3 Rondas")]
    public RondaConfig[] rondas;

    [Header("Parejas de Sprites")]
    public List<ParDeSprites> pairs;

    private int indiceRondaActual = 0;
    private float tiempoRestante;
    private bool juegoActivo = false;
    private bool juegoPausado = false;
    private int paresEncontrados = 0;
    private int totalParesAcumulados = 0; // Para llevar la cuenta total de puntos/pares en la partida

    [Header("Sistema de Vidas (Corazones)")]
    public int vidasActuales = 3;
    public int maxVidas = 3;
    public GameObject[] corazonesVidas;

    [Header("Referencias del Tablero")]
    public RectTransform gridTransform;
    public GameObject cardPrefab;
    public Sprite spriteDorsoCarta;

    private CardController primeraCartaSeleccionada;
    private CardController segundaCartaSeleccionada;
    private bool esperandoComparacion = false;

    [Header("Referencias de UI y Menús")]
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoPares;
    public TextMeshProUGUI textoTituloRonda;
    public GameObject panelGameOver;
    public TextMeshProUGUI textoPuntosGameOver; // El texto que mostrará "PUNTOS: X" en el menú de derrota
    public GameObject panelPausa;

    void Start()
    {
        vidasActuales = maxVidas;
        totalParesAcumulados = 0;

        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);

        ActualizarCorazonesUI();
        IniciarRonda(0);
    }

    void Update()
    {
        if (!juegoActivo || juegoPausado) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarUI();

            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                ManejarFalloRonda();
            }
        }
    }

    public void IniciarRonda(int indice)
    {
        if (rondas == null || indice >= rondas.Length)
        {
            Debug.Log("¡Felicidades! Has ganado todas las rondas.");
            return;
        }

        indiceRondaActual = indice;
        RondaConfig config = rondas[indiceRondaActual];

        tiempoRestante = config.tiempoLimite;
        paresEncontrados = 0;
        juegoActivo = true;
        juegoPausado = false;
        esperandoComparacion = false;
        primeraCartaSeleccionada = null;
        segundaCartaSeleccionada = null;

        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);

        if (textoTituloRonda != null)
            textoTituloRonda.text = "Ronda " + config.numeroRonda;

        ActualizarUI();
        GenerarTablero(config.totalPares);
    }

    void GenerarTablero(int numPares)
    {
        if (gridTransform == null || cardPrefab == null) return;

        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }

        List<DatosCartaTemp> listaCartasTemp = new List<DatosCartaTemp>();

        for (int i = 0; i < numPares; i++)
        {
            if (i >= pairs.Count) break;

            ParDeSprites parActual = pairs[i];

            DatosCartaTemp carta1 = new DatosCartaTemp();
            carta1.idPareja = i;
            carta1.spriteFrente = parActual.sprite1;
            listaCartasTemp.Add(carta1);

            DatosCartaTemp carta2 = new DatosCartaTemp();
            carta2.idPareja = i;
            carta2.spriteFrente = parActual.sprite2;
            listaCartasTemp.Add(carta2);
        }

        for (int i = 0; i < listaCartasTemp.Count; i++)
        {
            DatosCartaTemp temp = listaCartasTemp[i];
            int randomIndex = Random.Range(i, listaCartasTemp.Count);
            listaCartasTemp[i] = listaCartasTemp[randomIndex];
            listaCartasTemp[randomIndex] = temp;
        }

        foreach (var datos in listaCartasTemp)
        {
            GameObject nuevaCartaObj = Instantiate(cardPrefab, gridTransform);
            CardController cartaScript = nuevaCartaObj.GetComponent<CardController>();

            if (cartaScript != null)
            {
                cartaScript.ConfigurarCarta(datos.idPareja, datos.spriteFrente, spriteDorsoCarta, this);
            }
        }
    }

    private class DatosCartaTemp
    {
        public int idPareja;
        public Sprite spriteFrente;
    }

    public bool CanClickCards()
    {
        return juegoActivo && !juegoPausado && !esperandoComparacion;
    }

    public void CartaSeleccionada(CardController carta)
    {
        if (primeraCartaSeleccionada == null)
        {
            primeraCartaSeleccionada = carta;
            primeraCartaSeleccionada.Voltear(true);
        }
        else if (segundaCartaSeleccionada == null && carta != primeraCartaSeleccionada)
        {
            segundaCartaSeleccionada = carta;
            segundaCartaSeleccionada.Voltear(true);
            esperandoComparacion = true;

            Invoke("CompararCartas", 0.8f);
        }
    }

    void CompararCartas()
    {
        if (primeraCartaSeleccionada.idPareja == segundaCartaSeleccionada.idPareja)
        {
            primeraCartaSeleccionada.MarcarComoEncontrada();
            segundaCartaSeleccionada.MarcarComoEncontrada();

            paresEncontrados++;
            totalParesAcumulados++; // Suma un punto al marcador total
            ActualizarUI();

            RondaConfig config = rondas[indiceRondaActual];
            if (paresEncontrados >= config.totalPares)
            {
                juegoActivo = false;
                Debug.Log($"¡Ronda {config.numeroRonda} superada!");
                Invoke("AvanzarSiguienteRonda", 1.5f);
            }
        }
        else
        {
            primeraCartaSeleccionada.Voltear(false);
            segundaCartaSeleccionada.Voltear(false);
        }

        primeraCartaSeleccionada = null;
        segundaCartaSeleccionada = null;
        esperandoComparacion = false;
    }

    void AvanzarSiguienteRonda()
    {
        IniciarRonda(indiceRondaActual + 1);
    }

    void ManejarFalloRonda()
    {
        juegoActivo = false;
        vidasActuales--;
        ActualizarCorazonesUI();

        if (vidasActuales > 0)
        {
            Invoke("ReiniciarRondaActual", 1.5f);
        }
        else
        {
            PerderJuegoTotal();
        }
    }

    void ReiniciarRondaActual()
    {
        IniciarRonda(indiceRondaActual);
    }

    void PerderJuegoTotal()
    {
        juegoActivo = false;

        // Mostrar los puntos totales alcanzados en el menú de Game Over
        if (textoPuntosGameOver != null)
        {
            textoPuntosGameOver.text = "PUNTOS: " + totalParesAcumulados;
        }

        if (panelGameOver != null)
            panelGameOver.SetActive(true);
    }

    void ActualizarUI()
    {
        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + Mathf.Ceil(tiempoRestante) + "s";

        if (textoPares != null)
        {
            RondaConfig config = rondas[indiceRondaActual];
            textoPares.text = "Pares: " + paresEncontrados + " / " + config.totalPares;
        }
    }

    void ActualizarCorazonesUI()
    {
        if (corazonesVidas == null) return;
        for (int i = 0; i < corazonesVidas.Length; i++)
        {
            if (corazonesVidas[i] != null)
            {
                corazonesVidas[i].SetActive(i < vidasActuales);
            }
        }
    }

    public void BotonPausa()
    {
        juegoPausado = true;
        Time.timeScale = 0f;
        if (panelPausa != null) panelPausa.SetActive(true);
    }

    public void BotonPlay()
    {
        juegoPausado = false;
        Time.timeScale = 1f;
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    public void BotonIntentarDeNuevo()
    {
        Time.timeScale = 1f;
        vidasActuales = maxVidas;
        totalParesAcumulados = 0; // Reiniciar puntos
        ActualizarCorazonesUI();
        IniciarRonda(0);
    }

    public void BotonSalir()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}