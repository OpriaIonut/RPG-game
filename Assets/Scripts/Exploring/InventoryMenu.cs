using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public GameObject hiddenButton;
    public Transform itemSlotsParent;
    public GameObject itemSlotPrefab;
    public Text description;

    public GameObject playerSelectMenu;
    public GameObject playerSelectHiddenButton;

    [HideInInspector]
    public bool selectingPlayer = false;
    private Inventory inventory;
    private int selectedItemIndex;
    private bool menuIsActive = false;

    private List<GameObject> itemSlots = new List<GameObject>();
    private EventSystem eventSys;

    private void Start()
    {
        inventory = Inventory.instance;
        eventSys = EventSystem.current;
        playerSelectMenu.SetActive(false);
    }

    private void Update()
    {
        if (menuIsActive)
        {
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

            if (eventSys.currentSelectedGameObject != hiddenButton)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (selectingPlayer == false)
                    {
                        SelectingPlayer(true);
                    }
                    else if (eventSys.currentSelectedGameObject != playerSelectHiddenButton)
                    {
                        bool success = eventSys.currentSelectedGameObject.GetComponent<PlayerSelectMenuButton>().ChangeHealth(inventory.items[selectedItemIndex].first);
                       
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

                            eventSys.SetSelectedGameObject(hiddenButton);
                            description.text = "";
                        }
                    }
                }
            }
        }
    }

    public void ActivateInventoryMenu()
    {
        eventSys.SetSelectedGameObject(hiddenButton);
        menuIsActive = true;
        InitializeUI();
    }

    public void DeactivateInventoryMenu()
    {
        menuIsActive = false;
        SelectingPlayer(false);
    }

    public void SelectingPlayer(bool value)
    {
        selectingPlayer = value;
        playerSelectMenu.SetActive(value);

        if (value == false)
            eventSys.SetSelectedGameObject(hiddenButton);
        else
            eventSys.SetSelectedGameObject(playerSelectHiddenButton);
    }

    private void InitializeUI()
    {
        int aux = itemSlots.Count;
        for (int index = 0; index < inventory.items.Count - aux; index++)
        {
            itemSlots.Add(Instantiate(itemSlotPrefab, itemSlotsParent));
        }
        for(int index = 0; index < itemSlots.Count; index++)
        {
            itemSlots[index].transform.GetChild(0).GetComponent<Text>().text = "" + inventory.items[index].first.itemName;
            itemSlots[index].transform.GetChild(1).GetComponent<Text>().text = "" + inventory.items[index].last;
        }
    }
}
