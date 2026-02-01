using UnityEngine;
public class PowerupHealing : MonoBehaviour
{
    public float value = 0.20f;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerTest player = collision.GetComponent<PlayerTest>();
            if (player != null)
            {
                player.IncreaseLifeRegen(value);
                Destroy(gameObject);
            }
        }
    }
}
