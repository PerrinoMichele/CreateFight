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
    public AudioClip hitSound;

    private GameObject player;
    private int currentMaterialIndex = 0;
    private AudioSource audioSource;
    public int hitPoints = 3;
    private Color currentColor;
    private Color groudHeightColor = Color.gray;

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
        //if (transform.position.y < 0) { rend.material = mat2; }
        if (transform.position.y == 0) { outline.enabled = true; }
    }


    private void Update()
    {
        currentColor = rend.material.color;
        if (hitPoints == 0)
        {
            player.GetComponent<InputPlayer>().BlocksCollected++;
            player.GetComponent<InputPlayer>().UpdateBlockText();
            //audioSource.pitch = 1f;
            audioSource.PlayOneShot(hitSound);
            
            Destroy(this.gameObject);
            //navMeshSurface.BuildNavMesh();
        }
        if (!Physics.Raycast(transform.position, Vector3.down, 1f))
        {
            rend.material = mat2;
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


