using System.Collections;
using UnityEngine;

public class HardEnemyController : MonoBehaviour
{

    public Transform player;
    public float detectionRadius = 5f;
    public float attackRadius = 3.5f;
    public float moveSpeed = 2.0f;
    public float reboundForce = 5f;
    private Rigidbody2D rb;
    private float movementX;
    private bool isGrounded;
    private bool hasAnyGround;
    private bool wasGrounded;
    private bool frontGrounded;
    private bool backGrounded;
    private bool isAttacking;
    private float nextAttackTime = 0f;
    public float meleeAttackDamage = 2f;
    public float attackDamage = 6.5f;
    public float attackMoveSpeed = 10.0f;
    public float dashDuration = 0.8f;
    public float attackDelay = 5f;
    public float live = 40f;
    public float minFallSpeed = 10f;
    public float fallDamageMultiplier = 1.5f;
    private float maxFallSpeed;
    private bool isDead;
    public GameObject crate;
    private Animator animator;
    private bool takeDamage = false;
    public Transform groundCheck;
    public Transform groundCheckBehind;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;
    public float obstacleCheckRadius = 1.5f;
    public float jumpForce = 5f;
    public float jumpDistance = 1.0f;
    public float jumpCooldown = 0.5f;
    private float nextJumpTime = 0f;
    private float directionX;
    private RaycastHit2D hitWall;
    public float wallJumpHorizontalDistance = 3f;
    public float wallJumpHeight = 3.5f;
    public float wallJumpTime = 0.5f;
    public float maxJumpVelocityX = 6f;
    public float maxJumpVelocityY = 9f;
    private Vector2 targetPosition;
    private Vector2 delta ;
    private float jumpTime;
    private float vx;
    private float vy;
    private bool canJumpToWall;
    private bool canJumpToPlayer;
    private bool playerAlive;

    [Header("Sound Effects")]
    public AudioSource soundEffects; // The sound file to play
    public AudioClip attackSound; // The sound file to play
    public EnemyHealthController healthBar;
    private float maxLife;

    void Start()
    {
        maxLife = live;
        healthBar.SetMaxLife(maxLife);
        playerAlive = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("HumanFinn").GetComponent<Transform>();

        // Find the AudioSource component attached to this object
        soundEffects = GetComponent<AudioSource>();
        // If it doesn't exist, create one automatically
        if(soundEffects == null) soundEffects = gameObject.AddComponent<AudioSource>();

       
    }
    void Update()
    {
        animator.SetBool("isDead", isDead);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("hasGround", hasAnyGround);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("verticalSpeed", rb.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if (isAttacking || !playerAlive || isDead) return;

        UpdateGroundedState();
        UpdateObstacleState();
        Movement();
        UpdateFallState();
    }
    private void Movement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceToPlayer < detectionRadius)

