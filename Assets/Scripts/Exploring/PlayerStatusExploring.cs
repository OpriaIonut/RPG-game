using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusExploring : MonoBehaviour
{
    public StatusScriptableObject baseStatus;
    public Image healthBar;
    public Text healthText;
    public int playerIndex;

    [HideInInspector]
    public int currentHealth;

    private DataRetainer dataRetainer;

    private void Start()
    {
        dataRetainer = DataRetainer.instance;
        
        currentHealth = dataRetainer.GetPlayerHealth(playerIndex);
        healthBar.fillAmount = (float)currentHealth / baseStatus.health;
        healthText.text = "" + currentHealth + " / " + baseStatus.health;
    }
}
