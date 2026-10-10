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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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
        if (x == 0f && y == 0f)
        {
            anim.speed = 0f; // se queda quieta en el cuadro actual
            return;
        }

        anim.speed = 1f;

        string nueva;
        if (x != 0f)
        {
            nueva = "cassandra_caminar_lateral";
            sr.flipX = x < 0f; // izquierda = voltear
        }
        else if (y > 0f)
        {
            nueva = "cassandra_caminar_detras";
        }
        else
        {
            nueva = "cassandra_caminar_frente";
        }

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