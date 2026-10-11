using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 3f;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    Vector2 direccion;
    string animActual = "";
    string ultimaDireccion = "frente"; // frente, detras o lateral

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Si hay un diálogo abierto, Cassandra se queda quieta respirando
        if (DialogoNPC.Activo)
        {
            direccion = Vector2.zero;
            ActualizarAnimacion(0f, 0f);
            return;
        }

        var k = Keyboard.current;
        if (k == null) return;

        float x = 0f;
        float y = 0f;

        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        if (k.sKey.isPressed || k.downArrowKey.isPressed) y -= 1f;
        if (k.wKey.isPressed || k.upArrowKey.isPressed) y += 1f;

        direccion = new Vector2(x, y).normalized;
        ActualizarAnimacion(x, y);
    }

    void ActualizarAnimacion(float x, float y)
    {
        bool caminando = (x != 0f || y != 0f);

        if (caminando)
        {
            if (x != 0f)
            {
                ultimaDireccion = "lateral";
                sr.flipX = x < 0f; // izquierda = voltear
            }
            else if (y > 0f)
            {
                ultimaDireccion = "detras";
            }
            else
            {
                ultimaDireccion = "frente";
            }
        }

        string accion = caminando ? "caminar" : "respirar";
        string nueva = "cassandra_" + accion + "_" + ultimaDireccion;

        if (nueva != animActual)
        {
            anim.Play(nueva);
            animActual = nueva;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direccion * velocidad;
    }
}