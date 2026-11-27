using UnityEngine;
using UnityEngine.InputSystem;

public class RunCassandra : MonoBehaviour
{
    public float speed = 3f;
    public float runSpeed = 6f;

    [Header("Soporte de selección Leo / Cassandra")]
    public RuntimeAnimatorController controladorAlternoLeo;
    public Sprite spriteInicialLeo;
    public float escalaVisualLeo = 6.25f;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    Vector2 move;
    Vector2 lastDir = new Vector2(0f, -1f); // empieza mirando hacia abajo
    public Vector2 UltimaDireccion { get { return lastDir; } }
    bool running;
    bool usarFlipLateral = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Si el SpriteRenderer está en un hijo, lo encuentra igual
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        if (SeleccionPersonaje.EsLeo() && controladorAlternoLeo != null)
        {
            AplicarVisualLeo();
        }
    }

    void AplicarVisualLeo()
    {
        usarFlipLateral = false;

        SpriteRenderer baseSr = sr;
        if (anim != null) anim.enabled = false;
        if (baseSr != null) baseSr.enabled = false;

        GameObject visual = new GameObject("VisualLeo");
        visual.transform.SetParent(transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(escalaVisualLeo, escalaVisualLeo, 1f);

        SpriteRenderer nuevoSr = visual.AddComponent<SpriteRenderer>();
        if (baseSr != null)
        {
            nuevoSr.sortingLayerID = baseSr.sortingLayerID;
            nuevoSr.sortingOrder = baseSr.sortingOrder;
            nuevoSr.sharedMaterial = baseSr.sharedMaterial;
            nuevoSr.color = baseSr.color;
        }
        nuevoSr.sprite = spriteInicialLeo;

        Animator nuevoAnim = visual.AddComponent<Animator>();
        nuevoAnim.runtimeAnimatorController = controladorAlternoLeo;

        sr = nuevoSr;
        anim = nuevoAnim;
    }

    void Start()
    {
        if (anim != null)
        {
            anim.SetFloat("Horizontal", lastDir.x);
            anim.SetFloat("Vertical", lastDir.y);
        }
    }

    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;

        // Si hay un diálogo abierto, el personaje se queda quieto
        if (DialogoNPC.Activo)
        {
            move = Vector2.zero;
            running = false;
            if (anim != null) anim.SetBool("IsMoving", false);
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

        // El sprite lateral de Cassandra mira a la derecha (Leo tiene clips propios izq/der)
        if (usarFlipLateral && sr != null && lastDir.x != 0f) sr.flipX = lastDir.x < 0f;

        if (anim != null)
        {
            anim.SetBool("IsMoving", moving);
            anim.SetFloat("Horizontal", lastDir.x);
            anim.SetFloat("Vertical", lastDir.y);
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = running ? runSpeed : speed;
        rb.linearVelocity = move * currentSpeed;
    }
}