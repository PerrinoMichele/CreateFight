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
    public AudioClip woodSnap;

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
        if(GetComponent<Renderer>() != null) { rend = GetComponent<Renderer>(); }
        
        defaultColor = rend.material.color;
        audioSource = FindFirstObjectByType<AudioSource>();
        player = FindFirstObjectByType<InputPlayer>().gameObject;
        
    }


    private void Update()
    {
                if (transform.position.y == 1) return;

                currentMatName = rend.material.name.Replace(" (Instance)", "");
                float playerY = player.transform.position.y;
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                // Ensure the cube switches material only when necessary
                if (playerY < .8f && transform.position.y == 1 && distanceToPlayer < 3f)
                {
                    if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f) ||
                    (hit.collider.gameObject.tag != "Interactable" && hit.collider.gameObject.tag != "Indestructable"))
                    {
                        rend.material = ditherMat;
                        transform.Find("Cube").gameObject.SetActive(false);
                    }
                }

                else if (playerY > -0.8f || distanceToPlayer >= 3f || transform.position.y == -1)//MAKE 3 A VARIABLE
                {
                    if (currentMatName != defaultMat.name)
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
    
}


