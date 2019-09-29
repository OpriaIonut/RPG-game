using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    /* I will use directly the valuses in the scriptableObject because it won't be changed too often
     * When it is changed (level up) i will change the values from the scriptable object
     * It is the most efficient way that I can think of for when changing screens
     */
    public StatusScriptableObject baseStatus;   //The status of the character
    public Image turnIndicator;                 //Used to indicate the current turn and also the target when picking a target
    public GameObject healthBar;
    public Image healthBarFill;
    public Image mpBar;
    public Text mpText;
    public Text healthText;
    public Text damageText;
    public float defenseFactorCorrection = 10.8f; // At max defense (100) you take 1/10 of the damage
    public float dodgeFactorCorrection = 10.8f;   // At max speed (100) you have a 1/10 chance to dodge
    public int playerIndex;
    public bool dead = false;

    [HideInInspector]
    public bool guarding = false;   //Will become true when the player selects to guard, it halves the damage

    //These variables will hold the total status of the player (base status + equipment status)
    [HideInInspector] public int playerLevel;
    [HideInInspector] public int health;        //Current health
    [HideInInspector] public int currentMp;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int maxMP;
    [HideInInspector] public int defense;
    [HideInInspector] public int speed;
    [HideInInspector] public int strength;
    [HideInInspector] public int intelligence;
    [HideInInspector] public int dexterity;

    [HideInInspector] public int xpNeeded;
    [HideInInspector] public float currentXP;

    private EquipmentHolder equipmentHolder;
    private EnemyCombatAI enemyCombatAI;
    private MeshRenderer meshRender;        //When the player dies we deactivate it's meshRenderer
    private DataRetainer dataRetainer;
    private CombatStatistics combatStatistics;

    private void Start()
    {
        healthBar.SetActive(true);

        //Initialize variables
        meshRender = GetComponent<MeshRenderer>();
        combatStatistics = CombatStatistics.instance;
        equipmentHolder = EquipmentHolder.instance;
        dataRetainer = DataRetainer.instance;

        if (gameObject.tag == "Player")
        {
            playerLevel = dataRetainer.GetPlayerLevel(playerIndex);
        }
        
        maxHealth = baseStatus.health + (int)(((playerLevel / 10.0) + baseStatus.health / 8.0) * playerLevel);
        health = maxHealth;
        defense = baseStatus.defense + (int)((baseStatus.defense / 4.0) * playerLevel);
        speed = baseStatus.speed + (int)((baseStatus.speed / 4.0) * playerLevel);
        strength = baseStatus.strength + (int)((baseStatus.strength / 4.0) * playerLevel);
        dexterity = baseStatus.dexterity + (int)((baseStatus.dexterity / 4.0) * playerLevel);
        intelligence = baseStatus.intelligence + (int)((baseStatus.intelligence / 4.0) * playerLevel);
        maxMP = 2 * intelligence;
        currentMp = maxMP;

        //Calculate all status variables
        if (gameObject.tag == "Player")
        {
            //If it is a player we add the base status + the equipment status
            health = dataRetainer.GetPlayerHealth(playerIndex);
            currentMp = dataRetainer.GetPlayerMP(playerIndex);

            maxHealth += equipmentHolder.playersHealth[playerIndex];
            speed += equipmentHolder.playersSpeed[playerIndex];
            defense += equipmentHolder.playersDefense[playerIndex];
            strength += equipmentHolder.playersStrength[playerIndex];
            dexterity += equipmentHolder.playersDexterity[playerIndex];
            intelligence += equipmentHolder.playersIntelligence[playerIndex];

            xpNeeded = baseStatus.xp + (4 * (playerLevel ^ 3)) / 7;
            currentXP = dataRetainer.GetPlayerXP(playerIndex);
        }

        //Set the UI
        damageText.text = "";
        healthBarFill.fillAmount = (float)health / maxHealth;
        healthText.text = "" + health + "/" + maxHealth;
        mpBar.fillAmount = (float)currentMp / maxMP;
        mpText.text = "" + currentMp + "/" + maxMP;

        enemyCombatAI = EnemyCombatAI.instance;
    }

    //Called by multiple scripts; return true if we can restore health and if we did so, and false otherwise
    public bool RestoreHP(ItemScriptable item)
    {
        if (health == maxHealth)
            return false;

        if ((health <= 0 && item.revival == false) || (health != 0 && item.revival == true))
            return false;

        if (meshRender.enabled == false)
        {
            //When we revive the player we add it again to the enemyCombatAI list so that it can target us again
            meshRender.enabled = true;
            healthBar.SetActive(true);
            dead = false;
        }

        //Calculate how much we should restore
        int value = item.effectValue;
        if (item.effectWithPercentage)
        {
            value = (int)(value * maxHealth / 100f);
        }

        health += value;
        if (health > maxHealth)
            health = maxHealth;

        //Update the UI
        healthBarFill.fillAmount = (float)health / maxHealth;
        healthText.text = "" + health + "/" + maxHealth;
        return true;
    }

    public bool RestoreMP(ItemScriptable item)
    {
        if (health == 0)
            return false;

        if (currentMp == maxMP)
            return false;

        if (item.effectValue + currentMp > maxMP)
            currentMp = baseStatus.mana;
        else
            currentMp += item.effectValue;

        mpBar.fillAmount = (float)currentMp / maxMP;
        mpText.text = "" + currentMp + "/" + maxMP;
        return true;
    }

    //Method called from other functions
    //Critical damage is calculated outside this function and it will be passed normally onto ammount
    public bool TakeDamage(int ammount, bool criticalHit)
    {
        //Check if we can dodge the attack
        if (Random.Range(0f, 100f) < (speed / dodgeFactorCorrection))
        {
            //If so change the color of the text to something evident and stop the program
            damageText.color = Color.yellow;
            damageText.text = "Miss";
            StartCoroutine(DisableUI(1f));
            return false;
        }

        //Calculate the intake damage based  on the character defense
        //If the defense status is less than the defenseFactorCorrection it will actually increase the damage taken
        //We don't want that, so if it is less than the defenseFactorCorrection we will apply the whole damage
        int damage = ammount;
        if (defense > defenseFactorCorrection)
            damage = (int)(damage / (defense / defenseFactorCorrection));

        //If we are guarding then halve the damage
        if (guarding)
            damage /= 2;

        //Change the color of the text to suit the type of attack
        if (criticalHit == true)
            damageText.color = Color.red;
        else
            damageText.color = Color.black;

        //Change health and show UI
        health -= damage;
        damageText.text = "" + damage;

        healthText.text = "" + health + "/" + maxHealth;
        healthBarFill.fillAmount = (float)health / maxHealth;

        //Disable the damage text after a second
        StartCoroutine(DisableUI(1f));

        if (health <= 0)
        {
            //Deactivate object if it is dead
            dead = true;
            health = 0;
            healthText.text = "";
            damageText.text = "";
            meshRender.enabled = false;
            healthBar.SetActive(false);

            if(gameObject.tag == "Enemy")
                combatStatistics.AddEnemyKillData(this);

            return true;
        }
        else
            return false;
    }

    private IEnumerator DisableUI(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        damageText.text = "";
    }
}
