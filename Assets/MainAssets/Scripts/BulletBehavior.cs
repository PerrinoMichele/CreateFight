using UnityEngine;
using System.Collections;
using Sandbox3D;
using TMPro;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float distanceToTravel;
    [SerializeField] private float timeBetweenShots;

   
    public float speed = 10f;     // Bullet speed
    public float angle;
    public int maxBounces = 3;    // Number of times it moves


    private Transform splashAim;

    private int currentBounce = 0;
    private Vector3 startPosition;
    private Vector3 direction;
    private GameObject player;
    private AudioSource audioSource;
    public AudioClip slashSound;
    public AudioClip pingSound;
    public AudioClip dullSound;
    public AudioClip growlSound;
    public AudioClip impactSound;
    public GameObject smokeVFX;


    void Start()
    {
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        direction = (player.transform.forward + Vector3.up * angle).normalized;      // Move in the player's facing direction
        transform.rotation = player.transform.rotation;
        splashAim = GameObject.Find("RockBulletAim")?.transform;

        if (gameObject.name == "RockBullet(Clone)")
        {
            Vector3 pointA = player.transform.position;
            Vector3 pointC = new Vector3(0,0,0);
            pointA.y += 1;
            transform.position = pointA;
            
            if (splashAim == null)
            {
                pointC = player.GetComponent<InputPlayer>().FindNearestInteractable().transform.position;
            }

            else { pointC = splashAim.transform.position; }
            
            StartCoroutine(FlyPath(pointA, pointC));
        }
        else
        {
            StartCoroutine(MoveBullet());
        }

        audioSource = FindFirstObjectByType<AudioSource>();
        audioSource.PlayOneShot(slashSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable")
        {
            if (gameObject.name == "RockBullet(Clone)")
            {
                other.gameObject.GetComponent<Cube>().GetHit(2);
            }
            else
            {
                other.gameObject.GetComponent<Cube>().GetHit(1);
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Indestructable")
        {
            audioSource.PlayOneShot(pingSound, .3f);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Ground")
        {
            audioSource.PlayOneShot(dullSound);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Enemy")
        {
            if (gameObject.name == "RockBullet(Clone)") 
            {
                Destroy(gameObject);
                other.gameObject.GetComponent<HealthSystem>().GetHit(2);
                //player.GetComponent<Inventory>().itemsAmounts[2]++;
                //player.GetComponent<Inventory>().UpdateBlockText(2);
                //Destroy(other.gameObject);
                
            }
            else 
            {
                Destroy(gameObject);
                other.gameObject.GetComponent<HealthSystem>().GetHit(1);
                //audioSource.PlayOneShot(growlSound);
                other.gameObject.GetComponent<GeneralEnemy>().KnockBack();
                
            }              
        }

        //if (other.GetComponent<Bomb>() != null)
        //{
        //    audioSource.PlayOneShot(pingSound);
        //    other.GetComponent<Bomb>().ExplodeBomb();
        //}
    }

    IEnumerator FlyPath(Vector3 pointA, Vector3 pointC)
    {
        
        Vector3 pointB = (pointA + pointC) / 2f;
        pointB.y += 3;//make 3 Height variable

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / .5f;//make .5f Duration variable



            // Bezier curve formula
            Vector3 pos = Mathf.Pow(1 - t, 2) * pointA +
                          2 * (1 - t) * t * pointB +
                          Mathf.Pow(t, 2) * pointC;

            transform.position = pos;

            transform.Rotate(Vector3.right * 360f * Time.deltaTime);

            yield return null; // wait next frame
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