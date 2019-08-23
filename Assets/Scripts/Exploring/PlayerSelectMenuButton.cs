using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectMenuButton : MonoBehaviour
{
    public PlayerStatusExploring playerStatus;
    public Image healthbar;
    public Text healthText;
    public GameObject errorMessage;

    private float errorMessageTime = 0f;

    private void Start()
    {
        healthbar.fillAmount = (float)playerStatus.currentHealth / playerStatus.baseStatus.health;
        healthText.text = "" + playerStatus.currentHealth + " / " + playerStatus.baseStatus.health;
    }

    private void Update()
    {
        if(errorMessage.activeSelf == true)
            if (Time.time - errorMessageTime > 1f)
                errorMessage.SetActive(false);
    }

    public bool ChangeHealth(ItemScriptable item)
    {
        if (playerStatus.currentHealth == playerStatus.baseStatus.health)
        {
            errorMessage.SetActive(true);
            errorMessageTime = Time.time;
            return false;
        }

        int value = 0;
        if(playerStatus.currentHealth == 0)
        {
            if(item.revival)
            {
                value = item.effectValue;
                if (item.effectWithPercentage)
                    value = value * playerStatus.baseStatus.health / 100;
            }
        }
        else
        {
            value = item.effectValue;
            if (item.effectWithPercentage)
                value = value * playerStatus.baseStatus.health / 100;
        }

        playerStatus.ChangeHealth(value + playerStatus.currentHealth);
        healthbar.fillAmount = (float)playerStatus.currentHealth / playerStatus.baseStatus.health;
        healthText.text = "" + playerStatus.currentHealth + " / " + playerStatus.baseStatus.health;

        return true;
    }

    private void OnDisable()
    {
        errorMessage.SetActive(false);
    }
}
