using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct ItemPoolMember
{
    public GameObject itemObject;
    public float chance;
    [NonSerialized] public float realChance;
}

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public float enemiesSpawnFactor = 10f, enemiesSpawnDistance;
    public float enemyDropItemChance = 0.05f;
    public AnimationCurve enemySpawnCurve;
    public List<GameObject> enemyTypes;
    public List<ItemPoolMember> itemPool;
    public TankController playerController;
    private bool enemySpawnCoolingDown;
    public GameObject gameOverUIObject; 

    public static GameManager Instance;

    void Start()
    {
        enemySpawnCoolingDown = false;
        InitializeItemPool();

    }

    void InitializeItemPool()
    {
        float itemsTotalChance = 0;
        var temp = itemPool;
        for (int i = 0; i < temp.Count; ++i)
        {
            var item = temp[i];
            if (item.itemObject is null || item.itemObject.GetComponent<ItemController>() is null)
            {
                itemPool.Remove(item);
                continue;
            }
            itemsTotalChance += item.chance;
        }

        float sumChance = 0;
        for (int i = 0; i < itemPool.Count; ++i)
        {
            var item = itemPool[i];
            item.realChance = sumChance / itemsTotalChance;
            itemPool[i] = item;
            sumChance += item.chance;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetRandomItem()
    {
        float randomNumber = UnityEngine.Random.value;

        int left = 0, right = itemPool.Count - 1;
        int mid;
        while (left < right)
        {
            mid = (left + right) / 2;
            if (itemPool[mid].realChance <= randomNumber)
                left = mid + 1; 
            else
                right = mid - 1;
        }

        return itemPool[left].itemObject;
    }

    IEnumerator IEnemySpawn()
    {
        enemySpawnCoolingDown = true;
        int spawnNum = 1;
        float curveTime = enemySpawnCurve.Evaluate(Time.time);
        // if spawning speed is faster than tick speed;
        if (Time.deltaTime > enemySpawnCurve.Evaluate(Time.time))
            spawnNum = Mathf.CeilToInt(Time.deltaTime / curveTime);
        yield return new WaitForSeconds(curveTime * spawnNum);
        enemySpawnCoolingDown = false;
        SpawnEnemy(spawnNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemySpawnCoolingDown)
        {
            StartCoroutine(IEnemySpawn());
        }
    }

    static Vector2 RotateVector2(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    void SpawnEnemy(int num = 1)
    {
        for (int i=0;i<num;i++)
        {
            Vector2 spawnLocation = RotateVector2(new Vector2(enemiesSpawnDistance, 0), UnityEngine.Random.value * 2 * Mathf.PI);
            spawnLocation += new Vector2(playerController.transform.position.x, playerController.transform.position.y);
            Instantiate(enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)], spawnLocation, Quaternion.identity);
            
        }
    }

    public void GameOver()
    {
        Instantiate(gameOverUIObject);
        Time.timeScale = 0;
    }
}
