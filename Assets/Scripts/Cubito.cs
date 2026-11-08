using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cubito : MonoBehaviour
{
    public float fuerzaSalto = 8f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool toco =
            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

        if (toco)
        {
            rb.linearVelocity = new Vector2(0f, fuerzaSalto);
        }

        if (transform.position.y < -6.5f || transform.position.y > 6.5f)
        {
            Perder();
        }
    }

    void OnCollisionEnter2D(Collision2D otro)
    {
        Perder();
    }

    void Perder()
    {
        Puntaje.instancia.MostrarResultado();
    }
}