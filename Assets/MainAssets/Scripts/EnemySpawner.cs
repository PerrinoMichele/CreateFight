using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    public AudioSource musicManager;
    public AudioClip track1;
    public AudioClip track2;
    private Light light;
    public GameObject[] enemies;
    private bool startedMusic = false;
    private GameObject player;

    void Start()
    {
        light = FindFirstObjectByType<Light>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        Camera.main.backgroundColor = new Color(0.55f, 0.8f, 1f);
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
        musicManager.PlayOneShot(track1);
        Camera.main.backgroundColor = new Color(0.55f, 0.8f, 1f);
        light.colorTemperature = 5500;
        light.intensity = 1.5f;
        GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in currentEnemies)
        {
            Destroy(enemy);
        }

        yield return new WaitForSeconds(140);//140

        musicManager.PlayOneShot(track2);
        Camera.main.backgroundColor = new Color(0.8f, 0.5f, 1f);
        light.colorTemperature = 3400;
        light.intensity = .9f;

        for (int i = 0; i < transform.childCount && i < enemies.Length; i++)
        {
            Transform child = transform.GetChild(i);
            GameObject prefab = enemies[i];
            Instantiate(prefab, child.position, Quaternion.identity, child); // optional: attach it to child
        }

        yield return new WaitForSeconds(140);//140

        StartCoroutine(PlayMusic());
    }
}
