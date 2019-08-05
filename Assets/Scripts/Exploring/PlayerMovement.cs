using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("AI variables")]
    public bool controlableCharacter;
    public Transform mainPlayerTransform;
    public Vector3 offset;
    public EnemySpawner enemySpawner;
    public float minDistance;
    public float smoothing;

    private RaycastHit ray;

    [Header("Player variables")]
    public int movementSpeed;
    public Animator canvasAnim;

    private PlayerStatusExploring[] playerStatus;
    private DataRetainer dataRetainer;
    private Rigidbody rb;

    private float horizontalMovement;
    private float verticalMovement;

    private void Start()
    {
        dataRetainer = DataRetainer.instance;
        playerStatus = FindObjectsOfType<PlayerStatusExploring>();
        rb = GetComponent<Rigidbody>();

        foreach (PlayerStatusExploring status in playerStatus)
        {
            transform.position = dataRetainer.GetPlayerPosition(status.playerIndex);
        }
    }

    private void Update()
    {
        if(controlableCharacter)
        {
            horizontalMovement = Input.GetAxis("Horizontal");
            verticalMovement = Input.GetAxis("Vertical");
        }
    }

    private void FixedUpdate()
    {
        //The player that we control through input
        if (controlableCharacter)
        {
            if (horizontalMovement != 0 || verticalMovement != 0)
            {
                rb.MovePosition(transform.position + new Vector3(horizontalMovement, rb.velocity.y, verticalMovement) * movementSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            transform.LookAt(mainPlayerTransform);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out ray))
            {
                float targetDistance = ray.distance;
                if (targetDistance > minDistance)
                {
                    Vector3 destination = Vector3.MoveTowards(transform.position, mainPlayerTransform.position + offset, minDistance * movementSpeed * Time.fixedDeltaTime);
                    rb.MovePosition(destination);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(controlableCharacter == true && other.tag == "Enemy")
        {
            foreach(PlayerStatusExploring status in playerStatus)
            {
                dataRetainer.SetPlayerHealth(status.playerIndex, status.currentHealth);
                dataRetainer.SetPlayerPosition(status.playerIndex, status.gameObject.transform.position);
            }
            if(dataRetainer.defeatedEnemiesIndex.Contains(other.gameObject.GetComponent<MapEnemyMovement>().enemyIndex) == false)
                dataRetainer.AddEncounter(other.gameObject.GetComponent<MapEnemyMovement>().enemyIndex);

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
