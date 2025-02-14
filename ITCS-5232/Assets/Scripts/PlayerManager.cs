using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;

    [SerializeField] private Animator animator;

    Rigidbody characterRigidbody;

    private float movementSpeed;
    private float rotationSpeed;

    private void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
        movementSpeed = 5f;
        rotationSpeed = 5f;
    }

    private void FixedUpdate()
    {
        //Convert the user input into a directional vector
        Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if(movementDirection == Vector3.zero)
        {
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            animator.SetFloat("Speed", 1f);
        }

        //apply the direction to the position of the character multiplying it by its move speed and delta time
        characterRigidbody.MovePosition(playerTransform.position + movementDirection * Time.deltaTime * movementSpeed);

        Quaternion faceDirection = Quaternion.LookRotation(movementDirection);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, faceDirection, rotationSpeed * Time.deltaTime);
    }
}
