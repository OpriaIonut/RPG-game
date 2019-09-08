using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseHiddenButton;    //The main hidden button
    public GameObject pauseGameMenu;        //Reference to the UI pause menu
    public GameObject pauseGameButtons;     //Reference to the UI buttons from the pause menu (when opening another menu like the inventory we need to disable them so taht we don't toggle between them too)
    public GameObject inventoryMenu;        //Reference to the inventory UI menu so that we can toggle it
    public InventoryMenu inventoryScript;
    public GameObject equipmentMenu;        //Reference to the equipment UI menu so that we can toggle it
    public GameObject statusMenu;           //Reference to the status UI menu so that we can toggle it
    public GameObject[] playerObjects;     //When we pause the game we deactivate it

    private EventSystem eventSys;
    private List<MapEnemyMovement> enemyMovementScripts = new List<MapEnemyMovement>(); //Reerence to all the enemyMovement scripts so that we can stop the navmeshes from them when pausing the game
    private EquipmentMenu equipmentMenuScript;
    private StatusMenu statusMenuScript;

    //Booleans used for the transitions between menus
    private bool gameIsPaused = false;
    private bool inventoryActive = false;
    private bool equipmentActive = false;
    private bool statusActive = false;

    #region Singleton

    public static PauseMenu instance;

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
    
    private void Start()
    {
        //When we start the game turn everything off
        eventSys = EventSystem.current;
        inventoryMenu.SetActive(false);
        equipmentMenu.SetActive(false);
        statusMenu.SetActive(false);
        pauseGameMenu.SetActive(false);

        statusMenuScript = StatusMenu.instance;
        equipmentMenuScript = GetComponent<EquipmentMenu>();
    }

    private void Update()
    {
        //If we press the "Cancel" button
        if (Input.GetButtonDown("Cancel"))
        {
            //If the inventory is active, deactivate it
            if (inventoryActive)
                ToggleInventoryMenu();
            else if (equipmentActive)   //Else if the equipment is active, deactivate it
                ToggleEquipmentMenu();
            else if (statusActive)
                ToggleStatusMenu();
            else
                TogglePauseMenu();      //Otherwise, if no menu is active, toggle the pause menu
        }
    }
    
    //Called by the EnemySpawner (some enemies will be spawned with a delay and it is called at the moment they are spawned so that we can stop their movement too) (hopefuly the enemies are not spawned when the game is paused)
    public void AddEnemyMovementScript(ref MapEnemyMovement script)
    {
        enemyMovementScripts.Add(script);
    }

    public void TogglePauseMenu()
    {
        //Ativate/Deactivate the needed menus
        gameIsPaused = !gameIsPaused;
        pauseGameMenu.SetActive(gameIsPaused);
        pauseGameButtons.SetActive(gameIsPaused);

        //Activate/Stop the enemy navmeshes
        for (int index = 0; index < enemyMovementScripts.Count; index++)
        {
            enemyMovementScripts[index].StopNavmesh(gameIsPaused);
        }
        //Activate/Deactivate the players
        for (int index = 0; index < playerObjects.Length; index++)
        {
            playerObjects[index].SetActive(!gameIsPaused);
        }

        //If we are pausing the game set the focus to the hidden button
        if(gameIsPaused == true)
        {
            eventSys.SetSelectedGameObject(pauseHiddenButton);
        }
    }

    public void ToggleInventoryMenu()
    {
        //If in the inventory script we were selecting the player, stop selecting it and stop the fct.
        if(inventoryScript.selectingPlayer)
        {
            inventoryScript.SelectingPlayer(false);
            return;
        }

        //Activate/Deactivate the needen menus
        inventoryActive = !inventoryActive;
        inventoryMenu.SetActive(inventoryActive);

        pauseGameButtons.SetActive(!inventoryActive);

        if (inventoryActive == false)
        {
            //IF we are disabling the inventory menu, deactivate it and set the focus to the hidden button
            inventoryScript.DeactivateInventoryMenu();
            eventSys.SetSelectedGameObject(pauseHiddenButton);
        }
        else
            inventoryScript.ActivateInventoryMenu();    //Else activate the inventory menu
    }

    public void ToggleEquipmentMenu()
    {
        if(equipmentMenuScript.selectEquipment == true)
        {
            //If we were selecting the equipment, delete the slots (we always delete them when exiting that menu, seems cleaner but less efficient)
            equipmentMenuScript.DeleteEqupmentSlots();
            equipmentMenuScript.StartSelectingEquipment(false); //Stop selecting the equipment and 
            equipmentMenuScript.UpdateEquipmentSelectValueText(false);  //Hide the specific equipment UI. For more details go to implementation
            return;
        }

        //Activate/Deactivate everything that needs to be
        equipmentActive = !equipmentActive;
        equipmentMenu.SetActive(equipmentActive);
        equipmentMenuScript.ActivateEquipmentMenu(equipmentActive);
        pauseGameButtons.SetActive(!equipmentActive);

        //If we are disabling the equipment menu, set the focus to the hidden button
        if (equipmentActive == false)
            eventSys.SetSelectedGameObject(pauseHiddenButton);
    }

    public void ToggleStatusMenu()
    {
        statusActive = !statusActive;
        statusMenu.SetActive(statusActive);
        pauseGameButtons.SetActive(!equipmentActive);
        statusMenuScript.ToggleStatusMenu(statusActive);

        if (statusActive == false)
            eventSys.SetSelectedGameObject(pauseHiddenButton);
    }
}
