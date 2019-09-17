using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public StatusScriptableObject[] possibleEnemies;
    public GameObject[] possibleEnemiesPrefab;
    public int enemyCount;
    public bool randomEnemyCount;
}
