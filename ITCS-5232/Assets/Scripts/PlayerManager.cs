using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //Convert the user input into a directional vector
        Vector3 movementDirection = new Vector3(h, 0f, v);


        if(movementDirection == Vector3.zero)
        {
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Attack");
        }
        else
        {
            animator.SetFloat("Speed", 1f);
            animator.ResetTrigger("Attack");
        }

        //apply the direction to the position of the character multiplying it by its move speed and delta time
        characterRigidbody.MovePosition(playerTransform.position + movementDirection * Time.deltaTime * movementSpeed);

        Quaternion faceDirection = Quaternion.LookRotation(movementDirection);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, faceDirection, rotationSpeed * Time.deltaTime);

        if(Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger("Attack");
        }
        
    }
}
