using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectMenu : MonoBehaviour
{
    public GameObject playerSelectMenu;
    public Image[] healthbars;
    public Text[] healthText;

    public PlayerStatusExploring[] playerScripts;


    private void Start()
    {
        for(int index = 0; index < healthbars.Length; index++)
        {
            healthbars[index].fillAmount = (float)playerScripts[index].currentHealth / playerScripts[index].baseStatus.health;
            healthText[index].text = "" + playerScripts[index].currentHealth + " / " + playerScripts[index].baseStatus.health;
        }
    }

    public void SelectPlayer(int value)
    {

    }
}
