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
        currentMatName = rend.material.name.Replace(" (Instance)", "");
    }

    public void GetHit()
    {
        //if(gameObject.tag == "Interactable")
        //audioSource.pitch =  1+ hitPoints * .5f;
        if (gameObject.GetComponent<Wood>() != null) { }
        else { audioSource.PlayOneShot(hitSound); }

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
        if (currentColor != defaultColor)
        {

            rend.material.color = currentColor * 1.25f;
            hitPoints++;
        }
    }

    private void OnDestroy()
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
            player.GetComponent<Inventory>().itemsAmounts[1]++;
            player.GetComponent<Inventory>().UpdateBlockText(1);
            audioSource.PlayOneShot(hitSound);
        }
    }
}
