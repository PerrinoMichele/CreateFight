using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    public AudioSource musicManager;
    public AudioClip track1;
    public AudioClip track2;
    public Light light;
    public GameObject[] enemies;
    private bool startedMusic = false;
    private GameObject player;

    public int currentDay;
    public int timeReduction = 2;
    public int dayTimeDuration = 16;
    public int timeBetweenSpawns = 10;

    void Start()
    {
        //light = FindFirstObjectByType<Light>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        Camera.main.backgroundColor = new Color(0.55f, 0.8f, 1f);
        currentDay = 1;
    }

    private void Update()
    {
        if(player.transform.position.x > -10 && startedMusic == false)
        {
            print("Start");
            StartCoroutine(PlayMusic());
            startedMusic = true;
        }
    }


    private IEnumerator PlayMusic()
    {
        musicManager.Stop();
        musicManager.clip = track1;
        musicManager.Play();
        Camera.main.backgroundColor = new Color(0.55f, 0.8f, 1f);
        light.colorTemperature = 5500;
        light.intensity = .7f;
        //GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        //foreach (GameObject enemy in currentEnemies)
        //{
        //    Destroy(enemy);
        //}

        yield return new WaitForSeconds(dayTimeDuration);//140

        musicManager.Stop();
        musicManager.clip = track2;
        musicManager.Play();
        Camera.main.backgroundColor = new Color(0.8f, 0.5f, 1f);
        light.colorTemperature = 3400;
        light.intensity = .3f;

        for (int i = 0; i < transform.childCount && i < enemies.Length; i++)
        {
            Transform child = transform.GetChild(i);
            GameObject prefab = enemies[i];
            Instantiate(prefab, child.position, Quaternion.identity, child); // optional: attach it to child
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        //yield return new WaitForSeconds(120);//140

        if(currentDay < 5)
        {
            currentDay++;
            dayTimeDuration -= timeReduction;
            timeBetweenSpawns -= timeReduction;
        }

        StartCoroutine(PlayMusic());
    }
}
