using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillMenuCombat : MonoBehaviour
{
    //public Button firstSelectedButton;
    public GameObject descriptionTab;
    public GameObject skillSlotPrefab;
    public GameObject skillParent;

    public Status[] playersStatus;
    public Status[] enemyStatus;

    private GameObject lastSelectedButton;
    private TurnBaseScript turnBaseScript;
    private EventSystem eventSys;
    private CombatScript combatScript;

    private List<GameObject> skillSlots = new List<GameObject>();

    private int targetIndex = 0;

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
                if(selectingTarget)
                {

                }
                else
                {
                    combatScript.SelectSkillOption(false);
                }
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

            if(selectingTarget)
            {
                Targeting();
            }
        }
    }

    private void Targeting()
    {

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
        }
        else
        {
            lastSelectedButton = null;
            DestroySkillSlots();
            eventSys.SetSelectedGameObject(combatScript.firstSelectedButton.gameObject);
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
        if (lastSelectedButton == null)
            eventSys.SetSelectedGameObject(skillSlots[0]);
        else
            eventSys.SetSelectedGameObject(lastSelectedButton);
    }
}
