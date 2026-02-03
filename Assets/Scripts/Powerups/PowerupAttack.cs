using UnityEngine;

public class PowerupAttack : MonoBehaviour
{
    public float value = 0.15f;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerTest player = collision.GetComponent<PlayerTest>();
            if (player != null)
            {
                player.IncreaseAttackDamage(value);
                Destroy(gameObject);
            }
        }
    }
}
