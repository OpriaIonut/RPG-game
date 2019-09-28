using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillMenuCombat : MonoBehaviour
{
    public GameObject hiddenButton;
    public GameObject descriptionTab;
    public GameObject skillSlotPrefab;
    public GameObject skillParent;

    public Status[] playersStatus;
    public Status[] enemyStatus;

    private TurnBaseScript turnBaseScript;
    private EventSystem eventSys;
    private CombatScript combatScript;

    private List<GameObject> skillSlots = new List<GameObject>();

    private bool menuIsActive = false;
    private bool selectingTarget = false;
    private SkillScriptable selectedSkill;
    private float lastInputTime = 0;

    private void Start()
    {
        turnBaseScript = TurnBaseScript.instance;
        eventSys = EventSystem.current;
        combatScript = CombatScript.instance;
    }

    private void Update()
    {
        if(menuIsActive)
        {
            if(Input.GetButtonDown("Cancel"))
            {
                combatScript.SelectSkillOption(false);
            }

            if(Time.time - lastInputTime > 1f && Input.GetButtonDown("Interact"))
            {
                if(selectingTarget == false)
                {
                    FindSelectedSkill();
                    selectingTarget = true;
                    skillParent.SetActive(false);
                }
                else
                {

                }
            }

            Targeting();
        }
    }

    private void Targeting()
    {
        if(selectingTarget)
        {
            switch(selectedSkill.skillType)
            {
                case SkillType.Attack:

                    break;
                case SkillType.AttackBoost:

                    break;
                case SkillType.DefenseBoost:

                    break;
                case SkillType.HpRecovery:

                    break;
                case SkillType.Instakill:

                    break;
                case SkillType.MpRecovery:

                    break;
                case SkillType.Revival:

                    break;
            }
        }
    }

    private void FindSelectedSkill()
    {
        for(int index = 0; index < skillSlots.Count; index++)
        {
            if(skillSlots[index] == eventSys.currentSelectedGameObject)
            {
                selectedSkill = turnBaseScript.currentTurnCharacter.baseStatus.skills[index];
                return;
            }
        }
    }

    public void ActivateSkillMenu(bool value)
    {
        skillParent.SetActive(value);
        menuIsActive = value;
        selectingTarget = false;
        lastInputTime = Time.time;

        if(value == true)
        {
            GenerateSkillSlots();
            eventSys.SetSelectedGameObject(hiddenButton);
        }
        else
        {
            DestroySkillSlots();
            eventSys.SetSelectedGameObject(turnBaseScript.hiddenButton);
        }
    }

    private void DestroySkillSlots()
    {
        for(int index = 0; index < skillSlots.Count; index++)
        {
            Destroy(skillSlots[index]);
        }
        skillSlots.Clear();
    }

    private void GenerateSkillSlots()
    {
        for(int index = 0; index < turnBaseScript.currentTurnCharacter.baseStatus.skills.Length; index++)
        {
            skillSlots.Add(Instantiate(skillSlotPrefab, skillParent.transform));
            skillSlots[index].transform.GetChild(0).GetComponent<Text>().text = "" + turnBaseScript.currentTurnCharacter.baseStatus.skills[index].name;
            skillSlots[index].transform.GetChild(1).GetComponent<Text>().text = "" + turnBaseScript.currentTurnCharacter.baseStatus.skills[index].manaCost[turnBaseScript.currentTurnCharacter.baseStatus.skills[index].level];
        }
    }
}
