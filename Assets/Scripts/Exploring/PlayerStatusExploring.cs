using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusExploring : MonoBehaviour
{
    public StatusScriptableObject baseStatus;
    public Image healthBar;
    public Image mpBar;
    public Text mpText;
    public Text healthText;
    public int playerIndex;             //Used to diferentiate between players, somewhat better than giving each player a different tag

    [HideInInspector] public int playerLevel;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public int currentMP;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int maxMP;
    [HideInInspector] public int defense;
    [HideInInspector] public int speed;
    [HideInInspector] public int strength;
    [HideInInspector] public int intelligence;
    [HideInInspector] public int dexterity;

    [HideInInspector] public int currentXP = 0;
    [HideInInspector] public int xpNeeded;

    [HideInInspector]
    public EquipmentHolder equipmentHolder;
    private DataRetainer dataRetainer;

    private void Start()
    {
        dataRetainer = DataRetainer.instance;
        equipmentHolder = EquipmentHolder.instance;
        
        //Set the UI
        currentHealth = dataRetainer.GetPlayerHealth(playerIndex);
        currentMP = dataRetainer.GetPlayerMP(playerIndex);
        playerLevel = dataRetainer.GetPlayerLevel(playerIndex);

        UpdatePlayerStatus();

        healthBar.fillAmount = (float)currentHealth / maxHealth;
        healthText.text = "" + currentHealth + " / " + maxHealth;
        mpBar.fillAmount = (float)currentMP / maxMP;
        mpText.text = "" + currentMP + " / " + maxMP;
        
        xpNeeded = baseStatus.xp + (4 * (playerLevel ^ 3)) / 7;
        currentXP = dataRetainer.GetPlayerXP(playerIndex);
    }

    //Change the health and set the UI
    public void ChangeHealth(int value)
    {
        if (value > maxHealth)
            currentHealth = maxHealth;
        else if (value < 0)
            currentHealth = 0;
        else
            currentHealth = value;
        
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        healthText.text = "" + currentHealth + " / " + maxHealth;
    }

    public void ChangeMana(int value)
    {
        if (value > maxMP)
            currentMP = maxMP;
        else if (value < 0)
            currentMP = 0;
        else
            currentMP = value;

        mpBar.fillAmount = (float)currentMP / maxMP;
        mpText.text = "" + currentMP + " / " + maxMP;
    }

    public void UpdatePlayerStatus()
    {
        maxHealth = baseStatus.health + (int)(((playerLevel / 10.0) + baseStatus.health / 8.0) * playerLevel) + equipmentHolder.playersHealth[playerIndex];
        maxMP = 2 * (int)(baseStatus.intelligence + (baseStatus.intelligence / 4.0) * playerLevel);

        defense = baseStatus.defense + (int)((baseStatus.defense / 4.0) * playerLevel) + equipmentHolder.playersDefense[playerIndex];
        speed = baseStatus.speed + (int)((baseStatus.speed / 4.0) * playerLevel) + equipmentHolder.playersSpeed[playerIndex];
        strength = baseStatus.strength + (int)((baseStatus.strength / 4.0) * playerLevel) + equipmentHolder.playersStrength[playerIndex];
        dexterity = baseStatus.dexterity + (int)((baseStatus.dexterity / 4.0) * playerLevel) + equipmentHolder.playersDexterity[playerIndex];
        intelligence = baseStatus.intelligence + (int)((baseStatus.intelligence / 4.0) * playerLevel) + equipmentHolder.playersIntelligence[playerIndex];
    }
}
