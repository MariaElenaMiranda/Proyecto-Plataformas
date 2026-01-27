using UnityEngine;

public class PowerUpRandomizer : MonoBehaviour
{
    public GameObject[] powerupsList;

    void Start()
    {
        GenerarPowerup();
    }

    void GenerarPowerup()
    {
        if (powerupsList.Length > 0)
        {
            int i = Random.Range(0, powerupsList.Length);
            Instantiate(powerupsList[i], transform.position, transform.rotation);
        } else
        {
            Debug.Log("There are no powerups!");
        }

        Destroy(gameObject);
    }
}