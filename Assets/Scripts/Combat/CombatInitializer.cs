using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInitializer : MonoBehaviour
{
    public Status[] enemy1;
    public Status[] enemy2;
    public Status[] enemy3;
    public Status[] enemy4;

    private DataRetainer dataRetainer;

    private void Awake()
    {
        dataRetainer = DataRetainer.instance;

        for(int index = 0; index < enemy1.Length; index++)
        {
            if (enemy1[index].baseStatus == dataRetainer.enemiesEncountered[0])
            {
                enemy1[index].gameObject.SetActive(true);
                enemy1[index].playerLevel = dataRetainer.enemiesEncounteredLevel[0];
                break;
            }
        }
        for (int index = 0; index < enemy2.Length; index++)
        {
            if (enemy2[index].baseStatus == dataRetainer.enemiesEncountered[1])
            {
                enemy2[index].gameObject.SetActive(true);
                enemy2[index].playerLevel = dataRetainer.enemiesEncounteredLevel[1];
                break;
            }
        }
        for (int index = 0; index < enemy3.Length; index++)
        {
            if (enemy3[index].baseStatus == dataRetainer.enemiesEncountered[2])
            {
                enemy3[index].gameObject.SetActive(true);
                enemy3[index].playerLevel = dataRetainer.enemiesEncounteredLevel[2];
                break;
            }
        }
        for (int index = 0; index < enemy4.Length; index++)
        {
            if (enemy4[index].baseStatus == dataRetainer.enemiesEncountered[3])
            {
                enemy4[index].gameObject.SetActive(true);
                enemy4[index].playerLevel = dataRetainer.enemiesEncounteredLevel[3];
                break;
            }
        }
    }
}
