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
    //This is a workaround so that you can have a couple of starting items and equipments
    //You add items in here in the inspector and after that at start-up add them to the list
    public ItemScriptable[] startingItems;
    public EquipmentScriptable[] startingEquipment;

    //Holds the items and item counts for each item, when the count is 0 it is removed form the list
    public List<Pair<ItemScriptable, int>> items = new List<Pair<ItemScriptable, int>>();

    //Holds the equipments and a bool that is true if the equipment is equipped on a player
    //This bool will be set by the "EquipmentHolder" script after it instantiates everything
    public List<Pair<EquipmentScriptable, bool>> equipments = new List<Pair<EquipmentScriptable, bool>>();

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

    //Add tehe starting items that were set from the editor
    private void Start()
    {
        AddStartingItems();
    }

    private void AddStartingItems()
    {
        //First sorth the items based on their index (recovery -> elixirs -> revival)
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

        //Insert the items into the items list and raise the count if there are multiple identical items
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

        //Insert the equipment
        foreach(EquipmentScriptable equipment in startingEquipment)
        {
            equipments.Add(new Pair<EquipmentScriptable, bool>(equipment, false));
        }
        //Make the EquipmentHolder set the bool for the equipped items
        EquipmentHolder.instance.FindEquippedItems();
    }

    //Called by EquipmentHolder to set the equipped item, also called when changing equipment
    public void FindAndSetEquipped(EquipmentScriptable equipment, bool value)
    {
        for (int index = 0; index < equipments.Count; index++)
            if (equipments[index].first == equipment)
                equipments[index].second = value;
    }

    //Add item picked up on the map, called by the Item script
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

    //Add the equipment picked up on the map to the list
    public void AddEquipment(EquipmentScriptable equipment)
    {
        equipments.Add(new Pair<EquipmentScriptable, bool>(equipment, false));
    }

    //generate description for the items
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
