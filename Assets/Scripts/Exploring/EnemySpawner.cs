using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public SpawnPoint[] spawnPoints;     //Each spawnpoint will have at most one child
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
                SpawnEnemy(index);
            }
        }
    }

    public void SpawnEnemy(int index)
    {
        int enemyIndex = Random.Range(0, spawnPoints[index].possibleEnemiesPrefab.Length);
        MapEnemyMovement clone = Instantiate(spawnPoints[index].possibleEnemiesPrefab[enemyIndex], spawnPoints[index].transform).GetComponent<MapEnemyMovement>();

        EnemyEncounterHolder enemyEncounterScript = clone.GetComponent<EnemyEncounterHolder>();
        clone.enemyIndex = index;

        enemyEncounterScript.GenerateEnemies(spawnPoints[index]);
        pauseMenu.AddEnemyMovementScript(ref clone);
    }

    //Index is the index for the spawnPoint
    private IEnumerator SpawnWithDelay(int index)
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnEnemy(index);
        dataRetainer.DeleteEncounter(index);    //The enemy has been spawned so we need to delete it from our defeatedEnemiesIndex list
    }
}
