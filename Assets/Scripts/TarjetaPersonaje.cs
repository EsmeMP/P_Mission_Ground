using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TarjetaPersonaje : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Dibujo")]
    public Image retrato;            // arrastra Retrato
    public Sprite spriteQuieto;      // el sprite de frente, parado

    [Header("Animación al pasar el mouse")]
    public Sprite[] framesCaminar;   // los cuadros de caminar, en orden
    public float fps = 8f;

    private bool encima = false;
    private float tiempo = 0f;

    void Start()
    {
        if (retrato != null && spriteQuieto != null)
            retrato.sprite = spriteQuieto;
    }

    void Update()
    {
        if (!encima || framesCaminar == null || framesCaminar.Length == 0) return;

        tiempo += Time.unscaledDeltaTime * fps;
        int indice = (int)tiempo % framesCaminar.Length;
        retrato.sprite = framesCaminar[indice];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        encima = true;
        tiempo = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        encima = false;
        if (retrato != null && spriteQuieto != null)
            retrato.sprite = spriteQuieto;
    }
}