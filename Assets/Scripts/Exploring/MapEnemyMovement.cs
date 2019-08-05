using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapEnemyMovement : MonoBehaviour
{
    public LayerMask mask;                  //Used for raycasting, it specifies the colliders that we can detect
    public Transform coneCollider;          //Our line of sight
    public float targetFolowDistance;       //The distance from the initial position until it will stop and return to initial position
    public float sightDistance;             //How far we can see, z component of our cone
    public float sightRadius;               //How wide is our line of sight, x and y components of our cone
    public float movementRadius;            //The radius of the place where we can move
    public float idleTime;

    [HideInInspector]
    public int enemyIndex;                  //Used to diferentiate between enemies, it is mostly used by other scripts

    private NavMeshAgent navAgent;          //Reference to navmesh component
    private Transform target;               //Will be set to a value when the player enters our line of sight
    private Vector3 initialPosition;        //The initial position on which we started, is used for most movement calculations
    private bool lostTarget;                //Used to reset the destination of the navmesh only once when we lose the target
    private bool foundDestination = false;  //Used for calculating idle time accurately
    private float lastDestinationTime = 0;

    private Vector3 destination;            //Intermediary variable
    private RaycastHit hitInfo;             //Intermediary variable

    private void Start()
    {
        initialPosition = transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        
        //Scale the line of sight cone based on sightRadius and sightDistance.
        coneCollider.localScale = new Vector3(sightRadius, sightRadius, sightDistance / 2f - 0.7f);

        //Set and initial destination for the navmesh
        foundDestination = true;
        CalculateDestination();
        navAgent.SetDestination(destination);
    }

    private void Update()
    {
        //If we didn't see the player
        if (target == null)
        {
            //Simply move and draw a line to be able to see the orientation in scene view
            Movement();
            Debug.DrawLine(transform.position, transform.position + transform.forward * sightDistance, Color.blue);
        }
        else
        {
            //Otherwise check if we have lost the pllayer
                //We can lose the player if he exits our line of sight or we followed him too far from our initialPosition
            if (lostTarget == false)
            {
                //If we didn't lose him, draw a line for debuging and set the navmesh detination to him
                Debug.DrawLine(transform.position, target.position, Color.red);
                navAgent.SetDestination(target.position);
                //Check if we lost him
                if (Vector3.Distance(transform.position, target.position) > sightDistance || Vector3.Distance(transform.position, initialPosition) > targetFolowDistance)
                {
                    //If so, stop movement for a couple of seconds and then move back to initial position
                    lostTarget = true;
                    navAgent.SetDestination(transform.position);
                    StartCoroutine(LostTargetWaitTime(2f));
                }
            }
        }
    }

    private IEnumerator LostTargetWaitTime(float delay)
    {
        //Wait for a couple of seconds after which go to initial position and reset all variables used in following player
        yield return new WaitForSeconds(delay);
        lostTarget = false;
        target = null;
        sightDistance /= 2;                         //sight distance becomes half it's value because when we spotted the layer we doubled it
        navAgent.SetDestination(initialPosition);
        destination = initialPosition;
        foundDestination = true;
    }

    private void Movement()
    {
        //If we don't have a destination
        if (foundDestination == false)
        {
            //If we have finished our idle time
            if (Time.time - lastDestinationTime > idleTime)
            {
                //Calculate a new destination
                foundDestination = true;
                CalculateDestination();
                navAgent.SetDestination(destination);
            }
        }
        //Else if we have a destination and we reached it
        else if (Vector3.Distance(transform.position, destination) < 1f)
        {
            //Start idle time
            foundDestination = false;
            lastDestinationTime = Time.time;
        }
    }

    //Calculate the destination randomly within our radius
    private void CalculateDestination()
    {
        float degree = Random.Range(0f, 360f);

        destination = new Vector3(initialPosition.x + Mathf.Sin(degree * Mathf.PI / 180f) * Random.Range(0f, movementRadius),
                                  initialPosition.y,
                                  initialPosition.z + Mathf.Cos(degree * Mathf.PI / 180f) * Random.Range(0f, movementRadius));
    }

    //If something entered our line of sight cone
    private void OnTriggerEnter(Collider other)
    {
        Vector3 direction = other.gameObject.transform.position - transform.position;

        //Check if it is not obstructed by anything
        /*                                            This may not work, it may ignore walls     */
        if (target == null && Physics.Raycast(transform.position, direction, out hitInfo, sightDistance, mask))
        {
            if (hitInfo.collider.tag == "Player")
            {
                //If it is the player, check if it is the main player
                PlayerMovement playerStatus = hitInfo.collider.gameObject.GetComponent<PlayerMovement>();
                if (playerStatus != null && playerStatus.controlableCharacter == true)
                {
                    //If so, double the line of sight, set the navmesh destination to him and draw a red line for debugging purposes
                    target = hitInfo.collider.gameObject.transform;
                    navAgent.SetDestination(target.position);
                    sightDistance *= 2;
                    Debug.DrawLine(transform.position, target.position, Color.red);
                }
            }
        }
    }

    //Draw the movement circle of the enemy for debugging purposes
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(initialPosition, movementRadius);
    }
}
