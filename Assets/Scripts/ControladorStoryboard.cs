using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorStoryboard : MonoBehaviour
{
    [Header("Historia")]
    public Image imagenHistoria;      // arrastra imageStoryBoard
    public Sprite[] paginas;          // las 4 imágenes, en orden

    [Header("Botón")]
    public Button botonSkip;          // arrastra btn_Skip

    [Header("Siguiente escena")]
    public string escenaSiguiente = "Salones";

    private int indice = 0;

    void Start()
    {
        if (paginas.Length > 0)
            imagenHistoria.sprite = paginas[0];

        // Clic en la imagen = siguiente página
        Button botonImagen = imagenHistoria.GetComponent<Button>();
        if (botonImagen == null)
            botonImagen = imagenHistoria.gameObject.AddComponent<Button>();
        botonImagen.onClick.AddListener(Siguiente);

        // Skip = saltar toda la historia
        if (botonSkip != null)
            botonSkip.onClick.AddListener(Saltar);
    }

    public void Siguiente()
    {
        indice++;
        if (indice >= paginas.Length)
        {
            Saltar();
            return;
        }
        imagenHistoria.sprite = paginas[indice];
    }

    public void Saltar()
    {
        SceneManager.LoadScene(escenaSiguiente);
    }
}