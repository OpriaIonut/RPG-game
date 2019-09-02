using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The menu that appears when we try to use an item on the map
public class PlayerSelectMenuButton : MonoBehaviour
{
    public PlayerStatusExploring playerStatus;
    public Image healthbar;
    public Text healthText;
    public GameObject errorMessage;     //Refefrence to the error message that we will show if something doesn't work

    private Text errorMessageText;
    private float errorMessageTime = 0f;    //Used to hide the error message after a short while

    private void Start()
    {
        errorMessageText = errorMessage.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        //If the error message is active, hide it after 1 sec after it's last activation
        if (errorMessage.activeSelf == true)
            if (Time.time - errorMessageTime > 1f)
                errorMessage.SetActive(false);
        
        //Update the UI. The code got pretty complex from adding the equipment functionality so I added this in update
        //If there are problems with efficiency, change this
        healthbar.fillAmount = (float)playerStatus.currentHealth / (playerStatus.baseStatus.health + playerStatus.equipmentHolder.playersHealth[playerStatus.playerIndex]);
        healthText.text = "" + playerStatus.currentHealth + " / " + (playerStatus.baseStatus.health + playerStatus.equipmentHolder.playersHealth[playerStatus.playerIndex]);
    }

    //Called after selecting a player by the InventoryMenu script
    public bool ChangeHealth(ItemScriptable item)
    {
        //Check if the player is dead and we try to heal without reviving or the player is alive and we are trying to revive, if so write an error message and return false
        if ((playerStatus.currentHealth == 0 && item.revival == false) || playerStatus.currentHealth != 0 && item.revival == true)
        {
            errorMessage.SetActive(true);
            errorMessageText.text = "Cannot Revive";
            errorMessageTime = Time.time;
            return false;
        }

        //If we have max health write an error message and return false
        if (playerStatus.currentHealth == (playerStatus.baseStatus.health + playerStatus.equipmentHolder.playersHealth[playerStatus.playerIndex]))
        {
            errorMessage.SetActive(true);
            errorMessageTime = Time.time;
            errorMessageText.text = "Player has full HP";
            return false;
        }

        //Heal the player
        int value = item.effectValue;
        if (item.effectWithPercentage)
            value = value * playerStatus.baseStatus.health / 100;

        //Change it's health (by calling this fct. we also update it's UI) and return true because we were succesful
        playerStatus.ChangeHealth(value + playerStatus.currentHealth);
        return true;
    }

    //If we press 'Close' before the error message is turned off, then turn it off right away
    private void OnDisable()
    {
        errorMessage.SetActive(false);
    }
}
