using System.Collections;
using UnityEngine;

public class BossEnemyController : MonoBehaviour
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
    public float nextAttackTime = 2f;
    public float meleeAttackDamage = 5f;
    public float attackDamage = 10f;
    public float attackMoveSpeed = 15.0f;
    public float attackDelay = 2f;
    public float live = 150f;
    public float minFallSpeed = 10f;
    public float fallDamageMultiplier = 1.5f;
    private float maxFallSpeed;
    private bool isDead;
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
    private float jumpTime = 0.5f ;
    private float vx;
    private float vy;
    private bool canJumpToWall;
    private bool canJumpToPlayer;
    private bool playerAlive;
    private bool haveSword = true;
    public GameObject swordPrefab;
    public Transform firePoint;
    public float launchTime = 2f;
    public float launchDelay = 5f;

    [Header("Audio Settings")]
    public AudioSource soundEffects; // The speaker component
    public AudioClip meleeSound;  // Sword swing sound
    public AudioClip throwSound;  // Whoosh sound
    public EnemyHealthController healthBar;
    private float maxLife;

    public EnemyCooldownController cooldownBar;
    private float currentCooldown = 0f;

    void Start()
    {
        maxLife = live;
        healthBar.SetMaxLife(maxLife);
        playerAlive = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("HumanFinn").GetComponent<PlayerTest>().transform;

        // Find the AudioSource component attached to this object
        soundEffects = GetComponent<AudioSource>();
        // If it doesn't exist, create one automatically
        if(soundEffects == null) soundEffects = gameObject.AddComponent<AudioSource>();
        if (meleeSound == null ) meleeSound = Resources.Load<AudioClip>("Audio/BossSounds/SwordBoss");
        if (throwSound == null ) throwSound = Resources.Load<AudioClip>("Audio/BossSounds/ShootBoss");
    }
    void Update()
    {
        animator.SetBool("isDead", isDead);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("hasGround", hasAnyGround);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("takeDamage", takeDamage);
        animator.SetFloat("verticalSpeed", rb.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        UpdateGroundedState();
        UpdateObstacleState();
        UpdateFallState();
        UpdateSwordCooldownUI();
        if (isAttacking || !playerAlive || isDead || takeDamage) return;
        Movement();
    }
    private void Movement() {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);
        if (hasAnyGround && !isGrounded && player.position.y >= transform.position.y)
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



        if (distanceToPlayer < detectionRadius)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            RaycastHit2D sightCheck = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, groundLayer);
            LookUpdate();
            if (distanceToPlayer <= attackRadius && hitWall.collider == null)
            {
                Debug.Log("Atacando");
                Attack();
                return;
            }
            else if (distanceToPlayer > attackRadius && sightCheck.collider == null && haveSword)
            {
                LaunchSword();
            }
            if (hasAnyGround && !isGrounded && player.position.y < transform.position.y - 0.3f)
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
            if (isGrounded)
            {
                Debug.Log("move to Player" + isAttacking);
                if (horizontalDistance > 0.2f)
                {
                    movementX = Mathf.Sign(player.position.x - transform.position.x);
                }
                else
                {
                    movementX = 0;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
                if (canJumpToWall)
                {
                    Debug.Log("Jump to WALL" + canJumpToPlayer);
                    JumpToObstacle();
                    return;
                }
                if (canJumpToPlayer && player.position.y > transform.position.y + 1f && distanceToPlayer < obstacleCheckRadius || canJumpToPlayer && player.position.y < transform.position.y - 1f && distanceToPlayer < obstacleCheckRadius)
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
        //Debug.Log(playerAlive);
        if (isDead || !playerAlive)
        {
            rb.velocity = new Vector2(0f,rb.velocity.y);
            movementX = 0;
            isAttacking = false;
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
        float jumpDir = -directionX;
        targetPosition = new Vector2(
            player.position.x + jumpDir * wallJumpHorizontalDistance,
            player.position.y + wallJumpHeight
        );
        targetPosition = player.position;
        Debug.DrawLine(transform.position, targetPosition, Color.green, 1f);
        delta = targetPosition - (Vector2)transform.position;
        float distance = delta.magnitude;
        float timeScale = Mathf.Clamp(distance / 5f, 0.8f, 1.2f);
        //jumpTime *= timeScale;

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
        Debug.Log("Front Grounded: " + frontGrounded + " Back Grounded: " + backGrounded);
        hasAnyGround = frontGrounded || backGrounded;
        isGrounded = frontGrounded && backGrounded;
        if (!wasGrounded && hasAnyGround )
        {
            animator.SetTrigger("land");
            //ApplyFallDamage();
            maxFallSpeed = 0f;
            if (isAttacking)
            {
                EndAttack();
            }
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
    public void Attack()
    {
        if (isAttacking ||Time.time < nextAttackTime ||takeDamage ||!playerAlive) return;
        if (haveSword)
        {
            if (isGrounded)
            {
                isAttacking = true;
                movementX = 0;
                GroundAttack();
                nextAttackTime = Time.time + attackDelay;
            }
            else if(!hasAnyGround && player.position.y < transform.position.y - 0.5f){
                isAttacking = true;
                AirAttack();
                nextAttackTime = Time.time + attackDelay;
            }
        }
    }

    public void PlayMeleeSound()
    {
        if (meleeSound != null && soundEffects != null)
        {
            // Play melee sound
            soundEffects.PlayOneShot(meleeSound);
        }
    }

    public void PlayThrowSound()
    {
        if (throwSound != null && soundEffects != null)
        {
            // Play throw sound
            soundEffects.PlayOneShot(throwSound);
        }
    }

    public void TakeDamage(Vector2 direction, float amountDamage)
    {
        Debug.Log("Entre");
        if (!takeDamage && !isAttacking)
        {
            Debug.Log("Boss Take Damage");
            takeDamage = true;
            animator.SetTrigger("hit");
            Debug.Log("Vida antes: " + live);
            live -= amountDamage;
            Debug.Log("Vida después: " + live);
            healthBar.UpdateLife(live);
            if (live <= 0)
            {
                isDead = true;
                isAttacking = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
                healthBar.gameObject.SetActive(false);
                //Trigger Victory
                if (player != null)
                {
                    // Access Player script and trigger WinGame directly
                    player.GetComponent<PlayerTest>().WinGame();
                }
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

    private void GroundAttack()
    {
        float heightDiff = player.position.y - transform.position.y;
        if (Mathf.Abs(heightDiff) < 0.3f)
        {
            animator.SetTrigger("attack_mid");
        }
        else if (heightDiff >= 0.3f)
        {
            animator.SetTrigger("attack_up");
        }
        else
        {
            animator.SetTrigger("attack_low");
        }
    }

    public void LaunchSword()
    {
        if (swordPrefab == null || player == null || !isGrounded || isAttacking || Time.time < launchTime) return;
        isAttacking = true;
        movementX = 0;
        currentCooldown = 0f;
        cooldownBar.SetCooldown(launchDelay);
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetTrigger("throw_sword");
        launchTime = Time.time + launchDelay;
    }

    public void ExecuteSwordThrow()
    {
        if (swordPrefab == null || firePoint == null) return;
        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        GameObject newSword = Instantiate(swordPrefab, firePoint.position, rotation);
        SwordController projectileScript = newSword.GetComponent<SwordController>();
        if (projectileScript != null)
        {
            projectileScript.maxDistance = detectionRadius;
            projectileScript.damage = attackDamage;
        }
        haveSword = false;
        StartCoroutine(RecoverSwordCoroutine());
    }
    private void AirAttack()
    {
        animator.SetTrigger("air_attack");
    }

    private void UpdateSwordCooldownUI()
    {
        if (!haveSword)
        {
            currentCooldown += Time.deltaTime;
            cooldownBar.UpdateCooldown(currentCooldown);
        }
        else
        {
            cooldownBar.UpdateCooldown(cooldownBar.maxCooldown);
        }
    }
    IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(0.5f);
        takeDamage = false;
    }
    IEnumerator RecoverSwordCoroutine()
    {
        while (currentCooldown < launchDelay)
        {
            currentCooldown += Time.deltaTime;
            cooldownBar.UpdateCooldown(currentCooldown);
            yield return null;
        }
        haveSword = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void DeleteBody()
    {
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, obstacleCheckRadius);
    }
}
