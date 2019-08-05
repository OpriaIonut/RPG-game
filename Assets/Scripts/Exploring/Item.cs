using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemScriptable item;

    private Inventory inventory;
    private bool colliding = false;

    private void Start()
    {
        inventory = Inventory.instance;
    }

    private void Update()
    {
        if(colliding && Input.GetButtonDown("Interact"))
        {
            inventory.AddItem(item);
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
