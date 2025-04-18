using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Transform visualTransform;

    public AudioClip FuseLitSound;
    public AudioClip explosionSound;
    public GameObject explosionVFX;
    public Vector3[] neighbors;

    private Cube cube;
    private bool isBreaking = false;
    private AudioSource audioSource;

    private void Start()
    {
        cube = GetComponent<Cube>();
        audioSource = FindFirstObjectByType<AudioSource>();

        //if nothing below it, move it one tile below -- no floating bomb
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isBreaking)
        {
            if (collision.gameObject.tag != "Interactable" || collision.gameObject.tag != "Indestructable")
            {
                //fuse lighting sound
                //fuse on effect
                StartCoroutine(ExplodeBomb());


            }
        }
    }

    public IEnumerator ExplodeBomb()
    {
        InvokeRepeating("IncreaseScale", .5f, 1);
        InvokeRepeating("DecreaseScale", 0f, 1);

        yield return new WaitForSeconds(3f);


        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            StopAllCoroutines();
            CancelInvoke();
        }

        foreach (Vector3 neighbor in neighbors)
        {
            Vector3 spawnPos = transform.position + neighbor;
            Instantiate(explosionVFX, spawnPos, Quaternion.identity);
        }
        audioSource.PlayOneShot(explosionSound);
    }

    private void IncreaseScale()
    {
        visualTransform.localScale = new Vector3(.9f, .9f, .9f);
    }

    private void DecreaseScale()
    {
        visualTransform.localScale = new Vector3(1, 1, 1);
    }
}
