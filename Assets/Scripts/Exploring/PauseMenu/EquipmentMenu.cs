using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentMenu : MonoBehaviour
{
    public GameObject hiddenButton;         //Hidden button for the equipment menu(used to toggle between equipment slots
    public GameObject[] equipmentButtons;   //Equipment buttons that will appear when activating the menu (if you press on them you will change the equipment of that specific type
    public Text[] equipmentButtonsText;     //Text component on the equipment buttons
    public Text[] totalStatusText;          //This will show the total status given by all pieces of equipment
    public Text[] equipmentValueText;       //This will show the value given by the currently focused equipment piece
    public Text[] equipmentSelectValueText; //This will show the increased/decreased value when we change the equipment
    public Image[] playerTabs;              //Reference to the player tabs on the top of the screen, we change their color depending on the focused player
    public Color playerTabSelectedColor;    //The color for the selected player
    public Color selectEquipRaiseStatus;    //Color in case the equipment we are changing with will increase our status
    public Color selectEquipDecreaseStatus; //Color in case the equipment we are changing with will decrease our status 
    public Color equippedColor;             //Color for the equipment taht is currently equipped (in the equipment selection menu)

    public GameObject equipmentSelectMenu;          //Menu ectivated when we are trying to change the equipment of a specific type
    public GameObject equipmentSelectHiddenButton;
    public GameObject equipmentSlotPrefab;          //The slots prefab used to instantiate the equipment slots when changing equipment
    private List<GameObject> equipmentSlots = new List<GameObject>();   //Reference to the intantiated equipment slots
    public bool selectEquipment = false;            //Bool used to know if we are selecting the equipment in order to use that functionality

    private Color defaultSelectEquipColor;  //The default color of the equipment slot
    private Color playerTabDefaultColor;    //Default color for the player tab
    private EquipmentHolder equipmentHolder;//Reference to the script that holds the equipment for each player
    private EventSystem eventSys;
    private Inventory inventory;

    private EquipmentType selectedEquipmentType;    //The type of the currenly selected equipment. Used in generating equipment slots.
    private PlayerType selectedPlayerType;          //The type of the currenly selected player. Used in generating equipment slots.
    private bool equipmentMenuActive = false;       //Teh menu is always active so we need to know when to use the functionality
    private int selectedEquipmentIndex;             //We get this index by checking each equipmentButton with the currently focused button
    private int selectedPlayerIndex = 0;            //The current player, changes by horizontal movement
    private float lastInputTime = 0;
    private float menuActivationCooldown = 0f;      //When we open the meny it may use the input used to open the menu in code (dunno why it does that) so as a workaround I made the menu functionality have a delay after opening it

    private void Start()
    {
        eventSys = EventSystem.current;
        equipmentHolder = EquipmentHolder.instance;
        inventory = Inventory.instance;

        defaultSelectEquipColor = equipmentSelectValueText[0].color;
        playerTabDefaultColor = playerTabs[selectedPlayerIndex].color;
        playerTabs[selectedPlayerIndex].color = playerTabSelectedColor;

        //Deactivate the menu
        equipmentSelectMenu.SetActive(false);
        StartCoroutine(MyUpdate());             //Made my own update fct so that it goes only 10 times a second, should prove more efficient and can be used in other scripts
        //Update all the UI needed
        UpdateTotalStatusUI();
        UpdateEquipmentButtonsText();
        UpdateEquipmentSelectValueText(true);
    }

    private void Update()
    {
        //If we activated the menu wait a short while so that we don't use wrong input, for more details check menuActivationCooldown implementation
        if (equipmentMenuActive && Time.time - menuActivationCooldown > 0.25f)
        {
            if (Input.GetButtonDown("Interact") && eventSys.currentSelectedGameObject != null && eventSys.currentSelectedGameObject != equipmentSelectHiddenButton)
            {
                //If we press the "Interact" button and we are not selecting the equipment yet, start selecting the equipment and generate the slots
                if (selectEquipment == false)
                {
                    StartSelectingEquipment(true);
                    GenerateEquipmentSlots();
                }
                else
                {
                    //Otherwise, change the equipment with the focused equipment
                    equipmentHolder.ChangeEquipment(eventSys.currentSelectedGameObject.GetComponent<EquipmentSlotHolder>().equipment, selectedPlayerIndex, selectedEquipmentIndex);
                    //Update all UI
                    UpdateTotalStatusUI();
                    UpdateEquipmentButtonsText();

                    //Deactivate the select menu and delete the slots
                    DeleteEqupmentSlots();
                    StartSelectingEquipment(false);
                    UpdateEquipmentSelectValueText(false);
                }
            }

            if (Input.GetButtonDown("Cancel"))
            {
                if (selectEquipment == true)
                {
                    //If we press cancel and were selecting the equipment delete the slots and stop the selection
                    DeleteEqupmentSlots();
                    StartSelectingEquipment(false);
                }
            }
        }
    }

    //Update that goes about 10 times a second
    private IEnumerator MyUpdate()
    {
        while (true)
        {
            if (equipmentMenuActive)
            {
                //If we are not selecting the quipment
                if (!selectEquipment)
                {
                    //Make a delay for the input
                    if (Time.time - lastInputTime > 0.5f)
                    {
                        float input = Input.GetAxis("Horizontal");

                        //If we are moving horizontaly change the selected player
                        if (input != 0)
                        {
                            playerTabs[selectedPlayerIndex].color = playerTabDefaultColor;
                            if (input > 0 && selectedPlayerIndex < 3)
                                selectedPlayerIndex++;
                            else if (input < 0 && selectedPlayerIndex > 0)
                                selectedPlayerIndex--;
                            playerTabs[selectedPlayerIndex].color = playerTabSelectedColor;

                            //Update all UI
                            lastInputTime = Time.time;
                            UpdateTotalStatusUI();
                            UpdateEquipmentButtonsText();
                        }
                    }

                    //Get the currently selected equipment and update the UI
                    GetSelectedIndex();
                    UpdateEquipmentValue();
                }
                else
                {
                    //If we are selecting the equipment update the UI that will show what changes the equipment will bring to the status
                    UpdateEquipmentSelectValueText(true);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    //Generate the equiment slots. Called when starting to select equipment
    private void GenerateEquipmentSlots()
    {
        GameObject auxObj;
        Text aux;
        for(int index = 0; index < inventory.equipments.Count; index++)
        {
            if(inventory.equipments[index].first.equipmentType == selectedEquipmentType && inventory.equipments[index].first.playerType == selectedPlayerType)
            {
                //For every equipment, if it is of the type needed intantiate it
                auxObj = Instantiate(equipmentSlotPrefab, equipmentSelectMenu.transform);
                auxObj.GetComponent<EquipmentSlotHolder>().equipment = inventory.equipments[index].first;
                
                aux = auxObj.GetComponentInChildren<Text>();
                aux.text = inventory.equipments[index].first.equipmentName;

                //If it is equipped change it's color
                if(inventory.equipments[index].second == true)
                {
                    auxObj.GetComponent<Image>().color = equippedColor;
                }

                //Add it to the list so that we can access it and delete it when closing the menu
                equipmentSlots.Add(auxObj);
            }
        }
    }

    //Called when closing the equipment select menu. It deletes all UI slots and clears the list
    public void DeleteEqupmentSlots()
    {
        for (int index = 0; index < equipmentSlots.Count; index++)
        {
            Destroy(equipmentSlots[index]);
        }
        equipmentSlots.Clear();
    }

    public void StartSelectingEquipment(bool value)
    {
        //Deactivate the buttons so that we don't toggle between them
        selectEquipment = value;
        for (int index = 0; index < equipmentButtons.Length; index++)
        {
            equipmentButtons[index].SetActive(!value);
        }
        equipmentSelectMenu.SetActive(value);

        if(value == false)
        {
            //If we are deactivating the select menu set the focused obj as the hidden button
            eventSys.SetSelectedGameObject(hiddenButton);
        }
        else
        {
            //Otherwise find the quipment type (used to instatiate slots) based on the index
            switch(selectedEquipmentIndex)
            {
                case 0:
                    selectedEquipmentType = EquipmentType.Weapon;
                    break;
                case 1:
                    selectedEquipmentType = EquipmentType.Head;
                    break;
                case 2:
                    selectedEquipmentType = EquipmentType.Chest;
                    break;
                case 3:
                    selectedEquipmentType = EquipmentType.Shoulder;
                    break;
                case 4:
                    selectedEquipmentType = EquipmentType.Arms;
                    break;
                case 5:
                    selectedEquipmentType = EquipmentType.Waist;
                    break;
                case 6:
                    selectedEquipmentType = EquipmentType.Legs;
                    break;
            }

            //Also find the playerType (used to instantiate slots)
            switch (selectedPlayerIndex)
            {
                case 0:
                    selectedPlayerType = PlayerType.Player1;
                    break;
                case 1:
                    selectedPlayerType = PlayerType.Player2;
                    break;
                case 2:
                    selectedPlayerType = PlayerType.Player3;
                    break;
                case 3:
                    selectedPlayerType = PlayerType.Player4;
                    break;
            }

            //Set the focus to the equipmentSelectHiddenButton
            eventSys.SetSelectedGameObject(equipmentSelectHiddenButton);
        }
    }

    //Update the UI text for the buttons that are shown when opening the equipment menu
    private void UpdateEquipmentButtonsText()
    {
        EquipmentScriptable aux;
        for (int index = 0; index < equipmentButtonsText.Length; index++)
        {
            aux = equipmentHolder.GetEquipment(selectedPlayerIndex, index);
            if (aux != null)
            {
                equipmentButtonsText[index].text = "" + aux.equipmentType + " - " + aux.equipmentName;
            }
            else
            {
                equipmentButtonsText[index].text = "" + (EquipmentType)index + " - Empty";
            }
        }
    }

    //Update values for the change that the equipment that we are trying to change into will bring
    public void UpdateEquipmentSelectValueText(bool cond)
    {
        if (cond == false || eventSys.currentSelectedGameObject == null || eventSys.currentSelectedGameObject.GetComponent<EquipmentSlotHolder>() == null)
        {
            //Disable the text when closing the select equipment menu and on a few other cases
            equipmentSelectValueText[0].text = "";
            equipmentSelectValueText[1].text = "";
            equipmentSelectValueText[2].text = "";
            equipmentSelectValueText[3].text = "";
            equipmentSelectValueText[4].text = "";
            equipmentSelectValueText[5].text = "";
        }
        else if (eventSys.currentSelectedGameObject.GetComponent<EquipmentSlotHolder>().equipment == null)
        {
            EquipmentScriptable oldEquip = equipmentHolder.GetEquipment(selectedPlayerIndex, selectedEquipmentIndex);

            //If we are selecting the "none" option we will unequip the equipment so the status change will be 0 for everything
            equipmentSelectValueText[0].text = "(0)";
            equipmentSelectValueText[1].text = "(0)";
            equipmentSelectValueText[2].text = "(0)";
            equipmentSelectValueText[3].text = "(0)";
            equipmentSelectValueText[4].text = "(0)";
            equipmentSelectValueText[5].text = "(0)";

            if (oldEquip != null)
            {
                //This will make the text blue in case we had something equipped before
                ChangeEquipSelectColor(ref equipmentSelectValueText[0], 0, oldEquip.health);
                ChangeEquipSelectColor(ref equipmentSelectValueText[1], 0, oldEquip.defense);
                ChangeEquipSelectColor(ref equipmentSelectValueText[2], 0, oldEquip.speed);
                ChangeEquipSelectColor(ref equipmentSelectValueText[3], 0, oldEquip.strength);
                ChangeEquipSelectColor(ref equipmentSelectValueText[4], 0, oldEquip.intelligence);
                ChangeEquipSelectColor(ref equipmentSelectValueText[5], 0, oldEquip.dexterity);
            }
            else
            {
                //This will make the text it's default color in case we didn't have anything equipped before
                ChangeEquipSelectColor(ref equipmentSelectValueText[0], 0, 0);
                ChangeEquipSelectColor(ref equipmentSelectValueText[1], 0, 0);
                ChangeEquipSelectColor(ref equipmentSelectValueText[2], 0, 0);
                ChangeEquipSelectColor(ref equipmentSelectValueText[3], 0, 0);
                ChangeEquipSelectColor(ref equipmentSelectValueText[4], 0, 0);
                ChangeEquipSelectColor(ref equipmentSelectValueText[5], 0, 0);
            }
        }
        else
        {
            EquipmentScriptable newEquip = eventSys.currentSelectedGameObject.GetComponent<EquipmentSlotHolder>().equipment;
            EquipmentScriptable oldEquip = equipmentHolder.GetEquipment(selectedPlayerIndex, selectedEquipmentIndex);

            //Otherwise show the status change
            equipmentSelectValueText[0].text = "(" + newEquip.health + ")";
            equipmentSelectValueText[1].text = "(" + newEquip.defense + ")";
            equipmentSelectValueText[2].text = "(" + newEquip.speed + ")";
            equipmentSelectValueText[3].text = "(" + newEquip.strength + ")";
            equipmentSelectValueText[4].text = "(" + newEquip.intelligence + ")";
            equipmentSelectValueText[5].text = "(" + newEquip.dexterity + ")";

            //Based on the status change, change the color of the text to represent the change for every status
            int aux = 0;
            if (oldEquip != null)
                aux = oldEquip.health;
            ChangeEquipSelectColor(ref equipmentSelectValueText[0], newEquip.health, aux);

            aux = 0;
            if (oldEquip != null)
                aux = oldEquip.defense;
            ChangeEquipSelectColor(ref equipmentSelectValueText[1], newEquip.defense, aux);

            aux = 0;
            if (oldEquip != null)
                aux = oldEquip.speed;
            ChangeEquipSelectColor(ref equipmentSelectValueText[2], newEquip.speed, aux);

            aux = 0;
            if (oldEquip != null)
                aux = oldEquip.strength;
            ChangeEquipSelectColor(ref equipmentSelectValueText[3], newEquip.strength, aux);

            aux = 0;
            if (oldEquip != null)
                aux = oldEquip.intelligence;
            ChangeEquipSelectColor(ref equipmentSelectValueText[4], newEquip.intelligence, aux);

            aux = 0;
            if (oldEquip != null)
                aux = oldEquip.dexterity;
            ChangeEquipSelectColor(ref equipmentSelectValueText[5], newEquip.dexterity, aux);
        }
    }

    //Change the color of a text based on certain values. Used by UpdateEquipmentSelectValueText
    private void ChangeEquipSelectColor(ref Text text, int newEquipValue, int oldEquipValue)
    {
        if (newEquipValue > oldEquipValue)
            text.color = selectEquipRaiseStatus;
        else if (newEquipValue < oldEquipValue)
            text.color = selectEquipDecreaseStatus;
        else
            text.color = defaultSelectEquipColor;
    }

    //Update the UI that will show how much the currently focused equipment has (middle text in the status menu)
    private void UpdateEquipmentValue()
    {
        EquipmentScriptable aux = equipmentHolder.GetEquipment(selectedPlayerIndex, selectedEquipmentIndex);

        if (aux == null)
        {
            equipmentValueText[0].text = "(0)";
            equipmentValueText[1].text = "(0)";
            equipmentValueText[2].text = "(0)";
            equipmentValueText[3].text = "(0)";
            equipmentValueText[4].text = "(0)";
            equipmentValueText[5].text = "(0)";
        }
        else
        {
            equipmentValueText[0].text = "(" + aux.health + ")";
            equipmentValueText[1].text = "(" + aux.defense + ")";
            equipmentValueText[2].text = "(" + aux.speed + ")";
            equipmentValueText[3].text = "(" + aux.strength + ")";
            equipmentValueText[4].text = "(" + aux.intelligence + ")";
            equipmentValueText[5].text = "(" + aux.dexterity + ")";
        }
    }

    //Total UI given by adding all equipments together for the current player
    private void UpdateTotalStatusUI()
    {
        totalStatusText[0].text = "" + equipmentHolder.playersHealth[selectedPlayerIndex];
        totalStatusText[1].text = "" + equipmentHolder.playersDefense[selectedPlayerIndex];
        totalStatusText[2].text = "" + equipmentHolder.playersSpeed[selectedPlayerIndex];
        totalStatusText[3].text = "" + equipmentHolder.playersStrength[selectedPlayerIndex];
        totalStatusText[4].text = "" + equipmentHolder.playersIntelligence[selectedPlayerIndex];
        totalStatusText[5].text = "" + equipmentHolder.playersDexterity[selectedPlayerIndex];
    }

    //Find the index of the currently selected equipment
    private void GetSelectedIndex()
    {
        for(int index = 0; index < equipmentButtons.Length; index++)
            if(equipmentButtons[index] == eventSys.currentSelectedGameObject)
            {
                selectedEquipmentIndex = index;
                return;
            }
        selectedEquipmentIndex = -1;
    }

    //Called by the pause menu
    public void ActivateEquipmentMenu(bool value)
    {
        equipmentMenuActive = value;
        menuActivationCooldown = Time.time;

        if (value == true)
            eventSys.SetSelectedGameObject(hiddenButton);
    }
}
