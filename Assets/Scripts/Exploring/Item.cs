using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the class for an item/equipment as collectible on the map
public class Item : MonoBehaviour
{
    public bool itIsItem;                   //If false it is an equipment
    public ItemScriptable item;             //The item that will be added when interacting on the map
    public EquipmentScriptable equipment;   //The equipment that will be added when interacting on the map
    
    private Inventory inventory;
    private bool colliding = false;         //Set true with OnTriggerEnter if the trigger is with the player and false with OnTriggerExit

    private void Start()
    {
        inventory = Inventory.instance;
    }

    private void Update()
    {
        //If we are collidiong and we press the "Interact button
        if(colliding && Input.GetButtonDown("Interact"))
        {
            //Add the item/equipment to the inventory and delete it from the map
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
        //If we are colliding with the main player set the bool to true
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
