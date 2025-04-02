using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform hammerHead;

    Rigidbody characterRigidbody;

    private float movementSpeed;
    private float rotationSpeed;

    private float currentHealth;
    private float maxHealth;

    private float attackDistance;
    public int damageEnemy;

    public string levelName;

    private void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
        movementSpeed = 10f;
        rotationSpeed = 5f;
        currentHealth = 100;
        maxHealth = 100;
        attackDistance = 5f;

        damageEnemy = -25;

        ChangeHealth(0);

        GameManager.instance.player = this;
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            animator.SetTrigger("Attack");
        }

        if (h == 0 && v == 0)
        {
            return;
        }


        //apply the direction to the position of the character multiplying it by its move speed and delta time
        characterRigidbody.MovePosition(playerTransform.position + movementDirection * Time.deltaTime * movementSpeed);

        Quaternion faceDirection = Quaternion.LookRotation(movementDirection);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, faceDirection, rotationSpeed * Time.deltaTime);

        
        
    }

    public void ChangeHealth(int hp)
    {
        currentHealth += hp;
        if(currentHealth <= 0)
        {
            
            SceneManager.LoadScene(levelName);

        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        GUIManager.instance.UpdateHealthBar(currentHealth / maxHealth);
    }

    public void CheckHitOfPlayer()
    {
        if (GameManager.instance.enemyList.Capacity == 0)
        {
            return;
        }
        for (int i = 0; i < GameManager.instance.enemyList.Capacity; i++)
        {
            if (Vector3.Distance(GameManager.instance.enemyList[i].transform.position, playerTransform.position) < attackDistance)
            {
                GameManager.instance.enemyList[i].ChangeHealthOfEnemy(damageEnemy);
            }
        }
    }
    
}
