using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCooldownController : MonoBehaviour
{
    public Image fillImage;
    public float maxCooldown;

    public void SetCooldown(float cooldown)
    {
        maxCooldown = cooldown;
        fillImage.fillAmount = 1f; 
    }

    public void UpdateCooldown(float currentTime)
    {
        if (maxCooldown <= 0) return;
        float fill = Mathf.Clamp01(currentTime / maxCooldown);
        fillImage.fillAmount = fill;
    }
}
