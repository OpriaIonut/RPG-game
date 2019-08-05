using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 10f;   //Used for delay effect

    private Vector3 offset;         //The offset that it starts with

    private void Start()
    {
        offset = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;     //Calculate the position where it should be
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothing * Time.deltaTime);   //Smooth out the position to add delay effect
        transform.position = smoothedPosition;
    }
}
