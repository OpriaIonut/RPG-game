using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyToSpawn;     //EnemyPrefab
    public Transform[] spawnPoints;     //Each spawnpoint will have at most one child
    public float spawnDelay;            //Delay to spawn an enemy after we have encountered it

    private PauseMenu pauseMenu;
    private DataRetainer dataRetainer;

    private void Start()
    {
        pauseMenu = PauseMenu.instance;
        dataRetainer = DataRetainer.instance;

        for(int index = 0; index < spawnPoints.Length; index++)
        {
            //Check if we have encountered the enemy, if so spawn it with a delay
            if(dataRetainer.defeatedEnemiesIndex.Contains(index))
            {
                StartCoroutine(SpawnWithDelay(index));
            }
            else
            {
                //Otherwise spawn it instantly
                int enemyIndex = Random.Range(0, enemyToSpawn.Length);
                MapEnemyMovement clone = Instantiate(enemyToSpawn[enemyIndex], spawnPoints[index]).GetComponent<MapEnemyMovement>();
                clone.enemyIndex = index;
                pauseMenu.AddEnemyMovementScript(ref clone);
            }
        }
    }

    //Index is the index for the spawnPoint
    private IEnumerator SpawnWithDelay(int index)
    {
        yield return new WaitForSeconds(spawnDelay);
        int enemyIndex = Random.Range(0, enemyToSpawn.Length);      //Get a random enemy to spawn
        MapEnemyMovement clone = Instantiate(enemyToSpawn[index], spawnPoints[index]).GetComponent<MapEnemyMovement>();
        clone.enemyIndex = index;                                   //Spawn it and set it's index to what it's supposed to be
        pauseMenu.AddEnemyMovementScript(ref clone);                //Add it's script to the pause menu list so that we can pause the game safely
        dataRetainer.DeleteEncounter(index);    //The enemy has been spawned so we need to delete it from our defeatedEnemiesIndex list
    }
}
