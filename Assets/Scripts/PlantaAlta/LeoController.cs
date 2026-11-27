using UnityEngine;
using UnityEngine.InputSystem;

public class LeoController : MonoBehaviour
{
    public float speed = 3f;
    public float runSpeed = 16f;

    [Header("Soporte de selección Cassandra / Leo")]
    public RuntimeAnimatorController controladorAlternoCassandra;
    public Sprite spriteInicialCassandra;
    public float escalaVisualCassandra = 0.16f;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    Vector2 move;
    Vector2 lastDir = new Vector2(0f, -1f); // empieza mirando hacia abajo
    public Vector2 UltimaDireccion { get { return lastDir; } }
    bool running;
    bool usarFlipLateral = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Si vienes del minijuego, reaparece donde se quedó y mirando hacia el mismo lado
        if (EstadoJuego.tieneGuardado)
        {
            transform.position = EstadoJuego.posicionLeo;
            lastDir = EstadoJuego.direccionLeo;
            EstadoJuego.tieneGuardado = false;
        }

        if (!SeleccionPersonaje.EsLeo() && controladorAlternoCassandra != null)
        {
            AplicarVisualCassandra();
        }
    }

    void AplicarVisualCassandra()
    {
        usarFlipLateral = true;

        SpriteRenderer baseSr = sr;
        if (anim != null) anim.enabled = false;
        if (baseSr != null) baseSr.enabled = false;

        GameObject visual = new GameObject("VisualCassandra");
        visual.transform.SetParent(transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(escalaVisualCassandra, escalaVisualCassandra, 1f);

        SpriteRenderer nuevoSr = visual.AddComponent<SpriteRenderer>();
        if (baseSr != null)
        {
            nuevoSr.sortingLayerID = baseSr.sortingLayerID;
            nuevoSr.sortingOrder = baseSr.sortingOrder;
            nuevoSr.sharedMaterial = baseSr.sharedMaterial;
            nuevoSr.color = baseSr.color;
        }
        nuevoSr.sprite = spriteInicialCassandra;

        Animator nuevoAnim = visual.AddComponent<Animator>();
        nuevoAnim.runtimeAnimatorController = controladorAlternoCassandra;

        sr = nuevoSr;
        anim = nuevoAnim;
    }

    void Start()
    {
        if (usarFlipLateral && sr != null && lastDir.x != 0f)
            sr.flipX = lastDir.x < 0f;

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

        // Si hay un diálogo abierto, el personaje se queda quieto respirando
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive)
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

        if (usarFlipLateral && sr != null && lastDir.x != 0f)
            sr.flipX = lastDir.x < 0f;

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