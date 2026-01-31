using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpeed : MonoBehaviour
{
    public float value = 0.025f;
 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerTest player = collision.GetComponent<PlayerTest>();
            if (player != null)
            {
                player.IncreaseSpeedMovement(value);
                Destroy(gameObject);
            }
        }
    }
}
