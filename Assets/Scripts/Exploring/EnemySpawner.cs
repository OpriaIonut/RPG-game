using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public Transform[] spawnPoints;
    public float spawnDelay;

    private DataRetainer dataRetainer;

    private void Start()
    {
        dataRetainer = DataRetainer.instance;

        for(int index = 0; index < spawnPoints.Length; index++)
        {
            if(dataRetainer.defeatedEnemiesIndex.Contains(index))
            {
                StartCoroutine(SpawnWithDelay(index));
            }
            else
            {
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
        dataRetainer.DeleteEncounter(index);
    }
}
