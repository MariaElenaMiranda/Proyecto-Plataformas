using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EasyEnemyController : MonoBehaviour
{

    public Transform player;

    public float detectionRadius = 5f;
    public float moveSpeed = 2.0f;
    public float reboundForce = 5f;

    private Rigidbody2D rb;
    private float movementX;

    private bool isMoving;

    private Animator animator;
    private bool takeDamage = false;
    public float attackDamage = 10f;

    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        bool hasGround = hit.collider != null;



        if (distanceToPlayer < detectionRadius)
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

            if (hasGround)
            {
                movementX = direction.x ;

                isMoving = true;

            }
            else
            {
                movementX = 0;
                isMoving = false;
            }
        }
        else {
            movementX = 0; 
            isMoving = false;
        }

        //rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
        animator.SetBool("isMoving", isMoving);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Vector2 damageDirection = new Vector2(transform.position.x, 0);

            collision.gameObject.GetComponent<PlayerTest>().TakeDamage(damageDirection, attackDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("sword"))
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x, 0);

            TakeDamage(damageDirection, 1);
        }
    }

    private void FixedUpdate()
    {
        if(!takeDamage) { 
            rb.velocity = new Vector2(movementX * moveSpeed, rb.velocity.y);
   
        }
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);


        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }

    public void TakeDamage(Vector2 direction, int amountDamage)
    {
        if (!takeDamage)
        {
            takeDamage = true;
            Vector2 rebound = new Vector2(transform.position.x - direction.x, 1).normalized;
            rb.AddForce(rebound * reboundForce, ForceMode2D.Impulse);
            StartCoroutine(DisableDamage());
        }
    }

    IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(0.4f);
        takeDamage = false;
        rb.velocity = Vector2.zero;
    }
}
