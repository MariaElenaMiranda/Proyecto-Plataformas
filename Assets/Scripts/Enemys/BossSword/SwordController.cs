using UnityEngine;

public class SwordController : MonoBehaviour
{
    public float speed = 12f;
    public float damage = 7.5f;
    public float maxDistance;
    private Vector2 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        float distanceTravelled = Vector2.Distance(startPosition, transform.position);
        if (distanceTravelled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerTest player = collision.GetComponent<PlayerTest>();
            if (player != null)
            {
                player.TakeDamage(transform.position, damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            Destroy(gameObject);
        }
    }
}
