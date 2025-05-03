using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform trans;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject powerUpPrefab;
    private Transform target;
    private float rotationSpeed;
    private float radiusOfSatisfaction;
    
    private float enemySpeed;
    private float currentHealth;
    private float maxHealth;
    
    private float yOffset;
    private bool isDead;
    public AudioClip test;
    private WaitForSeconds discardBodyTimer;

    private float attackHitRange;
    private int damagePlayer;


    // Start is called before the first frame update
    void Start()
    {
        SetTarget(GameManager.instance.player.playerTransform);
        rotationSpeed = 5f;
        radiusOfSatisfaction = 5f;
        
        enemySpeed = 1f;
        currentHealth = 100;
        maxHealth = 100;

        damagePlayer = -5;
        attackHitRange = 2f;

        ChangeHealthOfEnemy(0);
        isDead = false;
        discardBodyTimer = new WaitForSeconds(10f);
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
            if(isDead == false)
            {
                Vector3 faceTarget = target.position - trans.position;
                Quaternion targetRotation = Quaternion.LookRotation(faceTarget);
                trans.rotation = Quaternion.Lerp(trans.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                Vector3 movementDirection = (target.position - trans.position).normalized;
                trans.position += (faceTarget * Time.deltaTime * enemySpeed);
            }
            else
            {
                animator.SetTrigger("Dead");
            }
        }
    }

    private void EnemyAction()
    {
        if(isDead == false)
        {
            if (target == null)
            {
                animator.SetFloat("Speed", 0f);
            }
            else if (Vector3.Distance(trans.position, target.position) < radiusOfSatisfaction)
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

    

    public void ChangeHealthOfEnemy(int hp)
    {
        currentHealth += hp;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if(currentHealth <= 0)
        { 
            GameManager.instance.enemyList.Remove(this);
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Dead");
            isDead = true;
            Instantiate(powerUpPrefab, (trans.position + new Vector3(0f, 3f, 0f)), Quaternion.identity);
            StartCoroutine(RunDiscardBodyTimer());
        }
    }

    public IEnumerator RunDiscardBodyTimer()
    {
        yield return discardBodyTimer;

        Destroy(this);
        Destroy(gameObject);

    }

    public void CheckHitOnPlayer()
    {
        if (target != null)
        {
            if (Vector3.Distance(trans.position, target.position) < attackHitRange)
            {
                GameManager.instance.player.ChangeHealth(damagePlayer);
            }
        }

    }

    public void EnemyAttackSoundEffects()
    {
        if (SFXManager.instance.playerAttackGrunt.isPlaying == false)
        {
            SFXManager.instance.enemyWepHit.Play();
        }
        for(int i = 0; i < SFXManager.instance.playerHitSound.Length; i++)
        {
            if (SFXManager.instance.playerHitSound[i].isPlaying == false)
            {
                SFXManager.instance.playerHitSound[UnityEngine.Random.Range(0, SFXManager.instance.playerHitSound.Length)].Play();
            }
        }
        
    }
}
