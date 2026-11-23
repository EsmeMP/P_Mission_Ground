using UnityEngine;

public class ElegirTorre : MonoBehaviour
{
    public Sprite[] torres;
    public SpriteRenderer torreArriba;
    public SpriteRenderer torreAbajo;

    public void Elegir()
    {
        Sprite elegido = torres[Random.Range(0, torres.Length)];
        torreArriba.sprite = elegido;
        torreAbajo.sprite = elegido;
    }

    void Start()
    {
        Elegir();
    }
}