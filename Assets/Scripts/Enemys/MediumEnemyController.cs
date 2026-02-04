using System.Collections;
using UnityEngine;

public class MediumEnemyController : MonoBehaviour
{
    [Header("Target Configuration")]
    public Transform player;

    [Header("Stats")]
    public float detectionRadius = 5f;
    public float attackRadius = 3.5f;
    public float moveSpeed = 2.0f;
    public float reboundForce = 5f;
    public float live = 25f;

    [Header("Attack Configuration")]
    public float attackDelay = 3f;
    public float meleeAttackDamage = 2f;
    public float attackDamage = 7.5f;
    private float nextAttackTime = 0f;

    [Header("Fall & Damage Settings")]
    public float minFallSpeed = 10f;
    public float fallDamageMultiplier = 1.5f;
    private float maxFallSpeed;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public Transform groundCheckBehind;
    private bool frontGrounded;
    private bool backGrounded;
    private bool hasAnyGround;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Internal Variables")]
    private Rigidbody2D rb;
    private float movementX;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isFalling;
    private bool isAttacking;
    private bool isDead;
    private Animator animator;
    private bool takeDamage = false;
    private bool playerAlive;

    [Header("Sound Effects")]
    public AudioSource soundEffects; // Reference to the speaker
    public AudioClip attackSound; // The sound file to play

    void Start()
    {
        playerAlive= true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Find the AudioSource component attached to this object
        soundEffects = GetComponent<AudioSource>();
        // If it doesn't exist, create one automatically
        if(soundEffects == null) soundEffects = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (playerAlive && !isDead)
        {
            Movement();
        }

        UpdateGroundedState();
        UpdateFallState();

        animator.SetBool("isDead", isDead);
        animator.SetBool("isAttacking", isAttacking);

        animator.SetBool("isGrounded", hasAnyGround);
        animator.SetFloat("verticalSpeed", rb.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(movementX));
    }

    private void Movement() {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (hasAnyGround && !isGrounded)
        {
            float facing = Mathf.Sign(transform.localScale.x);

            if (frontGrounded && !backGrounded)
            {
                movementX = -facing;
                return;
            }
            else if (backGrounded && !frontGrounded)
            {
                movementX = facing;
                return;
            }
        }

        if (distanceToPlayer < attackRadius)
        {
            Attack();
            return;
        }
        else if (distanceToPlayer < detectionRadius && distanceToPlayer > attackRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x > 0)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }

            if (isGrounded && playerAlive && !isDead)
            {
                movementX = direction.x;
            }
            else
            {
                movementX = 0;
            }
        }
        else
        {
            movementX = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Vector2 damageDirection = new Vector2(transform.position.x, 0);

            PlayerTest playerScript = collision.gameObject.GetComponent<PlayerTest>();
            if (isAttacking) {
                playerScript.TakeDamage(damageDirection, attackDamage * meleeAttackDamage);
            }
            else {
                playerScript.TakeDamage(damageDirection, attackDamage);
            }

            playerAlive = !playerScript.isDead;

            if (!playerAlive) {
                movementX = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("sword"))
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x, 0);
            PlayerTest player = collision.GetComponentInParent<PlayerTest>();

            TakeDamage(damageDirection, player.attackDamage);

        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (!takeDamage && hasAnyGround) {
            rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
        }
    }


    private void UpdateGroundedState()
    {
        wasGrounded = isGrounded;
        RaycastHit2D hitFront = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D hitBack = Physics2D.Raycast(groundCheckBehind.position, Vector2.down, groundCheckDistance, groundLayer);

        frontGrounded = hitFront.collider != null;
        backGrounded = hitBack.collider != null;

        hasAnyGround = frontGrounded || backGrounded;
        isGrounded = frontGrounded && backGrounded;

        if (!wasGrounded && isGrounded)
        {

            animator.SetTrigger("land");
            ApplyFallDamage();
            maxFallSpeed = 0f;
        }
    }

    private void UpdateFallState()
    {
        if (!isGrounded && rb.velocity.y < -0.1f)
        {
            isFalling = true;
            maxFallSpeed = Mathf.Max(maxFallSpeed, Mathf.Abs(rb.velocity.y));
        }
        else
        {
            isFalling = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
        if (groundCheckBehind != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundCheckBehind.position, groundCheckBehind.position + Vector3.down * groundCheckDistance);
        }

    }

    public void Attack() {
        if (isGrounded && !isFalling && Time.time >= nextAttackTime && !takeDamage)
        {
            movementX = 0;
            isAttacking = true;
            StartCoroutine(PerformAttack());
            nextAttackTime = Time.time + attackDelay;
        }
    }

    public void PlayAttackSound() // Method called by Animation Event
    {
        // Check if sound and speaker exist
        if (attackSound != null && soundEffects != null)
        {
            soundEffects.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch slightly for realism
            soundEffects.PlayOneShot(attackSound); // Play the sound once
        }
    }

    private void ApplyFallDamage()
    {
        float fallSpeed = Mathf.Abs(maxFallSpeed);

        if (fallSpeed >= minFallSpeed)
        {
            float damage = (fallSpeed - minFallSpeed) * fallDamageMultiplier;

            TakeDamage(new Vector2(transform.position.x, 0), damage);
        }
    }

    public void TakeDamage(Vector2 direction, float amountDamage)
    {
        if (!takeDamage && !isAttacking)
        {
            takeDamage = true;
            animator.SetTrigger("hit");
            live -= amountDamage;
            if (live <= 0)
            {
                isDead = true;
                isAttacking = false;
            }
            else
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 1).normalized;
                rb.AddForce(rebound * reboundForce, ForceMode2D.Impulse);
                StartCoroutine(DisableDamage());
            }
        }
    }

    IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(0.5f);
        takeDamage = false;
    }

    IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void DeleteBody() {
        Destroy(gameObject);
    }
}
