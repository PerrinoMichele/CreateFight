using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] public Camera mainCamera;
    [SerializeField] private float cameraHeight = 45f;
    [SerializeField] private float cameraZOffset = -20;
    [SerializeField] private bool cameraMovesOnX = false;

    private Vector3 pos;

    private void Start()
    {
        mainCamera = FindFirstObjectByType<Camera>();
        pos = transform.position;
        pos.y = cameraHeight;
        pos.z = cameraZOffset;
        pos.x = 0;
    }

    private void Update()
    {
        pos.z = gameObject.transform.position.z + cameraZOffset;
        if (cameraMovesOnX)
        {
            pos.x = gameObject.transform.position.x;//in the future camera should not show out of the map too much
        }

        mainCamera.transform.position = pos;//add lerp    
    }
}

