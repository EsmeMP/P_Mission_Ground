using UnityEngine;
using UnityEngine.InputSystem;

public class RunCassandra : MonoBehaviour
{
    public float speed = 3f;
    public float runSpeed = 6f;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    Vector2 move;
    Vector2 lastDir = new Vector2(0f, -1f); // empieza mirando hacia abajo
    public Vector2 UltimaDireccion { get { return lastDir; } }
    bool running;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Si el SpriteRenderer está en un hijo, lo encuentra igual
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        anim.SetFloat("Horizontal", lastDir.x);
        anim.SetFloat("Vertical", lastDir.y);
    }

    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;

        // Si hay un diálogo abierto, Cassandra se queda quieta
        if (DialogoNPC.Activo)
        {
            move = Vector2.zero;
            running = false;
            anim.SetBool("IsMoving", false);
            return;
        }

        float x = 0f, y = 0f;

        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        if (k.sKey.isPressed || k.downArrowKey.isPressed) y -= 1f;
        if (k.wKey.isPressed || k.upArrowKey.isPressed) y += 1f;

        // Sin diagonales: si hay horizontal, se ignora el vertical
        if (x != 0f) y = 0f;

        move = new Vector2(x, y);
        bool moving = move != Vector2.zero;

        // Shift para correr
        running = moving && (k.leftShiftKey.isPressed || k.rightShiftKey.isPressed);

        if (moving) lastDir = move;

        // El sprite lateral original mira a la derecha:
        // al ir a la izquierda se voltea
        if (sr != null && lastDir.x != 0f) sr.flipX = lastDir.x < 0f;

        anim.SetBool("IsMoving", moving);
        anim.SetFloat("Horizontal", lastDir.x);
        anim.SetFloat("Vertical", lastDir.y);
    }

    void FixedUpdate()
    {
        float currentSpeed = running ? runSpeed : speed;
        rb.linearVelocity = move * currentSpeed;
    }
}