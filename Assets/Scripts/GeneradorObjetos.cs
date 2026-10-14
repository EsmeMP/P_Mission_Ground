using UnityEngine;

public class GeneradorObjetos : MonoBehaviour
{
    public GameObject[] prefabs;
    public float intervalo = 1.2f;
    public float posicionY = 6.5f;
    public float limiteIzquierdo = -6f;
    public float limiteDerecho = 6f;

    float temporizador;

    void Update()
    {
        if (GitCatchManager.Instancia != null && GitCatchManager.Instancia.vidas <= 0)
            return;

        temporizador += Time.deltaTime;

        if (temporizador >= intervalo)
        {
            temporizador = 0f;
            Generar();
        }
    }

    void Generar()
    {
        if (prefabs == null || prefabs.Length == 0) return;

        int i = Random.Range(0, prefabs.Length);
        float x = Random.Range(limiteIzquierdo, limiteDerecho);
        Instantiate(prefabs[i], new Vector3(x, posicionY, 0f), Quaternion.identity);
    }
}