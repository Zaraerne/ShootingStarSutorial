using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenSpawns;

    [SerializeField] bool spawnEnemy = true;
    [SerializeField] GameObject waveUI;
    [SerializeField] GameObject[] enemyPrafabs;
    
    [SerializeField] float timeBetweenSpawns = 1f;
    [SerializeField] float timeBetweenWaves = 1f;
    WaitForSeconds waitTimeBetweenSpawns;
    WaitForSeconds waitTimeBetweenWaves;

    [SerializeField] int minEnemyAmout = 4;
    [SerializeField] int maxEnemyAmout = 10;

    [Header("=== BOSS SETTING ===")]
    [SerializeField] GameObject bossPrefabs;
    [SerializeField] int bossWaveNumber = 3;

    int waveNumber = 1;
    int enemyAmount;

    List<GameObject> enemyList;
    WaitUntil waitUnitNoEnemy;

    private void Awake()
    {
        base.Awake();
        enemyList = new List<GameObject>();
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitUnitNoEnemy = new WaitUntil(() => enemyList.Count == 0);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
    }

    IEnumerator Start()
    {
        while (spawnEnemy && GameManager.GameState != GameState.GameOver)
        {
            waveUI.SetActive(true);

            yield return waitTimeBetweenWaves;
            waveUI.SetActive(false);

            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
        
    }


    IEnumerator RandomlySpawnCoroutine()
    {
        if (waveNumber % bossWaveNumber == 0)
        {
            var boss =  PoolManager.Release(bossPrefabs);
            enemyList.Add(boss);
        }
        else
        {
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmout + waveNumber / bossWaveNumber, maxEnemyAmout);

            for (int i = 0; i < enemyAmount; i++)
            {
                enemyList.Add(PoolManager.Release(enemyPrafabs[Random.Range(0, enemyPrafabs.Length)]));

                yield return waitTimeBetweenSpawns;
            }
        }
        yield return waitUnitNoEnemy;
        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);

}
