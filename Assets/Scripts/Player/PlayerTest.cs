using System.Collections;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [Header("Movement Configuration")]
    public float moveSpeed = 4f;
    public float moveSprint => moveSpeed * 2f;
    public float playerMovement;
    public bool sprint = false;
    public float jumpForce = 5f;
    public float reboundForce = 5f;
    private bool isGrounded;
    private bool canDoubleJump;
    public float doubleJumpForce => jumpForce * 1.5f;
    public float lengthRay = 0.55f;
    public float fallMultiplier = 2.5f;
    private float defaultGravity;
    [Header("Mana Configuration")]
    public float mana = 100f;
    public float manaRegenPercent = 75f;
    public float maxMana = 100f;
    public float basicAttackManaCost = 4f;
    public float powerAttackManaMultiplier = 2f;
    public float PowerAttackManaCost => basicAttackManaCost * powerAttackManaMultiplier;
    [Header("Life Configuration")]
    public float live = 100f;
    public float liveRegenPercent = 50f;
    public float maxLive = 100f;
    [Header("Attack Configuration")]
    public float attackDamage = 7;
    public float powerAttackMultiplier = 1.75f;
    public float PowerAttackDamage => attackDamage * powerAttackMultiplier;
    public bool isDead;
    private bool isAttacking;
    private bool takeDamage;
    [Header("Powerups")]
    public int LifePowerUpQty = 0;
    public int ManaPowerUpQty = 0;
    public int AttackPowerUpQty = 0;
    public int SpeedPowerUpQty = 0;
    [Header("Others")]
    public float smoothTime = 0.1f;
    private float originalDamage;
    private float smoothSpeed;
    private Rigidbody2D rb;
    [Header("Auto Selection")]
    public LayerMask groundLayer;
    public Animator animator;
    public GameplaySystem gameplaySystem; // Reference to change scenes

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        gameplaySystem = GameObject.Find("GameplayManager").GetComponent<GameplaySystem>();
        animator = GameObject.Find("HumanFinn").GetComponent<Animator>();
        groundLayer = LayerMask.GetMask("ground");
    }
    void Update()
    {
        if (!isDead)
        {
            Movement();
            Gravity();
            RegenerateMana();
            RegenerateLive();
            Jump();

            if (Input.GetKey(KeyCode.F) && !isAttacking)
            {
                Attack();
            }
        }

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("takeDamage", takeDamage);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDead", isDead);
    }

    private void Movement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (!takeDamage)
        {
            if (moveInput != 0 && Input.GetKey(KeyCode.LeftShift) && mana >= 2) {
                sprint = true;
            }
            else sprint = false;

            if (sprint) playerMovement = moveSprint;
            else playerMovement = moveSpeed;

            float fullSpeed = moveInput * playerMovement;
            float SmoothSpeed = Mathf.SmoothDamp(
                rb.velocity.x,
                fullSpeed,
                ref smoothSpeed,
                smoothTime
            );

            rb.velocity = new Vector2(SmoothSpeed, rb.velocity.y);

            if (sprint)
            {
                mana -= 2 * Time.deltaTime;
                if (mana < 0) mana = 0;
            }

        }

        animator.SetFloat("movement", Mathf.Abs(moveInput));

        if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }


    }
    private void Jump()
    {
        isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;

        if (isGrounded) canDoubleJump = true;

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                rb.gravityScale = defaultGravity;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                canDoubleJump = false;
            }
        }
    }

    private void RegenerateMana()
    {
        if (!isAttacking && mana < maxMana && !takeDamage)
        {
            mana += (manaRegenPercent / 100) * Time.deltaTime;
            if (mana > maxMana)
            {
                mana = maxMana;
            }
        }
    }

    private void RegenerateLive()
    {
        if (!isAttacking && live < maxLive && !takeDamage && !isDead)
        {
            live += (liveRegenPercent / 100) * Time.deltaTime;
            if (live > maxLive)
            {
                live = maxLive;
            }
        }
    }

    public void TakeDamage(Vector2 direction, float amountDamage)
    {
        if (!takeDamage)
        {
            takeDamage = true;
            live -= amountDamage;

            if (live <= 0)
            {
                live = 0;
                StartCoroutine(DeathSequence()); // Start the death coroutine
                return;
            }
            if (!isDead)
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 1).normalized;
                rb.AddForce(rebound * reboundForce, ForceMode2D.Impulse);
            }
        }
    }

    public void DisableDamage()
    {
        takeDamage = false;
        rb.velocity = Vector2.zero;
    }

    public void Attack()
    {
        originalDamage = attackDamage;
        if (mana >= basicAttackManaCost)
        {
            if (sprint && mana >= PowerAttackManaCost)
            {
                isAttacking = true;
                mana -= PowerAttackManaCost;
                attackDamage = PowerAttackDamage;
            }
            else
            {
                isAttacking = true;
                mana -= basicAttackManaCost;
            }
        }
    }

    public void DisableAttack()
    {
        isAttacking = false;
        attackDamage = originalDamage;
    }

    private void Gravity()
    {
        if (rb.velocity.y <= 0)
        {
            rb.gravityScale = defaultGravity * fallMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * lengthRay);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player hit the bottom death zone
        if (collision.CompareTag("BottomLimit"))
        {
            // Start death sequence only if not already dead
            if (!isDead) StartCoroutine(DeathSequence());
        }
    }


    // Powerups
    public void IncreaseLifeRegen(float percent)
    {
        LifePowerUpQty++;
        this.liveRegenPercent += this.liveRegenPercent * percent;
    }
    public void IncreaseManaRegen(float percent)
    {
        ManaPowerUpQty++;
        this.manaRegenPercent += this.manaRegenPercent * percent;
    }
    public void IncreaseAttackDamage(float percent)
    {
        AttackPowerUpQty++;
        this.attackDamage += this.attackDamage * percent;
    }
    public void IncreaseSpeedMovement(float percent)
    {
        SpeedPowerUpQty++;
        this.moveSpeed += this.moveSpeed * percent;
    }

    IEnumerator DeathSequence()
    {
        isDead = true; // Set dead state
        rb.velocity = Vector2.zero; // Stop movement immediately to prevent sliding

        yield return new WaitForSeconds(1.5f); // Wait for death animation to finish (1.5 seconds)

        // Call the system to Fade Out and change scene
        if (gameplaySystem != null) gameplaySystem.ExitScene("GameOverMenu");
    }
}