using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusMenu : MonoBehaviour
{
    #region Singleton

    public static StatusMenu instance;

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

    public Image[] playerTabs;
    public Color selectedTabColor;
    public Text levelText;
    public Image xpBar;
    public Text xpText;
    public Text hpText;
    public Text mpText;
    public Text vitText;
    public Text defText;
    public Text agiText;
    public Text strText;
    public Text intText;
    public Text dexText;

    public PlayerStatusExploring[] playerStatus;

    private Color defaultTabColor;
    private EquipmentHolder equipmentHolder;
    private bool statusMenuActive = false;
    private int playerIndex = 0;
    private float lastInputTime = 0f;

    private void Start()
    {
        defaultTabColor = playerTabs[0].color;
        equipmentHolder = EquipmentHolder.instance;
        
        playerTabs[playerIndex].color = selectedTabColor;
    }

    private void Update()
    {
        if(statusMenuActive)
        {
            if (Time.time - lastInputTime > 0.5f)
            {
                float movement = Input.GetAxis("Horizontal");

                if (movement > 0 && playerIndex < 3)
                {
                    playerTabs[playerIndex].color = defaultTabColor;
                    playerIndex++;
                    playerTabs[playerIndex].color = selectedTabColor;
                    UpdateUI();
                    lastInputTime = Time.time;
                }
                else if (movement < 0 && playerIndex > 0)
                {
                    playerTabs[playerIndex].color = defaultTabColor;
                    playerIndex--;
                    playerTabs[playerIndex].color = selectedTabColor;
                    UpdateUI();
                    lastInputTime = Time.time;
                }
            }
        }
    }

    public void ToggleStatusMenu(bool value)
    {
        statusMenuActive = value;
        if(value == true)
            UpdateUI();
    }

    public void UpdateUI()
    {
        levelText.text = "Level " + playerStatus[playerIndex].playerLevel;
        xpBar.fillAmount = (float)playerStatus[playerIndex].currentXP / playerStatus[playerIndex].xpNeeded;
        xpText.text = "" + playerStatus[playerIndex].currentXP + "/" + playerStatus[playerIndex].xpNeeded;
        hpText.text = "HP: " + playerStatus[playerIndex].maxHealth;
        mpText.text = "MP: " + playerStatus[playerIndex].maxMP;
        vitText.text = "VIT: " + playerStatus[playerIndex].maxHealth + " (" + equipmentHolder.playersHealth[playerIndex] + ")";
        defText.text = "DEF: " + playerStatus[playerIndex].defense + " (" + equipmentHolder.playersDefense[playerIndex] + ")";
        agiText.text = "AGI: " + playerStatus[playerIndex].speed +  " (" + equipmentHolder.playersSpeed[playerIndex] + ")";
        strText.text = "STR: " + playerStatus[playerIndex].strength + " (" + equipmentHolder.playersStrength[playerIndex] + ")";
        intText.text = "INT: " + playerStatus[playerIndex].intelligence + " (" + equipmentHolder.playersIntelligence[playerIndex] + ")";
        dexText.text = "DEX: " + playerStatus[playerIndex].dexterity + " (" + equipmentHolder.playersDexterity[playerIndex] + ")";
    }
}
