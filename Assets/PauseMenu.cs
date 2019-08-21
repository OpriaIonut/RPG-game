using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseHiddenButton;
    public GameObject pauseGameMenu;
    public GameObject pauseGameButtons;
    public GameObject inventoryMenu;
    public InventoryMenu inventoryScript;

    private EventSystem eventSys;
    private GameObject[] playerObjects;
    private List<MapEnemyMovement> enemyMovementScripts = new List<MapEnemyMovement>();

    private bool gameIsPaused = false;
    private bool inventoryActive = false;

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
        eventSys = EventSystem.current;
        inventoryMenu.SetActive(false);
        pauseGameMenu.SetActive(false);
        inventoryScript.enabled = false;

        playerObjects = GameObject.FindGameObjectsWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryActive)
                ToggleInventoryMenu();
            else
                TogglePauseMenu();
        }
    }
    
    public void AddEnemyMovementScript(ref MapEnemyMovement script)
    {
        enemyMovementScripts.Add(script);
    }

    public void TogglePauseMenu()
    {
        gameIsPaused = !gameIsPaused;
        pauseGameMenu.SetActive(gameIsPaused);
        pauseGameButtons.SetActive(gameIsPaused);

        for (int index = 0; index < enemyMovementScripts.Count; index++)
        {
            enemyMovementScripts[index].StopNavmesh(gameIsPaused);
        }
        for (int index = 0; index < playerObjects.Length; index++)
        {
            playerObjects[index].SetActive(!gameIsPaused);
        }

        if(gameIsPaused == true)
        {
            eventSys.SetSelectedGameObject(pauseHiddenButton);
        }
    }

    public void ToggleInventoryMenu()
    {
        inventoryActive = !inventoryActive;
        inventoryMenu.SetActive(inventoryActive);
        inventoryScript.enabled = inventoryActive;

        pauseGameButtons.SetActive(!inventoryActive);

        if(inventoryActive == false)
            eventSys.SetSelectedGameObject(pauseHiddenButton);

        if(inventoryActive)
            inventoryScript.ActivateInventory();
    }
}
