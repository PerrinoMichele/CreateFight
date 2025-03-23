using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Cube : MonoBehaviour
{
    Outline outline;
    public GameObject player;
    public AudioClip digSound;

    private GameObject pickaxeButtonObject;
    private Button pickaxeButton;
    private AudioSource audioSource;
    private float distance;
    public int hitPoints = 4;

    private void Start()
    {
        outline = GetComponent<Outline>();
        audioSource = GetComponent<AudioSource>();
        player = FindFirstObjectByType<AvatarInput>().gameObject;
        pickaxeButtonObject = GameObject.Find("Pickaxe");

    }

    private void Update()
    {
        
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 1)
        {
            outline.enabled = false; 
        }
        else if (player.GetComponent<AvatarInput>().target == this.gameObject)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
            
        }

        if(hitPoints == 0)
        {
            player.GetComponent<AvatarInput>().blockIsStacked = true;
            Destroy(this.gameObject);
           
        }

    }

    public void GetHit()
    {
        if(outline.enabled)
        {
            audioSource.PlayOneShot(digSound);
            hitPoints --;
            StartCoroutine(IncreaseHitPoints());
        }
    }

    private IEnumerator IncreaseHitPoints()
    {
        yield return new WaitForSeconds(1.5f);
        hitPoints++;   
    }

}
