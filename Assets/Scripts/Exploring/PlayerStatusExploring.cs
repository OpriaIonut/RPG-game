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

    private DataRetainer dataRetainer;

    private void Start()
    {
        dataRetainer = DataRetainer.instance;
        
        //The health will not change throughout the scene ( for not ) so we update the UI elements only in Start
        currentHealth = dataRetainer.GetPlayerHealth(playerIndex);
        healthBar.fillAmount = (float)currentHealth / baseStatus.health;
        healthText.text = "" + currentHealth + " / " + baseStatus.health;
    }

    public void ChangeHealth(int value)
    {
        currentHealth = value;
        healthBar.fillAmount = (float)currentHealth / baseStatus.health;
        healthText.text = "" + currentHealth + " / " + baseStatus.health;
    }
}
