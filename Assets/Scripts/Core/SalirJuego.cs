using UnityEngine;

public class SalirJuego : MonoBehaviour
{
    public void Salir()
    {
        Debug.Log("Botón Salir presionado");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
