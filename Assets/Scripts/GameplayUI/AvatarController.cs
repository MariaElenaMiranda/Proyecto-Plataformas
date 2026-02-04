using TMPro;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [Header("Player")]
    public PlayerTest player;
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
        lifeQty.text = player.LifePowerUpQty.ToString();
        lifeRegen.text = $"{(player.liveRegenValue.ToString()):F2}";

        manaQty.text = player.ManaPowerUpQty.ToString();
        manaRegen.text = $"{(player.manaRegenValue.ToString()):F2}";

        attackQty.text = player.AttackPowerUpQty.ToString();
        totalAttack.text = $"{(player.attackDamage.ToString()):F2}";

        speedQty.text = player.SpeedPowerUpQty.ToString();
        totalSpeed.text = $"{(player.moveSpeed.ToString()):F2}";
    }
}
