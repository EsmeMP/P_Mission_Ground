using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Botones")]
    public Button botonPlay;      // arrastra btn_play
    public Button botonSalir;     // arrastra btn_salir

    [Header("Siguiente escena")]
    public string escenaHistoria = "storyBoard";

    void Start()
    {
        if (botonPlay != null)
            botonPlay.onClick.AddListener(Jugar);

        if (botonSalir != null)
            botonSalir.onClick.AddListener(Salir);
    }

    public void Jugar()
    {
        SceneManager.LoadScene(escenaHistoria);
    }

    public void Salir()
    {
        Debug.Log("Salir del juego");
        Application.Quit();   // en el editor no cierra nada, en el .exe sí
    }
}