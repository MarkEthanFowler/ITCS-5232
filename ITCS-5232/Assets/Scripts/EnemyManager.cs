using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform trans;
    [SerializeField] private Animator animator;
    private Transform target;
    private float rotationSpeed;
    private float radiusOfSatisfaction;
    private float attackHitRange;


    // Start is called before the first frame update
    void Start()
    {
        SetTarget(GameManager.instance.player.playerTransform);
        rotationSpeed = 5f;
        radiusOfSatisfaction = 2.5f;
        attackHitRange = 2f;
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
}
