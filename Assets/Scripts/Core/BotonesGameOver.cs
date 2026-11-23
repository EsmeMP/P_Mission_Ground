using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonesGameOver : MonoBehaviour
{
    public string escenaSalon = "Salones";

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Salir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaSalon);
    }
}