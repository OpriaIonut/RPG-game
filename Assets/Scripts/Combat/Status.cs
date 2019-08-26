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
    public Image healthBar;
    public Text healthText;
    public Text damageText;
    public float defenseFactorCorrection = 10f; // At max defense (100) you take 1/10 of the damage
    public float dodgeFactorCorrection = 10f;   // At max speed (100) you have a 1/10 chance to dodge
    public int playerIndex;
    public bool dead = false;

    [HideInInspector]
    public bool guarding = false;   //Will become true when the player selects to guard, it halves the damage

    [HideInInspector]
    public int health; //Current health
    
    private EnemyCombatAI enemyCombatAI;
    private MeshRenderer meshRender;
    private DataRetainer dataRetainer;

    private void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
        //Initialize variables
        if (gameObject.tag == "Player")
        {
            dataRetainer = DataRetainer.instance;
            health = dataRetainer.GetPlayerHealth(playerIndex);
        }
        else
        {
            health = baseStatus.health;
        }
        healthBar.fillAmount = (float)health / baseStatus.health; 
        healthText.text = "" + health + "/" + baseStatus.health;
        damageText.text = "";
        
        enemyCombatAI = EnemyCombatAI.instance;
    }
    
    public bool RestoreHP(ItemScriptable item)
    {
        if (health == baseStatus.health || (health <= 0 && item.revival == false) || (health != 0 && item.revival == true))
            return false;

        if (meshRender.enabled == false)
        {
            enemyCombatAI.RevivePlayer(this);
            meshRender.enabled = true;
            dead = false;
        }

        int value = item.effectValue;
        if (item.effectWithPercentage)
            value = value * baseStatus.health / 100;
        
        health += value;
        if (health > baseStatus.health)
            health = baseStatus.health;

        healthBar.fillAmount = (float)health / baseStatus.health;
        healthText.text = "" + health + "/" + baseStatus.health;
        return true;
    }

    //Method called from other functions
    //Critical damage is calculated outside this function and it will be passed normally onto ammount
    public bool TakeDamage(int ammount, bool criticalHit)
    {
        //Check if we can dodge the attack
        if (Random.Range(0f, 100f) < (baseStatus.speed / dodgeFactorCorrection))
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
        if (baseStatus.defense > defenseFactorCorrection)
            damage = (int)(damage / (baseStatus.defense / defenseFactorCorrection));

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
        healthText.text = "" + health + "/" + baseStatus.health;
        damageText.text = "" + damage;
        healthBar.fillAmount = (float)health / baseStatus.health;

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
