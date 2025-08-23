using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Unity.AI.Navigation;
using static UnityEngine.UI.Image;

public class Cube : MonoBehaviour
{
    Outline outline;
    [SerializeField] private float recoveryTime;

    public Renderer rend;
    public Material ditherMat;
    public Material defaultMat;
    public AudioClip hitSound;
    public AudioClip woodSnap;

    private GameObject player;
    private AudioSource audioSource;
    public int hitPoints = 3;
    private Color currentColor;
    private Color defaultColor;
    private string currentMatName;
    private EnemySpawner enemySpawner;

    void OnDrawGizmos()
    {
        //if (!Application.isPlaying)
        //{
        //    if (transform.position.y < 0) { rend.sharedMaterial = mat2; }
        //}
    }

    private void Start()
    {
        //GetComponent<BoxCollider>().enabled = false;
        GetComponent<Cube>().enabled = false;

        if(GetComponent<Renderer>() != null) { rend = GetComponent<Renderer>(); }
        
        defaultColor = rend.material.color;
        audioSource = FindFirstObjectByType<AudioSource>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        
    }


    private void Update()
    {
                if (transform.position.y == 1) return;

                if (hitPoints <= 0)
                {
                    if (gameObject.GetComponent<Wood>())
                    {
                        audioSource.PlayOneShot(woodSnap);
                    }
                    else if (gameObject.tag == "Indestructable")
                    {
                        audioSource.PlayOneShot(hitSound);
                    }
                    //else if (gameObject.GetComponent<Bomb>() && player != null)
                    //{
                    //    player.GetComponent<Inventory>().itemsAmounts[2]++;
                    //    player.GetComponent<Inventory>().UpdateBlockText(2);
                    //    audioSource.PlayOneShot(hitSound);
                    //}
                    else if (GetComponent<Bomb>())
                    {
                        return;
                    }

                    else if (gameObject.tag == "Interactable")
                    {
                        // TRANSFER BELOW------
                        //player.GetComponent<Inventory>().itemsAmounts[1]++;
                        //player.GetComponent<Inventory>().UpdateBlockText(1);
                        enemySpawner.SpawnPickup(1, transform.position);

                        audioSource.PlayOneShot(hitSound);
                    }
                    Destroy(this.gameObject);
                }

                currentMatName = rend.material.name.Replace(" (Instance)", "");
                float playerY = player.transform.position.y;
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                // Ensure the cube switches material only when necessary
                if (playerY < .8f && transform.position.y == 1 && distanceToPlayer < 3.5f)
                {
                    if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f) ||
                    (hit.collider.gameObject.tag != "Interactable" && hit.collider.gameObject.tag != "Indestructable"))
                    {
                        rend.material = ditherMat;
                        transform.Find("Cube").gameObject.SetActive(false);
                    }
                }

                else if (playerY > -0.8f || distanceToPlayer >= 3.5f || transform.position.y == -1)//MAKE 3 A VARIABLE
                {
                    if (currentMatName != defaultMat.name)
                    {
                        rend.material = defaultMat;
                    }
                }

                else
                {
                    if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f) ||
                        (hit.collider.gameObject.tag != "Interactable" && hit.collider.gameObject.tag != "Indestructable"))
                    {
                        rend.material = ditherMat;
                    }
                }
    }


    public void GetHit(int damage)
    {
        //if(gameObject.tag == "Interactable")
        //audioSource.pitch =  1+ hitPoints * .5f;
        if (gameObject.GetComponent<Wood>() != null) { }
        else { audioSource.PlayOneShot(hitSound); }

        hitPoints -= damage;
        StartCoroutine(IncreaseHitPoints(damage));

        if (currentMatName == defaultMat.name)
        {
            currentColor = rend.material.color;
            rend.material.color = currentColor * .8f;
        }
    }

    private IEnumerator IncreaseHitPoints(int damage)
    {
        yield return new WaitForSeconds(recoveryTime);
        currentColor = rend.material.color;
        if (currentColor != defaultColor)
        {

            rend.material.color = currentColor * 1.25f;
            hitPoints+= damage;
        }
    }


}


