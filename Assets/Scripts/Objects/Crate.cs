using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Crate : MonoBehaviour
{
    public float life = 20f;
    public int chance = 80;
    public GameObject[] powerupsList;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("sword"))
        {
            life -= 10; // no matter the player damage
            if (life <= 0)
            {
                GeneratePowerup();
                Destroy(gameObject);
            }
        }
    }

    void GeneratePowerup()
    {
        int random = Random.Range(0, 100);
        if (chance < random)
        {
            return;
        }
        if (powerupsList.Length > 0)
        {
            int i = Random.Range(0, powerupsList.Length);
            Vector2 position = new Vector2(transform.position.x, transform.position.y + 1);
            Instantiate(powerupsList[i], position, transform.rotation);
        } else
        {
            Debug.Log("There are no powerups added!");
        }
        Destroy(gameObject);
    }

}
