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

        UpdateUI();
        playerTabs[playerIndex].color = selectedTabColor;

        StartCoroutine(MyUpdate());
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

    private IEnumerator MyUpdate()
    {
        while(true)
        {
            if(statusMenuActive)
            {

            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ToggleStatusMenu(bool value)
    {
        statusMenuActive = value;
    }

    public void UpdateUI()
    {
        levelText.text = "Level " + playerStatus[playerIndex].baseStatus.level;
        //xpBar.fillAmount = 
        //xpText.text = "" + 
        hpText.text = "HP: " + (playerStatus[playerIndex].baseStatus.health + equipmentHolder.playersHealth[playerIndex]);
        mpText.text = "MP: " + playerStatus[playerIndex].baseStatus.mana ;
        vitText.text = "VIT: " + playerStatus[playerIndex].baseStatus.health + " (" + equipmentHolder.playersHealth[playerIndex] + ")";
        defText.text = "DEF: " + playerStatus[playerIndex].baseStatus.defense + " (" + equipmentHolder.playersDefense[playerIndex] + ")";
        agiText.text = "AGI: " + playerStatus[playerIndex].baseStatus.speed +  " (" + equipmentHolder.playersSpeed[playerIndex] + ")";
        strText.text = "STR: " + playerStatus[playerIndex].baseStatus.strength + " (" + equipmentHolder.playersStrength[playerIndex] + ")";
        intText.text = "INT: " + playerStatus[playerIndex].baseStatus.intelligence + " (" + equipmentHolder.playersIntelligence[playerIndex] + ")";
        dexText.text = "DEX: " + playerStatus[playerIndex].baseStatus.dexterity + " (" + equipmentHolder.playersDexterity[playerIndex] + ")";
    }
}
