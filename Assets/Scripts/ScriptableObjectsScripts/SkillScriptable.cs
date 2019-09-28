using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SkillType
{
    Attack,
    HpRecovery,
    Revival,
    MpRecovery,
    Instakill,
    DefenseBoost,
    AttackBoost
};

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill")]
[System.Serializable]
public class SkillScriptable : ScriptableObject
{
    public int level = 1;
    public float[] damageMultiplier = new float[5];
    public int[] manaCost = new int[5];
    public int[] numberOfUsesToLevelUp = new int[5];

    public SkillType skillType;
    public bool singleTarget;
    public PlayerType playerType;

    //public int[] skillLearnLevel = new int[5];
    //Weakness
}
