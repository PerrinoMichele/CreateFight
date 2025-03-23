using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Cube : MonoBehaviour
{
    Outline outline;
    private GameObject player;
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
        player = FindFirstObjectByType<AvatarMove>().gameObject;
    }

    private void Update()
    {
        

        if(hitPoints == 0)
        {
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
