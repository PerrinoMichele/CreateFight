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
    public GameObject rockBlockPrefab;
    public GameObject bombBlockPrefab;
    public GameObject blockVFX;

    private Transform splashAim;

    private int currentBounce = 0;
    private Vector3 startPosition;
    private Vector3 directionForward;
    private Vector3 directionDownward;
    private GameObject player;
    private AudioSource audioSource;
    public AudioClip slashSound;
    public AudioClip pingSound;
    public AudioClip dullSound;
    public AudioClip growlSound;
    public AudioClip impactSound;
    public GameObject smokeVFX;
    private bool hasCollided = false;


    void Start()
    {
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        
        directionDownward = (player.transform.forward + Vector3.down * angle).normalized;      // Move in the player's downward direction
        //transform.rotation = player.transform.rotation;
        splashAim = GameObject.Find("BombBulletAim")?.transform;

        if (gameObject.name == "BombBullet(Clone)")
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
            
            StartCoroutine(MoveBulletParable(pointA, pointC));
        }

        else if (gameObject.name == "RockBullet(Clone)")
        {
            StartCoroutine(MoveBulletForward());
            //StartCoroutine(MoveBulletSpread());

            //Vector3 p = player.transform.position;
            //p.y = 4f; // PLAYER SPAWN HEIGHT
            //player.transform.position = p;

            //player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            //StartCoroutine(MoveBulletDownward());
        }

        else
        {
            StartCoroutine(MoveBulletForward());
        }

        audioSource = FindFirstObjectByType<AudioSource>();
        if (gameObject.name == "WoodBullet(Clone)") { audioSource.PlayOneShot(slashSound, .1f); }
        else { audioSource.pitch = Random.Range(0.9f, 1.2f); audioSource.PlayOneShot(slashSound); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;

        if (other.gameObject.tag == "Interactable")
        {
            //Instantiate(smokeVFX, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.identity);
            Instantiate(smokeVFX, transform.position, Quaternion.identity);

            if (gameObject.name == "BombBullet(Clone)")
            {
                other.gameObject.GetComponent<Cube>().GetHit(2);
            }

            else if (gameObject.name == "RockBullet(Clone)")
            {
                other.gameObject.GetComponent<Cube>().GetHit(2);
                //Vector3Int bulletLastPosRounded = Vector3Int.RoundToInt(other.transform.position) + Vector3Int.up;

                //if (bulletLastPosRounded.y < 1)
                //{
                //    if (!Physics.CheckBox(bulletLastPosRounded + Vector3.down, Vector3.one * 0.2f, Quaternion.identity))
                //    {
                //        bulletLastPosRounded = bulletLastPosRounded + Vector3Int.down;
                //    }
                //    Instantiate(rockBlockPrefab, bulletLastPosRounded, Quaternion.identity);
                //    hasCollided = true;
                //    //if (!Physics.CheckSphere((Vector3)bulletLastPosRounded, 0.1f, ~0, QueryTriggerInteraction.Ignore))
                //    //{
                //    //    Instantiate(blockVFX, bulletLastPosRounded + Vector3.up, Quaternion.Euler(90f, 0f, 0f));
                //    //    Instantiate(rockBlockPrefab, bulletLastPosRounded, Quaternion.identity);
                //    //    hasCollided = true;
                //    //}
                //}
                //else
                //{
                //    other.gameObject.GetComponent<Cube>().GetHit(2);
                //}
            }

            else
            {
                other.gameObject.GetComponent<Cube>().GetHit(1);
            }
            Destroy(gameObject);
        }


        else if (other.gameObject.tag == "Indestructable")
        {
            audioSource.pitch = Random.Range(0.9f, 1.2f);
            audioSource.PlayOneShot(pingSound, .3f);
            Destroy(gameObject);
        }


        else if (other.gameObject.tag == "Ground")
        {          
            Instantiate(smokeVFX, transform.position, Quaternion.identity);

            //if (gameObject.name == "RockBullet(Clone)")
            //{
            //    Vector3Int bulletLastPosRounded = Vector3Int.RoundToInt(transform.position) + Vector3Int.up;
            //    if (bulletLastPosRounded.y < -1) { bulletLastPosRounded.y = -1; }

            //    if (bulletLastPosRounded.y < 1)
            //    {
            //        if (!Physics.CheckBox(bulletLastPosRounded + Vector3.down, Vector3.one * 0.2f, Quaternion.identity))
            //        {
            //            bulletLastPosRounded = bulletLastPosRounded + Vector3Int.down;
            //        }
            //        print("hitLava");
            //        Instantiate(rockBlockPrefab, bulletLastPosRounded, Quaternion.identity);
            //        hasCollided = true;
            //        //if (!Physics.CheckSphere((Vector3)bulletLastPosRounded, 0.1f, ~0, QueryTriggerInteraction.Ignore))
            //        //{
            //        //    Instantiate(blockVFX, bulletLastPosRounded + Vector3.up, Quaternion.Euler(90f, 0f, 0f));
            //        //    Instantiate(rockBlockPrefab, bulletLastPosRounded, Quaternion.identity);
            //        //    hasCollided = true;
            //        //    //else if (gameObject.name == "BombBullet(Clone)") { Instantiate(bombBlockPrefab, bulletLastPosRounded, Quaternion.identity); }
            //        //}
            //    }
            //}

            Destroy(gameObject);
        }


        else if (other.gameObject.tag == "Enemy")
        {
            //Instantiate(smokeVFX, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.identity);
            Instantiate(smokeVFX, transform.position, Quaternion.identity);
            
            if(gameObject.name == "RockBullet(Clone)")
            {
                other.gameObject.GetComponent<HealthSystem>().GetHit(2);
                Destroy(gameObject);
            }

            if (gameObject.name == "BombBullet(Clone)") 
            {
                other.gameObject.GetComponent<HealthSystem>().GetHit(3);

                //-----------other.gameObject.GetComponent<GeneralEnemy>().KnockBack();

                //Code to create block where enemy is-----------
                //Vector3Int bulletLastPosRounded = Vector3Int.RoundToInt(transform.position) ;
                //if (bulletLastPosRounded.y == -2) { bulletLastPosRounded.y = -1; }
                //if (bulletLastPosRounded.y == 1) { bulletLastPosRounded.y = 0; }

                //if (bulletLastPosRounded.y < 1)
                //{

                //    Vector3 checkPos = (Vector3)bulletLastPosRounded;

                //    // check if anything tagged "Interactable" is exactly here
                //    Collider[] hits = Physics.OverlapSphere(checkPos, 0.01f, ~0, QueryTriggerInteraction.Ignore);
                //    bool hasInteractable = System.Array.Exists(hits, c => c.CompareTag("Interactable"));

                //    if (!hasInteractable)
                //    {
                //        Instantiate(blockVFX, checkPos + Vector3.up, Quaternion.Euler(90f, 0f, 0f));

                //        if (gameObject.name == "RockBullet(Clone)")
                //            Instantiate(rockBlockPrefab, checkPos, Quaternion.identity);
                //        else if (gameObject.name == "BombBullet(Clone)")
                //            Instantiate(bombBlockPrefab, checkPos, Quaternion.identity);

                //        hasCollided = true;
                //    }

                //}

                Destroy(gameObject);

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

    IEnumerator MoveBulletParable(Vector3 pointA, Vector3 pointC)
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

    IEnumerator MoveBulletDownward()
    {
        //transform.position = player.transform.position + Vector3.down * 1.5f; // Reset start position
        transform.position = player.transform.position;
        Vector3 b = player.transform.position;
        b.y = 3f; // BULLET SPAWN HEIGHT
        transform.position = b;

        float timer = 0f;

        while (timer < 3)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null; // wait next frame
        }
        Destroy(gameObject);  
    }

    IEnumerator MoveBulletForward()
    {
        directionForward = (transform.forward + Vector3.up * angle).normalized;      // Move in the player's facing direction
        while (currentBounce < maxBounces)
        {
            //transform.position = player.transform.position; // Reset start position
            float distanceTraveled = 0f;

            while (distanceTraveled < distanceToTravel)  // Move the bullet for 1 meter
            {
                float step = speed * Time.deltaTime;
                transform.position += directionForward * step;
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
            //Instantiate(smokeVFX, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.identity);
            audioSource.pitch = Random.Range(0.9f, 1.2f);
            audioSource.PlayOneShot(impactSound);
        }
    }

}