using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform camera;
    public float multiplier;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = camera.position;
    }

    void LateUpdate()
    {
        Vector3 cameraMovement = camera.position;
        transform.position += cameraMovement - lastPosition;
        lastPosition = cameraMovement;
    }
}
