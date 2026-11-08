using UnityEngine;

public class ZonaPunto : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponent<Cubito>() != null)
        {
            Puntaje.instancia.Sumar();
        }
    }
}