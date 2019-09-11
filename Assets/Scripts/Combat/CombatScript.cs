using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TurnBaseScript))]
public class CombatScript : MonoBehaviour
{
    #region Singleton

    public static CombatScript instance;

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

    //To do: make boss fights unavoidable
    public int runAwayProbability = 30;             //The probability to run away from a battle
    public float criticalFactorCorrection = 5;      //When we have max dexterity(152) we have a 30% chance to give a critical hit
    public GameObject combatButtons;                //Reference to player buttons so we can disable and enable them on player turn
    public float inputWaitTime = 0.1f;              //How often to check for input
    public ItemMenuCombat itemMenu;

    private Status[] monsterParty;                  //Retains the status for the targets

    private TurnBaseScript turnManager;
    private bool pickTarget = false;                //Did the player choose an option that makes us need to pick a target?
    private int currentTargetIndex = 0;             //Used for picking target    
    private float lastInputTime = 0;

    private void Start()
    {
        //In the begining turn off player buttons
        combatButtons.SetActive(false);
        turnManager = TurnBaseScript.instance;

        //Find all enemies; you can't get only the status script from them so there was a need for a workaround
        GameObject[] monsterAux = GameObject.FindGameObjectsWithTag("Enemy");
        monsterParty = new Status[monsterAux.Length];
        for (int index = 0; index < monsterAux.Length; index++)
        {
            monsterParty[index] = monsterAux[index].GetComponent<Status>();
        }
    }

    //Cutom update function to check for input less often
    private void Update()
    {
        //If we picked an option that requires to pick a target
        if (pickTarget == true && Time.time - lastInputTime > inputWaitTime)
        {
            //Get the input on the horizontal axis (that's how the enemies are oriented atm)
            float movement = Input.GetAxis("Horizontal");
            if (movement != 0)
            {
                monsterParty[currentTargetIndex].turnIndicator.enabled = false; //Disable the turn indicator for the previous monster
                                                                                //Move the target index to where it should be
                if (movement > 0)
                {
                    currentTargetIndex++;
                    if (currentTargetIndex == monsterParty.Length)
                        currentTargetIndex = 0;
                }
                else
                {
                    currentTargetIndex--;
                    if (currentTargetIndex < 0)
                        currentTargetIndex = monsterParty.Length - 1;
                }
                monsterParty[currentTargetIndex].turnIndicator.enabled = true;  //Enable the turn indicator for current target
                lastInputTime = Time.time;
            }

            //If we chose to attack
            if (Input.GetAxis("Fire1") != 0)
            {
                //Turn off indicator and stop picking the target
                pickTarget = false;
                monsterParty[currentTargetIndex].turnIndicator.enabled = false;

                //Calculate critical damage probability
                int damage = turnManager.currentTurnCharacter.strength;
                bool criticalHit = false;

                if (UnityEngine.Random.Range(0, 100) < (int)(turnManager.currentTurnCharacter.dexterity / criticalFactorCorrection))
                {
                    criticalHit = true;
                    damage *= 3;
                }

                //Damage the enemy; it will return true if the enemy has died
                if (monsterParty[currentTargetIndex].TakeDamage(damage, criticalHit) == true)
                {
                    //If it has died take him out from all lists in turnBaseScript
                    turnManager.TakeOutCharacters(monsterParty[currentTargetIndex]);

                    //Take him out from our monsterPartyList too
                    for (int index = currentTargetIndex; index < monsterParty.Length - 1; index++)
                        monsterParty[index] = monsterParty[index + 1];

                    //Resize the array
                    Array.Resize(ref monsterParty, monsterParty.Length - 1);
                }
                //Set the monster index to 0 (because we resize the array, there will always be a monster at position 0, except for when all mosnters are dead)
                currentTargetIndex = 0;

                //If there are no more monsters then stop the game
                if (monsterParty.Length == 0)
                {
                    turnManager.GameWon();
                }
                else
                {
                    //Else end the turn
                    EndPlayerTurn();
                }
            }

            if(Input.GetButtonDown("Cancel"))
            {
                pickTarget = false;
                combatButtons.SetActive(true);
                monsterParty[currentTargetIndex].turnIndicator.enabled = false;
                turnManager.eventSystem.SetSelectedGameObject(turnManager.hiddenButton);
                currentTargetIndex = 0;
            }
        }
    }

    //Called on button press
    public void AttackAction()
    {
        //In order to attack we need to pick a target
        lastInputTime = Time.time;
        pickTarget = true;
        combatButtons.SetActive(false);                 //Also disabe UI
        monsterParty[currentTargetIndex].turnIndicator.enabled = true;   //And we need a visual representation to see who we pick
    }

    //Called on button press
    public void DefendAction()
    {
        combatButtons.SetActive(false);
        turnManager.currentTurnCharacter.guarding = true;
        EndPlayerTurn();    //End the player turn. I made it into a fct because I may add more stuff to it later
    }

    //Called on button press
    public void RunAwayAction()
    {
        //Calculate probability and execute resulting action
        combatButtons.SetActive(false);
        if (UnityEngine.Random.Range(0, 100) < runAwayProbability)
        {
            turnManager.RunAway();
        }
        else
        {
            EndPlayerTurn();
        }
    }

    //Called by the turnBaseScrpt when it is the turn of a player
    public void StartPlayerTurn()
    {
        combatButtons.SetActive(true);
    }

    //Called by ItemMenuCombat script to toggle the item menu
    public void SelectItemOption(bool value)
    {
        combatButtons.SetActive(!value);
        itemMenu.ActivateItemMenu(value);
    }

    //After we did an action simply change the turn
    public void EndPlayerTurn()
    {
        combatButtons.SetActive(false);
        turnManager.ChangeTurn();
    }
}
