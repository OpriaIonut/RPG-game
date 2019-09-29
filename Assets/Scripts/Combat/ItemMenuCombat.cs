using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemMenuCombat : MonoBehaviour
{
    public GameObject descriptionTab;   //The gameonject that will hold the description for the current selected item
    public GameObject itemSlotPrefab;   //The UI prefab that will hold an item
    public GameObject itemsParent;      //The parent for the prefab

    public Status[] playersStatus;      //We need the player status for each player so that we can restore his HP
    public GameObject[] errorMessage;   //The error message that will sit on each healthbar, we activate and deactivate it when the user tries something that wouldn't work (ex: heal a player with non-revival item, heal player when he has full hp)
    public Text[] errorMessageText;     //The text component on the errorMessage so taht we can make the player understand what went wrong
    private int selectedPlayerIndex;    //Used to toggle between players on horizontal movement and used to access status from 'playerStatus'

    private GameObject lastSelectedButton;
    private TurnBaseScript turnBaseScript;
    private CombatScript combatScript;
    private Inventory inventory;
    private EventSystem eventSys;

    private Text description;               //Text component on the the 'descriptionTab' used to write item description
    private int selectedItemIndex;          //Used to diferentiate between items and to access them in the inventory
    private bool selectingItem = false;     //The script is always active so we need a bool to know when we should actually use the script (check update)
    private bool selectingPlayer = false;   //We have picked an item and now we want to select a player
    private float lastInputTime = 0f;
    private float interactLastInputTime = 0f;

    //When we prin an error message we need to deactivate it after some time, but by that time we may have printed another error message
    //So we need to retain the time for all of them and deactivate them in turns
    private List<Pair<GameObject, float>> errorMessageTime = new List<Pair<GameObject, float>>();

    //Reference to the 'itemSlotPrefab's that we instantiated used to diferentiate bewteen items and access text components under it to show item info
    private List<GameObject> itemSlots = new List<GameObject>();

    private void Start()
    {
        //Initialize variables
        inventory = Inventory.instance;
        eventSys = EventSystem.current;
        combatScript = CombatScript.instance;
        turnBaseScript = TurnBaseScript.instance;
        description = descriptionTab.GetComponentInChildren<Text>();

        //Deactivate everything concerning this script (was useful when testing but wouldn't be too bad to keep them)
        //That is unless there are eficiency problems, in that case delete them but be careful that it doesn't mess up anything
        itemsParent.SetActive(false);
        descriptionTab.SetActive(false);
    }

    private void Update()
    {
        //If we printed an error message we need to check if we need to deactivate an error message
        //We also remove it from the list so there won't be efficiency issues later on
        for (int index = 0; index < errorMessageTime.Count; index++)
        {
            if (Time.time - errorMessageTime[index].second > 1f)
            {
                errorMessageTime[index].first.SetActive(false);
                errorMessageTime.RemoveAt(index);
            }
        }

        //If we are selecting an item (picked the item option in the player actions menu)
        if (selectingItem)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                //If we pressed exit and we were selecting the player stop the selection
                if (selectingPlayer)
                {
                    SelectingPlayer(false);

                    if (lastSelectedButton == null)
                        eventSys.SetSelectedGameObject(itemSlots[0]);
                    else
                        eventSys.SetSelectedGameObject(lastSelectedButton);
                    description.text = "";
                }
                else
                {
                    //If we were selecting an item stop the selection and activate the player actions menu
                    combatScript.SelectItemOption(false);
                }
            }

            if (selectingPlayer == false)
            {
                //If we are not selecting the player find the item that we are currently hovering (with the keyboard)
                for (int index = 0; index < itemSlots.Count; index++)
                {
                    //We do this by comparing all item slots with the currently selected gameobject in the event system
                    if (itemSlots[index] == eventSys.currentSelectedGameObject)
                    {
                        //Retain it's index and show the description
                        //It is not efficient by any means but gets the job done
                        selectedItemIndex = index;
                        description.text = inventory.items[index].first.description;
                        break;
                    }
                }
            }

            //We want to make the player selection with a delay
            if (selectingPlayer && Time.time - lastInputTime > 0.5f)
            {
                //Check for input
                float movement = Input.GetAxis("Horizontal");

                TogglePlayer(movement);
            }

            if (Time.time - interactLastInputTime > 0.5f &&  Input.GetButtonDown("Interact"))
            {
                //If we are not selecting the player start selecting it
                //We will have retained what item we selected in the 'selectedItemIndex'
                if (selectingPlayer == false)
                {
                    SelectingPlayer(true);
                }
                else
                {
                    bool itemUsed = false;

                    if (inventory.items[selectedItemIndex].first.recoverHP)
                    {
                        //Else, we have picked a player to use the item on, try to restore it's HP
                        bool success = playersStatus[selectedPlayerIndex].RestoreHP(inventory.items[selectedItemIndex].first);

                        //If it was a success
                        if (success)
                        {
                            itemUsed = true;
                        }
                    }
                    if (inventory.items[selectedItemIndex].first.recoverMana)
                    {
                        //Else, we have picked a player to use the item on, try to restore it's MP
                        bool success = playersStatus[selectedPlayerIndex].RestoreMP(inventory.items[selectedItemIndex].first);

                        //If it was a success
                        if (success)
                        {
                            itemUsed = true;
                        }
                    }

                    if (itemUsed)
                    {
                        //Stop selecting the player
                        SelectingPlayer(false);

                        //Decrease the item count
                        inventory.items[selectedItemIndex].second--;
                        if (inventory.items[selectedItemIndex].second == 0)
                        {
                            //If it is 0 we don't have any items so we destroy all instances of it and remove it from all lists
                            Destroy(itemSlots[selectedItemIndex]);
                            itemSlots.RemoveAt(selectedItemIndex);
                            inventory.items.RemoveAt(selectedItemIndex);
                        }
                        //Change the item slots UI (the text on each item)
                        InitializeUI();

                        //Disable the menu and end the turn
                        combatScript.SelectItemOption(false);
                        combatScript.EndPlayerTurn();
                    }
                    else
                    {
                        //If it wass not a success show an error message
                        errorMessage[selectedPlayerIndex].SetActive(true);

                        if (inventory.items[selectedItemIndex].first.recoverHP == true)
                        {
                            //Identify what type of error it is
                            if (inventory.items[selectedItemIndex].first.revival)
                                errorMessageText[selectedPlayerIndex].text = "Cannot revive";
                            else if (playersStatus[selectedPlayerIndex].health == 0)
                                errorMessageText[selectedPlayerIndex].text = "Player is dead";
                            else if (playersStatus[selectedPlayerIndex].health == playersStatus[selectedPlayerIndex].maxHealth)
                                errorMessageText[selectedPlayerIndex].text = "Player has full HP";
                        }
                        if (inventory.items[selectedItemIndex].first.recoverMana == true)
                        {
                            if (playersStatus[selectedPlayerIndex].health == 0)
                                errorMessageText[selectedPlayerIndex].text = "Player is dead";
                            else if (playersStatus[selectedPlayerIndex].currentMp == playersStatus[selectedPlayerIndex].maxMP)
                                errorMessageText[selectedPlayerIndex].text = "Max MP";
                        }

                        //Add it's time to the list so that we can deactivate it
                        errorMessageTime.Add(new Pair<GameObject, float>(errorMessage[selectedPlayerIndex], Time.time));
                    }
                }
            }
        }
    }

    private void TogglePlayer(float movement)
    {
        bool cond = false;
        if (inventory.items[selectedItemIndex].first.revival == true)
            cond = true;

        //Based on the movement
        if (movement > 0)
        {
            for (int index = selectedPlayerIndex + 1; index < playersStatus.Length; index++)
            {
                if (playersStatus[index].dead == cond)
                {
                    selectedPlayerIndex = index;
                    UpdatePlayerUI();           //Update their UI(the thing that indicates which player is selected
                    lastInputTime = Time.time;  //Retain the time so that we can delay the input
                    break;
                }
            }
        }
        else if (movement < 0)
        {
            for (int index = selectedPlayerIndex - 1; index >= 0; index--)
            {
                if (playersStatus[index].dead == cond)
                {
                    selectedPlayerIndex = index;
                    UpdatePlayerUI();           //Update their UI(the thing that indicates which player is selected
                    lastInputTime = Time.time;  //Retain the time so that we can delay the input
                    break;
                }
            }
        }
    }

    public void SelectingPlayer(bool value)
    {
        selectingPlayer = value;

        if (value == false)
        {
            //If we are not selecting the player deactivate the previously selected player UI (we could have selected a player prevously and pressed "Cancel") and enable the turn indicator for the player ho has his turn to act now
            playersStatus[selectedPlayerIndex].turnIndicator.enabled = false;
            turnBaseScript.GetCurrentCharacter().turnIndicator.enabled = true;
        }
        else
        {
            //If we are using a revival item we wat to toggle between dead players so we use cond to do that
            bool cond = false, cond2 = false;
            if (inventory.items[selectedItemIndex].first.revival == true)
                cond = true;

            //Else we are starting to select a player, we set the 
            lastSelectedButton = eventSys.currentSelectedGameObject;
            eventSys.SetSelectedGameObject(null);
            for (int index = 0; index < playersStatus.Length; index++)
            {
                if (playersStatus[index].dead == cond)
                {
                    selectedPlayerIndex = index;
                    cond2 = true;
                    break;
                }
            }

            //If we didn't find any players that meet the search criteria
            if (cond2 == false)
            {
                selectingPlayer = false;
                playersStatus[selectedPlayerIndex].turnIndicator.enabled = false;
                turnBaseScript.GetCurrentCharacter().turnIndicator.enabled = true;

                if (lastSelectedButton == null)
                    eventSys.SetSelectedGameObject(itemSlots[0]);
                else
                    eventSys.SetSelectedGameObject(lastSelectedButton);
                description.text = "There are no players dead!";
            }
            else
            {
                UpdatePlayerUI();
            }
        }
    }

    //Update the turn indicators for all players
    private void UpdatePlayerUI()
    {
        for (int index = 0; index < playersStatus.Length; index++)
        {
            playersStatus[index].turnIndicator.enabled = false;
        }
        playersStatus[selectedPlayerIndex].turnIndicator.enabled = true;
    }

    //Activate the item menu
    public void ActivateItemMenu(bool value)
    {
        //Set the "update guard variable" so that we can use the script
        selectingItem = value;
        interactLastInputTime = Time.time;

        //Activate/Deactivate the UI
        itemsParent.SetActive(value);
        descriptionTab.SetActive(value);

        //If we are activating the menu initialize the item slots together with other items
        if (value == true)
        {
            InitializeUI();
            if (lastSelectedButton == null)
                eventSys.SetSelectedGameObject(itemSlots[0]);
            else
                eventSys.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = null;

            if(combatScript.lastSelectedButton == null)
                eventSys.SetSelectedGameObject(combatScript.firstSelectedButton);
            else
                eventSys.SetSelectedGameObject(combatScript.lastSelectedButton);
        }
    }

    //Create and update the text for the item slots
    private void InitializeUI()
    {
        int aux = itemSlots.Count;
        for (int index = 0; index < inventory.items.Count - aux; index++)
        {
            itemSlots.Add(Instantiate(itemSlotPrefab, itemsParent.transform));
        }
        for (int index = 0; index < itemSlots.Count; index++)
        {
            itemSlots[index].transform.GetChild(0).GetComponent<Text>().text = "" + inventory.items[index].first.itemName;
            itemSlots[index].transform.GetChild(1).GetComponent<Text>().text = "" + inventory.items[index].second;
        }
    }
}
