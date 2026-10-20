using UnityEngine;

public class GeneradorObjetos : MonoBehaviour
{
    public GameObject[] prefabs;
    public float intervaloInicial = 1.3f;
    public float intervaloMinimo = 0.5f;
    public float velocidadInicial = 3f;
    public float velocidadMaxima = 7f;
    public int puntosPorNivel = 5;
    public float posicionY = 6.5f;
    public float limiteIzquierdo = -6f;
    public float limiteDerecho = 6f;

    float temporizador;

    void Update()
    {
        var gm = GitCatchManager.Instancia;
        if (gm != null && gm.vidas <= 0) return;

        int nivel = gm != null ? gm.puntos / puntosPorNivel : 0;
        float intervalo = Mathf.Max(intervaloMinimo, intervaloInicial - nivel * 0.1f);

        temporizador += Time.deltaTime;
        if (temporizador >= intervalo)
        {
            temporizador = 0f;
            Generar(nivel);
        }
    }

    void Generar(int nivel)
    {
        if (prefabs == null || prefabs.Length == 0) return;

        int i = Random.Range(0, prefabs.Length);
        float x = Random.Range(limiteIzquierdo, limiteDerecho);
        GameObject obj = Instantiate(prefabs[i], new Vector3(x, posicionY, 0f), Quaternion.identity);

        ObjetoCae cae = obj.GetComponent<ObjetoCae>();
        if (cae != null)
            cae.velocidad = Mathf.Min(velocidadMaxima, velocidadInicial + nivel * 0.4f);
    }
}