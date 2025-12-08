using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    public GameObject woodPickup;
    public GameObject rockPickup;
    public GameObject bombPickup;

    public AudioSource musicManager;
    public AudioClip track1;
    public AudioClip track2;
    public Light light;
    public GameObject[] enemies;
    private bool startedMusic = false;
    private GameObject player;

    public int currentDay;
    public int timeReduction = 2;
    public int dayTimeDuration = 16;//30
    public float timeBetweenSpawns = 10;

    void Start()
    {
        //light = FindFirstObjectByType<Light>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        //Camera.main.backgroundColor = new Color(0.55f, 0.8f, 1f);
        currentDay = 1;
    }

    private void Update()
    {
        //if(player.transform.position.x > -10 && startedMusic == false)
        //{
        //    print("Start");
        //    //StartCoroutine(PlayMusic());
        //    startedMusic = true;
        //}

        if (player.GetComponent<Inventory>().itemsAmounts[1] > 2 && !startedMusic) 
        {
            StartCoroutine(StartWave());
            startedMusic = true;
        }
    }

    public void SpawnPickup(int materialToSpawnIndex, Vector3 spawnLocation)
    {
        
        if (materialToSpawnIndex == 0) { Instantiate(woodPickup, spawnLocation, Quaternion.identity); }
        if (materialToSpawnIndex == 1) { Instantiate(rockPickup, spawnLocation, Quaternion.identity); }
        if (materialToSpawnIndex == 2) { Instantiate(bombPickup, spawnLocation, Quaternion.identity); }
    }


    private IEnumerator StartWave()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            int enemyType = Random.Range(0, enemies.Length);
            GameObject prefab = enemies[enemyType];

            Instantiate(prefab, child.position, Quaternion.identity, child); // optional: attach it to child
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        //yield return new WaitForSeconds(120);//140

        //if(currentDay < 5)
        //{
        //    currentDay++;
        //    dayTimeDuration -= timeReduction;
        //    timeBetweenSpawns -= timeReduction;
        //}

        StartCoroutine(StartWave());
    }
}
