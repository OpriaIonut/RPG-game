using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will hold the equipment for a single player
[System.Serializable]
public class EquipmentSlot
{
    public EquipmentScriptable weapon;
    public EquipmentScriptable head;
    public EquipmentScriptable chest;
    public EquipmentScriptable shoulder;
    public EquipmentScriptable arms;
    public EquipmentScriptable waist;
    public EquipmentScriptable legs;

    //Used to get the equipment based on an index
    public EquipmentScriptable GetType(int index)
    {
        switch(index)
        {
            case 0:     return weapon;
            case 1:     return head;
            case 2:     return chest;
            case 3:     return shoulder;
            case 4:     return arms;
            case 5:     return waist;
            case 6:     return legs;
            default:    return null;
        }
    }

    //Used to set an equipment based on an index
    public void SetType(EquipmentScriptable equipment, int index)
    {
        switch (index)
        {
            case 0: weapon = equipment;     break;
            case 1: head = equipment;       break;
            case 2: chest = equipment;      break;
            case 3: shoulder = equipment;   break;
            case 4: arms = equipment;       break;
            case 5: waist = equipment;      break;
            case 6: legs = equipment;       break;
        }
    }
}

//This script will hold the equipment for each player
public class EquipmentHolder : MonoBehaviour
{
    #region Singleton and Initialization

    public static EquipmentHolder instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        CalculateEquipmentStatus();
        DataRetainer.instance.Init();
    }

    #endregion

    //The equipment held by each player
    [Header("Equipment")]
    public EquipmentSlot player1Equipment;  
    public EquipmentSlot player2Equipment;
    public EquipmentSlot player3Equipment;
    public EquipmentSlot player4Equipment;

    //The total status that results from all equipments for all players
    [Header("Total status from equipment")]
    public int[] playersHealth = new int[4];
    public int[] playersDefense = new int[4];
    public int[] playersSpeed = new int[4];
    public int[] playersStrength = new int[4];
    public int[] playersIntelligence = new int[4];
    public int[] playersDexterity = new int[4];

    private StatusMenu statusMenu;
    private Inventory inventory;
    private DataRetainer dataRetainer;

    private void Start()
    {
        statusMenu = StatusMenu.instance;
        dataRetainer = DataRetainer.instance;
    }

    //Called by Inventory screep to find out which equipments from it's list is equipped and which is not
    public void FindEquippedItems()
    {
        inventory = Inventory.instance;

        //For this search through all players
        EquipmentScriptable aux;
        for (int index = 0; index < 4; index++)
        {
            //Through all equipments
            for (int index2 = 0; index2 < (int)EquipmentType.NumOfEquipmentTypes; index2++)
            {
                //Use the indexes to get the equipment and search for it in the inventory list.
                aux = GetEquipment(index, index2);
                inventory.FindAndSetEquipped(aux, true);
            }
        }
    }

    //Get The equipment using the indexes
    public EquipmentScriptable GetEquipment(int playerIndex, int equipmentIndex)
    {
        switch (playerIndex)
        {
            case 0:
                return player1Equipment.GetType(equipmentIndex);
            case 1:
                return player2Equipment.GetType(equipmentIndex);
            case 2:
                return player3Equipment.GetType(equipmentIndex);
            case 3:
                return player4Equipment.GetType(equipmentIndex);
            default:
                return null;
        }
    }

    //Change the equipment based on index, called when changing the equipment in the menu
    public void ChangeEquipment(EquipmentScriptable equipment, int playerIndex, int equipmentIndex)
    {
        //First of all find the player
        switch (playerIndex)
        {
            case 0:
                //Find it's current equipment and unequip it
                inventory.FindAndSetEquipped(player1Equipment.GetType(equipmentIndex), false);
                player1Equipment.SetType(equipment, equipmentIndex);    //Equip the new equipment
                break;
            case 1:
                //Same for all players
                inventory.FindAndSetEquipped(player2Equipment.GetType(equipmentIndex), false);
                player2Equipment.SetType(equipment, equipmentIndex);
                break;
            case 2:
                inventory.FindAndSetEquipped(player3Equipment.GetType(equipmentIndex), false);
                player3Equipment.SetType(equipment, equipmentIndex);
                break;
            case 3:
                inventory.FindAndSetEquipped(player4Equipment.GetType(equipmentIndex), false);
                player4Equipment.SetType(equipment, equipmentIndex);
                break;
        }
        //Recalculate the total equipment status
        CalculateEquipmentStatus();
        ChangePlayerHealth();   //Change the players health 
        inventory.FindAndSetEquipped(equipment, true);  //Set the new equipment as equipped
        statusMenu.UpdateUI();
    }

    //Change the player health for all players, called when changing equipment 
    public void ChangePlayerHealth()
    {
        for (int index = 0; index < 4; index++)
            dataRetainer.playerStatus[index].ChangeHealth(playersHealth[index] + dataRetainer.playerStatus[index].baseStatus.health);
    }

    //Calculate the total equipment status for all players
    public void CalculateEquipmentStatus()
    {
        for(int index = 0; index < 4; index++)
        {
            playersHealth[index] = 0;
            playersDefense[index] = 0;
            playersSpeed[index] = 0;
            playersStrength[index] = 0;
            playersIntelligence[index] = 0;
            playersDexterity[index] = 0;
        }

        for (int index1 = 0; index1 < 4; index1++)
        {
            for(int index2 = 0; index2 < (int)EquipmentType.NumOfEquipmentTypes; index2++)
            {
                if (GetEquipment(index1, index2) != null)
                {
                    playersHealth[index1] += GetEquipment(index1, index2).health;
                    playersDefense[index1] += GetEquipment(index1, index2).defense;
                    playersSpeed[index1] += GetEquipment(index1, index2).speed;
                    playersStrength[index1] += GetEquipment(index1, index2).strength;
                    playersIntelligence[index1] += GetEquipment(index1, index2).intelligence;
                    playersDexterity[index1] += GetEquipment(index1, index2).dexterity;
                }
            }
        }
    }
}
