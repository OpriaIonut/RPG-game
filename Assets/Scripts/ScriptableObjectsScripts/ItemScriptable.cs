using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemScriptable : ScriptableObject
{
    public string itemName;
    public bool revival;
    public bool effectWithPercentage;
    public int effectValue;
    public string description;
    public int itemIndex;       //Useful for sorting inventory
}
