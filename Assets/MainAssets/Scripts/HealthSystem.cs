using Sandbox3D;
using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    Outline outline;
    [SerializeField] private float recoveryTime;

    public Renderer rend;
    public Material defaultMat;
    public AudioClip hitSound;
    public AudioClip woodSnap;

    private GameObject player;
    private AudioSource audioSource;
    public int hitPoints = 3;
    private Color currentColor;
    private Color defaultColor;
    private string currentMatName;

    private void Start()
    {
        if (GetComponent<Renderer>() != null) { rend = GetComponent<Renderer>(); }

        defaultColor = rend.material.color;
        audioSource = FindFirstObjectByType<AudioSource>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;

    }

    private void Update()
    {
        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }

    }

    public void GetHit(int damage)
    {
        //if(gameObject.tag == "Interactable")
        //audioSource.pitch =  1+ hitPoints * .5f;
        if (gameObject.GetComponent<Wood>() != null) { }
        else if (gameObject.GetComponent<GeneralEnemy>())
        {
            audioSource.PlayOneShot(hitSound, .3f);
        }
        else 
        { 
            audioSource.PlayOneShot(hitSound); 
        
        }

        currentColor = rend.material.color;
        rend.material.color = currentColor * .8f;
        hitPoints-=damage;

        // Change all children's colors
        foreach (Renderer childRend in GetComponentsInChildren<Renderer>())
        {
            if (childRend != rend) // avoid setting twice on the parent
            childRend.material.color = currentColor * .8f;
        }

        // reset color
        StartCoroutine(IncreaseHitPoints(damage));
    }

    private IEnumerator IncreaseHitPoints(int damage)
    {
        yield return new WaitForSeconds(recoveryTime);
        currentColor = rend.material.color;
        if (currentColor != defaultColor)
        {

            rend.material.color = currentColor * 1.25f;
            // Change all children's colors
            foreach (Renderer childRend in GetComponentsInChildren<Renderer>())
            {
                if (childRend != rend) // avoid setting twice on the parent
                    childRend.material.color = currentColor * 1.25f;
            }

            hitPoints +=damage;
        }
    }

    private void OnDestroy()
    {

        //if (gameObject.GetComponent<Wood>())
        //{
        //    audioSource.PlayOneShot(woodSnap);
        //}
        if (gameObject.tag == "Indestructable")
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
            player.GetComponent<Inventory>().itemsAmounts[1]++;
            player.GetComponent<Inventory>().UpdateBlockText(1);
            audioSource.PlayOneShot(hitSound);
        }
        else if (gameObject.tag == "Enemy")
        {
            int randomNumber = Random.Range(1, 3);
            if (randomNumber == 1) 
            { 
                player.GetComponent<Inventory>().itemsAmounts[1] += 2;
                player.GetComponent<Inventory>().UpdateBlockText(1);
                audioSource.PlayOneShot(hitSound, .3f);
            }

            if (randomNumber == 2)
            {
                player.GetComponent<Inventory>().itemsAmounts[2] += 1;
                player.GetComponent<Inventory>().UpdateBlockText(2);
                audioSource.PlayOneShot(hitSound, .3f);
            }
        }
    }
}
