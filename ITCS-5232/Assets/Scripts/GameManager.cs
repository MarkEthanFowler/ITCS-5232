using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerManager player;
    public List<EnemyManager> enemyList;
    public EnemyManager enemyManager;

    private WaitForSeconds enemySpawnDelay;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    

    private void Awake()
    {
        //check to see if the singleton exists
        if(instance == null)
        {
            //Create the singleton
            instance = this;

            //prevent the game object from getting destroyed
            DontDestroyOnLoad(gameObject);
            //DontDestroyOnLoad(enemyPrefab);
            //DontDestroyOnLoad(spawnPoint);
        }
        else//singleton exists already destroy this game object
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        enemySpawnDelay = new WaitForSeconds(10f);
        StartCoroutine(RunEnemySpawnTimer());
    }

    public IEnumerator RunEnemySpawnTimer()
    {
        yield return enemySpawnDelay;

        SpawnEnemy();

        StartCoroutine(RunEnemySpawnTimer());
    }

    public void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyManager enemyScript = enemy.GetComponent<EnemyManager>();
        enemyList.Add(enemyScript);
    }
}
