using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectorPersonaje : MonoBehaviour
{
    [Header("Configuración de Carga")]
    public string escenaSiguiente = "Inicio_Juego";

    // Conectar al evento OnClick() del Botón del Personaje 1
    public void SeleccionarPersonaje1()
    {
        PlayerPrefs.SetInt("PersonajeSeleccionado", 1);
        PlayerPrefs.Save();
        Debug.Log(">>> GUARDADO EN PLAYERPREFS: 1 (Leo)");
        SceneManager.LoadScene(escenaSiguiente);
    }

    public void SeleccionarPersonaje2()
    {
        PlayerPrefs.SetInt("PersonajeSeleccionado", 2);
        PlayerPrefs.Save();
        Debug.Log(">>> GUARDADO EN PLAYERPREFS: 2 (Cassandra)");
        SceneManager.LoadScene(escenaSiguiente);
    }
}