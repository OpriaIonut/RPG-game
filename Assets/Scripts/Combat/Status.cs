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
    [HideInInspector] public int level;
    [HideInInspector] public int health;        //Current health
    [HideInInspector] public int currentMp;
    [HideInInspector] public int maxHealth;     
    [HideInInspector] public int defense;
    [HideInInspector] public int speed;
    [HideInInspector] public int strength;
    [HideInInspector] public int intelligence;
    [HideInInspector] public int dexterity;

    private EquipmentHolder equipmentHolder;
    private EnemyCombatAI enemyCombatAI;
    private MeshRenderer meshRender;        //When the player dies we deactivate it's meshRenderer
    private DataRetainer dataRetainer;

    private void Start()
    {
        healthBar.SetActive(true);
        //Initialize variables
        meshRender = GetComponent<MeshRenderer>();
        //Calculate all status variables
        if (gameObject.tag == "Player")
        {
            //If it is a player we add the base status + the equipment status
            equipmentHolder = EquipmentHolder.instance;
            dataRetainer = DataRetainer.instance;
            health = dataRetainer.GetPlayerHealth(playerIndex);
            currentMp = dataRetainer.GetPlayerMP(playerIndex);

            maxHealth = baseStatus.health + equipmentHolder.playersHealth[playerIndex];
            speed = baseStatus.speed + equipmentHolder.playersSpeed[playerIndex];
            defense = baseStatus.defense + equipmentHolder.playersDefense[playerIndex];
            strength = baseStatus.strength + equipmentHolder.playersStrength[playerIndex];
            dexterity = baseStatus.dexterity + equipmentHolder.playersDexterity[playerIndex];
            intelligence = baseStatus.intelligence + equipmentHolder.playersIntelligence[playerIndex];
        }
        else
        {
            //If it is an enemy we add just it's base status
            health = baseStatus.health;
            currentMp = baseStatus.mana;
            maxHealth = baseStatus.health;
            defense = baseStatus.defense;
            speed = baseStatus.speed;
            strength = baseStatus.strength;
            dexterity = baseStatus.dexterity;
            intelligence = baseStatus.intelligence;
        }
        //Set the UI
        damageText.text = "";
        healthBarFill.fillAmount = (float)health / maxHealth;
        healthText.text = "" + health + "/" + maxHealth;
        mpBar.fillAmount = (float)currentMp / baseStatus.mana;
        mpText.text = "" + currentMp + "/" + baseStatus.mana;

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
            enemyCombatAI.RevivePlayer(this);
            meshRender.enabled = true;
            dead = false;
        }

        //Calculate how much we should restore
        int value = item.effectValue;
        if (item.effectWithPercentage)
        {
            value = value * maxHealth / 100;
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

        if (currentMp == baseStatus.mana)
            return false;

        if (item.effectValue + currentMp > baseStatus.mana)
            currentMp = baseStatus.mana;
        else
            currentMp += item.effectValue;

        mpBar.fillAmount = (float)currentMp / baseStatus.mana;
        mpText.text = "" + currentMp + "/" + baseStatus.mana;
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
