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
            playersHealth[index] = playerStatus[index].baseStatus.health + equipmentHolder.playersHealth[playerStatus[index].playerIndex];
            playersMP[index] = playerStatus[index].baseStatus.mana;
            playersPosition[index] = playersTransform[index].position;
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
