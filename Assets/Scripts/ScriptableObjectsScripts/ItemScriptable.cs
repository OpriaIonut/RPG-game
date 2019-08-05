using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemScriptable : ScriptableObject
{
    public bool recovery;
    public bool revival;
    public bool effectWithPercentage;
    public int effectValue;
}
