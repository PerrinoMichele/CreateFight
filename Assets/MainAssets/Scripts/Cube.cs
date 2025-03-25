using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Cube : MonoBehaviour
{
    Outline outline;
    private GameObject player;
    public AudioClip popSound;
    public AudioClip thudSound;

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



    //below not used
    private void Update()
    {
        if(hitPoints == 0)
        {
            player.GetComponent<RightStick>().BlocksCollected++;
            player.GetComponent<RightStick>().UpdateBlockText();
            Destroy(this.gameObject);
        }
    }

    public void GetHit()
    {

        audioSource.PlayOneShot(popSound);
        hitPoints --;
        StartCoroutine(IncreaseHitPoints());
        
    }

    private IEnumerator IncreaseHitPoints()
    {
        yield return new WaitForSeconds(1.5f);
        hitPoints++;   
    }

}
