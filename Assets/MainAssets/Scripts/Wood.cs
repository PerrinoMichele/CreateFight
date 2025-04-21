using System.Collections;
using UnityEngine;

public class Wood : MonoBehaviour
{
    public AudioClip crackSound;

    private Cube cube;
    private bool isBreaking = false;
    private AudioSource audioSource;

    private void Start()
    {
        cube = GetComponent<Cube>();
        audioSource = FindFirstObjectByType<AudioSource>();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isBreaking)
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
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
        cube.GetHit();
        yield return new WaitForSeconds(.5f);
        cube.GetHit();
        yield return new WaitForSeconds(.5f);
        cube.GetHit();
        yield return new WaitForSeconds(.5f);
        cube.GetHit();
    }

}
