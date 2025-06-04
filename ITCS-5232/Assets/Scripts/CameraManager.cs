using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Transform cameraTransform;
    public Transform player;

    private Vector3 cameraOffset;
    private float cameraMoveSpeed;

    private float rotX, rotY, distance;

    private void Start()
    {
        cameraMoveSpeed = 3f;
        distance = 10f;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotY += mouseX;
        rotX -= mouseY;

        rotX = Mathf.Clamp(rotX, -40, 40);
        rotY = Mathf.Clamp(rotY, -40, 40);

        transform.localEulerAngles = new Vector3(rotX, rotY, 0);
        transform.position = player.position - transform.forward * distance + cameraTransform.up * 2f;
    }
}
