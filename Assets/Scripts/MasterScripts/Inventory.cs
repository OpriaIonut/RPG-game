using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pair<T1, T2>
{
    public T1 first;
    public T2 last;

    public Pair(T1 _first, T2 _last) { first = _first; last = _last; }
}

public class Inventory : MonoBehaviour
{
    public List<Pair<ItemScriptable, int>> items = new List<Pair<ItemScriptable, int>>();
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
        for(int index = 0; index < items.Count; index++)
        {
            if(item == items[index].first)
            {
                items[index].last++;
                return;
            }
        }
        items.Add(new Pair<ItemScriptable, int>(item, 1));
        items[items.Count - 1].first.description = GenerateDescription(items[items.Count - 1].first);
    }

    public void AddEquipment(EquipmentScriptable equipment)
    {
        equipments.Add(equipment);
    }

    private string GenerateDescription(ItemScriptable item)
    {
        string description = item.itemName + "\n\n";

        if (item.revival)
            description += "Revival\n";
        description += "Recovery\t" + item.effectValue;
        if (item.effectWithPercentage)
            description += " %";
        else
            description += " HP";

        return description;
    }
}
