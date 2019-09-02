using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used by multiple scripts to define the equipment type
public enum EquipmentType
{
    Weapon,
    Head,
    Chest,
    Shoulder,
    Arms,
    Waist,
    Legs,
    NumOfEquipmentTypes
};

//Used by multiple scripts to define player types
public enum PlayerType
{
    Player1,
    Player2,
    Player3,
    Player4
};

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipment")]
[System.Serializable]
public class EquipmentScriptable : ScriptableObject
{
    public string equipmentName;
    public PlayerType playerType;           //The player that can equip the item
    public EquipmentType equipmentType;     //The equipment type/slot that will have this equipment equipped
    public int health, defense, speed, strength, intelligence, dexterity;
}
