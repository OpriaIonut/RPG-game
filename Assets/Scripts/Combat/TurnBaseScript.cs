using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CombatScript))]
public class TurnBaseScript : MonoBehaviour
{
    #region Singleton

    public static TurnBaseScript instance;

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

    [Header("UI elements")]
    public Text[] turnText;                 // Reference to the UI elements that display the turns, ATM it is a text object but after time I may make them images
    public GameObject hiddenButton;         // Helps the transition of the player buttons
    public Text endBattleText;
    public Animator canvasAnimator;

    [HideInInspector]
    public Status currentTurn;              // Reference to the status of the character that is about to act (turnLayout[0])

    public List<Status> deadPlayers = new List<Status>();
    [HideInInspector]
    public Status[] characters;            // Reference to all characters in the scene
    private int[] turnWaitTime;             // Retains the wait time for all characters. It is calculated at the beginning because the speed doesn't change.
    private int[] turnLayout = new int[20]; // For each turn it contains the index of the character in the vector "characters". If there is no char in this turn then -1
    
    // References to other important scripts
    [HideInInspector]
    public EventSystem eventSystem;
    private CombatScript combatManager;
    private EnemyCombatAI enemyAI;
    private DataRetainer dataRetainer;

    private void Start()
    {
        // Initialize variables
        dataRetainer = DataRetainer.instance;
        combatManager = CombatScript.instance;
        enemyAI = GetComponent<EnemyCombatAI>();
        eventSystem = EventSystem.current;

        characters = FindObjectsOfType<Status>();
        turnWaitTime = new int[characters.Length];

        // Assign the default value to all locations in turnLayout
        for (int index = 0; index < turnLayout.Length; index++)
            turnLayout[index] = -1;

        DefaultInit();                              //Initialize the default turn layout
        StartCoroutine(ChangeTurnsWaitTime(0.5f));  //Start changing turns with delay
    }
    
    public void ChangeTurn()
    {
        //If on the current turn we have a character that has acted
        if(turnLayout[0] != -1)
        {
            //We calculate where should we put the character after he acts
            int targetIndex = turnWaitTime[turnLayout[0]];

            //If the position where we want to put him is empty then we will simply put him there
            if(turnLayout[targetIndex] == -1)
            {
                turnLayout[targetIndex] = turnLayout[0];
            }
            else
            {
                //Otherwise we check downwards in turnLayout until we find a position that is free and we add him there
                bool cond = false;
                for (int index = targetIndex + 1; index < turnLayout.Length; index++)
                {
                    if (turnLayout[index] == -1)
                    {
                        turnLayout[index] = turnLayout[0];
                        cond = true;
                        break;
                    }
                }
                //If we reached the end and couldn't find a free position then we check upwards
                if (cond == false)
                {
                    for (int index = targetIndex - 1; index > 5; index--)
                    {
                        if (turnLayout[index] == -1)
                        {
                            turnLayout[index] = turnLayout[0];
                            break;
                        }
                    }
                }
            }
        }
        //We shift the vector to the left in order to change turns
        for(int index = 0; index < turnLayout.Length - 1; index++)
        {
            turnLayout[index] = turnLayout[index + 1];
        }

        //The last value always will be -1 because we shift before
        turnLayout[19] = -1;
        SetUI();        //Set the UI elements

        //If the current turn doesn't have a character than we call this function again but with a delay
        if (turnLayout[0] == -1 || characters[turnLayout[0]].dead == true)
        {
            currentTurn = null;
            StartCoroutine("ChangeTurnsWaitTime", 0.5f);
        }
        else
        {
            //If we have a character than we retain it's status, we initialize the guarding variable to false
            currentTurn = characters[turnLayout[0]];
            currentTurn.guarding = false;
            if (currentTurn.gameObject.tag == "Player")
            {
                //If it is the player turn start the player interface
                eventSystem.SetSelectedGameObject(hiddenButton);
                combatManager.StartPlayerTurn();
            }
            else
            {
                //Otherwise start the enemyAI
                enemyAI.Attack();
            }
        }
    }

