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

    private GameObject player;
    private AudioSource audioSource;
    public int hitPoints = 3;
    private Color currentColor;
    private Color defaultColor;
    private string currentMatName;

    void OnDrawGizmos()
    {
        //if (!Application.isPlaying)
        //{
        //    if (transform.position.y < 0) { rend.sharedMaterial = mat2; }
        //}
    }


    private void Start()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
        outline = GetComponent<Outline>();
        audioSource = FindFirstObjectByType<AudioSource>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        if (transform.position.y >= 0) { outline.enabled = true; }
    }


    private void Update()
    {
        if (hitPoints == 0)
        {
            player.GetComponent<InputPlayer>().BlocksCollected++;
            player.GetComponent<InputPlayer>().UpdateBlockText();
            //audioSource.pitch = 1f;
            audioSource.PlayOneShot(hitSound);

            Destroy(this.gameObject);
        }

        currentMatName = rend.material.name.Replace(" (Instance)", "");
        float playerY = player.transform.position.y;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Ensure the cube switches material only when necessary
        if (playerY > -0.8f || distanceToPlayer >= 5f || transform.position.y == -1)
        {
            if(currentMatName != defaultMat.name)
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

    public void GetHit()
    {
        if(gameObject.tag == "Interactable")
        //audioSource.pitch =  1+ hitPoints * .5f;
        audioSource.PlayOneShot(hitSound);
        if (currentMatName == defaultMat.name)
        {
            currentColor = rend.material.color;
            rend.material.color = currentColor * .8f;
            hitPoints--;
            StartCoroutine(IncreaseHitPoints());
        }      
    }

    private IEnumerator IncreaseHitPoints()
    {
        yield return new WaitForSeconds(recoveryTime);
        currentColor = rend.material.color;
        if(currentColor != defaultColor)
        {
            
            rend.material.color = currentColor * 1.25f;
            hitPoints++;
        }

    }

}


