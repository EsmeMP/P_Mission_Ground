using UnityEngine;
using UnityEngine.SceneManagement;

public class menuPrincipal : MonoBehaviour
{
    // El nombre tiene que ser EXACTO al de tu escena de juego
    public string escenaDelJuego = "storyBoard";

    // Panel de opciones (lo conectamos en el paso 7, opcional)
    public GameObject panelOpciones;

    public void NuevaPartida()
    {
        SceneManager.LoadScene(escenaDelJuego);
    }

    public void AbrirOpciones()
    {
        if (panelOpciones != null) panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
    }

    public void Salir()
    {
        Debug.Log("Salir del juego");
        Application.Quit(); // Dentro del editor no cierra; solo en el juego ya exportado
    }
}