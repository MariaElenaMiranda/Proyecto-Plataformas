using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    // Variables de Movimiento
    public float moveSpeed = 5f;
    public float moveSprint => moveSpeed * 2f;
    public float playerMovement;
    public bool sprint = false;

    // Variables de Salto
    public float jumpForce = 5f;
    public float doubleJumpForce => jumpForce * 1.5f;
    public bool canDoubleJump = true;

    // Variables de Gravedad (Caída)
    public float fallMultiplier = 2.5f;
    private float defaultGravity;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale; // Guardamos la gravedad original
    }

    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        // 1. OBTENER ENTRADA
        float move = Input.GetAxisRaw("Horizontal");

        // 2. LÓGICA DE SPRINT
        if (move != 0 && Input.GetKey(KeyCode.LeftShift)) sprint = true;
        else sprint = false;

        if (sprint) playerMovement = moveSprint;
        else playerMovement = moveSpeed;

        // 3. APLICAR VELOCIDAD
        rb.velocity = new Vector2(move * playerMovement, rb.velocity.y);

        // 4. SALTO Y DOBLE SALTO
        bool isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;

        if (isGrounded) canDoubleJump = true;

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                rb.gravityScale = defaultGravity; // Resetear gravedad para el impulso
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                canDoubleJump = false;
            }
        }

        // 5. CAÍDA RÁPIDA (GRAVEDAD DINÁMICA)
        if (rb.velocity.y <= 0) rb.gravityScale = defaultGravity * fallMultiplier;
        else rb.gravityScale = defaultGravity;

        // 6. ORIENTACIÓN
        if (move > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (move < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
    }
}
