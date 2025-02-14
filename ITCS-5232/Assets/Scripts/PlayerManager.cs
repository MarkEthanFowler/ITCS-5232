using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;

    Rigidbody characterRigidbody;

    private float movementSpeed;
    private float rotationSpeed;

    private void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
        movementSpeed = 5;
        rotationSpeed = 5;
    }

    private void FixedUpdate()
    {
        //Convert the user input into a directional vector
        Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        //apply the direction to the position of the character multiplying it by its move speed and delta time
        characterRigidbody.MovePosition(playerTransform.position + movementDirection * Time.deltaTime * movementSpeed);

        Quaternion faceDirection = Quaternion.LookRotation(movementDirection);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, faceDirection, rotationSpeed);
    }
}
