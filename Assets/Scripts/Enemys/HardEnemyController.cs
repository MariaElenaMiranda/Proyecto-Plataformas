using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static System.Net.WebRequestMethods;

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
    private bool isFalling;
    

    private bool isAttacking;
    private float nextAttackTime = 0f;
    public float meleeAttackDamage = 2f;
    public float attackDamage = 10f;


    
    public float attackMoveSpeed = 10.0f;
    
    public float dashDuration = 0.8f;
    public float attackDelay = 3f;


    public float live = 20f;
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
    public int raysCount = 5;                
    public float jumpForce = 5f;
    public float jumpDistance = 1.0f;
    public float jumpCooldown = 0.5f;
    private float nextJumpTime = 0f;
    private float directionX;
    private RaycastHit2D hitWall;
    public float wallJumpHorizontalDistance = 3f;
    public float wallJumpHeight = 3.5f;
    public float wallJumpTime = 0.5f;


    private bool canJumpToWall;

    private bool canJumpToPlayer;


    private bool nextTouWall;
    private bool playerAlive;


    // Start is called before the first frame update
    void Start()
    {
        playerAlive= true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (playerAlive && !isDead)
        {
            
            UpdateGroundedState();
            
            UpdateObstacleState();
            Movement();
            //UpdateFallState();

            //animator.SetBool("isDead", isDead);
            //animator.SetBool("isAttacking", isAttacking);
            animator.SetBool("hasGround", hasAnyGround);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("verticalSpeed", rb.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(rb.velocity.x)); 
        }

    }       

    private void Movement() {
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);
        

        
       if (distanceToPlayer < detectionRadius)

        {
            if (hasAnyGround && !isGrounded && player.position.y < transform.position.y )
            {

                float facing = Mathf.Sign(transform.localScale.x);

                lookUpdate();

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
            else if(hasAnyGround && !isGrounded && player.position.y >= transform.position.y)
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
            }else if (isGrounded)
            {
                lookUpdate();

                if (playerAlive && !isDead)
                {

                    if (horizontalDistance > 0.2f)
                    {
                        movementX = Mathf.Sign(player.position.x - transform.position.x);
                        
                    }
                    else
                    {
                        movementX = 0;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }

                if (canJumpToWall )
                {
                    Debug.Log("Jump to WALL" + canJumpToPlayer);
                    JumpToObstacle();
                    return;
                }

                if (canJumpToPlayer && player.position.y > transform.position.y + 1f)
                {
                    Debug.Log("Jump to PLAYER");
                    JumpToPlayer();
                }
            }
            


            //CheckObstacles();
        }
        else
        {
            movementX = 0;
        }

        //if (distanceToPlayer <= attackRadius && !isAttacking)
        //{
            
        //}
    }
    private void lookUpdate() {

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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("sword"))
    //    {
    //        Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x, 0);
    //        PlayerTest player = collision.GetComponentInParent<PlayerTest>();

    //        TakeDamage(damageDirection, player.attackDamage);

    //    }
    //}

    private void FixedUpdate()
    {
        if (isDead || !playerAlive)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (hasAnyGround) {
        
            rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
        }
        //|| player.position.y < transform.position.y - 0.3f
    }
    private void JumpToObstacle()
    {
        if (Time.time < nextJumpTime || !isGrounded || !canJumpToWall) return;

        float jumpDir = -directionX;
        Vector2 targetPosition = new Vector2(
            transform.position.x + jumpDir * wallJumpHorizontalDistance,
            transform.position.y + wallJumpHeight
        );

        Debug.DrawLine(transform.position, targetPosition, Color.yellow, 1f);

        Vector2 delta = targetPosition - (Vector2)transform.position;

        float vx = delta.x / wallJumpTime;
        float vy = (delta.y - 0.5f * Physics2D.gravity.y * wallJumpTime * wallJumpTime) / wallJumpTime;

        rb.velocity = new Vector2(vx, vy);

        isGrounded = false;
        nextJumpTime = Time.time + jumpCooldown;
    }

    private void JumpToPlayer()
    {
        // Revisamos cooldown y que esté en el suelo
        if (Time.time < nextJumpTime || !isGrounded || !canJumpToPlayer) return;

        Vector2 targetPosition = player.position;
        Debug.DrawLine(transform.position, targetPosition, Color.green, 1f);

        Vector2 delta = targetPosition - (Vector2)transform.position;
        float jumpTime = 0.5f; 
        float vx = delta.x / jumpTime;
        float vy = (delta.y - 0.5f * Physics2D.gravity.y * jumpTime * jumpTime) / jumpTime;
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

        //// Logs para depuración
        //Debug.Log($"Wall: {hitWall.collider}, canJumpToWall: {canJumpToWall}, canJumpToPlayer: {canJumpToPlayer}");
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



    //private void UpdateFallState()
    //{
    //    if (!isGrounded && rb.velocity.y < -0.1f)
    //    {
    //        isFalling = true;
    //        maxFallSpeed = Mathf.Max(maxFallSpeed, Mathf.Abs(rb.velocity.y));
    //    }
    //    else
    //    {
    //        isFalling = false;
    //    }
    //}

    //private void CheckObstacles() {

    //    float hits = 0;

    //    if (!isGrounded || isDead || !playerAlive ||Time.time < nextJumpTime || isFalling) { 
    //        return;
    //    }

    //    float directionX = transform.localScale.x * -1;
    //    Vector2 rayOrigin = (Vector2)transform.position + new Vector2(directionX * 0.2f, 0.5f);

    //    for (int i = 0; i < raysCount; i++) {
    //        float range = Mathf.Lerp(0f, 70f, (float)i / (raysCount - 1));

    //        float angle = range * Mathf.Deg2Rad;

    //        float x = Mathf.Cos(angle) * directionX;
    //        float y = Mathf.Sin(angle);

    //        Vector2 hitDirection = new Vector2(x, y).normalized;

    //        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, hitDirection, obstacleCheckRadius, groundLayer);
    //        Debug.DrawRay(rayOrigin, hitDirection * obstacleCheckRadius, Color.cyan);



    //        if (hit.collider != null && !hit.collider.CompareTag("Player"))
    //        {
    //            hits++;
    //        }

    //        if (hits >= 2)
    //        {
    //            Jump();
    //        }

    //    }





    //}

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

        // 4. Detector de pared frontal (Raycast Wall)
        // Usamos la escala para saber hacia dónde apunta el rayo de la pared
        float directionX = (transform.localScale.x < 0) ? 1f : -1f;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 0.5f, 0);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(rayOrigin, Vector2.right * directionX * obstacleCheckRadius);

      
    }


    

    //public void Attack()
    //{
    //    if (Time.time >= nextAttackTime && !takeDamage && playerAlive)
    //    {
    //        isAttacking = true;
    //        animator.SetTrigger("attack");
    //        StartCoroutine(PerformAttack());
    //        nextAttackTime = Time.time + dashDuration + attackDelay;
    //    }
    //}

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
                rb.velocity = new Vector2(0, rb.velocity.y);
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

    /// <summary>
   
    /// </summary>
    /// <returns></returns>

    IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(0.5f);
        takeDamage = false;

    }

    //IEnumerator PerformAttack()
    //{

    //    float originalSpeed = moveSpeed;

    //    moveSpeed = attackMoveSpeed;

    //    animator.speed = 1.5f;
    //    float time = 0f;

    //    while (time < dashDuration) {
    //        if(time > 0.1f && Mathf.Abs(rb.velocity.x) < 0.1f) {
    //            rb.velocity = new Vector2(movementX * moveSpeed, Mathf.Max(rb.velocity.y, jumpForce * 1f));
    //            nextAttackTime = Time.time - dashDuration - attackDelay;
    //            break;
    //        }

    //        rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
    //        time += Time.deltaTime;
    //        yield return null;
    //    }

    //    if (!playerAlive)
    //    {
    //        movementX = 0;
    //        isAttacking = false;
    //        moveSpeed = originalSpeed;
    //        animator.speed = 1.0f;
    //    }
    //    else { 

    //        isAttacking = false;
    //        moveSpeed = originalSpeed;
    //        animator.speed = 1.0f;
    //    }

    //}

    //public void DeleteBody() { 
    //    Destroy(gameObject);
    //}
}
