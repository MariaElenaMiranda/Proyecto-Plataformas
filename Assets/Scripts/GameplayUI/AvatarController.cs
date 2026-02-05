using TMPro;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [Header("Player")]
    public PlayerTest player;
    public TextMeshProUGUI level; // level = all active powerups
    [Header("Life")]
    public int LifeRegPowerup = 0;
    public TextMeshProUGUI lifeQty;
    public TextMeshProUGUI lifeRegen;
    [Header("Mana")]
    public int ManaRegPowerup = 0;
    public TextMeshProUGUI manaQty;
    public TextMeshProUGUI manaRegen;
    [Header("Attack")]
    public int AttackPowerup = 0;
    public TextMeshProUGUI attackQty;
    public TextMeshProUGUI totalAttack;
    [Header("Speed")]
    public int SpeedPowerup = 0;
    public TextMeshProUGUI speedQty;
    public TextMeshProUGUI totalSpeed;
    void Start()
    {
        player = GameObject.Find("HumanFinn").GetComponent<PlayerTest>();
        level = GameObject.Find("Level").GetComponent<TextMeshProUGUI>();

        lifeQty = GameObject.Find("lifeQty").GetComponent<TextMeshProUGUI>();
        lifeRegen = GameObject.Find("lifeStat").GetComponent<TextMeshProUGUI>();

        manaQty = GameObject.Find("manaQty").GetComponent<TextMeshProUGUI>();
        manaRegen = GameObject.Find("manaStat").GetComponent<TextMeshProUGUI>();

        attackQty = GameObject.Find("attackQty").GetComponent<TextMeshProUGUI>();
        totalAttack = GameObject.Find("attackStat").GetComponent<TextMeshProUGUI>();

        speedQty = GameObject.Find("speedQty").GetComponent<TextMeshProUGUI>();
        totalSpeed = GameObject.Find("speedStat").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        int activePowerups = player.LifePowerUpQty + player.ManaPowerUpQty + player.AttackPowerUpQty + player.SpeedPowerUpQty;
        level.text = activePowerups.ToString("0.####");

        lifeQty.text = player.LifePowerUpQty.ToString();
        lifeRegen.text = player.liveRegenValue.ToString("0.####");

        manaQty.text = player.ManaPowerUpQty.ToString();
        manaRegen.text = player.manaRegenValue.ToString("0.####");

        attackQty.text = player.AttackPowerUpQty.ToString();
        totalAttack.text = player.attackDamage.ToString("0.####");

        speedQty.text = player.SpeedPowerUpQty.ToString();
        totalSpeed.text = player.moveSpeed.ToString("0.####");
    }
}
