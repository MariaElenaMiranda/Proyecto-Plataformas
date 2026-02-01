using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsController : MonoBehaviour
{
    [Header("Life")]
    public Image lifeBarFill;
    public float maxLife;
    public TextMeshProUGUI lifeText;
    [Header("Mana")]
    public Image manaBarFill;
    public float maxMana;
    public TextMeshProUGUI manaText;
    [Header("Player")]
    public PlayerTest player;


    void Start()
    {
        player = GameObject.Find("HumanFinn").GetComponent<PlayerTest>();
        lifeBarFill = GameObject.Find("LifeRegImg").GetComponent<Image>();
        lifeText = GameObject.Find("LifeRegText").GetComponent<TextMeshProUGUI>();
        manaBarFill = GameObject.Find("ManaRegImg").GetComponent<Image>();
        manaText = GameObject.Find("ManaRegText").GetComponent<TextMeshProUGUI>();
        maxLife = player.maxLive;
        maxMana = player.maxMana;
    }
    void Update()
    {
        float lifePercent = player.live / maxLife;
        float manaPercent = player.mana / maxMana;

        lifeBarFill.fillAmount = lifePercent;
        lifeText.text = $"{(lifePercent*100):F2}%";

        manaBarFill.fillAmount = manaPercent;
        manaText.text = $"{(manaPercent*100):F2}%";
    }
}
