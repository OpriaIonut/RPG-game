using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public GameObject hiddenButton;     //Hidden button that is first selected when opening the inventory (used to toggle between items)
    public Transform itemSlotsParent;   //The place where we will intantiate the itemSlotPrefabs
    public GameObject itemSlotPrefab;   //The UI prefab for the item slots
    public Text description;            //The text component onto which we will write the description of the current focused item

    //Reference to the player select menu components, will be activated after selecting an item
    public GameObject playerSelectMenu;
    public GameObject playerSelectHiddenButton;

    [HideInInspector]
    public bool selectingPlayer = false;    //True if we picked an item, marks whether or not we should now try to pick a player
    private Inventory inventory;
    private int selectedItemIndex;          //The index for the item as it appears in the itemSlots list
    private bool menuIsActive = false;      //The script will always be active so we stop/activate all functionality based on this bool

    private List<GameObject> itemSlots = new List<GameObject>();    //Reference to the itemSlots component in the scene
    private EventSystem eventSys;

    private void Start()
    {
        inventory = Inventory.instance;
        eventSys = EventSystem.current;
        playerSelectMenu.SetActive(false);  //First disable the playerMenu. The inventory menu will be disabed/activated by the PauseMenu (i know i know, bad way to do things, i'm sorry)
    }

    private void Update()
    {
        if (menuIsActive)
        {
            //If we are not selecting the player then find what item are we currently focused on
            //When we start selecting the player, the last item that we have focused will be the one chosen by the user
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

            //If we are not focusing the hidden button and we are pressing the "Interact" button
            if (eventSys.currentSelectedGameObject != hiddenButton)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    //If we are not selecting the player then start selecting it
                    if (selectingPlayer == false)
                    {
                        SelectingPlayer(true);
                    }
                    else if (eventSys.currentSelectedGameObject != playerSelectHiddenButton)
                    {
                        //Else if we are selecting the player and we are not focusing the hidden button
                        //Try to heal/revive the player
                        bool success = eventSys.currentSelectedGameObject.GetComponent<PlayerSelectMenuButton>().ChangeHealth(inventory.items[selectedItemIndex].first);
                       
                        if (success)
                        {
                            //If we are successful, stop selecting the player, decrease the item count of the used item
                            //To do: In the future make items usable multiple times without deactivating the player select menu
                            SelectingPlayer(false);

                            inventory.items[selectedItemIndex].second--;
                            if (inventory.items[selectedItemIndex].second == 0)
                            {
                                //If the count is 0 destroy the UI element in the scene and remove the item from all lists
                                SelectingPlayer(false);
                                Destroy(itemSlots[selectedItemIndex]);
                                itemSlots.RemoveAt(selectedItemIndex);
                                inventory.items.RemoveAt(selectedItemIndex);
                            }
                            //Update the UI (the count or even the names MAY have changed... i'm not to sure... sorry)
                            InitializeInventorySlots();

                            //Set the 
                            eventSys.SetSelectedGameObject(hiddenButton);
                            description.text = "";
                        }
                        //The cases where we have failed to heal the player are taken care of in the 'ChangeHealth' fct... i know, bad architecture...
                    }
                }
            }
        }
    }

    //Called by pause menu
    public void ActivateInventoryMenu()
    {
        eventSys.SetSelectedGameObject(hiddenButton);
        menuIsActive = true;
        InitializeInventorySlots();
    }

    //Called by pause menu
    public void DeactivateInventoryMenu()
    {
        menuIsActive = false;
        SelectingPlayer(false);
    }

    //Start/Stop selecting the player
    public void SelectingPlayer(bool value)
    {
        selectingPlayer = value;
        playerSelectMenu.SetActive(value);  //Activate/Deactivate the UI elements

        //Set the focused button to what it's supposed to be
        if (value == false)
            eventSys.SetSelectedGameObject(hiddenButton);
        else
            eventSys.SetSelectedGameObject(playerSelectHiddenButton);
    }

    private void InitializeInventorySlots()
    {
        //Instantiate the item slots if we don't have enough of them
        int aux = itemSlots.Count;
        for (int index = 0; index < inventory.items.Count - aux; index++)
        {
            itemSlots.Add(Instantiate(itemSlotPrefab, itemSlotsParent));
        }
        //Update the text for each of them
        for(int index = 0; index < itemSlots.Count; index++)
        {
            itemSlots[index].transform.GetChild(0).GetComponent<Text>().text = "" + inventory.items[index].first.itemName;
            itemSlots[index].transform.GetChild(1).GetComponent<Text>().text = "" + inventory.items[index].second;
        }
    }
}
