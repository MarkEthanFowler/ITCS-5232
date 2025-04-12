using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource enemyWepSwing;
    public AudioSource playerWepSwing;
    public AudioSource enemyWepHit;
    public AudioSource playerWepHit;
    public AudioSource playerAttackGrunt;
    public AudioSource[] enemyHitSound;
    public AudioSource[] playerHitSound;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
