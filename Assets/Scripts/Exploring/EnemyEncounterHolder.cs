using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEncounterHolder : MonoBehaviour
{
    public StatusScriptableObject[] enemies;
    public int[] enemyLevel;

    public void GenerateEnemies(SpawnPoint spawnScript)
    {
        enemies = new StatusScriptableObject[4];
        enemyLevel = new int[4];

        int count = spawnScript.enemyCount;
        if (spawnScript.randomEnemyCount)
            count = Random.Range(1, count + 1);
        
        for (int index = 0; index < count; index++)
        {
            enemies[index] = spawnScript.possibleEnemies[Random.Range(0, spawnScript.possibleEnemies.Length)];
            enemyLevel[index] = Random.Range(enemies[index].minLevel, enemies[index].maxLevel + 1);
        }
    }
}
