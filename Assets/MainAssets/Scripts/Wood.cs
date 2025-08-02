using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class Wood : MonoBehaviour
{
    public AudioClip crackSound;

    private HealthSystem cube;
    private bool isBreaking = false;
    private AudioSource audioSource;
    private GameObject player;
    public AudioClip hitSound;

    private void Start()
    {
        cube = GetComponent<HealthSystem>();
        audioSource = FindFirstObjectByType<AudioSource>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isBreaking)
        {
            if (collision.gameObject.tag == "Player" )
            {
                Destroy(gameObject, 1.5f);
                StartCoroutine(DestroyWood());
            }
        }
    }

    private IEnumerator DestroyWood()
    {
        audioSource.PlayOneShot(crackSound, 0.1f);
        isBreaking = true;
        cube.GetHit(1);
        yield return new WaitForSeconds(.5f);
        cube.GetHit(1);
        yield return new WaitForSeconds(.5f);
        cube.GetHit(1);
        yield return new WaitForSeconds(.5f);
        cube.GetHit(1);
    }

    private void OnDestroy()
    {
        player.GetComponent<Inventory>().itemsAmounts[0]++;
        player.GetComponent<Inventory>().UpdateBlockText(0);
        audioSource.PlayOneShot(hitSound);
        GameObject.Find("MapGen").GetComponent<mapGenerator>()
        .OnTreeDestroyed(transform.position, transform.rotation);
    }
}
