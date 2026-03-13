using UnityEngine;

public class PowerupJump : MonoBehaviour
{
    public float value = 0.025f;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerTest player = collision.GetComponent<PlayerTest>();
            if (player != null)
            {
                player.IncreaseJumpForce(value);
                Destroy(gameObject);
            }
        }
    }
}
