using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRetainer : MonoBehaviour
{
    public PlayerStatusExploring[] playerStatus;    //Reference to all player's status in order to instantiate it in Awake
    public int[] playersHealth = new int[4];        //Retains the current health for all players
    public Transform[] playersTransform;            //Used to initialize playersPosition. It will be null after changing scenes so we need to use another vector.

    public List<int> defeatedEnemiesIndex;          //It retains all the indexes of the enemies that we defeated and that haven't been spawned yet

    private Vector3[] playersPosition;              //Remembers the player's position when passing through scenes

    #region Singleton and Initialization

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

            //Initialize variables in awake; it would be too late in start because of other scripts
            playersPosition = new Vector3[playersTransform.Length];
            for (int index = 0; index < playersHealth.Length; index++)
            {
                playersHealth[index] = playerStatus[index].baseStatus.health;
                playersPosition[index] = playersTransform[index].position;
            }
        }
    }

    #endregion

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
        //Debug.Log("Get: " + index);
        if (index < 0 || index >= 4)
            return -1;

        return playersHealth[index];
    }
    public void SetPlayerHealth(int index, int value)
    {
        //Debug.Log("Set: " + index + " " + value);
        if (value <= 0)
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
