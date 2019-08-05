using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BaseStatus", menuName ="ScriptableObjects/BaseStatus")]
public class StatusScriptableObject : ScriptableObject
{
    public int health, defense, speed, strength, intelligence, dexterity;
}
