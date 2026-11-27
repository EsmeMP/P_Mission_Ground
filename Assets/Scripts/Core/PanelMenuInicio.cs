
using UnityEngine;
using System.Collections;

public class PanelMenuInicio : MonoBehaviour
{
    [Header("Panel del menú")]
    public GameObject panelMenu;

    [Header("Tiempo de espera")]
    public float tiempoEspera = 5f;

    void Start()
    {
        if (panelMenu != null)
        {
            panelMenu.SetActive(false);
            StartCoroutine(MostrarPanel());
        }
    }

    IEnumerator MostrarPanel()
    {
        yield return new WaitForSeconds(tiempoEspera);

        panelMenu.SetActive(true);
    }
}
