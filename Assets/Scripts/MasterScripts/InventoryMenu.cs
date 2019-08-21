using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public Image itemTab;

    public GameObject firstSelectedObject;
    public Text slotDescription;
    public InventorySlot[] inventorySlots;

    //public GameObject playerSelectMenu;
    //public GameObject playerSelectMenuHiddenButton;

    private InventorySlot auxSlot;
    private Text[] inventorySlotsText;
    private EventSystem eventSys;
    private Inventory inventory;
    private bool itemActiveMenu = true;
    private bool scriptStarted = false;
    //private bool selectingPlayer = false;

    private void Start()
    {
        inventory = Inventory.instance;
        eventSys = EventSystem.current;
        inventorySlotsText = new Text[inventorySlots.Length];
        for (int index = 0; index < inventorySlotsText.Length; index++)
        {
            inventorySlotsText[index] = inventorySlots[index].gameObject.GetComponentInChildren<Text>();
        }

        eventSys.SetSelectedGameObject(firstSelectedObject);
        SetInvetoryUI();

        scriptStarted = true;
    }

    private void Update()
    {
        if (eventSys.currentSelectedGameObject == null)
        {
            eventSys.SetSelectedGameObject(firstSelectedObject);
        }

        //if (selectingPlayer)
        //{
        //    if (Input.GetButtonDown("Interact"))
        //    {

        //    }
        //}
        //else
        //{
        auxSlot = eventSys.currentSelectedGameObject.GetComponent<InventorySlot>();
        if (itemActiveMenu)
        {
            slotDescription.text = auxSlot.itemDescription;

            //if (Input.GetButtonDown("Interact") && auxSlot.item != null)
            //{
            //    playerSelectMenu.SetActive(true);
            //    eventSys.SetSelectedGameObject(playerSelectMenuHiddenButton);
            //    selectingPlayer = true;
            //}
        }
        else
        {
            slotDescription.text = auxSlot.equipmentDescription;
        }
        //}
    }

    public void AddInfo(ItemScriptable item)
    {
        for (int index = 0; index < inventorySlots.Length; index++)
        {
            if (inventorySlots[index].item == null)
            {
                inventorySlots[index].item = item;
                inventorySlots[index].itemDescription = GenerateDescription(item);
                return;
            }
        }
    }
    public void AddInfo(EquipmentScriptable equipment)
    {
        for (int index = 0; index < inventorySlots.Length; index++)
        {
            if (inventorySlots[index].equipment == null)
            {
                inventorySlots[index].equipment = equipment;
                inventorySlots[index].equipmentDescription = GenerateDescription(equipment);
                return;
            }
        }
    }

    private string GenerateDescription(ItemScriptable item)
    {
        string text = item.itemName + "\n\n";
        if (item.recovery)
            text += "Recovery\n";
        if (item.revival)
            text += "Revival\n";
        text += item.effectValue;
        if (item.effectWithPercentage)
            text += " %";
        return text;
    }
    private string GenerateDescription(EquipmentScriptable equipment)
    {
        string text = equipment.equipmentName + "\n" + equipment.equipmentType + "\n\n";
        text += "HP:\t\t\t" + equipment.health + "\nDEF:\t\t" + equipment.defense + "\nSPEED:\t" + equipment.speed + "\nSTR:\t\t" + equipment.strength + "\nINT:\t\t\t" + equipment.intelligence + "\nDEX:\t\t" + equipment.dexterity;
        return text;
    }

    private void SetInvetoryUI()
    {
        for (int index = 0; index < inventorySlotsText.Length; index++)
        {
            if (inventorySlots[index].item != null)
                inventorySlotsText[index].text = inventorySlots[index].item.itemName;
            else
                inventorySlotsText[index].text = "";
        }
    }

    public void ActivateInventory()
    {
        if (scriptStarted == true)
            eventSys.SetSelectedGameObject(firstSelectedObject);
    }
}
