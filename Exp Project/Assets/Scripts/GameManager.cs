using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public float enemiesSpawnFactor = 10f, enemiesSpawnDistance;
    public float enemyDropItemChance = 0.05f;
    public AnimationCurve enemySpawnCurve;
    public List<GameObject> enemyTypes;
    private GameObject player;
    private bool enemySpawnCoolingDown;

    void Start()
    {
        enemySpawnCoolingDown = false;
        player = GameObject.FindGameObjectWithTag("Player");
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
            Vector2 spawnLocation = RotateVector2(new Vector2(enemiesSpawnDistance, 0), Random.value * 2 * Mathf.PI);
            spawnLocation += new Vector2(player.transform.position.x, player.transform.position.y);
            Instantiate(enemyTypes[Random.Range(0, enemyTypes.Count)], spawnLocation, Quaternion.identity);
            
        }
    }
}
