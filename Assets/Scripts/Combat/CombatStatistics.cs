using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatStatistics : MonoBehaviour
{
    #region Singleton

    public static CombatStatistics instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    #endregion

    [Header("EndBattle menu")]
    public GameObject endBattleMenu;
    public Text endBattleTitle;
    public Text endBattleXPText;
    public Text endBattleGoldText;
    public Image[] xpBars;
    public Text[] xpTexts;
    public Status[] playerStatus;

    #region LevelUpMenu
    [Header("LevelUp menu")]
    public GameObject playerLevelUpMenu;
    public Text playerLevelUpName;
    public Text playerLevelText;

    public Text hpText;
    public Text hpIncreaseText;
    
    public Text mpText;
    public Text mpIncreaseText;

    public Text defText;
    public Text defIncreaseText;

    public Text agiText;
    public Text agiIncreaseText;

    public Text strText;
    public Text strIncreaseText;

    public Text intText;
    public Text intIncreaseText;

    public Text dexText;
    public Text dexIncreaseText;
    #endregion

    private EquipmentHolder equipmentHolder;
    private DataRetainer dataRetainer;
    private Inventory inventory;
    private TurnBaseScript turnBaseScript;
    private int battleXP = 0, battleGold = 0;
    private bool endBattleScreen = false;
    private bool runAway;
    private bool animateXP = false;
    private float xpToAdd = 0;

    private List<Pair<int, int>> leveledUpPlayersIndex = new List<Pair<int, int>>();

    private void Start()
    {
        turnBaseScript = TurnBaseScript.instance;
        inventory = Inventory.instance;
        dataRetainer = DataRetainer.instance;
        equipmentHolder = EquipmentHolder.instance;
        endBattleMenu.SetActive(false);
        playerLevelUpMenu.SetActive(false);
    }

    private void Update()
    {
        if (endBattleScreen == true)
        {
            if (animateXP)
            {
                if (xpToAdd <= 0)
                {
                    animateXP = false;
                }
                else
                {
                    for (int index = 0; index < 4; index++)
                    {
                        playerStatus[index].currentXP += playerStatus[index].xpNeeded / 120f;
                        xpToAdd -= playerStatus[index].xpNeeded / 120f;

                        if (playerStatus[index].currentXP >= playerStatus[index].xpNeeded)
                        {
                            bool cond = false;
                            for (int index2 = 0; index2 < leveledUpPlayersIndex.Count; index2++)
                            {
                                if (leveledUpPlayersIndex[index2].first == index)
                                {
                                    cond = true;
                                    break;
                                }
                            }
                            if (cond == false)
                                leveledUpPlayersIndex.Add(new Pair<int, int>(index, playerStatus[index].playerLevel));

                            playerStatus[index].currentXP = 0;
                            playerStatus[index].playerLevel++;
                            playerStatus[index].xpNeeded = playerStatus[index].baseStatus.xp + (4 * (playerStatus[index].playerLevel ^ 3)) / 7;
                        }

                        xpBars[index].fillAmount = (float)playerStatus[index].currentXP / playerStatus[index].xpNeeded;
                        xpTexts[index].text = "" + (int)playerStatus[index].currentXP + "/" + playerStatus[index].xpNeeded;
                        animateXP = true;
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (leveledUpPlayersIndex.Count == 0)
                    {
                        for (int index = 0; index < 4; index++)
                        {
                            dataRetainer.SetPlayerXP(index, (int)playerStatus[index].currentXP);
                            dataRetainer.SetPlayerLevel(index, playerStatus[index].playerLevel);
                        }

                        if (runAway)
                            turnBaseScript.StartCoroutine(turnBaseScript.ChangeSceneWithDelay("", 2f));
                        else
                            turnBaseScript.StartCoroutine(turnBaseScript.ChangeSceneWithDelay("", 2f));

                        endBattleMenu.SetActive(false);
                        playerLevelUpMenu.SetActive(false);
                    }
                    else
                    {
                        endBattleMenu.SetActive(false);

                        SetLevelUpUI();

                        leveledUpPlayersIndex.RemoveAt(0);
                    }
                }
            }
        }
    }

    public void SetLevelUpUI()
    {
        playerLevelUpMenu.SetActive(true);
        playerLevelUpName.text = playerStatus[leveledUpPlayersIndex[0].first].gameObject.name;

        playerLevelText.text = "" + leveledUpPlayersIndex[0].second + "->" + playerStatus[leveledUpPlayersIndex[0].first].playerLevel;

        int aux;

        hpText.text = "HP: " + playerStatus[leveledUpPlayersIndex[0].first].maxHealth;
        aux = playerStatus[leveledUpPlayersIndex[0].first].baseStatus.health + (int)(((playerStatus[leveledUpPlayersIndex[0].first].playerLevel / 10.0) + playerStatus[leveledUpPlayersIndex[0].first].baseStatus.health / 8.0) * playerStatus[leveledUpPlayersIndex[0].first].playerLevel) + equipmentHolder.playersHealth[leveledUpPlayersIndex[0].first];
        hpIncreaseText.text = "+" + (aux - playerStatus[leveledUpPlayersIndex[0].first].maxHealth);
        playerStatus[leveledUpPlayersIndex[0].first].health += (aux - playerStatus[leveledUpPlayersIndex[0].first].maxHealth);

        mpText.text = "MP: " + playerStatus[leveledUpPlayersIndex[0].first].maxMP;
        aux = 2 * (int)(playerStatus[leveledUpPlayersIndex[0].first].baseStatus.intelligence + (playerStatus[leveledUpPlayersIndex[0].first].baseStatus.intelligence / 4.0) * playerStatus[leveledUpPlayersIndex[0].first].playerLevel);
        mpIncreaseText.text = "+" + (aux - playerStatus[leveledUpPlayersIndex[0].first].maxMP);
        playerStatus[leveledUpPlayersIndex[0].first].currentMp += (aux - playerStatus[leveledUpPlayersIndex[0].first].maxMP);

        defText.text = "DEF: " + playerStatus[leveledUpPlayersIndex[0].first].defense;
        aux = playerStatus[leveledUpPlayersIndex[0].first].baseStatus.defense + (int)((playerStatus[leveledUpPlayersIndex[0].first].baseStatus.defense / 4.0) * playerStatus[leveledUpPlayersIndex[0].first].playerLevel) + equipmentHolder.playersDefense[leveledUpPlayersIndex[0].first];
        defIncreaseText.text = "+" + (aux - playerStatus[leveledUpPlayersIndex[0].first].defense);

        agiText.text = "AGI: " + playerStatus[leveledUpPlayersIndex[0].first].speed;
        aux = playerStatus[leveledUpPlayersIndex[0].first].baseStatus.speed + (int)((playerStatus[leveledUpPlayersIndex[0].first].baseStatus.speed / 4.0) * playerStatus[leveledUpPlayersIndex[0].first].playerLevel) + equipmentHolder.playersSpeed[leveledUpPlayersIndex[0].first];
        agiIncreaseText.text = "+" + (aux - playerStatus[leveledUpPlayersIndex[0].first].speed);

        strText.text = "STR: " + playerStatus[leveledUpPlayersIndex[0].first].strength;
        aux = playerStatus[leveledUpPlayersIndex[0].first].baseStatus.strength + (int)((playerStatus[leveledUpPlayersIndex[0].first].baseStatus.strength / 4.0) * playerStatus[leveledUpPlayersIndex[0].first].playerLevel) + equipmentHolder.playersStrength[leveledUpPlayersIndex[0].first];
        strIncreaseText.text = "+" + (aux - playerStatus[leveledUpPlayersIndex[0].first].strength);

        dexText.text = "DEX: " + playerStatus[leveledUpPlayersIndex[0].first].dexterity;
        aux = playerStatus[leveledUpPlayersIndex[0].first].baseStatus.dexterity + (int)((playerStatus[leveledUpPlayersIndex[0].first].baseStatus.dexterity / 4.0) * playerStatus[leveledUpPlayersIndex[0].first].playerLevel) + equipmentHolder.playersDexterity[leveledUpPlayersIndex[0].first];
        dexIncreaseText.text = "+" + (aux - playerStatus[leveledUpPlayersIndex[0].first].dexterity);

        intText.text = "INT: " + playerStatus[leveledUpPlayersIndex[0].first].intelligence;
        aux = playerStatus[leveledUpPlayersIndex[0].first].baseStatus.intelligence + (int)((playerStatus[leveledUpPlayersIndex[0].first].baseStatus.intelligence / 4.0) * playerStatus[leveledUpPlayersIndex[0].first].playerLevel) + equipmentHolder.playersIntelligence[leveledUpPlayersIndex[0].first];
        intIncreaseText.text = "+" + (aux - playerStatus[leveledUpPlayersIndex[0].first].intelligence);
    }

    public void AddEnemyKillData(Status enemyStatus)
    {
        battleXP += enemyStatus.baseStatus.xp + enemyStatus.baseStatus.xp * enemyStatus.playerLevel * 25 / 100;
        battleGold += enemyStatus.baseStatus.gold + enemyStatus.baseStatus.gold * enemyStatus.playerLevel * 25 / 100;
    }

    public void EndBattle(bool run)
    {
        if (runAway)
        {
            battleGold /= 2;
            endBattleTitle.text = "Run Away";
        }
        else
            endBattleTitle.text = "Battle won";

        endBattleScreen = true;
        runAway = run;
        inventory.AddGold(battleGold);

        endBattleMenu.SetActive(true);
        endBattleXPText.text = "XP: " + battleXP;
        endBattleGoldText.text = "GOLD: " + battleGold;
        xpToAdd = battleXP * 4;     //There are 4 players so we need 4 times the xp gathered so that each player gets the xp gathered

        for (int index = 0; index < 4; index++)
        {
            xpBars[index].fillAmount = (float)playerStatus[index].currentXP / playerStatus[index].xpNeeded;
            xpTexts[index].text = "" + playerStatus[index].currentXP + "/" + playerStatus[index].xpNeeded;
            animateXP = true;
        }
    }
}
