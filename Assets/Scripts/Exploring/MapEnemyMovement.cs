using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapEnemyMovement : MonoBehaviour
{
    public Transform coneCollider;
    public float targetFolowDistance;       //The distance from the initial position until it will stop and return to initial position
    public float sightDistance;
    public float sightRadius;
    public float radius;
    public float idleTime;
    public LayerMask mask;

    [HideInInspector]
    public int enemyIndex;

    private Transform target;
    private bool lostTarget;
    private bool foundDestination = false;
    private float lastDestinationTime = 0;
    private Vector3 initialPosition;
    private Vector3 destination;
    private NavMeshAgent navAgent;
    private RaycastHit hitInfo;

    private void Start()
    {
        initialPosition = transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        
        coneCollider.localScale = new Vector3(sightRadius, sightRadius, sightDistance / 2f - 0.7f);

        foundDestination = true;
        CalculateDestination();
        navAgent.SetDestination(destination);
    }

    private void Update()
    {
        if (target == null)
        {
            Movement();
            Debug.DrawLine(transform.position, transform.position + transform.forward * sightDistance, Color.blue);
        }
        else
        {
            if (lostTarget == false)
            {
                Debug.DrawLine(transform.position, target.position, Color.red);
                navAgent.SetDestination(target.position);
                if (Vector3.Distance(transform.position, target.position) > sightDistance || Vector3.Distance(transform.position, initialPosition) > targetFolowDistance)
                {
                    lostTarget = true;
                    navAgent.SetDestination(transform.position);
                    StartCoroutine(LostTargetWaitTime(2f));
                }
            }
        }
    }

    private IEnumerator LostTargetWaitTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        lostTarget = false;
        target = null;
        sightDistance /= 2;
        navAgent.SetDestination(initialPosition);
        destination = initialPosition;
        foundDestination = true;
    }

    private void Movement()
    {
        if (foundDestination == false)
        {
            if (Time.time - lastDestinationTime > idleTime)
            {
                foundDestination = true;
                CalculateDestination();
                navAgent.SetDestination(destination);
            }
        }
        else if (Vector3.Distance(transform.position, destination) < 1f)
        {
            foundDestination = false;
            lastDestinationTime = Time.time;
        }
    }

    private void CalculateDestination()
    {
        float degree = Random.Range(0f, 360f);

        destination = new Vector3(initialPosition.x + Mathf.Sin(degree * Mathf.PI / 180f) * Random.Range(0f, radius),
                                  initialPosition.y,
                                  initialPosition.z + Mathf.Cos(degree * Mathf.PI / 180f) * Random.Range(0f, radius));
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 direction = other.gameObject.transform.position - transform.position;

        if (target == null && Physics.Raycast(transform.position, direction, out hitInfo, sightDistance, mask))
        {
            if (hitInfo.collider.tag == "Player")
            {
                PlayerMovement playerStatus = hitInfo.collider.gameObject.GetComponent<PlayerMovement>();
                if (playerStatus != null && playerStatus.controlableCharacter == true)
                {
                    target = hitInfo.collider.gameObject.transform;
                    navAgent.SetDestination(target.position);
                    sightDistance *= 2;
                    Debug.DrawLine(transform.position, target.position, Color.red);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(initialPosition, radius);
    }
}
