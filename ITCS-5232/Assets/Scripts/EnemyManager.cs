using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform trans;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform sword;
    private Transform target;
    private float rotationSpeed;
    private float radiusOfSatisfaction;
    private float attackHitRange;
    private float enemySpeed;
    Rigidbody rigidbody;
    private float currentHealth;
    private float maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        SetTarget(GameManager.instance.player.playerTransform);
        rotationSpeed = 5f;
        radiusOfSatisfaction = 2.5f;
        attackHitRange = 2f;
        enemySpeed = 1f;
        currentHealth = 100;
        maxHealth = 100;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtTarget();
        EnemyAction();
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void LookAtTarget()
    {
        if(target == null)
        {
            trans.rotation = Quaternion.Lerp(trans.rotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 faceTarget = target.position - trans.position;
            Quaternion targetRotation = Quaternion.LookRotation(faceTarget);
            trans.rotation = Quaternion.Lerp(trans.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 movementDirection = (target.position - trans.position).normalized;
            trans.position += (faceTarget * Time.deltaTime * enemySpeed);
        }
    }

    private void EnemyAction()
    {
        if(target == null)
        {
            animator.SetFloat("Speed", 0f);
        }
        else if(Vector3.Distance(trans.position, target.position) < radiusOfSatisfaction)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Attack");
        }
        else
        {
            animator.SetFloat("Speed", 1f);
            animator.ResetTrigger("Attack");
        }
    }

    public void CheckHit()
    {
        if(target != null)
        {
            if (Vector3.Distance(trans.position, target.position) < attackHitRange)
            {
                GameManager.instance.player.ChangeHealth(-5);
            }
        }
        
    }


}
