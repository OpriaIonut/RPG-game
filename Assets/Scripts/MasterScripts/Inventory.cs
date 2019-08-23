using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pair<T1, T2>
{
    public T1 first;
    public T2 second;

    public Pair(T1 _first, T2 _last) { first = _first; second = _last; }
}

public class Inventory : MonoBehaviour
{
    //This is a workaround so that you can have a couple of starting items
    //You add items in here in the inspector and after that at start-up add them to the list
    public ItemScriptable[] startingItems;

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

    private void Start()
    {
        AddStartingItems();
    }

    private void AddStartingItems()
    {
        for(int index = 0; index < startingItems.Length - 1; index++)
        {
            for(int index2 = index+1; index2 < startingItems.Length; index2++)
            {
                if (startingItems[index].itemIndex > startingItems[index2].itemIndex)
                {
                    ItemScriptable aux = startingItems[index];
                    startingItems[index] = startingItems[index2];
                    startingItems[index2] = aux;
                }
            }
        }

        foreach (ItemScriptable item in startingItems)
        {
            if(item.itemIndex >= items.Count)
            {
                items.Add(new Pair<ItemScriptable, int>(item, 1));
                items[items.Count - 1].first.description = GenerateDescription(item);
            }
            else
            {
                items[item.itemIndex].second++;
            }
        }
    }

    public void AddItem(ItemScriptable item)
    {
        for(int index = 0; index < items.Count; index++)
        {
            if(item == items[index].first)
            {
                items[index].second++;
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
