using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorStoryboard : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de la escena de selección de personajes")]
    public string escenaSiguiente = "Personajes";

    [Header("Referencias de UI")]
    public Image imagenDisplay;
    public TextMeshProUGUI textoBoton;

    [Header("Lista de Viñetas")]
    [Tooltip("Arrastra aquí todas las viñetas del cómic en orden")]
    public Sprite[] vinetas;

    private int indiceActual = 0;

    private void Start()
    {
        // Validar que hay viñetas añadidas
        if (vinetas != null && vinetas.Length > 0)
        {
            indiceActual = 0;
            MostrarVinetaActual();
        }
        else
        {
            Debug.LogWarning("No has asignado viñetas en la lista. Pasando a la siguiente escena...");
            SaltarAPersonajes();
        }
    }

    // Conectar al botón que va pasando viñetas una por una
    public void AvanzarOIniciar()
    {
        if (vinetas == null || vinetas.Length == 0) return;

        // Si ya estamos en la última viñeta, saltamos a la selección de personaje
        if (indiceActual >= vinetas.Length - 1)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(escenaSiguiente);
            return;
        }

        // Si faltan viñetas, avanzamos a la siguiente
        indiceActual++;
        MostrarVinetaActual();
    }

    // Conectar directamente al botón 'btn_Skip' si quieres saltarte todo el cómic
    public void SaltarAPersonajes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaSiguiente);
    }

    private void MostrarVinetaActual()
    {
        if (imagenDisplay != null && vinetas != null && indiceActual < vinetas.Length)
        {
            imagenDisplay.sprite = vinetas[indiceActual];
        }

        if (textoBoton != null)
        {
            if (indiceActual == vinetas.Length - 1)
            {
                textoBoton.text = "Continuar";
            }
            else
            {
                textoBoton.text = "Siguiente";
            }
        }
    }
}