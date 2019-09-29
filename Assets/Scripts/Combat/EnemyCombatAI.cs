using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatAI : MonoBehaviour
{
    #region Singleton

    public static EnemyCombatAI instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    #endregion

    public float criticalFactorCorrection = 5;      //When we have max dexterity(152) we have a 30% chance to give a critical hit

    private TurnBaseScript turnManager;
    private Status[] playerParty;                   //Retains the status for the targets

    private void Start()
    {
        turnManager = TurnBaseScript.instance;

        //Find all targets; you can't get only the status script from them so there was a need for a workaround
        GameObject[] playerAux = GameObject.FindGameObjectsWithTag("Player");
        playerParty = new Status[playerAux.Length];
        for (int index = 0; index < playerAux.Length; index++)
        {
            playerParty[index] = playerAux[index].GetComponent<Status>();
        }
    }

    //Called by turn baseScript
    public void Attack()
    {
        StartCoroutine(AttackCoroutine(1f));
    }

    //Attack with delays
    private IEnumerator AttackCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        //First pick the target
        int targetIndex = UnityEngine.Random.Range(0, playerParty.Length);

        if(playerParty[targetIndex].dead == true)
        {
            //If the player is dead we will look on the right side of the list
            bool cond = false;
            for(int index = targetIndex + 1; index < playerParty.Length; index++)
            {
                if(playerParty[index].dead == false)
                {
                    targetIndex = index;
                    cond = true;
                }
            }
            //If we didn't find any player alive in the right side then we look on the left side
            if(cond == false)
            {
                for (int index = targetIndex - 1; index >= 0; index--)
                {
                    if (playerParty[index].dead == false)
                    {
                        targetIndex = index;
                        cond = true;
                    }
                }
            }
        }

        playerParty[targetIndex].turnIndicator.enabled = true;

        //Wait a while
        yield return new WaitForSeconds(seconds);
        playerParty[targetIndex].turnIndicator.enabled = false;

        //Calculate the critical chance probability
        int damage = turnManager.currentTurnCharacter.strength;
        bool criticalHit = false;

        if (UnityEngine.Random.Range(0, 100) < (int)(turnManager.currentTurnCharacter.dexterity / criticalFactorCorrection))
        {
            criticalHit = true;
            damage *= 3;
        }

        //Damage the target, it returns true if it has died
        if (playerParty[targetIndex].TakeDamage(damage, criticalHit) == true)
        {
            playerParty[targetIndex].dead = true;
        }

        bool cond1 = false;
        for (int index = 0; index < playerParty.Length; index++)
            if (playerParty[index].dead == false)
                cond1 = true;

        //If there are no more players end the game
        if (cond1 == false)
        {
            turnManager.GameOver();
        }
        else
        {
            //Else end the turn
            EndTurn();
        }
    }

    public void EndTurn()
    {
        turnManager.ChangeTurn();
    }
}
