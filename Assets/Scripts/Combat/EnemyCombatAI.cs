using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatAI : MonoBehaviour
{
    public float criticalFactorCorrection = 4;      //When we have max dexterity(100) we have a 25% chance to give a critical hit

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
        //To do: make it pick targets according to certain criteria
        int targetIndex = UnityEngine.Random.Range(0, playerParty.Length - 1);
        playerParty[targetIndex].turnIndicator.enabled = true;

        //Wait a while
        yield return new WaitForSeconds(seconds);
        playerParty[targetIndex].turnIndicator.enabled = false;

        //Calculate the critical chance probability
        int damage = turnManager.currentTurn.baseStatus.strength;
        bool criticalHit = false;

        if (UnityEngine.Random.Range(0, 100) < (int)(turnManager.currentTurn.baseStatus.dexterity / criticalFactorCorrection))
        {
            criticalHit = true;
            damage *= 3;
        }

        //Damage the target, it returns true if it has died
        if (playerParty[targetIndex].TakeDamage(damage, criticalHit) == true)
        {
            //Take him out from all lists in turn base script
            turnManager.TakeOutCharacters(playerParty[targetIndex]);

            //Take him out from our array and resize it
            for (int index = targetIndex; index < playerParty.Length - 1; index++)
                playerParty[index] = playerParty[index + 1];
            Array.Resize(ref playerParty, playerParty.Length - 1);
        }

        //If there are no more players end the game
        if (playerParty.Length == 0)
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
