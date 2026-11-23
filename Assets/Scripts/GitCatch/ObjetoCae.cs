using UnityEngine;

public class ObjetoCae : MonoBehaviour
{
    public float velocidad = 3f;
    public float limiteInferior = -6.5f;
    public bool esBueno = true;

    void Update()
    {
        transform.position += Vector3.down * velocidad * Time.deltaTime;

        if (transform.position.y < limiteInferior)
        {
            if (esBueno && GitCatchManager.Instancia != null)
                GitCatchManager.Instancia.PerderVida();

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponent<MovimientoRecolector>() == null) return;

        if (GitCatchManager.Instancia != null)
        {
            if (esBueno)
                GitCatchManager.Instancia.SumarPunto();
            else
                GitCatchManager.Instancia.PerderVida();
        }

        Destroy(gameObject);
    }
}