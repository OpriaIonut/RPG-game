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

    public Status[] playersStatus;  //Used for the turnIndicator when selecting target

    private Status[] enemyStatus;

    private GameObject lastSelectedButton;
    private TurnBaseScript turnBaseScript;
    private EventSystem eventSys;
    private CombatScript combatScript;

    private List<GameObject> skillSlots = new List<GameObject>();

    private int targetIndex = 0;

    private bool menuIsActive = false;
    private bool selectingTarget = false;
    private SkillScriptable selectedSkill;
    private float lastInputTime = 0f;
    private float targetingLastInputTime = 0f;

    private void Start()
    {
        turnBaseScript = TurnBaseScript.instance;
        eventSys = EventSystem.current;
        combatScript = CombatScript.instance;

        //Find all enemies; you can't get only the status script from them so there was a need for a workaround
        GameObject[] monsterAux = GameObject.FindGameObjectsWithTag("Enemy");
        enemyStatus = new Status[monsterAux.Length];
        for (int index = 0; index < monsterAux.Length; index++)
        {
            enemyStatus[index] = monsterAux[index].GetComponent<Status>();
        }
    }

    private void Update()
    {
        if(menuIsActive)
        {
            if(Input.GetButtonDown("Cancel"))
            {
                CloseSelection();
            }

            if(Time.time - lastInputTime > 0.25f && Input.GetButtonDown("Interact"))
            {
                if(selectingTarget == false)
                {
                    SelectTarget();
                }
                else
                {
                    UseSkill();
                }
            }

            if(selectingTarget)
            {
                Targeting();
            }
        }
    }

    private void UseSkill()
    {
        if(selectedSkill.skillType == SkillType.Attack || selectedSkill.skillType == SkillType.Instakill)
        {
            if(selectedSkill.skillType == SkillType.Attack)
            {
                if(selectedSkill.singleTarget == true)
                {
                    int damage = turnBaseScript.currentTurnCharacter.strength;
                    damage += (int)(damage * selectedSkill.damageMultiplier[selectedSkill.level]);
                    bool criticalHit = false;

                    if (UnityEngine.Random.Range(0, 100) < (int)(turnBaseScript.currentTurnCharacter.dexterity / combatScript.criticalFactorCorrection))
                    {
                        criticalHit = true;
                        damage *= 3;
                    }

                    //Damage the enemy; it will return true if the enemy has died
                    if (enemyStatus[targetIndex].TakeDamage(damage, criticalHit) == true)
                    {
                        enemyStatus[targetIndex].dead = true;
                    }
                    selectingTarget = false;
                    CloseSelection();
                    combatScript.CheckEndBattle();
                }
                else
                {
                    for(int index = 0; index < enemyStatus.Length; index++)
                    {
                        int damage = turnBaseScript.currentTurnCharacter.strength;
                        damage += (int)(damage * selectedSkill.damageMultiplier[selectedSkill.level]);
                        bool criticalHit = false;

                        if (UnityEngine.Random.Range(0, 100) < (int)(turnBaseScript.currentTurnCharacter.dexterity / combatScript.criticalFactorCorrection))
                        {
                            criticalHit = true;
                            damage *= 3;
                        }

                        //Damage the enemy; it will return true if the enemy has died
                        if (enemyStatus[index].TakeDamage(damage, criticalHit) == true)
                        {
                            enemyStatus[index].dead = true;
                        }
                    }
                    selectingTarget = false;
                    CloseSelection();
                    combatScript.CheckEndBattle();
                }
            }
            else
            {

            }
        }
        else
        {
            if(selectedSkill.skillType == SkillType.AttackBoost)
            {

            }
            else if(selectedSkill.skillType == SkillType.DefenseBoost)
            {

            }
            else if(selectedSkill.skillType == SkillType.HpRecovery)
            {

            }
            else if(selectedSkill.skillType == SkillType.MpRecovery)
            {

            }
            else if(selectedSkill.skillType == SkillType.Revival)
            {

            }
        }
    }

    private void SelectTarget()
    {
        FindSelectedSkill();
        selectingTarget = true;

        if (selectedSkill.singleTarget == true)
        {
            if (selectedSkill.skillType == SkillType.Attack || selectedSkill.skillType == SkillType.Instakill)
            {
                for (int index = 0; index < enemyStatus.Length; index++)
                {
                    if (enemyStatus[index].dead == false)
                    {
                        targetIndex = index;
                        enemyStatus[index].turnIndicator.enabled = true;
                        break;
                    }
                }
            }
            else
            {
                turnBaseScript.currentTurnCharacter.turnIndicator.enabled = false;
                for (int index = 0; index < playersStatus.Length; index++)
                {
                    if (playersStatus[index].dead == false)
                    {
                        targetIndex = index;
                        playersStatus[index].turnIndicator.enabled = true;
                        break;
                    }
                }
            }
        }

        skillParent.SetActive(false);
    }

    private void CloseSelection()
    {
        if (selectingTarget)
        {
            for (int index = 0; index < enemyStatus.Length; index++)
                enemyStatus[index].turnIndicator.enabled = false;
            for (int index = 0; index < playersStatus.Length; index++)
                playersStatus[index].turnIndicator.enabled = false;

            turnBaseScript.currentTurnCharacter.turnIndicator.enabled = true;
            selectingTarget = false;
            skillParent.SetActive(true);

            eventSys.SetSelectedGameObject(null);
            if (lastSelectedButton == null)
                eventSys.SetSelectedGameObject(skillSlots[0]);
            else
                eventSys.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            combatScript.SelectSkillOption(false);
            skillParent.SetActive(false);

            if (combatScript.lastSelectedButton == null)
                eventSys.SetSelectedGameObject(combatScript.firstSelectedButton);
            else
                eventSys.SetSelectedGameObject(combatScript.lastSelectedButton);
        }
    }

    private void Targeting()
    {
        if (selectedSkill.singleTarget == true)
        {
            float movement = Input.GetAxis("Horizontal");

            if (movement != 0 && Time.time - targetingLastInputTime > 0.5f)
            {
                if (selectedSkill.skillType == SkillType.Attack || selectedSkill.skillType == SkillType.Instakill)
                {
                    if (movement > 0f)
                    {
                        enemyStatus[targetIndex].turnIndicator.enabled = false;
                        for (int index = targetIndex + 1; index < enemyStatus.Length; index++)
                        {
                            if (enemyStatus[index].dead == false)
                            {
                                targetIndex = index;
                                break;
                            }
                        }
                        enemyStatus[targetIndex].turnIndicator.enabled = true;
                    }
                    else
                    {
                        enemyStatus[targetIndex].turnIndicator.enabled = false;
                        for (int index = targetIndex - 1; index >= 0; index--)
                        {
                            if (enemyStatus[index].dead == false)
                            {
                                targetIndex = index;
                                break;
                            }
                        }
                        enemyStatus[targetIndex].turnIndicator.enabled = true;
                    }
                }
                else
                {
                    if (movement > 0f)
                    {
                        playersStatus[targetIndex].turnIndicator.enabled = false;
                        for (int index = targetIndex + 1; index < playersStatus.Length; index++)
                        {
                            if (playersStatus[index].dead == false)
                            {
                                targetIndex = index;
                                break;
                            }
                        }
                        playersStatus[targetIndex].turnIndicator.enabled = true;
                    }
                    else
                    {
                        playersStatus[targetIndex].turnIndicator.enabled = false;
                        for (int index = targetIndex - 1; index >= 0; index--)
                        {
                            if (playersStatus[index].dead == false)
                            {
                                targetIndex = index;
                                break;
                            }
                        }
                        playersStatus[targetIndex].turnIndicator.enabled = true;
                    }
                }
                targetingLastInputTime = Time.time;
            }
        }
        else
        {
            if (selectedSkill.skillType == SkillType.Attack || selectedSkill.skillType == SkillType.Instakill)
            {
                for (int index = 0; index < enemyStatus.Length; index++)
                {
                    if (enemyStatus[index].dead == false)
                        enemyStatus[index].turnIndicator.enabled = true;
                }
            }
            else
            {
                for (int index = 0; index < playersStatus.Length; index++)
                {
                    if (playersStatus[index].dead == false)
                        playersStatus[index].turnIndicator.enabled = true;
                }
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
