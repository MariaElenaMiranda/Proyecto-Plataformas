using UnityEngine;

public class PowerupAttack : MonoBehaviour
{
    public float value = 0.175f;
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
