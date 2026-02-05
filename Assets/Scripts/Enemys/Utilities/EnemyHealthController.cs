using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthController : MonoBehaviour
{
    public Image fillImage;
    private float maxLife;

    public void SetMaxLife(float life)
    {
        maxLife = life;
        fillImage.fillAmount = 1f;
        Debug.Log("Como esta la maxLife: " + maxLife);
    }

    public void UpdateLife(float currentLife)
    {
        fillImage.fillAmount = currentLife / maxLife;
        Debug.Log("Como esta la barra: " + currentLife / maxLife);
    }
}