    private IEnumerator ChangeTurnsWaitTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ChangeTurn();
    }

    private void DefaultInit()
    {
        //Calculate the max speed which will be used in the character delay time formula
        int maxSpeed = 0;
        foreach(Status aux in characters)
        {
            if(aux.baseStatus.speed > maxSpeed)
            {
                maxSpeed = aux.baseStatus.speed;
            }
        }
        
        for(int participantsIndex = 0; participantsIndex < characters.Length; participantsIndex++)
        {
            /* Formula: turns - turns * (speed / maxSpeed) + minDelay
             * We need to clamp it because it can go over the max limit
             * minDelay - the fastest player will have a speed of one (attack every turn) so we need an offset
             */
            turnWaitTime[participantsIndex] = Mathf.Clamp((int)(turnLayout.Length - turnLayout.Length * ((float)characters[participantsIndex].baseStatus.speed / maxSpeed) + 5), 5, turnLayout.Length - 1);

            //After we calculate the wait time for the current player we place it where it's turn should be
            //If the position is occupided
            if (turnLayout[turnWaitTime[participantsIndex]] != -1)
            {
                //We check downwards until we find a free position
                bool cond = false;
                for (int index = turnWaitTime[participantsIndex] + 1; index < turnLayout.Length; index++)
                {
                    if(turnLayout[index] == -1)
                    {
                        turnLayout[index] = participantsIndex;
                        cond = true;
                        break;
                    }
                }
                //If that didn't work then we search upwards
                if(cond == false)
                {
                    for(int index = turnWaitTime[participantsIndex] - 1; index > 5; index--)
                    {
                        if (turnLayout[index] == -1)
                        {
                            turnLayout[index] = participantsIndex;
                            break;
                        }
                    }
                }
            }
            else
            {
                //If the position is free, we simply put him there
                turnLayout[turnWaitTime[participantsIndex]] = participantsIndex;
            }
        }
        //We set the currentTurn status script to what it should be
        if (turnLayout[0] == -1)
            currentTurn = null;
        else
            currentTurn = characters[turnLayout[0]];

        //Finally we initialize the UI
        SetUI();
    }

    private void SetUI()
    {
        for(int index = 0; index < turnLayout.Length; index++)
        {
            //Empty the text for all turns that don't have a player
            if (turnLayout[index] == -1 || characters[turnLayout[index]].dead == true)
                turnText[index].text = "";
            else
                turnText[index].text = characters[turnLayout[index]].name;
        }

        //Disable every turn indicator
        for(int index = 0; index < characters.Length; index++)
        {
            characters[index].turnIndicator.enabled = false;
        }
        //Enable the turn indicator for the character that needs to act, if there is such a character
        if(turnLayout[0] != -1 && characters[turnLayout[0]].dead == false)
            characters[turnLayout[0]].turnIndicator.enabled = true;
    }

    public Status GetCurrentCharacter()
    {
        if (turnLayout[0] == -1)
            return null;
        return characters[turnLayout[0]];
    }

    //A character has died so we want to remove him from all lists
    public void TakeOutCharacters(Status character)
    {
        if (character.tag != "Player")
        {
            bool cond = false;
            for (int index = 0; index < characters.Length - 1; index++)
            {
                //We find the character
                if (character == characters[index])
                {
                    cond = true;
                }
                //We shift all arrays and resize them
                if (cond == true)
                {
                    turnWaitTime[index] = turnWaitTime[index + 1];
                    characters[index] = characters[index + 1];
                }
            }
            Array.Resize(ref characters, characters.Length - 1);
            Array.Resize(ref turnWaitTime, turnWaitTime.Length - 1);

            //We erase him from the turns list
            //Because we shifted all values in the array, the last position will give us out of bounds.
            for (int index2 = 0; index2 < turnLayout.Length; index2++)
            {
                if (turnLayout[index2] == characters.Length)
                {
                    turnLayout[index2] = -1;
                    break;
                }
            }
        }
    }

    #region Scene Changers
    public void GameWon()
    {
        foreach(Status status in characters)
        {
            if (status.gameObject.tag == "Player")
                dataRetainer.SetPlayerHealth(status.playerIndex, status.health);
        }
        StartCoroutine(ChangeSceneWithDelay("Battle won", 2f));
    }

    public void GameOver()
    {
        foreach (Status status in characters)
        {
            if (status.gameObject.tag == "Player")
                dataRetainer.SetPlayerHealth(status.playerIndex, status.health);
        }
        StartCoroutine(ChangeSceneWithDelay("Battle lost", 2f));
    }

    public void RunAway()
    {
        foreach (Status status in characters)
        {
            if (status.gameObject.tag == "Player")
                dataRetainer.SetPlayerHealth(status.playerIndex, status.health);
        }
        StartCoroutine(ChangeSceneWithDelay("Run away", 2f));
    }

    private IEnumerator ChangeSceneWithDelay(string displayText, float waitTime)
    {
        canvasAnimator.SetTrigger("EndBattle");
        endBattleText.text = displayText;
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene("ExploringSceneSample");
    }
    #endregion
}
