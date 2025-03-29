using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class Cube : MonoBehaviour
{
    Outline outline;
    [SerializeField] private float recoveryTime;
    private GameObject player;

    public Renderer rend;
    public AudioClip hitSound;
    public NavMeshSurface navMeshSurface;

    private int currentMaterialIndex = 0;

    private AudioSource audioSource;
    public int hitPoints = 3;
    private Color currentColor;
    private Color groudHeightColor = Color.gray;

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (transform.position.y < 0) { rend.sharedMaterial.color = groudHeightColor; }
        }
        
    }


    private void Start()
    {
        //navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        //navMeshSurface.BuildNavMesh();
        rend = GetComponent<Renderer>();
        outline = GetComponent<Outline>();
        audioSource = FindFirstObjectByType<AudioSource>();
        //audioSource.PlayOneShot(popSound);
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        if (transform.position.y < 0) { rend.material.color = groudHeightColor; }

    }



    //below not used
    private void Update()
    {
        currentColor = rend.material.color;
        if (hitPoints == 0)
        {
            player.GetComponent<InputPlayer>().BlocksCollected++;
            player.GetComponent<InputPlayer>().UpdateBlockText();
            //audioSource.pitch = 1f;
            audioSource.PlayOneShot(hitSound);
            //navMeshSurface.BuildNavMesh();
            Destroy(this.gameObject);
        }
    }

    public void GetHit()
    {
        //audioSource.pitch =  1+ hitPoints * .5f;
        audioSource.PlayOneShot(hitSound);
        rend.material.color = currentColor * .8f;
        hitPoints --;
        StartCoroutine(IncreaseHitPoints());
        
    }

    private IEnumerator IncreaseHitPoints()
    {
        yield return new WaitForSeconds(recoveryTime);
        rend.material.color = currentColor * 1.25f;
        hitPoints++;   
    }

}


