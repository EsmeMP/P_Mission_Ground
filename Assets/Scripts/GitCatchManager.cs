using UnityEngine;

public class GitCatchManager : MonoBehaviour
{
    public static GitCatchManager Instancia;

    public int vidas = 3;
    public int puntos = 0;
    public GameObject[] corazones;

    void Awake()
    {
        Instancia = this;
    }

    public void SumarPunto()
    {
        puntos++;
        Debug.Log("Puntos: " + puntos);
    }

    public void PerderVida()
    {
        if (vidas <= 0) return;

        vidas--;
        if (corazones != null && vidas < corazones.Length && corazones[vidas] != null)
            corazones[vidas].SetActive(false);

        Debug.Log("Vidas: " + vidas);

        if (vidas <= 0)
            Debug.Log("Game Over");
    }
}