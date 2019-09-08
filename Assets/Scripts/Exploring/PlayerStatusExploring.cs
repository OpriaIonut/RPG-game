using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusExploring : MonoBehaviour
{
    public StatusScriptableObject baseStatus;
    public Image healthBar;
    public Text healthText;
    public int playerIndex;             //Used to diferentiate between players, somewhat better than giving each player a different tag
    
    [HideInInspector]
    public int currentHealth;

    [HideInInspector]
    public EquipmentHolder equipmentHolder;
    private DataRetainer dataRetainer;

    private void Start()
    {
        dataRetainer = DataRetainer.instance;
        equipmentHolder = EquipmentHolder.instance;
        
        //Set the UI
        currentHealth = dataRetainer.GetPlayerHealth(playerIndex);
        healthBar.fillAmount = (float)currentHealth / (baseStatus.health + equipmentHolder.playersHealth[playerIndex]);
        healthText.text = "" + currentHealth + " / " + (baseStatus.health + equipmentHolder.playersHealth[playerIndex]);
    }

    //Change the health and set the UI
    public void ChangeHealth(int value)
    {
        if (value > (baseStatus.health + equipmentHolder.playersHealth[playerIndex]))
            currentHealth = (baseStatus.health + equipmentHolder.playersHealth[playerIndex]);
        else if (value < 0)
            currentHealth = 0;
        else
            currentHealth = value;
        healthBar.fillAmount = (float)currentHealth / (baseStatus.health + equipmentHolder.playersHealth[playerIndex]);
        healthText.text = "" + currentHealth + " / " + (baseStatus.health + equipmentHolder.playersHealth[playerIndex]);
    }
}
