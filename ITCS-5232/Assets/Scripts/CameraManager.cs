using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Transform cameraTransform;

    private Vector3 cameraOffset;
    private float cameraMoveSpeed;

    private void Start()
    {
        cameraOffset = new Vector3(0f, 12f, -14f);
        cameraMoveSpeed = 5f;
    }

    private void Update()
    {
        float yPos = 2f;

        Vector3 cameraEndPosition = new Vector3(playerManager.playerTransform.position.x, yPos, playerManager.playerTransform.position.z) + cameraOffset;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraEndPosition, Time.deltaTime * cameraMoveSpeed);
    }
}