        {
            LookUpdate();
            if (hasAnyGround && !isGrounded && player.position.y < transform.position.y)
            {
                Debug.Log("fall to Player");
                float facing = Mathf.Sign(transform.localScale.x);

                if (playerAlive && !isDead)
                {

                    if (horizontalDistance > 0.2f)
                    {
                        movementX = Mathf.Sign(player.position.x - transform.position.x);
                        return;
                    }
                    else
                    {
                        movementX = 0;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        return;
                    }
                }
            }
            else if(hasAnyGround && !isGrounded && player.position.y >= transform.position.y )
            {
                Debug.Log("acomodares");
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
            else if (isGrounded)
            {
                if (playerAlive && !isDead)
                {
                    Debug.Log("move to Player" + isAttacking);
                    if (horizontalDistance > 0.2f )
                    {
                        movementX = Mathf.Sign(player.position.x - transform.position.x);
                    }
                    else
                    {
                        movementX = 0;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }

                if(distanceToPlayer < attackRadius && hitWall.collider == null && hitWall.collider == null && player.position.y <= transform.position.y+0.5f)
                {
                    Debug.Log("Atacando" );
                    Attack();
                    return;
                }

                if (canJumpToWall )
                {
                    Debug.Log("Jump to WALL" + canJumpToPlayer);
                    JumpToObstacle();
                    return;
                }

                if (canJumpToPlayer && player.position.y > transform.position.y + 1f  && distanceToPlayer < obstacleCheckRadius)
                {
                    Debug.Log("Jump to PLAYER");
                    JumpToPlayer();
                    return;
                }
            }
        }
        else
        {
            Debug.Log("Idle");
            movementX = 0;
        }
    }
    private void LookUpdate() {
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x < 0)
        {
            directionX = 1f;
        }
        else if (direction.x > 0)
        {
            directionX = -1f;
        }
        transform.localScale = new Vector3(directionX, transform.localScale.y, transform.localScale.z);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 damageDirection = new Vector2(transform.position.x, 0);

            PlayerTest playerScript = collision.gameObject.GetComponent<PlayerTest>();
            if (isAttacking)
            {
                playerScript.TakeDamage(damageDirection, attackDamage * meleeAttackDamage);
            }
            else
            {
                playerScript.TakeDamage(damageDirection, attackDamage);
            }

            playerAlive = !playerScript.isDead;

            if (!playerAlive)
            {
                isAttacking = false;
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
        Debug.Log(playerAlive);
        if (isDead || !playerAlive)
        {
            rb.velocity = new Vector2(0f,rb.velocity.y);
            movementX = 0;
            return;
        }
        if (hasAnyGround && !takeDamage) {
            rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
        }
    }
    private void JumpToObstacle()
    {
        if (Time.time < nextJumpTime || !isGrounded || !canJumpToWall || isAttacking) return;

        float jumpDir = -directionX;
        targetPosition = new Vector2(
            transform.position.x + jumpDir * wallJumpHorizontalDistance,
            transform.position.y + wallJumpHeight
        );

        Debug.DrawLine(transform.position, targetPosition, Color.yellow, 1f);

        delta = targetPosition - (Vector2)transform.position;

        vx = delta.x / wallJumpTime;
        vy = (delta.y - 0.5f * Physics2D.gravity.y * wallJumpTime * wallJumpTime) / wallJumpTime;
        movementX = vx;
        vx = Mathf.Clamp(vx, -maxJumpVelocityX, maxJumpVelocityX);
        vy = Mathf.Clamp(vy, 0f, maxJumpVelocityY);
        rb.velocity = new Vector2(vx, vy);

        isGrounded = false;
        nextJumpTime = Time.time + jumpCooldown;
    }
    private void JumpToPlayer()
    {
        if (Time.time < nextJumpTime || !isGrounded || !canJumpToPlayer || isAttacking) return;

        targetPosition = player.position;
        Debug.DrawLine(transform.position, targetPosition, Color.green, 1f);

        float distance = delta.magnitude;
        float timeScale = Mathf.Clamp(distance / 5f, 0.8f, 1.2f);
        jumpTime *= timeScale;

        vx = delta.x / jumpTime;
        vy = (delta.y - 0.5f * Physics2D.gravity.y * jumpTime * jumpTime) / jumpTime;
        vx = Mathf.Clamp(vx, -maxJumpVelocityX, maxJumpVelocityX);
        vy = Mathf.Clamp(vy, 0f, maxJumpVelocityY);
        movementX = vx;
        rb.velocity = new Vector2(vx, vy);

        isGrounded = false;
        nextJumpTime = Time.time + jumpCooldown;
    }
    private void UpdateGroundedState()
    {
        wasGrounded = hasAnyGround;
        RaycastHit2D hitFront = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D hitBack = Physics2D.Raycast(groundCheckBehind.position, Vector2.down, groundCheckDistance, groundLayer);

        frontGrounded = hitFront.collider != null;
        backGrounded = hitBack.collider != null;
        hasAnyGround = frontGrounded || backGrounded;
        isGrounded = frontGrounded && backGrounded;

        if (!wasGrounded && hasAnyGround )
        {
            animator.SetTrigger("land");
            ApplyFallDamage();
            maxFallSpeed = 0f;
        }
    }
    private void UpdateObstacleState()
    {
        if (!isGrounded) {
            canJumpToWall = false;
            canJumpToPlayer = false;
            return;
        }
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + 0.5f);
        float wallDir = Mathf.Sign(movementX);

        if (wallDir == 0) wallDir = Mathf.Sign(player.position.x - transform.position.x);

        hitWall = Physics2D.Raycast(rayOrigin, Vector2.right * wallDir, obstacleCheckRadius, groundLayer);
        Debug.DrawRay(rayOrigin, Vector2.right * wallDir * obstacleCheckRadius, Color.cyan);

        bool wallCloseEnough = hitWall.collider != null;
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, detectionRadius, groundLayer);
        Debug.DrawRay(transform.position, Vector2.up * detectionRadius, Color.magenta);

        canJumpToWall = wallCloseEnough && hitUp.collider == null;
        canJumpToPlayer = hitUp.collider == null && !canJumpToWall;
    }
    private void UpdateFallState()
    {
        if (!hasAnyGround && rb.velocity.y < -0.1f)
        {
            maxFallSpeed = Mathf.Max(maxFallSpeed, Mathf.Abs(rb.velocity.y));
        }
    }
    private void ApplyFallDamage()
    {
        float fallSpeed = Mathf.Abs(maxFallSpeed);
        if (fallSpeed >= minFallSpeed && !isAttacking)
        {
            float damage = (fallSpeed - minFallSpeed) * fallDamageMultiplier;
            TakeDamage(new Vector2(transform.position.x, 0), damage);
        }
    }
    public void Attack()
    {
        if (isAttacking ||!isGrounded ||Time.time < nextAttackTime ||takeDamage ||!playerAlive) return;

        isAttacking = true;
        movementX = Mathf.Sign(player.position.x - transform.position.x);
        animator.SetTrigger("attack");
        StartCoroutine(PerformAttack());
        nextAttackTime = Time.time + attackDelay;
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

    public void TakeDamage(Vector2 direction, float amountDamage)
    {
        if (!takeDamage && !isAttacking)
        {
            takeDamage = true;
            animator.SetTrigger("hit");
            live -= amountDamage;
            healthBar.UpdateLife(live);
            if (live <= 0)
            {
                isDead = true;
                isAttacking = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
                healthBar.gameObject.SetActive(false);
            }
            else
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 1).normalized;
                rb.velocity = Vector2.zero;
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
        float originalSpeed = moveSpeed;
        moveSpeed = attackMoveSpeed;
        animator.speed = 1.5f;
        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }
        isAttacking = false;
        moveSpeed = originalSpeed;
        animator.speed = 1f;
    }
    public void DeleteBody()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y + 1);
        crate = Instantiate(crate, position, transform.rotation);
        crate.GetComponent<Crate>().qty = 7;
        crate.GetComponent<Crate>().chance = 100;
        Destroy(gameObject);
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
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundCheckBehind.position, groundCheckBehind.position + Vector3.down * groundCheckDistance);
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector2.up * detectionRadius);
        float directionX = (transform.localScale.x < 0) ? 1f : -1f;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 0.5f, 0);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(rayOrigin, Vector2.right * directionX * obstacleCheckRadius);
    }
}
