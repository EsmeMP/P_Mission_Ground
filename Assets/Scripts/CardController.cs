using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [HideInInspector] public int idPareja;
    private Sprite spriteFrenteCarta;
    private Sprite spriteDorsoCarta;
    private GameManager gameManager;
    private bool estaVolteada = false;
    private bool encontrada = false;

    private Image imagenUI;

    void Awake()
    {
        imagenUI = GetComponent<Image>();
    }

    public void ConfigurarCarta(int id, Sprite spriteFrente, Sprite spriteDorso, GameManager manager)
    {
        idPareja = id;
        spriteFrenteCarta = spriteFrente;
        spriteDorsoCarta = spriteDorso;
        gameManager = manager;
        estaVolteada = false;
        encontrada = false;

        MostrarDorso();

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickCarta());
        }
    }

    public void OnClickCarta()
    {
        if (estaVolteada || encontrada || !gameManager.CanClickCards()) return;
        gameManager.CartaSeleccionada(this);
    }

    public void Voltear(bool voltear)
    {
        estaVolteada = voltear;
        if (voltear)
        {
            if (imagenUI != null && spriteFrenteCarta != null)
            {
                imagenUI.sprite = spriteFrenteCarta;
            }
        }
        else
        {
            MostrarDorso();
        }
    }

    void MostrarDorso()
    {
        if (imagenUI != null && spriteDorsoCarta != null)
        {
            imagenUI.sprite = spriteDorsoCarta;
        }
    }

    public void MarcarComoEncontrada()
    {
        encontrada = true;
    }
}