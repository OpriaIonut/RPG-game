using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("AI variables")]
    public bool controlableCharacter;       //Is this caracter the main one
    public Transform mainPlayerTransform;   //All players will have a reference to the main player so that they can follow him
    public Vector3 offset;                  //Offset of each player so that they don't go straight for the player
    public float minDistance;               //If the distance between the the position of the AI and the main player is grater than this then start moving towards it
    public float smoothing;                 //Used for smooth follow movement

    [Header("Player variables")]
    public int movementSpeed;
    public Animator canvasAnim;             //This script loads the scene so we need a reference to the canvas anim

    /*Retains the status of every player so that we can save the data when changing scenes 
     * It can't be done separately in every script because then it would save data and change scene even when non-controlable players collide with enemies */
    private PlayerStatusExploring[] playerStatus;
    private DataRetainer dataRetainer;
    private Rigidbody rb;

    //Intermediary variables
    private float horizontalMovement;
    private float verticalMovement;

    private void Start()
    {
        dataRetainer = DataRetainer.instance;
        playerStatus = FindObjectsOfType<PlayerStatusExploring>();
        rb = GetComponent<Rigidbody>();

        //Get the player position that it was on before changing scenes ( or initial position if it is the first scene )
        transform.position = dataRetainer.GetPlayerPosition(GetComponent<PlayerStatusExploring>().playerIndex);
    }

    private void Update()
    {
        //If we can control the character check for movement input. Input is best capturen in Update not in FixedUpdate
        if (controlableCharacter)
        {
            horizontalMovement = Input.GetAxis("Horizontal");
            verticalMovement = Input.GetAxis("Vertical");
        }
    }

    private void FixedUpdate()
    {
        //If we can control the character and we have received an imput
        if (controlableCharacter)
        {
            if (horizontalMovement != 0 || verticalMovement != 0)
            {
                //Move the character by rigidbody so that we can also check for collisions
                rb.MovePosition(transform.position + new Vector3(horizontalMovement, rb.velocity.y, verticalMovement) * movementSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            //Else, if it is an AI, look at the target and calculate the distance between the AI and the player + offset
            transform.LookAt(mainPlayerTransform);
            float targetDistance = Vector3.Distance(transform.position, mainPlayerTransform.position + offset);

            if (targetDistance > minDistance)
            {
                //If it is bigger than our minimum permited idstance, move towards it smoothly
                Vector3 destination = Vector3.MoveTowards(transform.position, mainPlayerTransform.position + offset, minDistance * movementSpeed * Time.fixedDeltaTime);
                rb.MovePosition(destination);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If we collided with an enemy with the main character
        if (controlableCharacter == true && other.tag == "Enemy")
        {
            //Retain all players health in data retainer
            foreach (PlayerStatusExploring status in playerStatus)
            {
                dataRetainer.SetPlayerHealth(status.playerIndex, status.currentHealth);
                dataRetainer.SetPlayerMP(status.playerIndex, status.currentMP);
                dataRetainer.SetPlayerPosition(status.playerIndex, status.gameObject.transform.position);
            }
            //Check if we encountered the same enemy before and it hasn't respawned yet
            if (dataRetainer.defeatedEnemiesIndex.Contains(other.gameObject.GetComponent<MapEnemyMovement>().enemyIndex) == false)
            {
                //If it has respawned, remember it's index so that we can spawn it with a delay next time we load this scene
                dataRetainer.AddEncounter(other.gameObject.GetComponent<MapEnemyMovement>().enemyIndex);
            }

            dataRetainer.SaveEncounter(other.GetComponent<EnemyEncounterHolder>());

            //Load scene to delay so we can finish the animation
            StartCoroutine(LoadSceneWithDelay(2f));
        }
    }

    private IEnumerator LoadSceneWithDelay(float delay)
    {
        canvasAnim.SetTrigger("EnemyEncounter");
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("CombatSceneSample");
    }
}
