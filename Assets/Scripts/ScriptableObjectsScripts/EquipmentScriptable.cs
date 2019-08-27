using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public enum PlayerType
{
    Player1,
    Player2,
    Player3,
    Player4
};

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipment")]
public class EquipmentScriptable : ScriptableObject
{
    public string equipmentName;
    public PlayerType compatiblePlayer;
    public EquipmentType equipmentType;
    public int health, defense, speed, strength, intelligence, dexterity;
}
