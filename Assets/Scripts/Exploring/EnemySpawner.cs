using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;     //EnemyPrefab
    public Transform[] spawnPoints;     //Each spawnpoint will have at most one child
    public float spawnDelay;            //Delay to spawn an enemy after we have encountered it

    private DataRetainer dataRetainer;

    private void Start()
    {
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
                MapEnemyMovement clone = Instantiate(enemyToSpawn, spawnPoints[index]).GetComponent<MapEnemyMovement>();
                clone.enemyIndex = index;
            }
        }
    }

    private IEnumerator SpawnWithDelay(int index)
    {
        yield return new WaitForSeconds(spawnDelay);
        MapEnemyMovement clone = Instantiate(enemyToSpawn, spawnPoints[index]).GetComponent<MapEnemyMovement>();
        clone.enemyIndex = index;
        dataRetainer.DeleteEncounter(index);    //The enemy has been spawned so we need to delete it from our defeatedEnemiesIndex list
    }
}
