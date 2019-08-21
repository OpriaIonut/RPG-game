using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipment")]
public class EquipmentScriptable : ScriptableObject
{
    public string equipmentName;
    public string equipmentType;
    public int health, defense, speed, strength, intelligence, dexterity;
}
