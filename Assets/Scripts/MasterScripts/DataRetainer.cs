using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRetainer : MonoBehaviour
{
    public PlayerStatusExploring[] playerStatus;    //Reference to all player's status in order to instantiate it in Awake
    public int[] playersHealth = new int[4];        //Retains the current health for all players
    public int[] playersMP = new int[4];
    public Transform[] playersTransform;            //Used to initialize playersPosition. It will be null after changing scenes so we need to use another vector.
    public StatusScriptableObject[] enemiesEncountered = new StatusScriptableObject[4];
    public int[] enemiesEncounteredLevel = new int[4];

    public List<int> defeatedEnemiesIndex;          //It retains all the indexes of the enemies that we defeated and that haven't been spawned yet

    private Vector3[] playersPosition;              //Remembers the player's position when passing through scenes
    private EquipmentHolder equipmentHolder;

    private int[] playersXP = new int[4];
    private int[] playersLevel = new int[4];

    #region Singleton and Initialization

    public static DataRetainer instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    //Called by equipment holder after it's awake because we need that to be done before the initialization of the variables of this script
    public void Init()
    {
        equipmentHolder = EquipmentHolder.instance;

        playersPosition = new Vector3[playersTransform.Length];
        for (int index = 0; index < playersHealth.Length; index++)
        {
            playersLevel[index] = 1;

            playersHealth[index] = playerStatus[index].baseStatus.health + (int)(((playersLevel[index] / 10.0) + playerStatus[index].baseStatus.health / 8.0) * playersLevel[index]) + equipmentHolder.playersHealth[playerStatus[index].playerIndex];
            playersMP[index] = 2 * (int)(playerStatus[index].baseStatus.intelligence + (playerStatus[index].baseStatus.intelligence / 4.0) * playersLevel[index]);
            playersPosition[index] = playersTransform[index].position;
        }
    }

    //Called when we enter an exploring map by the pause menu script
    public void ExploringInit()
    {
        //When we pass scenes we lose the reference to the player status script so we remake it
        playerStatus = FindObjectsOfType<PlayerStatusExploring>();

        for(int index = 0; index < 3; index++)
        {
            for(int index2 = index+1; index2 < 4; index2++)
            {
                if(playerStatus[index].playerIndex > playerStatus[index2].playerIndex)
                {
                    PlayerStatusExploring aux = playerStatus[index];
                    playerStatus[index] = playerStatus[index2];
                    playerStatus[index2] = aux;
                }
            }
        }
    }

    public void SetPlayerPosition(int index, Vector3 pos)
    {
        playersPosition[index] = pos;
    }
    public Vector3 GetPlayerPosition(int index)
    {
        return playersPosition[index];
    }

    public int GetPlayerHealth(int index)
    {
        if (index < 0 || index >= 4)
            return -1;

        return playersHealth[index];
    }
    public void SetPlayerHealth(int index, int value)
    {
        if (value <= 0)
            playersHealth[index] = 1;
        else
            playersHealth[index] = value;
    }

    public int GetPlayerMP(int index)
    {
        if (index < 0 || index >= 4)
            return -1;

        return playersMP[index];
    }
    public void SetPlayerMP(int index, int value)
    {
        if (value <= 0)
            playersMP[index] = 1;
        else
            playersMP[index] = value;
    }

    public int GetPlayerXP(int index)
    {
        if (index < 0 || index > 3)
            return -1;
        return playersXP[index];
    }
    public void SetPlayerXP(int index, int value)
    {
        if (index < 0 || index > 3)
            return;

        playersXP[index] = value;
    }

    public int GetPlayerLevel(int index)
    {
        if (index < 0 || index > 3)
            return -1;
        return playersLevel[index];
    }
    public void SetPlayerLevel(int index, int value)
    {
        if (index < 0 || index > 3)
            return;

        playersLevel[index] = value;
    }

    public void SaveEncounter(EnemyEncounterHolder enemyHolder)
    {
        for (int index = 0; index < enemyHolder.enemies.Length; index++)
        {
            enemiesEncountered[index] = enemyHolder.enemies[index];
            enemiesEncounteredLevel[index] = enemyHolder.enemyLevel[index];
        }
    }

    public void AddEncounter(int index)     //Called by PlayerMovementScript when we encounter an enemy so that we don't spawn it imediately after we reload the scene
    {
        defeatedEnemiesIndex.Add(index);
    }
    public void DeleteEncounter(int index)  //The enemy has been spawned so we delete it from the list. Called by EnemySpawner
    {
        defeatedEnemiesIndex.Remove(index);
    }
}
