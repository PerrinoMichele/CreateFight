using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Cube : MonoBehaviour
{
    Outline outline;
    private GameObject player;

    public Renderer rend;
    public AudioClip popSound;
    public AudioClip thudSound;
    public AudioClip hitSound;

    private int currentMaterialIndex = 0;
    private GameObject pickaxeButtonObject;
    private Button pickaxeButton;
    private AudioSource audioSource;
    private float distance;
    public int hitPoints = 3;
    private Color currentColor;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        outline = GetComponent<Outline>();
        audioSource = GetComponent<AudioSource>();
        player = FindFirstObjectByType<AvatarMove>().gameObject;
    }



    //below not used
    private void Update()
    {
        currentColor = rend.material.color;
        if (hitPoints == 0)
        {
            player.GetComponent<RightStick>().BlocksCollected++;
            player.GetComponent<RightStick>().UpdateBlockText();
            player.GetComponent<AudioSource>().pitch = 1f;
            player.GetComponent<AudioSource>().PlayOneShot(hitSound);
            Destroy(this.gameObject);
        }
    }

    public void GetHit()
    {
        GetComponent<AudioSource>().pitch = 1 + hitPoints * .5f;
        audioSource.PlayOneShot(hitSound);
        rend.material.color = currentColor * .8f;
        hitPoints --;
        StartCoroutine(IncreaseHitPoints());
        
    }

    private IEnumerator IncreaseHitPoints()
    {
        yield return new WaitForSeconds(1.5f);
        rend.material.color = currentColor * 1.25f;
        hitPoints++;   
    }

}


