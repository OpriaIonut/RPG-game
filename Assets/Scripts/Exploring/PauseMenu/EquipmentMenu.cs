using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentMenu : MonoBehaviour
{
    public GameObject hiddenButton;

    private EventSystem eventSys;
    private bool equipmentMenuActive = false;

    private void Start()
    {
        eventSys = EventSystem.current;
    }

    private void Update()
    {
        if(equipmentMenuActive)
        {

        }
    }

    public void ActivateEquipmentMenu(bool value)
    {
        equipmentMenuActive = value;

        if(value == true)
            eventSys.SetSelectedGameObject(hiddenButton);
    }
}
