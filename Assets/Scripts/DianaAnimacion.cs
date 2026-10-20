using System.Collections;
using UnityEngine;

public class DianaAnimacion : MonoBehaviour
{
    public float minRespirar = 4f;
    public float maxRespirar = 8f;
    public float duracionExplicando = 2f;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    IEnumerator Start()
    {
        while (true)
        {
            anim.Play("diana_respirar");
            yield return new WaitForSeconds(Random.Range(minRespirar, maxRespirar));

            anim.Play("diana_explicar");
            yield return new WaitForSeconds(duracionExplicando);
        }
    }
}