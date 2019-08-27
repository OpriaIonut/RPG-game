using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will hold the equipment for each player
public class EquipmentHolder : MonoBehaviour
{
    #region Singleton and Initialization

    public static EquipmentHolder instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    #endregion

    //These variables will hold the equipment for each player, divided on equipment type in the following order
    //Make sure you assign them correctly
    [Tooltip("0.Weapon; 1.Head; 2.Chest; 3.Shoulder; 4.Arms; 5.Waist; 6.Legs")]
    public EquipmentScriptable[] player1Equipment = new EquipmentScriptable[(int)EquipmentType.NumOfEquipmentTypes];

    [Tooltip("0.Weapon; 1.Head; 2.Chest; 3.Shoulder; 4.Arms; 5.Waist; 6.Legs")]
    public EquipmentScriptable[] player2Equipment = new EquipmentScriptable[(int)EquipmentType.NumOfEquipmentTypes];

    [Tooltip("0.Weapon; 1.Head; 2.Chest; 3.Shoulder; 4.Arms; 5.Waist; 6.Legs")]
    public EquipmentScriptable[] player3Equipment = new EquipmentScriptable[(int)EquipmentType.NumOfEquipmentTypes];

    [Tooltip("0.Weapon; 1.Head; 2.Chest; 3.Shoulder; 4.Arms; 5.Waist; 6.Legs")]
    public EquipmentScriptable[] player4Equipment = new EquipmentScriptable[(int)EquipmentType.NumOfEquipmentTypes];
}
