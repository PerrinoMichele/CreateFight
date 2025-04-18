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
    public AudioClip impactSound;
    public GameObject smokeVFX;


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
            if (gameObject.name == "RockBullet(Clone)") 
            { 
                Destroy(other);
                player.GetComponent<Inventory>().itemsAmounts[2]++;
                player.GetComponent<Inventory>().UpdateBlockText(2);
            }
            audioSource.PlayOneShot(growlSound);
            other.gameObject.GetComponent<GeneralEnemy>().KnockBack();
            Destroy(gameObject);
        }

        //if (other.GetComponent<Bomb>() != null)
        //{
        //    audioSource.PlayOneShot(pingSound);
        //    other.GetComponent<Bomb>().ExplodeBomb();
        //}
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
        Destroy(gameObject); // Destroy after max bounces 
    }
    void OnDestroy()
    {
        // Optional: check if game is not quitting
        if (smokeVFX != null)
        {
            Instantiate(smokeVFX, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.identity);
            audioSource.PlayOneShot(impactSound);
        }
    }

}