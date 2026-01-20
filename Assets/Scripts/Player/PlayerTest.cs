using UnityEngine;

public class PlayerTest : MonoBehaviour
{

    public float moveSpeed = 5f;

    public float jumpForce = 7f;
    public float reboundForce = 7f;

    public float mana = 100f;
    public float manaRegenPorcent = 5f;
    public float maxMana = 100f;

    public float live = 100f;
    public float liveRegenPorcent = 5f;
    public float maxLive = 100f;
    public float lengthRay = 0.1f;

    public LayerMask groundLayer;

    private bool isGrounded;
    private bool canDoubleJump;
    private bool takeDamage;
    private bool isDead;
    private bool isAttacking;
    private Rigidbody2D rb;

    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
        movement();
        RegenerateMana();
        RegenerateLive();
        //Salto
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, lengthRay, groundLayer);

        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if(Input.GetButtonDown("Jump") && !takeDamage)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }

        if (Input.GetKey(KeyCode.Z) && !isAttacking ) {
            Attack();
        }
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("takeDamage", takeDamage);
        animator.SetBool("isAttacking", isAttacking);
    }

    private void movement()
    {
        float moveSpeedX = Input.GetAxisRaw("Horizontal") * moveSpeed;

        animator.SetFloat("movement", moveSpeedX * moveSpeed);

        if (moveSpeedX < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (moveSpeedX > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }

        Vector3 position = transform.position;

        if (!takeDamage)
        {

            transform.position = new Vector3(position.x + moveSpeedX * Time.deltaTime, position.y, position.z);
        }

    }
    private void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private void RegenerateMana()
    {
        if(!isAttacking && mana < maxMana && !takeDamage)
        {
            mana += (manaRegenPorcent/100) * Time.deltaTime;
            if (mana > maxMana)
            {
                mana = maxMana;
            }
        }
    }

    private void RegenerateLive()
    {
        if (!isAttacking && live < maxLive && !takeDamage)
        {
            live += (liveRegenPorcent / 100) * Time.deltaTime;
            if (live > maxLive)
            {
                live = maxLive;
            }
        }
    }

    public void TakeDamage(Vector2 direction , float amountDamage) {
        if (!takeDamage)
        {
            takeDamage = true;
            live -= amountDamage;

            if(live <= 0)
            {
                live = 0;
                isDead = true;
                return;
            }
            Vector2 rebound = new Vector2(transform.position.x - direction.x, 1).normalized;
            rb.AddForce(rebound * reboundForce, ForceMode2D.Impulse);
        }
    }

    public void DisableDamage() {
        takeDamage = false;
        rb.velocity = Vector2.zero;
    }

    public void Attack()
    {
        if (mana >= 5) { 
            isAttacking = true;
            mana -= 5f;
        }
    }

    public void DisableAttack()
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * lengthRay);
    }
    //// Variables de Movimiento
    //public float moveSpeed = 5f;
    //public float moveSprint => moveSpeed * 2f;
    //public float playerMovement;
    //public bool sprint = false;

    //// Variables de Salto
    //public float jumpForce = 5f;
    //public float doubleJumpForce => jumpForce * 1.5f;
    //public bool canDoubleJump = true;

    //// Variables de Gravedad (Caída)
    //public float fallMultiplier = 2.5f;
    //private float defaultGravity;

    //// Variables de suavizado
    //public float smoothTime = 0.1f;
    //private float smoothSpeed;

    //private Rigidbody2D rb;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody2D>();
    //    defaultGravity = rb.gravityScale; // Guardamos la gravedad original
    //}

    //void Update()
    //{
    //    Movement();
    //}

    //private void Movement()
    //{
    //    // 1. OBTENER ENTRADA
    //    float move = Input.GetAxisRaw("Horizontal");

    //    // 2. LÓGICA DE SPRINT
    //    if (move != 0 && Input.GetKey(KeyCode.LeftShift)) sprint = true;
    //    else sprint = false;

    //    if (sprint) playerMovement = moveSprint;
    //    else playerMovement = moveSpeed;

    //    // 3. APLICAR VELOCIDAD
    //    float fullSpeed = move * playerMovement;
    //    float SmoothSpeed = Mathf.SmoothDamp(
    //        rb.velocity.x,
    //        fullSpeed,
    //        ref smoothSpeed,
    //        smoothTime
    //    );
    //    rb.velocity = new Vector2(SmoothSpeed, rb.velocity.y);

    //    // 4. SALTO Y DOBLE SALTO
    //    bool isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;

    //    if (isGrounded) canDoubleJump = true;

    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        if (isGrounded)
    //        {
    //            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    //        }
    //        else if (canDoubleJump)
    //        {
    //            rb.gravityScale = defaultGravity; // Resetear gravedad para el impulso
    //            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    //            canDoubleJump = false;
    //        }
    //    }

    //    // 5. CAÍDA RÁPIDA (GRAVEDAD DINÁMICA)
    //    if (rb.velocity.y <= 0) rb.gravityScale = defaultGravity * fallMultiplier;
    //    else rb.gravityScale = defaultGravity;

    //    // 6. ORIENTACIÓN
    //    if (move > 0)
    //    {
    //        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    //    }
    //    else if (move < 0)
    //    {
    //        transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
    //    }
    //}
}
 