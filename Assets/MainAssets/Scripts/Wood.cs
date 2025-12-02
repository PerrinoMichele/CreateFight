using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Wood : MonoBehaviour
{
    public AudioClip crackSound;

    private HealthSystem cube;
    private bool isBreaking = false;
    private AudioSource audioSource;
    private GameObject player;
    public AudioClip hitSound;
    private EnemySpawner enemySpawner;

    private void Start()
    {
        cube = GetComponent<HealthSystem>();
        audioSource = FindFirstObjectByType<AudioSource>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            DestroyWood();
        }
    }

    public void DestroyWood()
    {
        if (!isBreaking)
        {
             StartCoroutine(RemoveWoodHealth());
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!isBreaking)
        {
            if (collision.gameObject.GetComponent<BulletBehavior>())
            {
                StartCoroutine(RemoveWoodHealth());
            }
        }
    }

    private IEnumerator RemoveWoodHealth()
    {
        audioSource.PlayOneShot(crackSound, 0.6f);
        isBreaking = true;
        var t = transform;

        t.localScale = new Vector3(0.8f, 1f, 0.8f);   // shrink XZ
        yield return new WaitForSeconds(0.1f);
        t.localScale = Vector3.one;                  // back to normal

        cube.GetHit(1);
        yield return new WaitForSeconds(.4f);

        t.localScale = new Vector3(0.8f, 1f, 0.8f);   // shrink XZ
        yield return new WaitForSeconds(0.1f);
        t.localScale = Vector3.one;                  // back to normal

        cube.GetHit(1);
        yield return new WaitForSeconds(.4f);
        t.localScale = new Vector3(0.8f, 1f, 0.8f);   // shrink XZ
        yield return new WaitForSeconds(0.1f);
        t.localScale = Vector3.one;                  // back to normal

        cube.GetHit(1);
        yield return new WaitForSeconds(.4f);
        cube.GetHit(1);

        enemySpawner.SpawnPickup(0, transform.position);
        enemySpawner.SpawnPickup(0, transform.position);
        enemySpawner.SpawnPickup(0, transform.position);
        enemySpawner.SpawnPickup(0, transform.position);

        audioSource.PlayOneShot(hitSound);
        GameObject.Find("MapGen").GetComponent<mapGenerator>()
        .OnTreeDestroyed(transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
