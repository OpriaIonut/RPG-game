using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemMenuCombat : MonoBehaviour
{
    public GameObject hiddenButton;
    public GameObject descriptionTab;
    public GameObject itemSlotPrefab;
    public GameObject itemsParent;

    public Status[] playersStatus;
    public GameObject[] errorMessage;
    public Text[] errorMessageText;
    private int playerIndex;

    private TurnBaseScript turnBaseScript;
    private CombatScript combatScript;
    private int selectedItemIndex;
    private Text description;
    private Inventory inventory;
    private EventSystem eventSys;
    private bool selectingItem = false;
    private bool selectingPlayer = false;
    private float lastInputTime = 0f;

    private List<Pair<GameObject, float>> errorMessageTime = new List<Pair<GameObject, float>>();

    private List<GameObject> itemSlots = new List<GameObject>();

    private void Start()
    {
        inventory = Inventory.instance;
        eventSys = EventSystem.current;
        combatScript = CombatScript.instance;
        turnBaseScript = TurnBaseScript.instance;


        description = descriptionTab.GetComponentInChildren<Text>();

        itemsParent.SetActive(false);
        descriptionTab.SetActive(false);
    }

    private void Update()
    {
        for(int index = 0; index < errorMessageTime.Count; index++)
            if(Time.time - errorMessageTime[index].last > 1f)
            {
                errorMessageTime[index].first.SetActive(false);
                errorMessageTime.RemoveAt(index);
            }

        if (selectingItem)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (selectingPlayer)
                {
                    SelectingPlayer(false);
                    eventSys.SetSelectedGameObject(hiddenButton);
                    description.text = "";
                }
                else
                {
                    combatScript.SelectItemOption(false);
                }
            }

            if (selectingPlayer == false)
            {
                for (int index = 0; index < itemSlots.Count; index++)
                {
                    if (itemSlots[index] == eventSys.currentSelectedGameObject)
                    {
                        selectedItemIndex = index;
                        description.text = inventory.items[index].first.description;
                        break;
                    }
                }
            }

            if (selectingPlayer && Time.time - lastInputTime > 0.5f)
            {
                float movement = Input.GetAxis("Horizontal");

                if (movement > 0)
                {
                    if (playerIndex < playersStatus.Length - 1)
                    {
                        playerIndex++;
                        UpdatePlayerUI();
                        lastInputTime = Time.time;
                    }
                }
                else if (movement < 0)
                {
                    if (playerIndex > 0)
                    {
                        playerIndex--;
                        UpdatePlayerUI();
                        lastInputTime = Time.time;
                    }
                }
            }

            if (eventSys.currentSelectedGameObject != hiddenButton)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (selectingPlayer == false)
                    {
                        SelectingPlayer(true);
                    }
                    else
                    {
                        bool success = playersStatus[playerIndex].RestoreHP(inventory.items[selectedItemIndex].first);

                        if (success)
                        {
                            SelectingPlayer(false);

                            inventory.items[selectedItemIndex].last--;
                            if (inventory.items[selectedItemIndex].last == 0)
                            {
                                Destroy(itemSlots[selectedItemIndex]);
                                itemSlots.RemoveAt(selectedItemIndex);
                                inventory.items.RemoveAt(selectedItemIndex);
                            }
                            InitializeUI();

                            combatScript.SelectItemOption(false);
                            combatScript.EndPlayerTurn();
                        }
                        else
                        {
                            errorMessage[playerIndex].SetActive(true);

                            if (playersStatus[playerIndex].health == 0)
                                errorMessageText[playerIndex].text = "Player is dead";
                            else
                                errorMessageText[playerIndex].text = "Player has full HP";

                            errorMessageTime.Add(new Pair<GameObject, float>(errorMessage[playerIndex], Time.time));
                        }
                    }
                }
            }
        }
    }

    public void SelectingPlayer(bool value)
    {
        selectingPlayer = value;

        if (value == false)
        {
            playersStatus[playerIndex].turnIndicator.enabled = false;
            turnBaseScript.GetCurrentCharacter().turnIndicator.enabled = true;
        }
        else
        {
            eventSys.SetSelectedGameObject(null);
            playerIndex = 0;
            UpdatePlayerUI();
        }
    }

    private void UpdatePlayerUI()
    {
        for (int index = 0; index < playersStatus.Length; index++)
        {
            playersStatus[index].turnIndicator.enabled = false;
        }
        playersStatus[playerIndex].turnIndicator.enabled = true;
    }

    public void ActivateItemMenu(bool value)
    {
        selectingItem = value;
        itemsParent.SetActive(value);
        descriptionTab.SetActive(value);

        if (value == true)
        {
            InitializeUI();
            eventSys.SetSelectedGameObject(hiddenButton);
        }
        else
        {
            eventSys.SetSelectedGameObject(turnBaseScript.hiddenButton);
        }
    }

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
            itemSlots[index].transform.GetChild(1).GetComponent<Text>().text = "" + inventory.items[index].last;
        }
    }
}
