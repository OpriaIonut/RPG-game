using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRetainer : MonoBehaviour
{
    public PlayerStatusExploring[] playerStatus;    //Reference to all player's status in order to instantiate it in Awake
    public int[] playersHealth = new int[4];        //Retains the current health for all players
    public Transform[] playersTransform;
    
    public List<int> defeatedEnemiesIndex;          //It retains all the indexes of the enemies that we defeated and that haven't been spawned yet

    #region Singleton

    public static DataRetainer instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            //Initialize the player health variables in awake because it would be too late in start because of other scripts
            for (int index = 0; index < playersHealth.Length; index++)
            {
                playersHealth[index] = playerStatus[index].baseStatus.health;
            }
        }
    }

    #endregion

    public int GetPlayerHealth(int index)
    {
        if (index < 0 || index >= 4)
            return -1;

        return playersHealth[index];
    }

    public void SetPlayerPosition(int index, Vector3 pos)
    {
        playersTransform[index].position = pos;
    }
    public Vector3 GetPlayerPosition(int index)
    {
        return playersTransform[index].position;
    }

    public void SetPlayerHealth(int index, int value)
    {
        if (value < 0)
            playersHealth[index] = 1;
        else
            playersHealth[index] = value;
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
