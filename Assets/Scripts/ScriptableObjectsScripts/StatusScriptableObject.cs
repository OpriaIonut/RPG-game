using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BaseStatus", menuName ="ScriptableObjects/BaseStatus")]
public class StatusScriptableObject : ScriptableObject
{
    public int health, mana, defense, speed, strength, intelligence, dexterity;
    public int xp;      //For player it will hold the xp needed for next level up, for enemy it will hold xp given
    public SkillScriptable[] skills;

    [Header("Only for enemy status")]
    public int gold;    //Only for enemy, it will hold gold given
    public int minLevel, maxLevel;
}
