using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool itIsItem;
    public ItemScriptable item;
    public EquipmentScriptable equipment;

    private InventoryMenu inventoryCanvas;
    private Inventory inventory;
    private bool colliding = false;

    private void Start()
    {
        inventory = Inventory.instance;
        inventoryCanvas = FindObjectOfType<InventoryMenu>();
    }

    private void Update()
    {
        if(colliding && Input.GetButtonDown("Interact"))
        {
            if (itIsItem)
            {
                inventory.AddItem(item);
            }
            else
            {
                inventory.AddEquipment(equipment);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject.GetComponent<PlayerMovement>().controlableCharacter == true)
        {
            colliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.GetComponent<PlayerMovement>().controlableCharacter == true)
        {
            colliding = false;
        }
    }
}
