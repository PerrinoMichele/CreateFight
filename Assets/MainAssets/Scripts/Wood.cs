using System.Collections;
using UnityEngine;

public class Wood : MonoBehaviour
{
    public AudioClip crackSound;

    private HealthSystem cube;
    private bool isBreaking = false;
    private AudioSource audioSource;

    private void Start()
    {
        cube = GetComponent<HealthSystem>();
        audioSource = FindFirstObjectByType<AudioSource>();
        
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
        audioSource.PlayOneShot(crackSound);
        isBreaking = true;
        cube.GetHit(1);
        yield return new WaitForSeconds(.5f);
        cube.GetHit(1);
        yield return new WaitForSeconds(.5f);
        cube.GetHit(1);
        yield return new WaitForSeconds(.5f);
        cube.GetHit(1);
    }

}
