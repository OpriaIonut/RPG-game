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

    private Text errorMessageText;
    private float errorMessageTime = 0f;

    private void Start()
    {
        errorMessageText = errorMessage.GetComponentInChildren<Text>();
        healthbar.fillAmount = (float)playerStatus.currentHealth / playerStatus.baseStatus.health;
        healthText.text = "" + playerStatus.currentHealth + " / " + playerStatus.baseStatus.health;
    }

    private void Update()
    {
        if (errorMessage.activeSelf == true)
            if (Time.time - errorMessageTime > 1f)
                errorMessage.SetActive(false);
    }

    public bool ChangeHealth(ItemScriptable item)
    {
        if ((playerStatus.currentHealth == 0 && item.revival == false) || playerStatus.currentHealth != 0 && item.revival == true)
        {
            errorMessage.SetActive(true);
            errorMessageText.text = "Cannot Revive";
            errorMessageTime = Time.time;
            return false;
        }

        if (playerStatus.currentHealth == playerStatus.baseStatus.health)
        {
            errorMessage.SetActive(true);
            errorMessageTime = Time.time;
            errorMessageText.text = "Player has full HP";
            return false;
        }

        int value = item.effectValue;
        if (item.effectWithPercentage)
            value = value * playerStatus.baseStatus.health / 100;

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
