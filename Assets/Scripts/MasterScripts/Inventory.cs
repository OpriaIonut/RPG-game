using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int inventorySize;
    public List<ItemScriptable> items;
    public List<EquipmentScriptable> equipments;

    #region Singleton

    public static Inventory instance;

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
        }
    }

    #endregion

    public void AddItem(ItemScriptable item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
        }
    }

    public void AddItem(EquipmentScriptable equipment)
    {
        if (equipments.Count < inventorySize)
        {
            equipments.Add(equipment);
        }
    }
}
