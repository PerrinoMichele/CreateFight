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
    public Material mat2;
    public Material mat1;
    public AudioClip hitSound;

    private GameObject player;
    private AudioSource audioSource;
    public int hitPoints = 3;
    private Color currentColor;

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
        outline = GetComponent<Outline>();
        audioSource = FindFirstObjectByType<AudioSource>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        if (transform.position.y == 0) { outline.enabled = true; }
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
        if(transform.position.y == -1) { return; }

        //Code to change materials
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f) || hit.collider.gameObject.tag != "Interactable" || hit.collider.gameObject.tag != "Indestructable ")
        {
            if (player.transform.position.y < -.8f && Vector3.Distance(transform.position, player.transform.position) < 3f)
            {
                rend.material = mat2;
            }
            else if(rend.material == mat2) { rend.material = mat1; }
        }
        else if (rend.material == mat2) { rend.material = mat1; }
    }

    public void GetHit()
    {
        //audioSource.pitch =  1+ hitPoints * .5f;
        audioSource.PlayOneShot(hitSound);
        currentColor = rend.material.color;
        rend.material.color = currentColor * .8f;
        hitPoints --;
        StartCoroutine(IncreaseHitPoints());
        
    }

    private IEnumerator IncreaseHitPoints()
    {
        yield return new WaitForSeconds(recoveryTime);
        currentColor = rend.material.color;
        rend.material.color = currentColor * 1.25f;
        hitPoints++;   
    }

}


