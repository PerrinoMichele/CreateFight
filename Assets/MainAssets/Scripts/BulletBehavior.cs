using UnityEngine;
using System.Collections;
using Sandbox3D;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float distanceToTravel;
    [SerializeField] private float timeBetweenShots;

    public float speed = 10f;     // Bullet speed
    public float angle;
    public int maxBounces = 3;    // Number of times it moves
    private int currentBounce = 0;
    private Vector3 startPosition;
    private Vector3 direction;
    private GameObject player;
    private AudioSource audioSource;
    public AudioClip slashSound;
    public AudioClip pingSound;
    public AudioClip growlSound;


    void Start()
    {
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        direction = (player.transform.forward + Vector3.up * angle).normalized;      // Move in the player's facing direction
        transform.rotation = player.transform.rotation;
        StartCoroutine(MoveBullet());
        audioSource = FindFirstObjectByType<AudioSource>();
        audioSource.PlayOneShot(slashSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable")
        {
            other.gameObject.GetComponent<Cube>().GetHit();
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Indestructable")
        {
            audioSource.PlayOneShot(pingSound);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Enemy")
        {
            audioSource.PlayOneShot(growlSound);
            other.gameObject.GetComponent<GeneralEnemy>().KnockBack();
            Destroy(gameObject);
        }
    }

    IEnumerator MoveBullet()
    {
        while (currentBounce < maxBounces)
        {
            transform.position = player.transform.position; // Reset start position
            float distanceTraveled = 0f;

            while (distanceTraveled < distanceToTravel)  // Move the bullet for 1 meter
            {
                float step = speed * Time.deltaTime;
                transform.position += direction * step;
                distanceTraveled += step;
                yield return null;
            }

            yield return new WaitForSeconds(timeBetweenShots); // Small delay before next move
            currentBounce++;
        }
        Destroy(gameObject); // Destroy after 3 moves
    }
}