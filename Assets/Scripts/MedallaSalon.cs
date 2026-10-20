using UnityEngine;
using System.Collections;

public class MedallaSalon : MonoBehaviour
{
    public Transform jugador;
    public float espera = 1.5f;
    public float duracionVuelo = 1f;
    public float escalaFinal = 0.3f;

    static bool yaEntregada;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        yaEntregada = false;
    }

    void Start()
    {
        if (GitCatchManager.MedallaGanada && !yaEntregada)
        {
            yaEntregada = true;
            StartCoroutine(Animar());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator Animar()
    {
        Vector3 posInicial = transform.position;
        Vector3 escalaInicial = transform.localScale;

        yield return new WaitForSeconds(espera);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionVuelo;
            float suave = t * t;

            Vector3 destino = Camera.main.WorldToScreenPoint(jugador.position);
            transform.position = Vector3.Lerp(posInicial, destino, suave);
            transform.localScale = Vector3.Lerp(escalaInicial, escalaInicial * escalaFinal, suave);
            yield return null;
        }

        ContadorMedallas.Sumar();
        gameObject.SetActive(false);
    }
}